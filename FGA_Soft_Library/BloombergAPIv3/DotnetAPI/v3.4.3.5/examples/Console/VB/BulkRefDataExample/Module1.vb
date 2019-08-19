'*
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT
'* WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED,
'* INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES
'* OF MERCHANTABILITY AND/OR FITNESS FOR A  PARTICULAR
'* PURPOSE.
'*
'** BulfRefDataExample.vb
'**
'** This Example shows how to Retrieve reference data/Bulk reference 
'** data using Server API
'** Usage: 
'**      		-s			<security	= CAC Index>
'**      		-f			<field		= INDX_MWEIGHT>
'**      		-ip 		<ipAddress	= localhost>
'**      		-p 			<tcpPort	= 8194>
'** e.g. BulfRefDataExample -s "CAC Index" -f INDX_MWEIGHT -ip localhost -p 8194
'*

Imports [Event] = Bloomberglp.Blpapi.Event
Imports Element = Bloomberglp.Blpapi.Element
Imports InvalidRequestException = Bloomberglp.Blpapi.InvalidRequestException
Imports Message = Bloomberglp.Blpapi.Message
Imports Name = Bloomberglp.Blpapi.Name
Imports Request = Bloomberglp.Blpapi.Request
Imports Service = Bloomberglp.Blpapi.Service
Imports Session = Bloomberglp.Blpapi.Session
Imports SessionOptions = Bloomberglp.Blpapi.SessionOptions
Imports Datatype = Bloomberglp.Blpapi.Schema.Datatype

Imports ArrayList = System.Collections.ArrayList

Namespace Bloomberglp.Blpapi.Examples

    Class BulkRefDataExample

        Private Shared ReadOnly SECURITY_DATA As Name = New Name("securityData")
        Private Shared ReadOnly SECURITY As Name = New Name("security")
        Private Shared ReadOnly FIELD_DATA As Name = New Name("fieldData")
        Private Shared ReadOnly RESPONSE_ERROR As Name = New Name("responseError")
        Private Shared ReadOnly SECURITY_ERROR As Name = New Name("securityError")
        Private Shared ReadOnly FIELD_EXCEPTIONS As Name = New Name("fieldExceptions")
        Private Shared ReadOnly FIELD_ID As Name = New Name("fieldId")
        Private Shared ReadOnly ERROR_INFO As Name = New Name("errorInfo")
        Private Shared ReadOnly CATEGORY As Name = New Name("category")
        Private Shared ReadOnly MESSAGE As Name = New Name("message")

        Private d_host As String
        Private d_port As Integer
        Private d_securities As ArrayList
        Private d_fields As ArrayList

        Shared Sub Main(ByVal args() As String)

            System.Console.WriteLine("Reference Data/Bulk Reference Data Example")
            Dim example As BulkRefDataExample = New BulkRefDataExample()
            example.run(args)

            System.Console.WriteLine("Press ENTER to quit")
            System.Console.Read()
        End Sub

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            d_host = "localhost"
            d_port = 8194
            d_securities = New ArrayList()
            d_fields = New ArrayList()
        End Sub

        ''' <summary>
        ''' Read command line arguments, 
        ''' Establish a Session
        ''' Identify and Open refdata Service
        ''' Send ReferenceDataRequest to the Service 
        ''' Event Loop and Response Processing
        ''' </summary>
        ''' <param name="args"></param>
        ''' <remarks></remarks>
        Private Sub run(ByVal args() As String)
            If Not parseCommandLine(args) Then
                Return
            End If

            Dim sessionOptions As SessionOptions = New SessionOptions()
            sessionOptions.ServerHost = d_host
            sessionOptions.ServerPort = d_port

            System.Console.WriteLine("Connecting to " & d_host + ":" & _
                                     d_port.ToString())
            Dim session As Session = New Session(sessionOptions)
            Dim sessionStarted As Boolean = session.Start()
            If Not sessionStarted Then
                System.Console.Error.WriteLine("Failed to start session.")
                Return
            End If
            If Not session.OpenService("//blp/refdata") Then
                System.Console.Error.WriteLine("Failed to open //blp/refdata")
                Return
            End If

            Try
                sendRefDataRequest(session)
            Catch e As InvalidRequestException
                System.Console.WriteLine(e.ToString())
            End Try

            ' wait for events from session.
            eventLoop(session)

            session.Stop()

        End Sub

        ''' <summary>
        ''' Polls for an event or a message in an event loop
        ''' and Processes the event generated
        ''' </summary>
        ''' <param name="session"></param>
        ''' <remarks></remarks>
        Private Sub eventLoop(ByVal session As Session)
            Dim done As Boolean = False
            While Not done
                Dim eventObj As [Event] = session.NextEvent()
                If eventObj.Type = [Event].EventType.PARTIAL_RESPONSE Then
                    System.Console.WriteLine("Processing Partial Response")
                    processResponseEvent(eventObj)
                ElseIf eventObj.Type = [Event].EventType.RESPONSE Then
                    System.Console.WriteLine("Processing Response")
                    processResponseEvent(eventObj)
                    done = True
                Else
                    Dim msg As Message
                    For Each msg In eventObj
                        System.Console.WriteLine(msg.AsElement)
                        If eventObj.Type = [Event].EventType.SESSION_STATUS Then
                            If msg.MessageType.Equals("SessionTerminated") Then
                                done = True
                            End If
                        End If
                    Next
                End If
            End While
        End Sub

        ''' <summary>
        ''' Function to handle response event
        ''' </summary>
        ''' <param name="eventObj"></param>
        ''' <remarks></remarks>
        Private Sub processResponseEvent(ByVal eventObj As [Event])
            Dim msg As Message
            For Each msg In eventObj
                If msg.HasElement(RESPONSE_ERROR) Then
                    printErrorInfo("REQUEST FAILED: ", msg.GetElement(RESPONSE_ERROR))
                    Continue For
                End If
                Dim securities As Element = msg.GetElement(SECURITY_DATA)
                Dim numSecurities As Integer = securities.NumValues
                System.Console.WriteLine(vbCrLf & "Processing " & numSecurities & " securities:")
                Dim secCnt As Integer
                For secCnt = 0 To numSecurities - 1 Step +1
                    Dim eleSecurity As Element = securities.GetValueAsElement(secCnt)
                    Dim ticker As String = eleSecurity.GetElementAsString(SECURITY)
                    System.Console.WriteLine(vbCrLf & "Ticker: " & ticker)
                    If eleSecurity.HasElement("securityError") Then
                        printErrorInfo("SECURITY FAILED: ", _
                                        eleSecurity.GetElement(SECURITY_ERROR))
                        Continue For
                    End If
                    Dim fields As Element = eleSecurity.GetElement(FIELD_DATA)
                    If fields.NumElements > 0 Then
                        System.Console.WriteLine("FIELD" & vbTab & vbTab & "VALUE")
                        System.Console.WriteLine("-----" & vbTab & vbTab & "-----")
                        Dim numElements As Integer = fields.NumElements
                        Dim eleCtr As Integer
                        For eleCtr = 0 To numElements - 1 Step +1
                            Dim field As Element = fields.GetElement(eleCtr)
                            ' Checking if the field is Bulk field
                            If field.Datatype = Datatype.SEQUENCE Then
                                processBulkField(field)
                            Else
                                processRefkField(field)
                            End If
                        Next
                    End If
                    System.Console.WriteLine("")
                    Dim fieldExceptions As Element = eleSecurity.GetElement(FIELD_EXCEPTIONS)
                    If fieldExceptions.NumValues > 0 Then
                        System.Console.WriteLine("FIELD" & vbTab & vbTab & "EXCEPTION")
                        System.Console.WriteLine("-----" & vbTab & vbTab & "---------")
                        Dim k As Integer
                        For k = 0 To fieldExceptions.NumValues - 1 Step +1
                            Dim fieldException As Element = _
                                            fieldExceptions.GetValueAsElement(k)
                            printErrorInfo(fieldException.GetElementAsString(FIELD_ID) _
                                            & vbTab & vbTab, _
                                            fieldException.GetElement(ERROR_INFO))

                        Next
                    End If
                Next
            Next
        End Sub

        ''' <summary>
        ''' Read the reference bulk field contents
        ''' </summary>
        ''' <param name="refBulkfield"></param>
        ''' <remarks></remarks>
        Private Sub processBulkField(ByVal refBulkfield As Element)
            System.Console.WriteLine(vbCrLf & refBulkfield.Name.ToString())
            ' Get the total number of Bulk data points
            Dim numofBulkValues As Integer = refBulkfield.NumValues
            Dim bvCtr As Integer
            For bvCtr = 0 To numofBulkValues - 1 Step bvCtr + 1
                Dim bulkElement As Element = refBulkfield.GetValueAsElement(bvCtr)
                ' Get the number of sub fields for each bulk data element
                Dim numofBulkElements As Integer = bulkElement.NumElements
                Dim beCtr As Integer
                ' Read each field in Bulk data
                For beCtr = 0 To numofBulkElements - 1
                    Dim elem As Element = bulkElement.GetElement(beCtr)
                    System.Console.WriteLine(vbTab & vbTab & elem.Name.ToString() & _
                                            " = " & elem.GetValueAsString())
                Next
            Next

        End Sub

        ''' <summary>
        ''' Read the reference field contents
        ''' </summary>
        ''' <param name="reffield"></param>
        ''' <remarks></remarks>
        Private Sub processRefkField(ByVal reffield As Element)
            System.Console.WriteLine(reffield.Name.ToString() & vbTab & vbTab & _
                                    reffield.GetValueAsString())
        End Sub

        ''' <summary>
        ''' Function to create and send ReferenceDataRequest
        ''' </summary>
        ''' <param name="session"></param>
        ''' <remarks></remarks>
        Private Sub sendRefDataRequest(ByVal session As Session)
            Dim refDataService As Service = Session.GetService("//blp/refdata")
            Dim request As Request = refDataService.CreateRequest("ReferenceDataRequest")

            ' Add securities to request
            Dim securities As Element = request.GetElement("securities")

            Dim i As Integer
            For i = 0 To d_securities.Count - 1 Step +1
                securities.AppendValue(CType(d_securities(i), String))
            Next

            ' Add fields to request
            Dim fields As Element = request.GetElement("fields")
            For i = 0 To d_fields.Count - 1 Step +1
                fields.AppendValue(CType(d_fields(i), String))
            Next

            System.Console.WriteLine("Sending Request: " & _
                                    request.AsElement().ToString())

            Session.SendRequest(request, Nothing)
        End Sub

        ''' <summary>
        ''' Function to parse the command line arguments
        ''' </summary>
        ''' <param name="args"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function parseCommandLine(ByVal args() As String) As Boolean
            Dim i As Integer
            For i = 0 To args.Length - 1 Step +1
                If String.Compare(args(i), "-s", True) = 0 Then
                    d_securities.Add(args(i + 1))
                ElseIf String.Compare(args(i), "-f", True) = 0 Then
                    d_fields.Add(args(i + 1))
                ElseIf String.Compare(args(i), "-ip", True) = 0 Then
                    d_host = args(i + 1)
                ElseIf String.Compare(args(i), "-p", True) = 0 Then
                    d_port = Integer.Parse(args(i + 1))
                ElseIf String.Compare(args(i), "-h", True) = 0 Then
                    printUsage()
                    Return False
                End If
            Next

            ' handle default arguments
            If d_securities.Count = 0 Then
                d_securities.Add("CAC Index")
            End If

            If d_fields.Count = 0 Then
                d_fields.Add("INDX_MWEIGHT")
            End If

            Return True
        End Function

        ''' <summary>
        ''' Prints error information
        ''' </summary>
        ''' <param name="leadingStr"></param>
        ''' <param name="errorInfo"></param>
        ''' <remarks></remarks>
        Private Sub printErrorInfo(ByVal leadingStr As String, _
                                    ByVal errorInfo As Element)
            System.Console.WriteLine(leadingStr & _
                                    errorInfo.GetElementAsString(CATEGORY) _
                                    & " (" _
                                    & errorInfo.GetElementAsString(MESSAGE) _
                                    & ")")
        End Sub

        ''' <summary>
        ''' Print usage of the Program
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub printUsage()
            System.Console.WriteLine("Usage:")
            System.Console.WriteLine("	Retrieve reference data/Bulk reference data" _
                                        & " using Server API")
            System.Console.WriteLine("      [-s         <security   = CAC Index>")
            System.Console.WriteLine("      [-f         <field      = INDX_MWEIGHT>")
            System.Console.WriteLine("      [-ip        <ipAddress  = localhost>")
            System.Console.WriteLine("      [-p         <tcpPort    = 8194>")
        End Sub
    End Class

End Namespace
