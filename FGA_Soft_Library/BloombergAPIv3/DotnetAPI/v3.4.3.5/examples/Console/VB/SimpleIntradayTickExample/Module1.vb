'--------------------------------------------------------------------------
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A  PARTICULAR PURPOSE.'
' --------------------------------------------------------------------------

Imports Bloomberglp.Blpapi

Namespace Bloomberglp.Blpapi.Examples
    Module SimpleIntradayTickExample

        Sub Main(ByVal args As String())

            Dim serverHost As String = "localhost"
            Dim serverPort As Int32 = 8194
            Dim sessionOptions As SessionOptions = New SessionOptions()
            sessionOptions.ServerHost = serverHost
            sessionOptions.ServerPort = serverPort
            System.Console.WriteLine("Connecting to " + serverHost.ToString() + ":" + _
            serverPort.ToString())
            Dim Session As Session = New Session(sessionOptions)
            Dim sessionStarted As Boolean = Session.Start()

            If Not sessionStarted Then
                System.Console.WriteLine("Failed to start session.")
                Return
            End If

            If Not Session.OpenService("//blp/refdata") Then
                System.Console.Error.WriteLine("Failed to open //blp/refdata")
                Return
            End If

            Dim refDataService As Service = Session.GetService("//blp/refdata")
            Dim req As Request = refDataService.CreateRequest("IntradayTickRequest")

            req.Set("security", "VOD LN Equity")
            req("eventTypes").AppendValue("TRADE")
            req("eventTypes").AppendValue("AT_TRADE")
            
            Dim tradedOn As System.DateTime = getPreviousTradingDate()
            'times are GMT time
            req.Set("startDateTime", New Datetime _
            (tradedOn.Year, tradedOn.Month, tradedOn.Day, 10, 30, 0, _
                0))
            req.Set("endDateTime", New Datetime _
            (tradedOn.Year, tradedOn.Month, tradedOn.Day, 10, 35, 0, _
                0))
            req.Set("includeConditionCodes", True)
            System.Console.WriteLine("Sending Request: " + req.AsElement.ToString())
            Session.SendRequest(req, Nothing)

            While (True)
                Dim eventObj As [Event] = Session.NextEvent()
                For Each msg As Message In eventObj
                    System.Console.WriteLine(msg.AsElement)
                    If eventObj.Type = [Event].EventType.RESPONSE Then
                        Exit While
                    End If
                Next
            End While

            System.Console.WriteLine("Press ENTER to quit")
            Console.ReadLine()

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
