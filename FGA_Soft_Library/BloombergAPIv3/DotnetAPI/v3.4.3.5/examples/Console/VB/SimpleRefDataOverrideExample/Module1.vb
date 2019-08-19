'--------------------------------------------------------------------------
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A  PARTICULAR PURPOSE.'
' --------------------------------------------------------------------------

Imports Bloomberglp.Blpapi

Module SimpleHistoryExample

    Sub Main()

        Dim serverHost As String = "localhost"
        Dim serverPort As Integer = 8194

        Dim sessionOptions As New SessionOptions()

        sessionOptions.ServerHost = serverHost
        sessionOptions.ServerPort = serverPort

        System.Console.WriteLine("Connecting to " + serverHost + ":" + serverPort.ToString())

        Dim session As New Session(sessionOptions)
        Dim sessionStarted As Boolean = session.Start()

        If Not sessionStarted Then
            System.Console.WriteLine("Failed to start session.")
            Return
        End If

        If Not session.OpenService("//blp/refdata") Then
            System.Console.Error.WriteLine("Failed to open //blp/refdata")
            Return
        End If

        Dim refDataService As Service = session.GetService("//blp/refdata")
        Dim request As Request = refDataService.CreateRequest("ReferenceDataRequest")
        Dim securities As Element = request.GetElement("securities")

        securities.AppendValue("IBM US Equity")
        securities.AppendValue("VOD LN Equity")

        Dim fields As Element = request.GetElement("fields")

        fields.AppendValue("PX_LAST")
        fields.AppendValue("DS002")
        fields.AppendValue("EQY_WEIGHTED_AVG_PX")

        ' add overrides 
        Dim [overrides] As Element = request("overrides")
        Dim override1 As Element = [overrides].AppendElement()

        override1.SetElement("fieldId", "VWAP_START_TIME")
        override1.SetElement("value", "9:30")

        Dim override2 As Element = [overrides].AppendElement()

        override2.SetElement("fieldId", "VWAP_END_TIME")
        override2.SetElement("value", "11:30")

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

End Module
