'--------------------------------------------------------------------------
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A  PARTICULAR PURPOSE.'
' --------------------------------------------------------------------------

Imports Bloomberglp.Blpapi
Imports ArrayList = System.Collections.ArrayList
Imports Thread = System.Threading.Thread
Imports System.Collections.Generic

Namespace BB.Blpapi.Examples
    Module NameEnumerationExample

        Private d_host As String
        Private d_port As Integer
        Private d_sessionOptions As SessionOptions
        Private d_session As Session
        Private d_securities As List(Of String)
        Private d_options As List(Of String)
        Private d_subscriptions As List(Of Subscription)
        Private d_subscriptionDataMsgEnumTable As NameEnumerationTable
        Private d_subscriptionStatusMsgEnumTable As NameEnumerationTable
        Private Const BLP_MKTDATA_SVC As String = "//blp/mktdata"

        Sub Main(ByVal args As String())

            System.Console.WriteLine("Realtime Event Handler Example")

            d_host = "localhost"
            d_port = 8194
            d_sessionOptions = New SessionOptions()
            d_securities = New List(Of String)()
            d_options = New List(Of String)()
            d_subscriptions = New List(Of Subscription)()
            d_subscriptionDataMsgEnumTable = New NameEnumerationTable(New SubscriptionDataMsgType())
            d_subscriptionStatusMsgEnumTable = New NameEnumerationTable(New SubscriptionStatusMsgType())

            If Not parseCommandLine(args) Then
                Return
            End If

            d_sessionOptions.ServerHost = d_host
            d_sessionOptions.ServerPort = d_port

            Dim success As Boolean = createSession()

            If Not success Then
                Return
            End If

            If Not d_session.OpenService(BLP_MKTDATA_SVC) Then
                System.Console.[Error].WriteLine("Failed to open service: " + BLP_MKTDATA_SVC)
                d_session.[Stop]()
                Return
            End If

            System.Console.WriteLine("Subscribing...")
            d_session.Subscribe(d_subscriptions)
            System.Console.WriteLine("Press ENTER to quit")
            System.Console.Read()
            d_session.Stop()

        End Sub

        Public Class SubscriptionDataMsgType
            Implements Bloomberglp.Blpapi.NameEnumeration
            Public Const BID As Integer = 1
            Public Const ASK As Integer = 2
            Public Const LAST_PRICE As Integer = 3
        End Class

        Public Class SubscriptionStatusMsgType
            Implements NameEnumeration
            Public Const SUBSCRIPTION_STARTED As Integer = 1
            Public Const SUBSCRIPTION_FAILURE As Integer = 2
            Public Const SUBSCRIPTION_TERMINATED As Integer = 3

            Public Class NameBindings
                Public Const SUBSCRIPTION_STARTED As String = "SubscriptionStarted"
                Public Const SUBSCRIPTION_FAILURE As String = "SubscriptionFailure"
                Public Const SUBSCRIPTION_TERMINATED As String = "SubscriptionTerminated"
            End Class
        End Class

        Private Function parseCommandLine(ByVal args As String()) As Boolean

            For i As Integer = 0 To args.Length - 1
                If String.Compare(args(i), "-s", True) = 0 Then
                    d_securities.Add(args(i + 1))
                ElseIf String.Compare(args(i), "-o", True) = 0 Then
                    d_options.Add(args(i + 1))
                ElseIf String.Compare(args(i), "-ip", True) = 0 Then
                    d_host = args(i + 1)
                ElseIf String.Compare(args(i), "-p", True) = 0 Then
                    Dim outPort As Integer = 0
                    If Integer.TryParse(args(i + 1), outPort) Then
                        d_port = outPort
                    End If
                End If
                If String.Compare(args(i), "-h", True) = 0 Then
                    printUsage()
                    Return False
                End If
            Next

            If d_securities.Count = 0 Then
                d_securities.Add("IBM US Equity")
            End If

            For Each security As String In d_securities
                d_subscriptions.Add(New Subscription(security, "BID,ASK,LAST_PRICE", "", _
                New CorrelationID(security)))
            Next

            Return True

        End Function

        Private Sub printUsage()

            System.Console.WriteLine("Usage:")
            System.Console.WriteLine(vbTab & "Retrieve realtime data from ServerApi")
            System.Console.WriteLine(vbTab & vbTab & "[-s <security= IBM US Equity>")
            System.Console.WriteLine(vbTab & vbTab & "[-ip " & vbTab & vbTab & "<ipAddress" & vbTab & "= localhost>")
            System.Console.WriteLine(vbTab & vbTab & "[-p " & vbTab & vbTab & "<tcpPort" & vbTab & "= 8194>")
            System.Console.WriteLine("Press ENTER to quit")

        End Sub

        Private Function createSession() As Boolean

            If d_session IsNot Nothing Then
                d_session.Stop()
            End If

            System.Console.WriteLine("Connecting to " + d_host + ":" + d_port.ToString())
            d_session = New Session(d_sessionOptions, New EventHandler(AddressOf processEvent))
            Return d_session.Start()

        End Function

        Public Sub processEvent(ByVal eventObj As [Event], ByVal session As Session)

            Try
                Select Case eventObj.Type
                    Case [Event].EventType.SUBSCRIPTION_DATA
                        processSubscriptionDataEvent(eventObj, session)
                        Exit Select
                    Case [Event].EventType.SUBSCRIPTION_STATUS
                        processSubscriptionStatus(eventObj, session)
                        Exit Select
                    Case Else
                        System.Console.WriteLine("Processing event: " + _
                        eventObj.Type.ToString())
                        printEvent(eventObj, session)
                        Exit Select
                End Select
            Catch e As System.Exception
                System.Console.WriteLine(e.ToString())
            End Try

        End Sub

        Private Sub processSubscriptionDataEvent(ByVal eventObj As [Event], ByVal session As Session)

            System.Console.WriteLine("Processing SUBSCRIPTION_DATA")

            For Each msg As Message In eventObj
                Dim topic As String = DirectCast(msg.CorrelationID.[Object], String)
                For Each field As Element In msg.Elements
                    Select Case d_subscriptionDataMsgEnumTable(field.Name)
                        Case SubscriptionDataMsgType.BID, SubscriptionDataMsgType.ASK, _
                            SubscriptionDataMsgType.LAST_PRICE
                            If True Then
                                System.Console.WriteLine _
                                (System.DateTime.Now.ToString("s") + ": " + _
                                topic + " " + field.Name.ToString() + " " + _
                                field.GetValueAsString())
                            End If
                            Exit Select
                    End Select
                Next
            Next
        End Sub

        Private Sub processSubscriptionStatus(ByVal eventObj As [Event], ByVal session As Session)

            System.Console.Out.WriteLine("Processing SUBSCRIPTION_STATUS")

            For Each msg As Message In eventObj
                Dim topic As String = DirectCast(msg.CorrelationID.[Object], String)
                Select Case d_subscriptionStatusMsgEnumTable(msg.MessageType)
                    Case SubscriptionStatusMsgType.SUBSCRIPTION_STARTED
                        If True Then
                            System.Console.Out.WriteLine("Subscription for: " + topic + " started")
                        End If
                        Exit Select

                    Case SubscriptionStatusMsgType.SUBSCRIPTION_FAILURE
                        If True Then
                            System.Console.Out.WriteLine("Subscription for: " + topic + _
                            " failed")
                            printEvent(eventObj, session)
                        End If
                        Exit Select

                    Case SubscriptionStatusMsgType.SUBSCRIPTION_TERMINATED
                        If True Then
                            System.Console.Out.WriteLine("Subscription for: " + topic + _
                            " has been terminated")
                            printEvent(eventObj, session)
                        End If
                        Exit Select

                    Case Else
                        System.Console.Out.WriteLine("Unhandled subscription status: " + _
                        msg.MessageType.ToString())
                        Exit Select
                End Select
            Next

        End Sub

        Private Sub printEvent(ByVal eventObj As [Event], ByVal session As Session)

            System.Console.WriteLine("Processing " + eventObj.Type.ToString())
            For Each msg As Message In eventObj
                System.Console.WriteLine(msg)
            Next

        End Sub

    End Module
End Namespace
