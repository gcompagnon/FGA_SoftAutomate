'--------------------------------------------------------------------------
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.'
' --------------------------------------------------------------------------
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Bloomberglp.Blpapi

Namespace Bloomberglp.Blpapi.Examples
    Module SubscriptionCorrelationExample

        Private d_session As Session
        Private d_sessionOptions As SessionOptions
        Private d_securityList As List(Of String)
        Private d_gridWindow As GridWindow

        Public Class GridWindow

            Private d_name As String
            Private d_securityList As List(Of String)

            Public Sub New(ByVal name As String, ByVal securityList As List(Of String))

                d_name = name
                d_securityList = securityList

            End Sub

            Public Sub processSecurityUpdate(ByVal msg As Message, ByVal row As Long)

                System.Console.WriteLine(d_name + ": row " + row.ToString() + _
                " got update for " + d_securityList(CInt(row)))

            End Sub

        End Class

        Sub Main()

            d_sessionOptions = New SessionOptions()
            d_sessionOptions.ServerHost = "localhost"
            d_sessionOptions.ServerPort = 8194
            d_securityList = New List(Of String)()
            d_securityList.Add("IBM US Equity")
            d_securityList.Add("VOD LN Equity")
            d_gridWindow = New GridWindow("SecurityInfo", d_securityList)

            If Not createSession() Then
                Return
            End If

            Dim subscriptionList As New List(Of Subscription)()

            For i As Integer = 0 To d_securityList.Count - 1
                subscriptionList.Add(New Subscription(d_securityList(i), _
                "LAST_PRICE", New CorrelationID(i)))
            Next

            d_session.Subscribe(subscriptionList)

            While True

                Dim eventObj As [Event] = d_session.NextEvent()
                For Each msg As Message In eventObj
                    If eventObj.Type = [Event].EventType.SUBSCRIPTION_DATA Then
                        Dim row As Long = msg.CorrelationID.Value
                        d_gridWindow.processSecurityUpdate(msg, row)
                    End If
                Next

            End While

        End Sub

        Private Function createSession() As Boolean

            System.Console.WriteLine("Connecting to " + d_sessionOptions.ServerHost + _
            ":" + d_sessionOptions.ServerPort.ToString())
            d_session = New Session(d_sessionOptions)

            If Not d_session.Start() Then
                System.Console.Error.WriteLine("Failed to connect to ServerApi!")
                Return False
            End If

            If Not d_session.OpenService("//blp/mktdata") Then
                System.Console.Error.WriteLine("Failed to open //blp/mktdata")
                Return False
            End If

            Return True

        End Function

    End Module
End Namespace