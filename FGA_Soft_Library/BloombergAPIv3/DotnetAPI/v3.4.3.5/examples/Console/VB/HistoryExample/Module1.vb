'---------------------------------------------------------------------------
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A  PARTICULAR PURPOSE.
' --------------------------------------------------------------------------
Imports Bloomberglp.Blpapi
Imports [Datetime] = Bloomberglp.Blpapi.Datetime
Imports [Data] = Bloomberglp.Blpapi.Schema.Datatype
Imports System
Imports System.Collections.ArrayList

Module HistoryExample_VB

    Private ReadOnly RESPONSE_ERROR As New Name("responseError")
    Private ReadOnly CATEGORY As New Name("category")
    Private ReadOnly MESSAGE As New Name("message")
    Private ReadOnly SECURITY_DATA As New Name("securityData")
    Private ReadOnly SECURITY_ERROR As New Name("securityError")
    Private ReadOnly FIELD_ID As New Name("fieldId")
    Private ReadOnly FIELD_DATA As New Name("fieldData")
    Private ReadOnly FIELD_EXCEPTIONS As New Name("fieldExceptions")
    Private ReadOnly ERROR_INFO As New Name("errorInfo")
    Private ReadOnly _DATE As New Name("date")

    Private d_host As String
    Private d_port As Integer
    Private d_securities As ArrayList
    Private d_fields As ArrayList
    Private d_startDate As String
    Private d_endDate As String

    Public Sub Main(ByVal args As String())

        System.Console.WriteLine("Intraday Rawticks Example")

        d_host = "localhost"
        d_port = 8194
        d_securities = New ArrayList()
        d_fields = New ArrayList()
        d_startDate = "null"
        d_endDate = "null"

        run(args)

        System.Console.WriteLine("Press ENTER to quit")
        Console.ReadLine()

    End Sub

    Private Sub run(ByVal args As String())

        If Not parseCommandLine(args) Then Return

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

        Dim refDataService As Service = session.GetService("//blp/refdata")
        Dim request As Request = refDataService.CreateRequest("HistoricalDataRequest")

        Dim securities As Element = request.GetElement("securities")
        For i As Integer = 0 To d_securities.Count - 1
            securities.AppendValue(DirectCast(d_securities(i), String))
        Next

        ' Add fields to request 
        Dim fields As Element = request.GetElement("fields")
        For i As Integer = 0 To d_fields.Count - 1
            fields.AppendValue(DirectCast(d_fields(i), String))
        Next

        request.Set("startDate", d_startDate)
        request.Set("endDate", d_endDate)

        System.Console.WriteLine("Sending Request: " + request.AsElement.ToString())
        session.SendRequest(request, Nothing)

        ' wait for events from session. 
        eventLoop(session)

        session.Stop()
    End Sub

    Private Function parseCommandLine(ByVal args As String()) As Boolean
        Dim dateFormat As String = "yyyyMMdd"

        For i As Integer = 0 To args.Length - 1
            If String.Compare(args(i), "-s", True) = 0 Then
                d_securities.Add(args(i + 1))
            ElseIf String.Compare(args(i), "-f", True) = 0 Then
                d_fields.Add(args(i + 1))
            ElseIf String.Compare(args(i), "-sd", True) = 0 Then
                d_startDate = args(i + 1)
            ElseIf String.Compare(args(i), "-ed", True) = 0 Then
                d_endDate = args(i + 1)
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

        If d_securities.Count = 0 Then
            d_securities.Add("IBM US Equity")
        End If

        If d_fields.Count = 0 Then
            d_fields.Add("PX_LAST")
        End If

        If Not isDateTimeValid(d_startDate, dateFormat) Then
            d_startDate = "20090901"
        End If

        If Not isDateTimeValid(d_endDate, dateFormat) Then
            d_endDate = "20090930"
        End If

        Return True

    End Function

    ''' <summary>
    ''' Validate if date or data/time is valid
    ''' </summary>
    ''' <param name="dateTimeValue"></param>
    ''' <param name="format"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function isDateTimeValid(ByVal dateTimeValue As String, ByVal format As String) As Boolean
        Dim outDateTime As System.DateTime
        Dim formatProvider As IFormatProvider = New System.Globalization.CultureInfo("en-US", True)
        Return System.DateTime.TryParseExact(dateTimeValue, format, formatProvider, _
                System.Globalization.DateTimeStyles.AllowWhiteSpaces, outDateTime)
    End Function

    Private Sub printUsage()

        System.Console.WriteLine("Usage:")
        System.Console.WriteLine("	Retrieve historical data ")
        System.Console.WriteLine("      [-s     <security   = IBM US Equity>")
        System.Console.WriteLine("      [-f		<field		= PX_LAST>")
        System.Console.WriteLine("      [-sd    <startDateTime  = 2007-03-26T09:30:00>")
        System.Console.WriteLine("      [-ed    <endDateTime    = 2007-03-26T10:30:00>")
        System.Console.WriteLine("      [-ip    <ipAddress  = localhost>")
        System.Console.WriteLine("      [-p     <tcpPort    = 8194>")

    End Sub


    Private Sub eventLoop(ByVal session As Session)
        'run through all events expected - signified by a "RESPONSE" event
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
            If (ProcessExceptions(msg) = False) Then
                If (ProcessErrors(msg) = False) Then
                    ProcessFields(msg)
                End If
            End If
        Next

    End Sub

    Private Function ProcessErrors(ByVal msg As Message)

        Dim securityData As Element = msg.GetElement(SECURITY_DATA)

        If securityData.HasElement(SECURITY_ERROR) Then
            Dim _security_error As Element = securityData.GetElement(SECURITY_ERROR)
            Dim _error_message As Element = _security_error.GetElement(MESSAGE)
            System.Console.WriteLine(_error_message)
            Return True
        End If
        Return False
    End Function

    Private Function ProcessExceptions(ByVal msg As Message) As Boolean

        Dim securityData As Element = msg.GetElement(SECURITY_DATA)
        Dim _field_exceptions As Element = securityData.GetElement(FIELD_EXCEPTIONS)

        If _field_exceptions.NumValues > 0 Then
            Dim element As Element = _field_exceptions.GetValueAsElement(0)
            Dim _field_id As Element = element.GetElement(FIELD_ID)
            Dim _error_info As Element = element.GetElement(ERROR_INFO)
            Dim _error_message As Element = _error_info.GetElement(MESSAGE)
            System.Console.WriteLine(_field_id)
            System.Console.WriteLine(_error_message)
            Return True
        End If
        Return False
    End Function

    Private Sub printErrorInfo(ByVal leadingStr As String, ByVal errorInfo As Element)

        System.Console.WriteLine(leadingStr + _
        errorInfo.GetElementAsString(CATEGORY) + " (" + _
        errorInfo.GetElementAsString(MESSAGE) + ")")

    End Sub


    Private Sub ProcessFields(ByVal msg As Message)

        Dim delimiter As String = ControlChars.Tab + ControlChars.Tab
        'Print out the date column header
        System.Console.Write("DATE" + delimiter)

        'Print out the field column headers
        For k As Integer = 0 To d_fields.Count - 1
            System.Console.Write(d_fields(k).ToString() + delimiter)
        Next
        System.Console.WriteLine("")
        System.Console.WriteLine("")

        Dim securityData As Element = msg.GetElement(SECURITY_DATA)
        Dim fieldData As Element = securityData.GetElement(FIELD_DATA)

        'Iterate through all field values returned in the message
        If (fieldData.NumValues > 0) Then

            For j As Integer = 0 To fieldData.NumValues - 1

                Dim element As Element = fieldData.GetValueAsElement(j)
                'Print out the date
                Dim theDate As Datetime = element.GetElementAsDatetime(_DATE)
                System.Console.Write(theDate.ToString() + ControlChars.Tab)

                'Check for the presence of all the fields requested
                For k As Integer = 0 To d_fields.Count - 1

                    Dim _temp_field_str As String = d_fields(k).ToString()
                    If (element.HasElement(_temp_field_str)) Then

                        Dim temp_field As Element = element.GetElement(_temp_field_str)
                        Dim TEMP_FIELD_STR As Name = New Name(_temp_field_str)

                        Dim datatype As Integer = temp_field.Datatype.GetHashCode()
                        ' Extract the value dependent on the dataype and print to the console
                        Select Case (datatype)

                            Case Data.BOOL 'Bool

                                Dim field1 As Boolean
                                field1 = element.GetElementAsBool(TEMP_FIELD_STR)
                                System.Console.Write(field1 + delimiter)

                            Case Data.CHAR 'Char

                                Dim field1 As Char
                                field1 = element.GetElementAsChar(TEMP_FIELD_STR)
                                System.Console.Write(field1 + delimiter)

                            Case Data.INT32 'Int32

                                Dim field1 As Int32
                                field1 = element.GetElementAsInt32(TEMP_FIELD_STR)
                                System.Console.Write(field1 + delimiter)

                            Case Data.INT64 'Int64

                                Dim field1 As Int64
                                field1 = element.GetElementAsInt64(TEMP_FIELD_STR)
                                System.Console.Write(field1 + delimiter)

                            Case Data.FLOAT32 'Float32

                                Dim field1 As Double
                                field1 = element.GetElementAsFloat32(TEMP_FIELD_STR)
                                System.Console.Write(field1 + delimiter)

                            Case Data.FLOAT64 'Float64

                                Dim field1 As Double
                                field1 = element.GetElementAsFloat64(TEMP_FIELD_STR)
                                System.Console.Write(field1.ToString() + delimiter)

                            Case Data.STRING 'String

                                Dim field1 As String
                                field1 = element.GetElementAsString(TEMP_FIELD_STR)
                                System.Console.Write(field1 + delimiter)

                            Case Data.DATE 'Date

                                Dim field1 As Datetime
                                field1 = element.GetElementAsDatetime(TEMP_FIELD_STR)
                                System.Console.Write(theDate.DayOfMonth.ToString())

                            Case Data.TIME 'Time

                                Dim field1 As Datetime
                                field1 = element.GetElementAsDatetime(TEMP_FIELD_STR)
                                System.Console.Write(theDate.ToString())

                            Case Data.DATETIME 'Datetime

                                Dim field1 As Datetime
                                field1 = element.GetElementAsDatetime(TEMP_FIELD_STR)
                                System.Console.Write(theDate.ToString())

                            Case Else

                                Dim field1 As String
                                field1 = element.GetElementAsString(TEMP_FIELD_STR)
                                System.Console.Write(field1 + delimiter)

                        End Select
                    End If
                Next
                System.Console.WriteLine("")
            Next
        End If
    End Sub

End Module