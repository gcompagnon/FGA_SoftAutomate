' ==========================================================
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT 
'  WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED,    
' INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES   
' OF MERCHANTABILITY AND/OR FITNESS FOR A  PARTICULAR   
' PURPOSE.								
' ==========================================================
' Purpose of this example:
' - Make asynchronous and synchronous Intraday Tick
'   request using //blp/refdata service.
' ==========================================================
Imports [Event] = Bloomberglp.Blpapi.Event
Imports Message = Bloomberglp.Blpapi.Message
Imports Element = Bloomberglp.Blpapi.Element
Imports Name = Bloomberglp.Blpapi.Name
Imports Request = Bloomberglp.Blpapi.Request
Imports Service = Bloomberglp.Blpapi.Service
Imports Session = Bloomberglp.Blpapi.Session
Imports SessionOptions = Bloomberglp.Blpapi.SessionOptions
Imports Subscription = Bloomberglp.Blpapi.Subscription
'Imports EventHandler = Bloomberglp.Blpapi.EventHandler
Imports CorrelationID = Bloomberglp.Blpapi.CorrelationID
Imports System.IO
Namespace Bloomberglp.Blpapi.Examples
    Public Class Form1
        Private ReadOnly EXCEPTIONS As Name = New Name("exceptions")
        Private ReadOnly FIELD_ID As Name = New Name("fieldId")
        Private ReadOnly REASON As Name = New Name("reason")
        Private ReadOnly CATEGORY As Name = New Name("category")
        Private ReadOnly DESCRIPTION As Name = New Name("description")
        Private ReadOnly ERROR_CODE As Name = New Name("errorCode")
        Private ReadOnly SOURCE As Name = New Name("source")
        Private ReadOnly SECURITY_ERROR As Name = New Name("securityError")
        Private ReadOnly MESSAGE As Name = New Name("message")
        Private ReadOnly RESPONSE_ERROR As Name = New Name("responseError")
        Private ReadOnly SECURITY_DATA As Name = New Name("securityData")
        Private ReadOnly FIELD_EXCEPTIONS As Name = New Name("fieldExceptions")
        Private ReadOnly ERROR_INFO As Name = New Name("errorInfo")
        Private ReadOnly MARKET_BAR_START As Name = New Name("MarketBarStart")
        Private ReadOnly MARKET_BAR_END As Name = New Name("MarketBarEnd")
        Private ReadOnly MARKET_BAR_UPDATE = New Name("MarketBarUpdate")
        Private ReadOnly TIME As Name = New Name("TIME")
        Private ReadOnly OPEN As Name = New Name("OPEN")
        Private ReadOnly HIGH As Name = New Name("HIGH")
        Private ReadOnly LOW As Name = New Name("LOW")
        Private ReadOnly _CLOSE As Name = New Name("CLOSE")
        Private ReadOnly VOLUME As Name = New Name("VOLUME")
        Private ReadOnly NUMBER_OF_TICKS As Name = New Name("NUMBER_OF_TICKS")

        Private Const MKTBAR_SERVICE As String = "//blp/mktbar"

        Private d_sessionOptions As SessionOptions
        Private d_session As Session
        Private d_subscriptions As List(Of Subscription)
        Private d_fields As List(Of Name)
        Private d_realtimeOutputFile As TextWriter
        Private d_outputFileName As String
        Private d_isSubscribed As Boolean = False


        Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim serverHost As String = "localhost"
            Dim serverPort As Integer = 8194

            ' set bar fields
            d_fields = New List(Of Name)
            Dim fields As Name() = {TIME, OPEN, HIGH, LOW, _CLOSE, VOLUME, NUMBER_OF_TICKS}
            d_fields.AddRange(fields)

            d_subscriptions = New List(Of Subscription)

            ' set sesson options
            d_sessionOptions = New SessionOptions()
            d_sessionOptions.ServerHost = serverHost
            d_sessionOptions.ServerPort = serverPort
            ' initialize UI controls
            initUI()
        End Sub

#Region "methods"
        ''' <summary>
        ''' Initialize form controls
        ''' </summary>
        Private Sub initUI()
            dateTimePickerStartTime.Value = System.DateTime.Now.ToUniversalTime().AddMinutes(2)
            dateTimePickerEndTime.Value = System.DateTime.Now.ToUniversalTime().AddMinutes(12)
        End Sub

        ''' <summary>
        ''' Add securities to grid
        ''' </summary>
        ''' <param name="securities"></param>
        Private Sub addSecurities(ByVal securities As String)
            ' Tokenize the string into what (we hope) are Security strings
            Dim sep As Char() = {vbCrLf, vbTab, ","c}
            Dim words As String() = securities.Split(sep)
            For Each security As String In words
                If security.Trim().Length > 0 Then
                    ' add security
                    Dim groupIndex As Integer = listViewRTIBData.Groups.Count + 1
                    Dim group As ListViewGroup = Nothing
                    group = listViewRTIBData.Groups.Add(groupIndex.ToString(), security.Trim())
                    ' add 1st item to group
                    Dim listItem As ListViewItem = CreateGroupItem(group, Color.White)
                    listViewRTIBData.Items.Add(listItem)
                End If
            Next
            setControlStates()
        End Sub

        Private Function CreateGroupItem(ByVal group As ListViewGroup, ByVal backgroundColor As Color) As ListViewItem
            Dim index As Integer
            Dim listItem As ListViewItem = New ListViewItem("", group)
            For index = 0 To d_fields.Count - 1
                listItem.SubItems.Add("")
                listItem.SubItems(index).BackColor = backgroundColor
            Next index
            ' set tool tip
            listItem.ToolTipText = String.Empty
            Return listItem
        End Function

        ''' <summary>
        ''' Create session
        ''' </summary>
        ''' <returns></returns>
        Private Function createSession() As Boolean
            If d_session Is Nothing Then
                toolStripStatusLabel1.Text = "Connecting..."
                ' create new session
                d_session = New Session(d_sessionOptions, New Bloomberglp.Blpapi.EventHandler(AddressOf processEvent))
            End If
            Return d_session.Start()
        End Function

        ''' <summary>
        ''' Manage control states
        ''' </summary>
        Private Sub setControlStates()
            Dim isTimeValid As Boolean = Not (dateTimePickerStartTime.Value >= dateTimePickerEndTime.Value And _
                                            (dateTimePickerEndTime.Checked And dateTimePickerStartTime.Checked))
            buttonSendRequest.Enabled = listViewRTIBData.Groups.Count > 0 And _
                                        dateTimePickerStartTime.Value < dateTimePickerEndTime.Value
            buttonClearData.Enabled = (listViewRTIBData.Groups.Count > 0 And Not d_isSubscribed)
            buttonClearAll.Enabled = (listViewRTIBData.Groups.Count > 0 And Not d_isSubscribed)
            buttonStopSubscribe.Enabled = d_isSubscribed
            panelTimeMessage.Visible = Not isTimeValid
            panelTimeMessage.BringToFront()
        End Sub

        ''' <summary>
        ''' Set row color
        ''' </summary>
        ''' <param name="group"></param>
        ''' <param name="cellColor"></param>
        Private Sub setGroupColor(ByVal group As ListViewGroup, ByVal cellColor As Color)
            Dim index As Integer
            For Each item As ListViewItem In group.Items
                For index = 0 To item.SubItems.Count - 1
                    item.SubItems(index).BackColor = cellColor
                Next index
            Next
        End Sub

        ''' <summary>
        ''' Open output file
        ''' </summary>
        Private Sub openOutputFile()
            If checkBoxOutputFile.Checked Then
                If d_realtimeOutputFile Is Nothing Then
                    d_outputFileName = Application.StartupPath + "\\realtimeBarOutput" + System.DateTime.Now.ToString("MMddyyyy_HHmmss") + ".txt"
                    d_realtimeOutputFile = New StreamWriter("realtimeBarOutput" + System.DateTime.Now.ToString("MMddyyyy_HHmmss") + ".txt")
                End If
                textBoxOutputFile.Text = d_outputFileName
                textBoxOutputFile.Visible = checkBoxOutputFile.Checked
            End If
        End Sub

        ''' <summary>
        ''' Clear security data rows
        ''' </summary>
        Private Sub clearData()
            Dim count As Integer = 0
            For Each group As ListViewGroup In listViewRTIBData.Groups
                ' check for unsubscribed securities
                If group.Items.Count > 0 And (group.Items(0).BackColor.IsEmpty Or _
                    Not (group.Items(0).BackColor = Color.LightGreen Or _
                     group.Items(0).BackColor = Color.Yellow Or _
                     group.Items(0).BackColor = Color.Red)) Then
                    ' clear item in group
                    For count = group.Items.Count - 1 To 1 Step -1
                        listViewRTIBData.Items.Remove(group.Items(count))
                    Next count
                    ' clear sub-item data for last bar
                    For count = 0 To group.Items(0).SubItems.Count - 1
                        group.Items(0).SubItems(count).Text = String.Empty
                    Next count
                End If
            Next
            toolStripStatusLabel1.Text = String.Empty
        End Sub

        ''' <summary>
        ''' Remove all securities and fields from grid
        ''' </summary>
        Private Sub clearAll()
            d_isSubscribed = False
            listViewRTIBData.Items.Clear()
            listViewRTIBData.Groups.Clear()
            setControlStates()
        End Sub

        ''' <summary>
        ''' Stop all subscriptions
        ''' </summary>
        Public Sub StopSubscription()
            Dim index As Integer
            If (Not d_subscriptions Is Nothing) And d_isSubscribed Then
                ' unsubscribe all securities
                d_session.Unsubscribe(d_subscriptions)
                d_subscriptions.Clear()
                ' set all securities to white color for unsubscribe
                For Each group As ListViewGroup In listViewRTIBData.Groups
                    For Each item As ListViewItem In group.Items
                        For index = 0 To item.SubItems.Count - 1
                            item.SubItems(index).BackColor = Color.White
                        Next index
                    Next
                Next
                toolStripStatusLabel1.Text = "Subscription stopped"
            End If
            d_isSubscribed = False
        End Sub
#End Region

#Region "Control Events"
        ''' <summary>
        ''' Allow only numeric keys for subscription interval
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub textBoxInterval_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs)
            ' only allow 0 to 9, backspace, left and right keys
            If Not ((e.KeyValue >= 48 And e.KeyValue <= 57) Or e.KeyData = Keys.Back Or _
                e.KeyData = Keys.Left Or e.KeyData = Keys.Right) Then
                e.SuppressKeyPress = True
            End If
        End Sub

        ''' <summary>
        ''' Output subscription data to file
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub checkBoxOutputFile_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles checkBoxOutputFile.CheckedChanged
            If d_isSubscribed And checkBoxOutputFile.Checked Then
                openOutputFile()
            Else
                If Not checkBoxOutputFile.Checked Then
                    ' close output file
                    If Not d_realtimeOutputFile Is Nothing Then
                        d_realtimeOutputFile.Flush()
                        d_realtimeOutputFile.Close()
                        d_realtimeOutputFile = Nothing
                    End If
                    textBoxOutputFile.Visible = False
                End If
            End If
        End Sub

        ''' <summary>
        ''' Add security button
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub buttonAddSecurity_Click(ByVal sender As Object, ByVal e As EventArgs) Handles buttonAddSecurity.Click
            If textBoxSecurity.Text.Trim().Length > 0 Then
                addSecurities(textBoxSecurity.Text.Trim())
                textBoxSecurity.Text = String.Empty
                setControlStates()
            End If
        End Sub

        ''' <summary>
        ''' Enter key pressed to add security to grid
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub textBoxSecurity_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles textBoxSecurity.KeyDown
            If e.KeyCode = Keys.Return Then
                buttonAddSecurity_Click(sender, New EventArgs())
            End If
        End Sub

        ''' <summary>
        ''' Stop subscription button
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub buttonStopSubscribe_Click(ByVal sender As Object, ByVal e As EventArgs) Handles buttonStopSubscribe.Click
            StopSubscription()
            setControlStates()
        End Sub

        ''' <summary>
        ''' Clear data button
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub buttonClearData_Click(ByVal sender As Object, ByVal e As EventArgs) Handles buttonClearData.Click
            clearData()
        End Sub

        ''' <summary>
        ''' Remove securities and fields button
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub buttonClearAll_Click(ByVal sender As Object, ByVal e As EventArgs) Handles buttonClearAll.Click
            clearAll()
            setControlStates()
        End Sub

        ''' <summary>
        ''' Allow drag and drop of securities and fields
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub listViewRTIBData_DragDrop(ByVal sender As Object, ByVal e As DragEventArgs) Handles listViewRTIBData.DragDrop
            ' Get the entire text object that has been dropped on us.
            Dim tmp As String = e.Data.GetData(DataFormats.Text).ToString()
            ' Tokenize the string into what (we hope) are Security strings
            Dim sep As Char() = {vbCrLf, vbTab}
            Dim words As String() = tmp.Split(sep)
            For Each sec As String In words
                If sec.Trim().Length > 0 Then
                    Dim group As ListViewGroup = Nothing
                    ' add security
                    group = listViewRTIBData.Groups.Add(listViewRTIBData.Groups.Count.ToString(), sec.Trim())
                    ' add 1st item to group
                    Dim listItem As ListViewItem = CreateGroupItem(group, Color.White)
                    listViewRTIBData.Items.Add(listItem)
                End If
            Next
            setControlStates()
        End Sub

        ''' <summary>
        ''' Mouse drag over grid
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub listViewRTIBData_DragEnter(ByVal sender As Object, ByVal e As DragEventArgs) Handles listViewRTIBData.DragEnter
            If e.Data.GetDataPresent(DataFormats.Text) Then
                e.Effect = DragDropEffects.Copy
            Else
                e.Effect = DragDropEffects.None
            End If
        End Sub


        ''' <summary>
        ''' Subscribe to securities
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub buttonSendRequest_Click(ByVal sender As Object, ByVal e As EventArgs) Handles buttonSendRequest.Click
            Dim interval As Double
            Dim options As List(Of String) = New List(Of String)
            Dim tempSubscriptions As List(Of Subscription) = New List(Of Subscription)

            clearData()
            ' create session
            If Not createSession() Then
                toolStripStatusLabel1.Text = "Failed to start session."
                Return
            End If
            ' open market data service
            If Not d_session.OpenService(MKTBAR_SERVICE) Then
                toolStripStatusLabel1.Text = "Failed to open " + MKTBAR_SERVICE
                Return
            End If
            toolStripStatusLabel1.Text = "Connected sucessfully"
            If numericUpDownIntervalSize.Value > 0 Then
                Dim securities As List(Of String) = New List(Of String)
                Dim sec As String
                For Each row As ListViewGroup In listViewRTIBData.Groups
                    ' check for unsubscribed securities
                    If row.Items.Count > 0 And (row.Items(0).BackColor.IsEmpty Or _
                        Not (row.Items(0).BackColor = Color.LightGreen Or _
                         row.Items(0).BackColor = Color.Yellow Or _
                         row.Items(0).BackColor = Color.Red)) Then
                        sec = String.Empty
                        If row.Header.StartsWith("/") Then
                            If Not row.Header.StartsWith(MKTBAR_SERVICE) Then
                                ' add //blp/mktbar in front of security identifier
                                sec = MKTBAR_SERVICE + row.Header
                            End If
                        Else
                            ' add //blp/mktbar/ticker/ in front of security identifier
                            sec = MKTBAR_SERVICE + "/ticker/" + row.Header
                        End If

                        ' set main bar field
                        Dim fields As List(Of String) = New List(Of String)
                        fields.Add("LAST_PRICE")

                        ' set bar interval, start time and end time
                        options.Clear()
                        interval = CType(numericUpDownIntervalSize.Value, Double)
                        options.Add("interval=" + interval.ToString())
                        If dateTimePickerStartTime.Checked Then
                            ' start time format HH:mm in GMT
                            options.Add("start_time=" + dateTimePickerStartTime.Value.ToString("HH:mm"))
                        End If
                        If dateTimePickerEndTime.Checked Then
                            ' end time format HH:mm in GMT
                            options.Add("end_time=" + dateTimePickerEndTime.Value.ToString("HH:mm"))
                        End If
                        ' create subscription object
                        Dim Subscription As Subscription = New Subscription(sec, fields, options, New CorrelationID(row))
                        ' add subscription to temp subscription list
                        tempSubscriptions.Add(Subscription)
                        ' add to application subscription list
                        d_subscriptions.Add(Subscription)
                        ' store subscription string
                        row.Tag = Subscription.SubscriptionString
                    End If
                Next
            End If

            If tempSubscriptions.Count > 0 Then
                ' open output file
                openOutputFile()
                ' subscribe to securities
                d_session.Subscribe(tempSubscriptions)
                d_isSubscribed = True
                setControlStates()
                toolStripStatusLabel1.Text = "Subscribed to securities."
            End If
        End Sub

        ''' <summary>
        ''' Realtime Intraday Bar delete security
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub listViewRTIBData_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles listViewRTIBData.KeyDown
            Dim count As Integer
            If e.KeyCode = Keys.Delete Then
                If Not d_isSubscribed Then
                    If listViewRTIBData.SelectedItems.Count > 0 Then
                        Dim selectedItem As ListViewItem = listViewRTIBData.SelectedItems(0)
                        Dim group As ListViewGroup = selectedItem.Group
                        For count = group.Items.Count - 1 To 0 Step -1
                            listViewRTIBData.Items.Remove(group.Items(count))
                        Next
                        listViewRTIBData.Groups.Remove(group)
                    End If
                End If
            End If
        End Sub

        Private Sub dateTimePickerStartTime_ValueChanged(ByVal sender As Object, ByVal e As EventArgs) Handles dateTimePickerStartTime.ValueChanged, dateTimePickerEndTime.ValueChanged
            setControlStates()
        End Sub

        ''' <summary>
        ''' Show subscription string in tooltip
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub listViewRTIBData_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles listViewRTIBData.MouseMove
            Dim item As ListViewItem = listViewRTIBData.GetItemAt(e.X, e.Y)
            If item Is Nothing Then
                ' clear tool tip
                toolTip1.SetToolTip(listViewRTIBData, "")
            Else
                If item.Group.Tag Is Nothing Then
                    ' clear tool tip
                    toolTip1.SetToolTip(listViewRTIBData, "")
                Else
                    ' display subscription string
                    toolTip1.SetToolTip(listViewRTIBData, item.Group.Tag.ToString())
                End If
            End If
        End Sub
#End Region

#Region "Bloomberg API Events"
        ''' <summary>
        ''' close output file on form close
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs)
            If Not d_realtimeOutputFile Is Nothing Then
                d_realtimeOutputFile.Flush()
                d_realtimeOutputFile.Close()
                d_realtimeOutputFile = Nothing
            End If
        End Sub

        ''' <summary>
        ''' Data Event
        ''' </summary>
        ''' <param name="eventObj"></param>
        ''' <param name="sess"></param>
        Private Sub processEvent(ByVal eventObj As [Event], ByVal sess As Session)
            If InvokeRequired Then
                Invoke(New EventHandler(AddressOf processEvent), New Object() {eventObj, sess})
            Else
                Try
                    Select Case (eventObj.Type)
                        Case [Event].EventType.SUBSCRIPTION_DATA
                            ' process subscription data
                            processRequestDataEvent(eventObj, sess)
                        Case [Event].EventType.SUBSCRIPTION_STATUS
                            ' process subscription status
                            processRequestStatusEvent(eventObj, sess)
                        Case Else
                            ' Other status events
                            processMiscEvents(eventObj, sess)
                    End Select
                Catch e As System.Exception
                    toolStripStatusLabel1.Text = e.Message.ToString()
                End Try
            End If
        End Sub

        ''' <summary>
        ''' Process subscription data
        ''' </summary>
        ''' <param name="eventObj"></param>
        ''' <param name="session"></param>
        Private Sub processRequestDataEvent(ByVal eventObj As [Event], ByVal session As Session)
            Dim group As ListViewGroup = Nothing
            Dim item As ListViewItem = Nothing

            ' process message
            For Each msg As Message In eventObj
                ' get correlation id
                group = CType(msg.CorrelationID.Object, ListViewGroup)
                ' output to file
                If checkBoxOutputFile.Checked Then
                    d_realtimeOutputFile.WriteLine(msg.TopicName + ":\n" + msg.ToString())
                    d_realtimeOutputFile.Flush()
                End If
                ' Get security item to update
                If msg.MessageType Is MARKET_BAR_UPDATE Then
                    If group.Items(0).BackColor = Color.Yellow Then
                        ' set waiting for subscription to start color
                        setGroupColor(group, Color.LightGreen)
                    End If
                    ' get last group item to update
                    item = group.Items(group.Items.Count - 1)
                Else
                    If msg.MessageType Is MARKET_BAR_START Then
                        If group.Items.Count = 1 And group.Items(0).SubItems(1).Text.Length = 0 Then
                            ' use empty row
                            item = group.Items(group.Items.Count - 1)
                        Else
                            ' create new group item
                            item = CreateGroupItem(group, Color.LightGreen)
                            listViewRTIBData.Items.Add(item)
                        End If
                    Else
                        ' MarketBarEnd, create new group seperator item
                        item = CreateGroupItem(group, Color.White)
                        listViewRTIBData.Items.Add(item)
                        ' set waiting for subscription to start color
                        setGroupColor(group, Color.Yellow)
                        Continue For
                    End If
                End If
                item.Text = group.Header
                Dim index As Integer = 0
                For Each field As Element In msg.Elements
                    index = d_fields.IndexOf(field.Name)
                    item.SubItems(index + 1).Text = field.GetValueAsString()
                Next
            Next
            ' allow application to update UI
            Application.DoEvents()
        End Sub


        ''' <summary>
        ''' Request status event
        ''' </summary>
        ''' <param name="eventObj"></param>
        ''' <param name="sess"></param>
        Private Sub processRequestStatusEvent(ByVal eventObj As [Event], ByVal sess As Session)
            Dim dataList As List(Of String) = New List(Of String)
            ' process status message
            For Each msg As Message In eventObj
                Dim group As ListViewGroup = CType(msg.CorrelationID.Object, ListViewGroup)
                If msg.MessageType.Equals(Bloomberglp.Blpapi.Name.GetName("SubscriptionStarted")) Then
                    ' set waiting for subscription to start color
                    setGroupColor(group, Color.Yellow)
                Else
                    ' check for subscription failure
                    If msg.MessageType.Equals(Bloomberglp.Blpapi.Name.GetName("SubscriptionFailure")) Then
                        ' set exception color
                        setGroupColor(group, Color.Red)
                        If msg.HasElement(REASON) Then
                            Dim cause As Element = msg.GetElement(REASON)
                            Dim message As String = cause.GetElementAsString(DESCRIPTION)
                            group.Items(0).SubItems(1).Text = message
                        End If
                    End If
                End If
            Next
        End Sub

        ''' <summary>
        ''' Process miscellaneous events
        ''' </summary>
        ''' <param name="eventObj"></param>
        ''' <param name="session"></param>
        Private Sub processMiscEvents(ByVal eventObj As [Event], ByVal session As Session)
            For Each msg As Message In eventObj
                Select Case (msg.MessageType.ToString())
                    Case "SessionStarted"
                        ' "Session Started"
                        toolStripStatusLabel1.Text = msg.MessageType.ToString()
                    Case "SessionStopped", "SessionTerminated"
                        ' "Session Terminated"
                        toolStripStatusLabel1.Text = msg.MessageType.ToString()
                    Case "SessionStartupFailure"
                        ' Failed to start session
                        toolStripStatusLabel1.Text = msg.MessageType.ToString()
                    Case "ServiceOpened"
                        ' "Reference Service Opened"
                    Case "RequestFailure"
                        Dim cause As Element = msg.GetElement(REASON)
                        Dim message As String = String.Concat("Error: Source-", cause.GetElementAsString(SOURCE), _
                            ", Code-", cause.GetElementAsString(ERROR_CODE), ", category-", cause.GetElementAsString(CATEGORY), _
                            ", desc-", cause.GetElementAsString(DESCRIPTION))
                        toolStripStatusLabel1.Text = message
                    Case Else
                        toolStripStatusLabel1.Text = msg.MessageType.ToString()
                End Select
            Next
        End Sub
#End Region
    End Class
End Namespace
