'--------------------------------------------------------------------------
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A  PARTICULAR PURPOSE.'
' --------------------------------------------------------------------------

Imports Bloomberglp.Blpapi
Imports System.Collections.ArrayList

Namespace Bloomberglp.Blpapi.Examples

    Module EqsDataExample

        Private ReadOnly DATA As New Name("data")
        Private ReadOnly SECURITY_DATA As New Name("securityData")
        Private ReadOnly SECURITY As New Name("security")
        Private ReadOnly FIELD_DATA As New Name("fieldData")
        Private ReadOnly RESPONSE_ERROR As New Name("responseError")
        Private ReadOnly SECURITY_ERROR As New Name("securityError")
        Private ReadOnly FIELD_EXCEPTIONS As New Name("fieldExceptions")
        Private ReadOnly FIELD_ID As New Name("fieldId")
        Private ReadOnly ERROR_INFO As New Name("errorInfo")
        Private ReadOnly CATEGORY As New Name("category")
        Private ReadOnly MESSAGE As New Name("message")
        Private d_host As String
        Private d_port As Integer
        Private d_screenName As String
        Private d_screenType As String

        Sub Main(ByVal args As String())

            System.Console.WriteLine("EQS Data Example")

            d_host = "localhost"
            d_port = 8194
            d_screenType = "PRIVATE"
            d_screenName = String.Empty

            If Not parseCommandLine(args) Then
                Return
            End If

            Dim sessionOptions As New SessionOptions()
            sessionOptions.ServerHost = d_host
            sessionOptions.ServerPort = d_port

            System.Console.WriteLine("Connecting to " + d_host + ":" + d_port.ToString())

            Dim session As New Session(sessionOptions)
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
                sendEqsDataRequest(session)
            Catch e As InvalidRequestException
                System.Console.WriteLine(e.ToString())
            End Try

            ' wait for events from session. 
            eventLoop(session)

            session.Stop()
            System.Console.WriteLine("Press ENTER to quit")
            System.Console.Read()
        End Sub

        Private Function parseCommandLine(ByVal args As String()) As Boolean

            For i As Integer = 0 To args.Length - 1
                If String.Compare(args(i), "-s", True) = 0 Then
                    d_screenName = args(i + 1).Trim()
                ElseIf String.Compare(args(i), "-t", True) = 0 Then
                    d_screenType = args(i + 1).ToUpper
                ElseIf String.Compare(args(i), "-ip", True) = 0 Then
                    d_host = args(i + 1)
                ElseIf String.Compare(args(i), "-p", True) = 0 Then
                    Dim outPort As Integer = 0
                    If Integer.TryParse(args(i + 1), outPort) Then
                        d_port = outPort
                    End If
                ElseIf String.Compare(args(i), "-h", True) = 0 Then
                    printUsage()
                    Return False
                End If
            Next

            ' handle default arguments 
            If d_screenName.Length = 0 Then
                printUsage()
                Return False
            End If

            Return True
        End Function

        Private Sub printUsage()

            System.Console.WriteLine("Usage:")
            System.Console.WriteLine(vbTab & "Retrieve EQS data")
            System.Console.WriteLine(vbTab & vbTab & "[-s     <screenName	= S&P500>")
            System.Console.WriteLine(vbTab & vbTab & "[-f     <screenType	= GLOBAL or PRIVATE>")
            System.Console.WriteLine(vbTab & vbTab & "[-ip    <ipAddress = localhost>")
            System.Console.WriteLine(vbTab & vbTab & "[-p     <tcpPort   = 8194>")

        End Sub

        Private Sub sendEqsDataRequest(ByVal session As Session)

            Dim refDataService As Service = session.GetService("//blp/refdata")
            Dim request As Request = refDataService.CreateRequest("BeqsRequest")
            request.Set("screenName", d_screenName)
            request.Set("screenType", New Name(d_screenType))

            System.Console.WriteLine("Sending Request: " + request.AsElement.ToString())
            session.SendRequest(request, Nothing)

        End Sub

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
                    For Each msg As Message In eventObj
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

        Private Sub processResponseEvent(ByVal eventObj As [Event])

            For Each msg As Message In eventObj
                If msg.HasElement(RESPONSE_ERROR) Then
                    printErrorInfo("REQUEST FAILED: ", msg.GetElement(RESPONSE_ERROR))
                    Continue For
                End If

                Dim dataElement As Element = msg.GetElement(DATA)
                Dim securities As Element = dataElement.GetElement(SECURITY_DATA)
                Dim numSecurities As Integer = securities.NumValues

                System.Console.WriteLine("Processing " + numSecurities.ToString() + " securities:")

                For i As Integer = 0 To numSecurities - 1
                    Dim security__1 As Element = securities.GetValueAsElement(i)
                    Dim ticker As String = security__1.GetElementAsString(SECURITY)
                    System.Console.WriteLine(vbLf & "Ticker: " + ticker)
                    If security__1.HasElement("securityError") Then
                        printErrorInfo(vbTab & "SECURITY FAILED: ", _
                        security__1.GetElement(SECURITY_ERROR))
                        Continue For
                    End If

                    Dim fields As Element = security__1.GetElement(FIELD_DATA)

                    If fields.NumElements > 0 Then
                        System.Console.WriteLine("FIELD" & vbTab & vbTab & "VALUE")
                        System.Console.WriteLine("-----" & vbTab & vbTab & "-----")
                        Dim numElements As Integer = fields.NumElements
                        For j As Integer = 0 To numElements - 1
                            Dim field As Element = fields.GetElement(j)
                            System.Console.WriteLine(field.Name.ToString() + _
                            vbTab & vbTab + field.GetValueAsString())
                        Next
                    End If

                    System.Console.WriteLine("")

                    Dim fieldExceptions As Element = security__1.GetElement(FIELD_EXCEPTIONS)

                    If fieldExceptions.NumValues > 0 Then
                        System.Console.WriteLine("FIELD" & vbTab & vbTab & "EXCEPTION")
                        System.Console.WriteLine("-----" & vbTab & vbTab & "---------")
                        For k As Integer = 0 To fieldExceptions.NumValues - 1
                            Dim fieldException As Element = fieldExceptions.GetValueAsElement(k)
                            printErrorInfo(fieldException.GetElementAsString(FIELD_ID) + _
                            vbTab & vbTab, fieldException.GetElement(ERROR_INFO))
                        Next
                    End If
                Next
            Next

        End Sub

        Private Sub printErrorInfo(ByVal leadingStr As String, ByVal errorInfo As Element)

            System.Console.WriteLine(leadingStr + errorInfo.GetElementAsString(CATEGORY) _
            + " (" + errorInfo.GetElementAsString(MESSAGE) + ")")

        End Sub

    End Module
End Namespace
