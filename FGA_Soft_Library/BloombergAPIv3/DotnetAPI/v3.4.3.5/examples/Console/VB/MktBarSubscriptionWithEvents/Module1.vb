' ==========================================================
'  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT 
'   WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED,    
'  INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES   
'  OF MERCHANTABILITY AND/OR FITNESS FOR A  PARTICULAR   
'  PURPOSE.								
'  ==========================================================

' *****************************************************************************
' Module.vb: 
'	This program demonstrates how to make a subscription to particular security/
'	ticker to get realtime streaming updates at specified interval using 
'	"interval" options available. It uses the Market Bar service(//blp/mktbar) 
'	provided by API. Program does the following:
'		1. Establishes a session which facilitates connection to the bloomberg 
'		   network.
'		2. Initiates the Market Bar Service(//blp/mktbar) for realtime
'		   data.
''		3. Creates and sends the request via the session.
'			- Creates a subscription list
'			- Adds securities, fields and options to subscription list
'			  Option specifies the interval duration for market bars, the start and end times.
'			- Subscribes to realtime market bars
'		4. Event Handling of the responses received.
'        5. Parsing of the message data.
' Usage: 
'    MktBarSubscriptionWithEvents -h 
'Print the usage for the program on the console
'
'	SimpleSubscriptionIntervalExample
'	   If you run the program with default values, program prints the streaming 
'	   updates on the console for two default securities specfied
'	   1. Ticker - "//blp/mktbar/ticker/IBM US Equity"
'	   2. Ticker - "//blp/mktbar/ticker/VOD LN Equity"
'	   for field LAST_PRICE, interval=5, start_time=<local time + 2 minutes>, 
'                               end_time=<local_time+32 minutes>
'
'   example usage:
'	SimpleSubscriptionIntervalExample
'	SimpleSubscriptionIntervalExample -ip localhost -p 8194
'	SimpleSubscriptionIntervalExample -p 8194 -s "//blp/mktbar/ticker/VOD LN Equity" 
'                                       -s "//blp/mktbar/ticker/IBM US Equity"
'									    -f "LAST_PRICE" -o "interval=5.0"
'                                       -o "start_time=15:00" -o "end_time=15:30"
'
'	Prints the response on the console of the command line requested data

'******************************************************************************/

Imports Bloomberglp.Blpapi
Imports ArrayList = System.Collections.ArrayList
Imports Thread = System.Threading.Thread
Imports System.Collections.Generic

Module Module1

    Private ReadOnly _TIME As New Name("TIME")
    Private ReadOnly _OPEN As New Name("OPEN")
    Private ReadOnly _HIGH As New Name("HIGH")
    Private ReadOnly _LOW As New Name("LOW")
    Private ReadOnly _CLOSE As New Name("CLOSE")
    Private ReadOnly _NUMBER_OF_TICKS As New Name("NUMBER_OF_TICKS")
    Private ReadOnly _VOLUME As New Name("VOLUME")

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

        If Not d_session.OpenService("//blp/mktbar") Then
            System.Console.Error.WriteLine("Failed to open service //blp/mktbar")
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
    '*****************************************************************************
    '   Function    : createSession
    '  Description : This function creates a session object and opens the market 
    '                 bar service.  Returns false on failure of either.
    'Arguments: none()
    'Returns: bool()
    '*****************************************************************************/

    Private Function createSession() As Boolean

        If d_session IsNot Nothing Then
            d_session.Stop()
        End If

        System.Console.WriteLine("Connecting to " + d_host + ":" + d_port.ToString())
        d_session = New Session(d_sessionOptions, New EventHandler(AddressOf processEvent))
        Return d_session.Start()
    End Function

    '*****************************************************************************
    'Function    : processEvent
    'Description : Processes session events
    'Arguments   : Event, Session
    'Returns     : void
    '*****************************************************************************/

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
    '*****************************************************************************
    'Function    : processSubscriptionStatus
    'Description : Processes subscription status messages returned from Bloomberg
    'Arguments   : Event, Session
    'Returns     : void
    '*****************************************************************************/

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

    '*****************************************************************************
    'Function    : processSubscriptionDataEvent
    'Description : Processes all field data returned from Bloomberg
    'Arguments   : Event, Session
    'Returns     : void
    '*****************************************************************************/

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
                CheckAspectFields(msg)
            Next
        Next

    End Sub

    '*****************************************************************************
    'Function    : processMiscEvents
    'Description : Processes any message returned from Bloomberg
    'Arguments   : Event, Session
    'Returns     : void
    '*****************************************************************************/

    Private Sub processMiscEvents(ByVal eventObj As [Event], ByVal session As Session)

        System.Console.WriteLine("Processing " + eventObj.Type.ToString())

        For Each msg As Message In eventObj
            System.Console.WriteLine(System.DateTime.Now.ToString("s") + ": " + _
            msg.MessageType.ToString() + vbLf)
        Next

    End Sub

    '*****************************************************************************
    'Function    : parseCommandLine
    'Description : This function parses input arguments and/or sets default arguments
    '               Only returns false on -h.
    'Arguments   : string array
    'Returns: bool()
    '*****************************************************************************/
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
                d_port = Integer.Parse(args(i + 1))
            End If
            If String.Compare(args(i), "-h", True) = 0 Then
                printUsage()
                Return False
            End If
        Next

        If d_fields.Count = 0 Then
            d_fields.Add("LAST_PRICE")
        End If

        If d_securities.Count = 0 Then
            d_securities.Add("//blp/mktbar/ticker/IBM US Equity")
            d_securities.Add("//blp/mktbar/ticker/VOD LN Equity")
        End If

        If d_options.Count = 0 Then
            Static start_time_str As String
            Static end_time_str As String
            start_time_str = "start_time=" + System.DateTime.Now.AddMinutes(2).ToString("HH:mm")
            end_time_str = "end_time=" + System.DateTime.Now.AddMinutes(32).ToString("HH:mm")

            d_options.Add("interval=5")
            'd_options.Add(start_time_str)
            'd_options.Add(end_time_str)
        End If

        For Each security As String In d_securities
            d_subscriptions.Add(New Subscription(security, d_fields, d_options, _
            New CorrelationID(security)))
        Next

        Return True

    End Function

    '/*****************************************************************************
    'Function    : CheckAspectFields
    'Description : Processes any field that can be contained within the market
    '               bar message.
    'Arguments: Message()
    'Returns: void()
    '*****************************************************************************/

    Private Sub CheckAspectFields(ByVal msg As Message)
        ' extract data for each specific element
        ' it's anticipated that an application will require this data
        ' in the correct format.  this is retrieved for demonstration
        ' but is not used later in the code.
        If (msg.AsElement.HasElement(_TIME)) Then

            Dim time As Datetime = msg.GetElementAsDatetime(_TIME)
            Dim time_str As String = msg.GetElementAsString(_TIME)
            System.Console.WriteLine("Time : " + time_str + "\n")
        End If
        If (msg.AsElement.HasElement(_OPEN)) Then

            Dim open As Integer = msg.GetElementAsInt32(_OPEN)
            Dim open_str As String = msg.GetElementAsString(_OPEN)
            System.Console.WriteLine("Open : " + open_str + "\n")
        End If
        If (msg.AsElement.HasElement(_HIGH)) Then

            Dim high As Integer = msg.GetElementAsInt32(_HIGH)
            Dim high_str As String = msg.GetElementAsString(_HIGH)
            System.Console.WriteLine("High : " + high_str + "\n")
        End If
        If (msg.AsElement.HasElement(_LOW)) Then

            Dim low As Integer = msg.GetElementAsInt32(_LOW)
            Dim low_str As String = msg.GetElementAsString(_LOW)
            System.Console.WriteLine("Low : " + low_str + "\n")
        End If
        If (msg.AsElement.HasElement(_CLOSE)) Then

            Dim close As Integer = msg.GetElementAsInt32(close)
            Dim close_str As String = msg.GetElementAsString(close)
            System.Console.WriteLine("Close : " + close_str + "\n")
        End If
        If (msg.AsElement.HasElement(_NUMBER_OF_TICKS)) Then

            Dim number_of_ticks As Integer = msg.GetElementAsInt32(number_of_ticks)
            Dim number_of_ticks_str As String = msg.GetElementAsString(number_of_ticks)
            System.Console.WriteLine("Number of Ticks : " + number_of_ticks_str + "\n")
        End If
        If (msg.AsElement.HasElement(_VOLUME)) Then

            Dim volume As Single = msg.GetElementAsInt64(volume)
            Dim volume_str As String = msg.GetElementAsString(volume)
            System.Console.WriteLine("Volume : " + volume_str + "\n")
        End If
        System.Console.WriteLine("\n")

    End Sub
    '*****************************************************************************
    'Function    : printUsage
    'Description : This function prints instructions for use to the console
    'Arguments: none()
    'Returns: void()
    '*****************************************************************************/

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