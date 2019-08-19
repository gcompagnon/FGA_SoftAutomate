' --------------------------------------------------------------------------
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A  PARTICULAR PURPOSE.'
' --------------------------------------------------------------------------

Imports Bloomberglp.Blpapi

Namespace Bloomberglp.Blpapi.Examples
    Module SimpleSubscriptionExample

        Sub Main()

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

            If Not Session.OpenService("//blp/mktdata") Then
                System.Console.Error.WriteLine("Failed to open //blp/mktdata")
                Return
            End If

            Dim security1 As String = "IBM US Equity"
            Dim security2 As String = "/cusip/912828GM6@BGN"
            Dim subscriptions As System.Collections.Generic.List(Of Subscription) = _
                New System.Collections.Generic.List(Of Subscription)()

            subscriptions.Add(New Subscription(security1, "LAST_PRICE,BID,ASK", "", _
                New CorrelationID(security1)))
            subscriptions.Add(New Subscription(security2, "LAST_PRICE,BID,ASK,BID_YIELD,ASK_YIELD", _
                "", New CorrelationID(security2)))
            Session.Subscribe(subscriptions)

            While True
                Dim eventObj As [Event] = Session.NextEvent()
                For Each msg As Message In eventObj
                    System.Console.WriteLine(msg.AsElement.ToString())
                Next
                If eventObj.Type = [Event].EventType.RESPONSE Then
                    Exit While
                End If
            End While

        End Sub

    End Module
End Namespace
