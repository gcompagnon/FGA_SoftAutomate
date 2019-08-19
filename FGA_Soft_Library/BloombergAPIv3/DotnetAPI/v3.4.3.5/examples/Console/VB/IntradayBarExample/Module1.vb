'--------------------------------------------------------------------------
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A  PARTICULAR PURPOSE.'
' --------------------------------------------------------------------------
Imports Bloomberglp.Blpapi

Namespace Bloomberglp.Blpapi.Examples
    Module IntradayBarExample
        Private ReadOnly BAR_DATA As New Name("barData")
        Private ReadOnly BAR_TICK_DATA As New Name("barTickData")
        Private ReadOnly OPEN As New Name("open")
        Private ReadOnly HIGH As New Name("high")
        Private ReadOnly LOW As New Name("low")
        Private ReadOnly CLOSE As New Name("close")
        Private ReadOnly VOLUME As New Name("volume")
        Private ReadOnly NUM_EVENTS As New Name("numEvents")
        Private ReadOnly TIME As New Name("time")
        Private ReadOnly RESPONSE_ERROR As New Name("responseError")
        Private ReadOnly CATEGORY As New Name("category")
        Private ReadOnly MESSAGE As New Name("message")
        Private d_host As String
        Private d_port As Integer
        Private d_security As String
        Private d_eventType As String
        Private d_barInterval As Integer
        Private d_gapFillInitialBar As Boolean
        Private d_startDateTime As String
        Private d_endDateTime As String

        Sub Main(ByVal args As String())

            System.Console.WriteLine("Intraday Bars Example")
            d_host = "localhost"
            d_port = 8194
            d_barInterval = 60
            d_security = "IBM US Equity"
            d_eventType = "TRADE"
            d_gapFillInitialBar = False
            d_startDateTime = "2008-08-11T13:30:00"
            d_endDateTime = "2008-08-11T21:30:00"

            If Not parseCommandLine(args) Then Return

            Dim sessionOptions As New SessionOptions()

            sessionOptions.ServerHost = d_host
            sessionOptions.ServerPort = d_port

            System.Console.WriteLine("Connecting to " + d_host + ":" + d_port.ToString())

            Dim session As New Session(sessionOptions)
            Dim sessionStarted As Boolean = session.Start()

            If Not sessionStarted Then
                System.Console.[Error].WriteLine("Failed to start session.")
                Return
            End If

            sendIntradayBarRequest(session)
            ' wait for events from session. 
            eventLoop(session)
            session.Stop()


        End Sub

        Private Function parseCommandLine(ByVal args As String()) As Boolean
            Dim flag As Boolean = True
            Dim dateTimeFormat As String = "yyyy-MM-ddTHH:mm:ss"

            For i As Integer = 0 To args.Length - 1
                If String.Compare(args(i), "-s", True) = 0 Then
                    d_security = args(i + 1)
                ElseIf String.Compare(args(i), "-e", True) = 0 Then
                    d_eventType = args(i + 1)
                ElseIf String.Compare(args(i), "-ip", True) = 0 Then
                    d_host = args(i + 1)
                ElseIf String.Compare(args(i), "-p", True) = 0 Then
                    Dim outPort As Integer = 0
                    If Integer.TryParse(args(i + 1), outPort) Then
                        d_port = outPort
                    End If
                ElseIf String.Compare(args(i), "-b", True) = 0 Then
                    d_barInterval = Integer.Parse(args(i + i))
                ElseIf String.Compare(args(i), "-g", True) = 0 Then
                    d_gapFillInitialBar = True
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
                ElseIf String.Compare(args(i), "-h", True) = 0 Then
                    printUsage()
                    Return False
                    End If
            Next

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

        Private Sub sendIntradayBarRequest(ByVal session As Session)

            session.OpenService("//blp/refdata")

            Dim refDataService As Service = session.GetService("//blp/refdata")
            Dim request As Request = refDataService.CreateRequest("IntradayBarRequest")

            ' only one security/eventType per request 
            request.Set("security", d_security)
            request.Set("eventType", d_eventType)
            request.Set("interval", d_barInterval)
            request.Set("startDateTime", d_startDateTime)
            request.Set("endDateTime", d_endDateTime)

            If d_gapFillInitialBar Then
                request.[Set]("gapFillInitialBar", d_gapFillInitialBar)
            End If

            System.Console.WriteLine("Sending Request: " + request.AsElement.ToString())
            session.SendRequest(request, Nothing)
        End Sub

        Private Sub printUsage()

            System.Console.WriteLine("Usage:")
            System.Console.WriteLine("  Retrieve intraday bars from ServerApi")
            System.Console.WriteLine("      [-s     <security  = IBM US Equity>")
            System.Console.WriteLine("      [-e     <event     = TRADE>")
            System.Console.WriteLine("      [-b     <barInterval= 60>")
            System.Console.WriteLine("      [-sd    <startDateTime  = 2007-03-26T09:30:00>")
            System.Console.WriteLine("      [-ed    <endDateTime    = 2007-03-26T10:30:00>")
            System.Console.WriteLine("      [-g     <gapFillInitialBar = false>")
            System.Console.WriteLine("      [-ip    <ipAddress  = localhost>")
            System.Console.WriteLine("      [-p     <tcpPort    = 8194>")
            System.Console.WriteLine("1) All times are in GMT.")
            System.Console.WriteLine("2) Only one security can be specified.")
            System.Console.WriteLine("3) Only one event can be specified.")

        End Sub

        Private Sub eventLoop(ByVal session As Session)

            Dim done As Boolean = False

            While Not done
                Dim eventObj As [Event] = session.NextEvent()
                If eventObj.Type = [Event].EventType.PARTIAL_RESPONSE Then
                    System.Console.WriteLine("Processing Partial Response")
                    processResponseEvent(eventObj, session)
                ElseIf eventObj.Type = [Event].EventType.RESPONSE Then
                    System.Console.WriteLine("Processing Response")
                    processResponseEvent(eventObj, session)
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

        ' return true if processing is completed, false otherwise 
        Private Sub processResponseEvent(ByVal eventObj As [Event], ByVal session As Session)

            For Each msg As Message In eventObj
                If msg.HasElement(RESPONSE_ERROR) Then
                    printErrorInfo("REQUEST FAILED: ", msg.GetElement(RESPONSE_ERROR))
                    Continue For
                End If
                processMessage(msg)
            Next

        End Sub

        Private Sub printErrorInfo(ByVal leadingStr As String, ByVal errorInfo As Element)

            System.Console.WriteLine(leadingStr + errorInfo.GetElementAsString _
            (CATEGORY) + " (" + errorInfo.GetElementAsString(MESSAGE) + ")")

        End Sub

        Private Sub processMessage(ByVal msg As Message)

            Dim data As Element = msg.GetElement(BAR_DATA).GetElement(BAR_TICK_DATA)
            Dim numBars As Integer = data.NumValues

            System.Console.WriteLine("Response contains " + numBars.ToString() + " bars")
            System.Console.WriteLine("Datetime" & vbTab & vbTab & "Open" & vbTab & vbTab & _
                "High" & vbTab & vbTab & "Low" & vbTab & vbTab & _
                "Close" + vbTab & vbTab & "NumEvents" & vbTab & "Volume")

            For i As Integer = 0 To numBars - 1
                Dim bar As Element = data.GetValueAsElement(i)
                Dim time__1 As Datetime = bar.GetElementAsDate(TIME)
                Dim open__2 As Double = bar.GetElementAsFloat64(OPEN)
                Dim high__3 As Double = bar.GetElementAsFloat64(HIGH)
                Dim low__4 As Double = bar.GetElementAsFloat64(LOW)
                Dim close__5 As Double = bar.GetElementAsFloat64(CLOSE)
                Dim numEvents As Integer = bar.GetElementAsInt32(NUM_EVENTS)
                Dim volume__6 As Long = bar.GetElementAsInt64(VOLUME)
                Dim sysDatetime As Date = time__1.ToSystemDateTime()
                System.Console.WriteLine(sysDatetime.ToString("s") + vbTab + _
                    open__2.ToString("C") + vbTab & vbTab + high__3.ToString("C") + _
                        vbTab & vbTab + low__4.ToString("C") + vbTab & vbTab + _
                        close__5.ToString("C") + vbTab & vbTab + numEvents.ToString() + _
                        vbTab & vbTab + volume__6.ToString())
            Next

            System.Console.WriteLine("Press ENTER to quit")
            Console.ReadLine()

        End Sub

    End Module
End Namespace

