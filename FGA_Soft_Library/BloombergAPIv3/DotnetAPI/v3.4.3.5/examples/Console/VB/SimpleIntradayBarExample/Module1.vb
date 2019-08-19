'--------------------------------------------------------------------------
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A  PARTICULAR PURPOSE.'
' --------------------------------------------------------------------------

Imports Bloomberglp.Blpapi

Namespace Bloomberglp.Blpapi.Examples
    Module SimpleIntradayBarExample

        Sub Main()

            Dim serverHost As String = "localhost"
            Dim serverPort As Integer = 8194

            Dim sessionOptions As New SessionOptions()
            sessionOptions.ServerHost = serverHost
            sessionOptions.ServerPort = serverPort

            System.Console.WriteLine("Connecting to " + serverHost + ":" + _
            serverPort.ToString())

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
            Dim request As Request = refDataService.CreateRequest("IntradayBarRequest")

            request.Set("security", "IBM US Equity")
            request.Set("eventType", "TRADE")
            request.Set("interval", 60)
            ' bar interval in minutes 
            Dim tradedOn As System.DateTime = getPreviousTradingDate()
            'times are GMT time
            request.Set("startDateTime", New Datetime _
            (tradedOn.Year, tradedOn.Month, tradedOn.Day, 13, 30, 0, _
                0))
            request.Set("endDateTime", New Datetime _
            (tradedOn.Year, tradedOn.Month, tradedOn.Day, 21, 30, 0, _
                0))

            System.Console.WriteLine("Sending Request: " + request.AsElement.ToString())
            session.SendRequest(request, Nothing)

            While True
                Dim eventObj As [Event] = session.NextEvent()
                For Each msg As Message In eventObj
                    System.Console.WriteLine(msg.AsElement)
                Next
                If eventObj.Type = [Event].EventType.RESPONSE Then
                    Exit While
                End If
            End While

            System.Console.WriteLine("Press ENTER to quit")
            System.Console.Read()

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
