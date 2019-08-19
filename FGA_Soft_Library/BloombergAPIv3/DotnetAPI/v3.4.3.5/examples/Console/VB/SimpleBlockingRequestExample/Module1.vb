'--------------------------------------------------------------------------
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A  PARTICULAR PURPOSE.'
' --------------------------------------------------------------------------
Imports Bloomberglp.Blpapi
Imports System
Imports System.Collections.Generic
Imports System.Text

Namespace BB.Blpapi.Examples
    Module SimpleBlockingRequestExample

        Private LAST_PRICE As New Name("LAST_PRICE")

        Sub Main()

            Dim serverHost As String = "localhost"
            Dim serverPort As Integer = 8194

            Dim sessionOptions As New SessionOptions()

            sessionOptions.ServerHost = serverHost
            sessionOptions.ServerPort = serverPort

            System.Console.WriteLine("Connecting to " + serverHost + ":" + serverPort.ToString())

            Dim session As New Session(sessionOptions, New Bloomberglp.Blpapi.EventHandler _
            (AddressOf processEvent))

            If Not session.Start() Then
                System.Console.Error.WriteLine("Failed to start session.")
                Return
            End If

            If Not session.OpenService("//blp/mktdata") Then
                System.Console.Error.WriteLine("Failed to open //blp/mktdata")
                Return
            End If

            If Not session.OpenService("//blp/refdata") Then
                System.Console.Error.WriteLine("Failed to open //blp/refdata")
                Return
            End If

            System.Console.WriteLine("Subscribing to IBM US Equity")

            Dim s As New Subscription("IBM US Equity", "LAST_PRICE", "")
            Dim subscriptions As New List(Of Subscription)()

            subscriptions.Add(s)
            session.Subscribe(subscriptions)

            System.Console.WriteLine("Requesting reference data IBM US Equity")

            Dim refDataService As Service = session.GetService("//blp/refdata")
            Dim request As Request = refDataService.CreateRequest("ReferenceDataRequest")

            request.GetElement("securities").AppendValue("IBM US Equity")
            request.GetElement("fields").AppendValue("DS002")

            Dim eventQueue As New EventQueue()

            session.SendRequest(request, eventQueue, Nothing)

            While True
                Dim eventObj As [Event] = eventQueue.NextEvent()
                For Each msg As Message In eventObj
                    System.Console.WriteLine(msg)
                Next
                If eventObj.Type = [Event].EventType.RESPONSE Then
                    Exit While
                End If
            End While

            System.Console.Read()
            System.Console.WriteLine("Exiting")

        End Sub

        Public Sub processEvent(ByVal eventObj As [Event], ByVal session As Session)

            Try
                If eventObj.Type = [Event].EventType.SUBSCRIPTION_DATA Then
                    For Each msg As Message In eventObj
                        If msg.HasElement(LAST_PRICE) Then
                            Dim field As Element = msg.GetElement(LAST_PRICE)
                            System.Console.WriteLine(eventObj.Type.ToString() + ": " + _
                            field.Name.ToString() + " = " + field.GetValueAsString())
                        End If
                    Next
                End If
            Catch e As Exception
                System.Console.WriteLine(e.ToString())
            End Try

        End Sub

    End Module
End Namespace
