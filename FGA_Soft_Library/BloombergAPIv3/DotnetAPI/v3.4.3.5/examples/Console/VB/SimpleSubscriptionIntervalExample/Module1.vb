'---------------------------------------------------------------------------
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A  PARTICULAR PURPOSE.
' --------------------------------------------------------------------------

Imports Bloomberglp.Blpapi

Namespace Bloomberglp.Blpapi.Examples
    Module SimpleSubscriptionIntervalExample

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
                System.Console.[Error].WriteLine("Failed to start session.")
                Return
            End If

            If Not session.OpenService("//blp/mktdata") Then
                System.Console.[Error].WriteLine("Failed to open //blp/mktdata")
                Return
            End If

            Dim subscriptions As New System.Collections.Generic.List(Of Subscription)()

            ' subscribe with a 1 second interval 
            Dim security As String = "IBM US Equity"
            subscriptions.Add(New Subscription(security, _
                "LAST_PRICE,BID,ASK", "interval=1.0", New CorrelationID(security)))
            session.Subscribe(subscriptions)

            While True
                Dim eventObj As [Event] = session.NextEvent()
                For Each msg As Message In eventObj
                    If eventObj.Type = [Event].EventType.SUBSCRIPTION_DATA OrElse _
                      eventObj.Type = [Event].EventType.SUBSCRIPTION_STATUS Then
                        Dim topic As String = DirectCast(msg.CorrelationID.[Object], String)
                        System.Console.WriteLine(topic + ": " + msg.AsElement.ToString())
                    Else
                        System.Console.WriteLine(msg.AsElement)
                    End If
                Next
            End While

        End Sub

    End Module
End Namespace