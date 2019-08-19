'--------------------------------------------------------------------------
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A  PARTICULAR PURPOSE.'
' --------------------------------------------------------------------------

Imports Bloomberglp.Blpapi
Imports System
Imports System.Collections.Generic
Imports System.Text

Namespace Bloomberglp.Blpapi.Examples
    Module CorrelationExample
        
        Private d_session As Session
        Private d_sessionOptions As SessionOptions
        Private d_refDataService As Service
        Private d_secInfoWindow As Window

        Sub Main()

            ' Demonstrates use of CorrelationID. 
            d_sessionOptions = New SessionOptions()
            d_sessionOptions.ServerHost = "localhost"
            d_sessionOptions.ServerPort = 8194
            d_secInfoWindow = New Window("SecurityInfo")

            If Not createSession() Then
                Return
            End If

            Dim request As Request = d_refDataService.CreateRequest("ReferenceDataRequest")

            request.GetElement("securities").AppendValue("IBM US Equity")
            request.GetElement("fields").AppendValue("PX_LAST")
            request.GetElement("fields").AppendValue("DS002")

            d_session.SendRequest(request, New CorrelationID(d_secInfoWindow))

            While True
                Dim eventObj As [Event] = d_session.NextEvent()
                For Each msg As Message In eventObj
                    If eventObj.Type = [Event].EventType.RESPONSE OrElse eventObj.Type = _
                        [Event].EventType.PARTIAL_RESPONSE Then
                        DirectCast((msg.CorrelationID.[Object]) _
                            , Window).displaySecurityInfo(msg)
                    End If
                Next
                If eventObj.Type = [Event].EventType.RESPONSE Then
                    ' received final response 
                    Exit While
                End If
            End While

            System.Console.WriteLine("Press ENTER to quit")
            System.Console.Read()

        End Sub

        Public Class Window
            'A helper class to simulate a GUI window. 
            Private d_name As String

            Public Sub New(ByVal name As String)
                d_name = name
            End Sub

            Public Sub displaySecurityInfo(ByVal msg As Message)
                System.Console.WriteLine(d_name + ": " + msg.AsElement.ToString())
            End Sub
        End Class

        Private Function createSession() As Boolean

            System.Console.WriteLine("Connecting to " + d_sessionOptions.ServerHost + _
            ":" + d_sessionOptions.ServerPort.ToString())
            d_session = New Session(d_sessionOptions)

            If Not d_session.Start() Then
                System.Console.WriteLine("Failed to connect to ServerApi!")
                Return False
            End If

            If Not d_session.OpenService("//blp/refdata") Then
                System.Console.WriteLine("Failed to open //blp/refdata")
                d_session.Stop()
                d_session = Nothing
                Return False
            End If

            d_refDataService = d_session.GetService("//blp/refdata")
            Return True

        End Function

    End Module

End Namespace
