'*********************************************************
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT *
'* WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED,    *
'* INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES   *
'* OF MERCHANTABILITY AND/OR FITNESS FOR A  PARTICULAR   *
'* PURPOSE.                                              *
'*********************************************************
'*
'** SimpleCustomVWAPExample.cs
'** 
'**  This program demonstrate how to make subscription to particular 
'**  security/ticker to get realtime VWAP updates using Polling method. 
'**  It uses Market Data service(//blp/mktvwap) provided by API.
'**  It does following:
'**    1. Establishing a session which facilitates connection to the bloomberg 
'**       network.
'**    2. Initiating the Market VWAP Service(//blp/mktvwap) for realtime
'**       vwap data.
'**    3. Creating and sending request to the session.
'**        - Creating subscription list
'**        - Add securities and vwap fields to subscription list
'**        - Specifies VWAP Overrides option
'**        - Subscribe to realtime data
'**    4. Event Handling of the responses received.
'** Usage: 
'**  SimpleCustomVWAPExample -h
'**     Print the usage for the program on the console
'**
'**  SimpleCustomVWAPExample
'**     Run the program with default values. Prints the realtime VWAP updates 
'**     on the console for three default securities specfied
'**     1. Ticker - "IBM US Equity"
'**     2. Ticker - "6758 JT Equity" 
'**     3. Ticker - "VOD LN Equity"
'**
'**  Subscribing to Bloomberg defined VWAP and VWAP Volume
'**  SimpleCustomVWAPExample -s "AAPL US Equity" -f VWAP -f RT_VWAP_VOLUME 
'**                         -o VWAP_START_TIME=11:00
'**  
'**  Subscribing to Market defined VWAP and VWAP Volume
'**  SimpleCustomVWAPExample -s "AAPL US Equity" -f MARKET_DEFINED_VWAP_REALTIME 
'**                        -f RT_MKT_VWAP_VOLUME -o VWAP_START_TIME=11:00
'**  
'**  Subscribing to both Bloomberg defined & Market defined VWAP and VWAP Volume
'**  SimpleCustomVWAPExample -s "AAPL US Equity" -f VWAP -f RT_VWAP_VOLUME 
'**                      -f MARKET_DEFINED_VWAP_REALTIME -f RT_MKT_VWAP_VOLUME
'**
'**  Prints the response on the console of the command line requested data
'*

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Bloomberglp.Blpapi
Imports Datatype = Bloomberglp.Blpapi.Schema.Datatype

Namespace Bloomberglp.Blpapi.Examples

    Class SimpleCustomVWAPExample

        Private d_host As String
        Private d_port As Integer
        Private d_securities As List(Of String)
        Private d_fields As List(Of String)
        Private d_overrides As List(Of String)


        Shared Sub Main(ByVal args() As String)
            Dim example As SimpleCustomVWAPExample = New SimpleCustomVWAPExample()
            example.run(args)
            System.Console.WriteLine("Press ENTER to quit")
            System.Console.Read()
        End Sub

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            d_host = "localhost"
            d_port = 8194
            d_securities = New List(Of String)
            d_fields = New List(Of String)
            d_overrides = New List(Of String)
        End Sub

        ''' <summary>
        ''' Read command line arguments, 
        ''' Establish a Session
        ''' Open mktvwap Service
        ''' Subscribe to securities and fields
        ''' Event Loop
        ''' </summary>
        ''' <param name="args"></param>
        ''' <remarks></remarks>
        Private Sub run(ByVal args() As String)
            If Not parseCommandLine(args) Then
                Return
            End If

            Dim sessionOptions As SessionOptions = New SessionOptions()
            sessionOptions.ServerHost = d_host
            sessionOptions.ServerPort = d_port

            System.Console.WriteLine("Connecting to " & d_host + ":" & _
                                     d_port.ToString())

            Dim Session As Session = New Session(sessionOptions)
            Dim sessionStarted As Boolean = Session.Start()
            If Not sessionStarted Then
                System.Console.Error.WriteLine("Failed to start session.")
                Return
            End If

            If Not Session.OpenService("//blp/mktvwap") Then
                System.Console.Error.WriteLine("Failed to open //blp/mktvwap")
                Return
            End If

            sessionOptions.DefaultSubscriptionService = "//blp/mktvwap"

            ' User must be enabled for real-time data for the exchanges the securities
            ' they are monitoring for custom VWAP trade. 
            ' Otherwise, Subscription will fail for those securities and/or you will 
            ' receive #N/A N/A instead of valid tick data.
            Dim subscriptions As List(Of Subscription) = New List(Of Subscription)
            Dim security As String
            For Each security In d_securities
                subscriptions.Add(New Subscription(security, _
                                                    d_fields, _
                                                    d_overrides, _
                                                    New CorrelationID(security)))
            Next

            Session.Subscribe(subscriptions)

            ' wait for events from session.
            eventLoop(Session)

        End Sub

        ''' <summary>
        ''' Polls for an event or a message in an event loop
        ''' and Processes the event generated
        ''' </summary>
        ''' <param name="session"></param>
        ''' <remarks></remarks>
        Private Sub eventLoop(ByVal session As Session)
            While (True)
                Dim eventObj As [Event] = session.NextEvent()
                Dim msg As Message
                For Each msg In eventObj
                    If eventObj.Type = [Event].EventType.SUBSCRIPTION_STATUS Then
                        System.Console.WriteLine("Processing SUBSCRIPTION_STATUS")
                        Dim topic As String = CType(msg.CorrelationID.Object, String)
                        System.Console.WriteLine(System.DateTime.Now.ToString("s") & _
                                                ": " & _
                                                topic & _
                                                ": " & _
                                                msg.AsElement.ToString())
                    ElseIf eventObj.Type = [Event].EventType.SUBSCRIPTION_DATA Then
                        System.Console.WriteLine(vbCrLf & "Processing SUBSCRIPTION_DATA")
                        Dim topic As String = CType(msg.CorrelationID.Object, String)
                        System.Console.WriteLine(System.DateTime.Now.ToString("s") & _
                                                 ": " & _
                                                topic & _
                                                " - " & _
                                                msg.MessageType.ToString())
                        Dim field As Element
                        For Each field In msg.Elements
                            System.Console.WriteLine(vbTab & vbTab & field.Name.ToString() & _
                                                    " = " & _
                                                    field.GetValueAsString())
                        Next
                    Else
                        System.Console.WriteLine(msg.AsElement)
                    End If
                Next
            End While
        End Sub


        ''' <summary>
        ''' Parses the command line arguments
        ''' </summary>
        ''' <param name="args"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function parseCommandLine(ByVal args As String()) As Boolean

            For i As Integer = 0 To args.Length - 1
                If String.Compare(args(i), "-s", True) = 0 Then
                    d_securities.Add(args(i + 1))
                ElseIf String.Compare(args(i), "-f", True) = 0 Then
                    d_fields.Add(args(i + 1))
                ElseIf String.Compare(args(i), "-o", True) = 0 Then
                    d_overrides.Add(args(i + 1))
                ElseIf String.Compare(args(i), "-ip", True) = 0 Then
                    d_host = args(i + 1)
                ElseIf String.Compare(args(i), "-p", True) = 0 Then
                    Dim outPort As Integer = 0
                    If Integer.TryParse(args(i + 1), outPort) Then
                        d_port = outPort
                    End If
                ElseIf String.Compare(args(i), "-h", True) = 0 Then
                    printUsage()
                    Return False
                End If
            Next

            ' handle default arguments 
            If d_securities.Count = 0 Then
                d_securities.Add("IBM US Equity")
                d_securities.Add("VOD LN Equity")
                d_securities.Add("6758 JT Equity")
            End If

            If d_fields.Count = 0 Then
                ' Subscribing to Bloomberg defined VWAP and VWAP Volume
                d_fields.Add("VWAP")
                d_fields.Add("RT_VWAP_VOLUME")
                ' Subscribing to Market defined VWAP and VWAP Volume
                d_fields.Add("MARKET_DEFINED_VWAP_REALTIME")
                d_fields.Add("RT_MKT_VWAP_VOLUME")
            End If

            If d_overrides.Count = 0 Then
                d_overrides.Add("VWAP_START_TIME=09:00")
            End If

            Return True
        End Function

        ''' <summary>
        ''' Print usage of the Program
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub printUsage()
            System.Console.WriteLine("Usage:")
            System.Console.WriteLine("    Retrieve customized realtime vwap data using Bloomberg V3 Api")
            System.Console.WriteLine("      [-s         <security   = ""IBM US Equity"">]")
            System.Console.WriteLine("      [-f         <field      = VWAP>]")
            System.Console.WriteLine("      [-o         <overrides  = VWAP_START_TIME=09:00>]")
            System.Console.WriteLine("      [-ip        <ipAddress  = localhost>]")
            System.Console.WriteLine("      [-p         <tcpPort    = 8194>]")
            System.Console.WriteLine("Notes:")
            System.Console.WriteLine("Multiple securities, vwap fields & overrides can be specified.")
        End Sub

    End Class
End Namespace
