'---------------------------------------------------------------------------
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A  PARTICULAR PURPOSE.
' --------------------------------------------------------------------------
Imports Bloomberglp.Blpapi
Imports Bloomberglp.Blpapi.Datetime
Imports System.Collections.ArrayList

Namespace BB.Blpapi.Examples
    Module IntradayTickExample

        Private ReadOnly TICK_DATA As New Name("tickData")
        Private ReadOnly COND_CODE As New Name("conditionCodes")
        Private ReadOnly SIZE As New Name("size")
        Private ReadOnly TIME As New Name("time")
        Private ReadOnly TYPE As New Name("type")
        Private ReadOnly VALUE As New Name("value")
        Private ReadOnly RESPONSE_ERROR As New Name("responseError")
        Private ReadOnly CATEGORY As New Name("category")
        Private ReadOnly MESSAGE As New Name("message")
        Private d_host As String
        Private d_port As Integer
        Private d_security As String
        Private d_events As ArrayList
        Private d_conditionCodes As Boolean
        Private d_startDateTime As String
        Private d_endDateTime As String

        Sub Main(ByVal args As String())

            System.Console.WriteLine("Intraday Rawticks Example")

            d_host = "localhost"
            d_port = 8194
            d_security = "IBM US Equity"
            d_events = New ArrayList()
            d_conditionCodes = False
            d_startDateTime = "2008-08-11T15:30:00"
            d_endDateTime = "2008-08-11T15:35:00"

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

            sendIntradayTickRequest(session)

            ' wait for events from session. 
            eventLoop(session)

            session.Stop()

            System.Console.WriteLine("Press ENTER to quit")
            Console.ReadLine()

        End Sub

        Private Function parseCommandLine(ByVal args As String()) As Boolean
            Dim flag As Boolean = True
            Dim dateTimeFormat As String = "yyyy-MM-ddTHH:mm:ss"

            For i As Integer = 0 To args.Length - 1
                If String.Compare(args(i), "-s", True) = 0 Then
                    d_security = args(i + 1)
                ElseIf String.Compare(args(i), "-e", True) = 0 Then
                    d_events.Add(args(i + 1))
                ElseIf String.Compare(args(i), "-cc", True) = 0 Then
                    d_conditionCodes = True
                ElseIf String.Compare(args(i), "-sd", True) = 0 Then
                    d_startDateTime = args(i + 1)
                    If Not isDateTimeValid(d_startDateTime, dateTimeFormat) Then
                        flag = False
                        System.Console.WriteLine("Invalid start date/time: " + d_startDateTime + ".")
                    End If
                ElseIf String.Compare(args(i), "-ed", True) = 0 Then
                    d_endDateTime = args(i + 1)
                    If Not isDateTimeValid(d_endDateTime, dateTimeFormat) Then
                        flag = False
                        System.Console.WriteLine("Invalid end date/time: " + d_endDateTime + ".")
                    End If
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

            If d_events.Count = 0 Then
                d_events.Add("TRADE")
            End If

            Return flag

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
            System.Console.WriteLine("  Retrieve intraday rawticks from ServerApi")
            System.Console.WriteLine("      [-s     <security   = IBM US Equity>")
            System.Console.WriteLine("      [-e     <event      = TRADE>")
            System.Console.WriteLine("      [-sd    <startDateTime  = 2007-03-26T09:30:00>")
            System.Console.WriteLine("      [-ed    <endDateTime    = 2007-03-26T10:30:00>")
            System.Console.WriteLine("      [-cc    <includeConditionCodes = false>")
            System.Console.WriteLine("      [-ip    <ipAddress  = localhost>")
            System.Console.WriteLine("      [-p     <tcpPort    = 8194>")
            System.Console.WriteLine("Notes:")
            System.Console.WriteLine("1) All times are in GMT.")
            System.Console.WriteLine("2) Only one security can be specified.")

        End Sub

        Private Sub sendIntradayTickRequest(ByVal session As Session)

            Dim refDataService As Service = session.GetService("//blp/refdata")
            Dim request As Request = refDataService.CreateRequest("IntradayTickRequest")

            request.Set("security", d_security)

            ' Add fields to request 
            Dim eventTypes As Element = request.GetElement("eventTypes")

            For i As Integer = 0 To d_events.Count - 1
                eventTypes.AppendValue(DirectCast(d_events(i), String))
            Next

            ' All times are in GMT 
            request.Set("startDateTime", d_startDateTime)
            request.Set("endDateTime", d_endDateTime)

            If d_conditionCodes Then
                request.Set("includeConditionCodes", True)
            End If

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
                processMessage(msg)
            Next

        End Sub

        Private Sub processMessage(ByVal msg As Message)

            Dim data As Element = msg.GetElement(TICK_DATA).GetElement(TICK_DATA)
            Dim numItems As Integer = data.NumValues
            System.Console.WriteLine("TIME" & vbTab & vbTab & vbTab & "TYPE" & vbTab & _
                "VALUE" & vbTab & vbTab & "SIZE" & vbTab & "CC")
            System.Console.WriteLine("----" & vbTab & vbTab & vbTab & "----" & vbTab & _
                "-----" & vbTab & vbTab & "----" & vbTab & "--")

            For i As Integer = 0 To numItems - 1
                Dim item As Element = data.GetValueAsElement(i)
                Dim time__1 As String = item.GetElementAsString(TIME)
                Dim type__2 As String = item.GetElementAsString(TYPE)
                Dim value__3 As Double = item.GetElementAsFloat64(VALUE)
                Dim size__4 As Integer = item.GetElementAsInt32(SIZE)
                Dim cc As String = ""
                If item.HasElement(COND_CODE) Then
                    cc = item.GetElementAsString(COND_CODE)
                End If
                ' Dim sysDatetime As New System.DateTime(time__1.[Year], time__1.Month, time__1.DayOfMonth, time__1.Hour, time__1.Minute, time__1.Second, _
                'time__1.MilliSecond)
                System.Console.WriteLine(time__1 + vbTab + type__2 + vbTab + value__3.ToString("C") + vbTab & vbTab + size__4.ToString() + vbTab + cc)
            Next
        End Sub

        Private Sub printErrorInfo(ByVal leadingStr As String, ByVal errorInfo As Element)

            System.Console.WriteLine(leadingStr + _
            errorInfo.GetElementAsString(CATEGORY) + " (" + _
            errorInfo.GetElementAsString(MESSAGE) + ")")

        End Sub

        Private Function getPreviousTradingDate() As System.DateTime

            Dim tradedOn As System.DateTime = System.DateTime.Now

            tradedOn = tradedOn.AddDays(-1)

            If tradedOn.DayOfWeek = DayOfWeek.Sunday Then
                tradedOn = tradedOn.AddDays(-2)
            ElseIf tradedOn.DayOfWeek = DayOfWeek.Saturday Then
                tradedOn = tradedOn.AddDays(-1)
            End If

            Return tradedOn

        End Function


    End Module
End Namespace
