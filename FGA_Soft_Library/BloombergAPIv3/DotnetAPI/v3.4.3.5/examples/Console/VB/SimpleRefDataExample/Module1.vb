'---------------------------------------------------------------------------
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A  PARTICULAR PURPOSE.
' --------------------------------------------------------------------------

Imports Bloomberglp.Blpapi

Namespace Bloomberglp.Blpapi.Examples
    Module SimpleRefDataExample

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

            If Not Session.OpenService("//blp/refdata") Then
                System.Console.Error.WriteLine("Failed to open //blp/refdata")
                Return
            End If

            Dim refDataService As Service = Session.GetService("//blp/refdata")
            Dim req As Request = refDataService.CreateRequest("ReferenceDataRequest")
            Dim securities As Element = req.GetElement("securities")

            securities.AppendValue("IBM US Equity")
            securities.AppendValue("/cusip/912828GM6@BGN")

            Dim fields As Element = req.GetElement("fields")

            fields.AppendValue("PX_LAST")
            fields.AppendValue("DS002")
            System.Console.WriteLine("Sending Request: " + req.AsElement.ToString())
            Session.SendRequest(req, Nothing)

            While True
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

    End Module
End Namespace
