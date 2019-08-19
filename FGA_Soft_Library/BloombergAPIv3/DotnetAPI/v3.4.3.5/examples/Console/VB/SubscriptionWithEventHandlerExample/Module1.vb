'--------------------------------------------------------------------------
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A  PARTICULAR PURPOSE.'
' --------------------------------------------------------------------------

Imports Bloomberglp.Blpapi
Imports ArrayList = System.Collections.ArrayList
Imports Thread = System.Threading.Thread
Imports System.Collections.Generic

Namespace Bloomberglp.Blpapi.Examples
    Module SubscriptionWithEventHandlerExample

        Private ReadOnly EXCEPTIONS As New Name("exceptions")
        Private ReadOnly FIELD_ID As New Name("fieldId")
        Private ReadOnly REASON As New Name("reason")
        Private ReadOnly CATEGORY As New Name("category")
        Private ReadOnly DESCRIPTION As New Name("description")

        Private d_host As String
        Private d_port As Integer
        Private d_sessionOptions As SessionOptions
        Private d_session As Session
        Private d_securities As List(Of String)
        Private d_fields As List(Of String)
        Private d_options As List(Of String)
        Private d_subscriptions As List(Of Subscription)

        Sub Main(ByVal args As String())

            System.Console.WriteLine("Realtime Event Handler Example")

            d_host = "localhost"
            d_port = 8194
            d_sessionOptions = New SessionOptions()
            d_securities = New List(Of String)()
            d_fields = New List(Of String)()
            d_options = New List(Of String)()
            d_subscriptions = New List(Of Subscription)()

            If Not parseCommandLine(args) Then
                Return
            End If

            d_sessionOptions.ServerHost = d_host
            d_sessionOptions.ServerPort = d_port

            Dim success As Boolean = createSession()

            If Not success Then
                Return
            End If

            System.Console.WriteLine("Connected successfully" & vbLf)

            If Not d_session.OpenService("//blp/mktdata") Then
                System.Console.Error.WriteLine("Failed to open service //blp/mktdata")
                d_session.Stop()
                Return
            End If

            System.Console.WriteLine("Subscribing..." & vbLf)
            d_session.Subscribe(d_subscriptions)

            ' wait for enter key to exit application 
            System.Console.Read()

            d_session.Stop()
            System.Console.WriteLine("Exiting.")

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
                        processMiscEvents(eventObj, session)
                        Exit Select
                End Select
            Catch e As System.Exception
                System.Console.WriteLine(e.ToString())
            End Try

        End Sub

        Private Sub processSubscriptionStatus(ByVal eventObj As [Event], ByVal session As Session)

            System.Console.WriteLine("Processing SUBSCRIPTION_STATUS")

            For Each msg As Message In eventObj

                Dim topic As String = DirectCast(msg.CorrelationID.[Object], String)
                System.Console.WriteLine(System.DateTime.Now.ToString("s") + ": " + _
                topic.ToString() + " - " + msg.MessageType.ToString())
                If msg.HasElement(REASON) Then
                    ' This can occur on SubscriptionFailure. 
                    Dim reason__1 As Element = msg.GetElement(REASON)
                    System.Console.WriteLine _
                    (vbTab + reason__1.GetElement(CATEGORY).GetValueAsString() + _
                    ": " + reason__1.GetElement(DESCRIPTION).GetValueAsString())
                End If
                If msg.HasElement(EXCEPTIONS) Then
                    ' This can occur on SubscriptionStarted if at least 
                    ' one field is good while the rest are bad. 
                    Dim exceptions__2 As Element = msg.GetElement(EXCEPTIONS)
                    For i As Integer = 0 To exceptions__2.NumValues - 1
                        Dim exInfo As Element = exceptions__2.GetValueAsElement(i)
                        Dim fieldId As Element = exInfo.GetElement(FIELD_ID)
                        Dim reason__1 As Element = exInfo.GetElement(REASON)
                        System.Console.WriteLine(vbTab + fieldId.GetValueAsString() + _
                            ": " + reason__1.GetElement(CATEGORY).GetValueAsString())
                    Next
                End If
                System.Console.WriteLine("")
            Next

        End Sub

        Private Sub processSubscriptionDataEvent(ByVal eventObj As [Event], ByVal session As Session)

            System.Console.WriteLine("Processing SUBSCRIPTION_DATA")

            For Each msg As Message In eventObj
                Dim topic As String = DirectCast(msg.CorrelationID.Object, String)
                System.Console.WriteLine(System.DateTime.Now.ToString("s") + ": " + _
                topic + " - " + msg.MessageType.ToString())

                For Each field As Element In msg.Elements
                    If field.IsNull Then
                        System.Console.WriteLine(vbTab & vbTab + field.Name.ToString() + _
                        " is NULL")
                        Continue For
                    End If

                    ' Assume all values are scalar. 
                    System.Console.WriteLine(vbTab & vbTab + field.Name.ToString() + _
                    " = " + field.GetValueAsString())
                Next
            Next

        End Sub

        Private Sub processMiscEvents(ByVal eventObj As [Event], ByVal session As Session)

            System.Console.WriteLine("Processing " + eventObj.Type.ToString())

            For Each msg As Message In eventObj
                System.Console.WriteLine(System.DateTime.Now.ToString("s") + ": " + _
                msg.MessageType.ToString() + vbLf)
            Next

        End Sub

        Private Function parseCommandLine(ByVal args As String()) As Boolean

            For i As Integer = 0 To args.Length - 1
                If String.Compare(args(i), "-s", True) = 0 Then
                    d_securities.Add(args(i + 1))
                ElseIf String.Compare(args(i), "-f", True) = 0 Then
                    d_fields.Add(args(i + 1))
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

            If d_fields.Count = 0 Then
                d_fields.Add("LAST_PRICE")
                d_fields.Add("TIME")
            End If

            If d_securities.Count = 0 Then
                d_securities.Add("IBM US Equity")
            End If

            For Each security As String In d_securities
                d_subscriptions.Add(New Subscription(security, d_fields, d_options, _
                New CorrelationID(security)))
            Next

            Return True

        End Function

        Private Sub printUsage()

            System.Console.WriteLine("Usage:")
            System.Console.WriteLine(vbTab & "Retrieve realtime data from ServerApi")
            System.Console.WriteLine(vbTab & vbTab & "[-s " & vbTab & vbTab & "<security" & vbTab & "= IBM US Equity>")
            System.Console.WriteLine(vbTab & vbTab & "[-f " & vbTab & vbTab & "<field" & vbTab & vbTab & "= LAST_PRICE>")
            System.Console.WriteLine(vbTab & vbTab & "[-o " & vbTab & vbTab & vbTab & "<subscriptionOptions>")
            System.Console.WriteLine(vbTab & vbTab & "[-ip " & vbTab & vbTab & "<ipAddress" & vbTab & "= localhost>")
            System.Console.WriteLine(vbTab & vbTab & "[-p " & vbTab & vbTab & "<tcpPort" & vbTab & "= 8194>")
            System.Console.WriteLine("Press ENTER to quit")

        End Sub

    End Module
End Namespace
