' ==========================================================
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT 
'  WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED,    
' INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES   
' OF MERCHANTABILITY AND/OR FITNESS FOR A  PARTICULAR   
' PURPOSE.								
' ==========================================================
' Purpose of this example:
' - Subscribe and de-subscribe to securities using
'   //blp/mktdata service.
' - Subscribe to security with a set update interval in
'   seconds.
' - Subscribe to delay data stream.
' ==========================================================
Imports [Event] = Bloomberglp.Blpapi.Event
Imports Message = Bloomberglp.Blpapi.Message
Imports Element = Bloomberglp.Blpapi.Element
Imports Name = Bloomberglp.Blpapi.Name
Imports Request = Bloomberglp.Blpapi.Request
Imports Service = Bloomberglp.Blpapi.Service
Imports Session = Bloomberglp.Blpapi.Session
Imports SessionOptions = Bloomberglp.Blpapi.SessionOptions
Imports CorrelationID = Bloomberglp.Blpapi.CorrelationID
Imports Subscription = Bloomberglp.Blpapi.Subscription
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
        Private ReadOnly FORCE_DELAY As Name = New Name(" [FD]")

        Private d_sessionOptions As SessionOptions
        Private d_session As Session
        Private d_subscriptions As List(Of Subscription)
        Private d_isSubscribed As Boolean = False
        Private d_realtimeOutputFile As TextWriter
        Private d_outputFileName As String

        ''' <summary>
        ''' form load event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim serverHost As String = "localhost"
            Dim serverPort As Integer = 8194

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
            dataGridViewData.Columns.Add("security", "security")
        End Sub
        ''' <summary>
        ''' Add securities to grid
        ''' </summary>
        ''' <param name="securities"></param>
        Private Sub addSecurities(ByVal securities As String)
            ' Tokenize the string into what (we hope) are Security strings
            Dim sep As Char() = {vbCrLf, vbTab, ","c}
            Dim words As String() = securities.Split(sep)
            ' check delay subscription
            Dim delay As String = String.Empty
            'check for force delay
            If (checkBoxForceDelay.Checked) Then
                delay = FORCE_DELAY.ToString()
            End If
            For Each security As String In words
                If (security.Trim().Length > 0) Then
                    ' add security
                    dataGridViewData.Rows.Add(security.Trim() + delay)
                End If
            Next
            setControlStates()
        End Sub
        ''' <summary>
        ''' Add fields to grid
        ''' </summary>
        ''' <param name="fields"></param>
        Private Sub addFields(ByVal fields As String)
            ' Tokenize the string into what (we hope) are Security strings
            Dim sep As Char() = {vbCrLf, vbTab, ","c}
            Dim words As String() = fields.Split(sep)
            For Each field As String In words
                If (field.Trim().Length > 0) Then
                    ' add fields
                    If (Not dataGridViewData.Columns.Contains(field.Trim())) Then
                        dataGridViewData.Columns.Add(field.Trim(), field.Trim())
                        dataGridViewData.Columns(field.Trim()).SortMode = DataGridViewColumnSortMode.NotSortable
                    End If
                End If
            Next
            setControlStates()
            toolStripStatusLabel1.Text = String.Empty
        End Sub
        ''' <summary>
        ''' Create session
        ''' </summary>
        ''' <returns></returns>
        Private Function createSession() As Boolean
            If (IsNothing(d_session)) Then
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
            buttonSendRequest.Enabled = dataGridViewData.Rows.Count > 0 And _
                                        dataGridViewData.Columns.Count > 1 And Not d_isSubscribed
            buttonClearFields.Enabled = dataGridViewData.Columns.Count > 1 And Not d_isSubscribed
            buttonClearData.Enabled = buttonSendRequest.Enabled
            buttonClearAll.Enabled = (dataGridViewData.Rows.Count > 0 Or _
                                        dataGridViewData.Columns.Count > 1) And Not d_isSubscribed
            buttonStopSubscribe.Enabled = d_isSubscribed
            labelSecurity.Enabled = Not d_isSubscribed
            textBoxSecurity.Enabled = Not d_isSubscribed
            buttonAddSecurity.Enabled = Not d_isSubscribed
            labelField.Enabled = Not d_isSubscribed
            textBoxField.Enabled = Not d_isSubscribed
            buttonAddField.Enabled = Not d_isSubscribed
            labelInterval.Enabled = Not d_isSubscribed
            textBoxInterval.Enabled = Not d_isSubscribed
            checkBoxForceDelay.Enabled = Not d_isSubscribed
        End Sub
        ''' <summary>
        ''' Open output file
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub openOutputFile()
            If (checkBoxOutputFile.Checked) Then
                If (IsNothing(d_realtimeOutputFile)) Then
                    d_outputFileName = Application.StartupPath + "\realtimeOut" + System.DateTime.Now.ToString("MMddyyyy_HHmmss") + ".txt"
                    d_realtimeOutputFile = New StreamWriter("realtimeOut" + System.DateTime.Now.ToString("MMddyyyy_HHmmss") + ".txt")
                End If
                textBoxOutputFile.Text = d_outputFileName
                textBoxOutputFile.Visible = checkBoxOutputFile.Checked
            End If
        End Sub

        ''' <summary>
        ''' Clear security data cell
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub clearData()
            If (dataGridViewData.Rows.Count > 0) Then
                For Each row As DataGridViewRow In dataGridViewData.Rows
                    For index As Integer = 1 To dataGridViewData.Columns.Count - 1
                        row.Cells(index).Value = String.Empty
                    Next
                Next
            End If
            toolStripStatusLabel1.Text = String.Empty
        End Sub
        ''' <summary>
        ''' Clear security and data
        ''' </summary>
        Private Sub clearFields()
            For index As Integer = dataGridViewData.Columns.Count - 1 To 1 Step -1
                dataGridViewData.Columns.RemoveAt(index)
            Next
        End Sub
        ''' <summary>
        ''' Remove all securities and fields from grid
        ''' </summary>
        Private Sub clearAll()
            d_isSubscribed = False
            dataGridViewData.Rows.Clear()
            clearFields()
            setControlStates()
        End Sub
        ''' <summary>
        ''' Stop all subscriptions
        ''' </summary>
        Public Sub StopSubscription()
            If (Not IsNothing(d_subscriptions) And d_isSubscribed) Then
                d_session.Unsubscribe(d_subscriptions)
                ' set all securities to white color for unsubscribe
                For Each row As DataGridViewRow In dataGridViewData.Rows
                    For Each cell As DataGridViewCell In row.Cells
                        cell.Style.BackColor = Color.White
                    Next
                Next
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
        Private Sub textBoxInterval_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles textBoxInterval.KeyDown
            ' only allow 0 to 9, backspace, left and right keys
            If Not (((e.KeyValue >= 48 And e.KeyValue <= 57) Or e.KeyData = Keys.Back Or _
                e.KeyData = Keys.Left Or e.KeyData = Keys.Right)) Then
                e.SuppressKeyPress = True
            End If
        End Sub
        ''' <summary>
        ''' Output subscription data to file
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub checkBoxOutputFile_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles checkBoxOutputFile.CheckedChanged
            If (d_isSubscribed And checkBoxOutputFile.Checked) Then
                openOutputFile()
            Else
                If (Not checkBoxOutputFile.Checked) Then
                    ' close output file
                    If (Not IsNothing(d_realtimeOutputFile)) Then
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
            If (textBoxSecurity.Text.Trim().Length > 0) Then
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
            If (e.KeyCode = Keys.Return) Then
                buttonAddSecurity_Click(sender, New EventArgs())
            End If
        End Sub
        ''' <summary>
        ''' Add field button
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub buttonAddField_Click(ByVal sender As Object, ByVal e As EventArgs) Handles buttonAddField.Click
            If (textBoxField.Text.Trim().Length > 0) Then
                addFields(textBoxField.Text.ToUpper().Trim())
                textBoxField.Text = String.Empty
                setControlStates()
            End If
        End Sub
        ''' <summary>
        ''' Enter key pressed to add field to grid
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub textBoxField_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles textBoxField.KeyDown
            If (e.KeyCode = Keys.Return) Then
                buttonAddField_Click(sender, New EventArgs())
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
        ''' Remove all fields button
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub buttonClearFields_Click(ByVal sender As Object, ByVal e As EventArgs) Handles buttonClearFields.Click
            clearFields()
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
        Private Sub dataGridViewData_DragDrop(ByVal sender As Object, ByVal e As DragEventArgs) Handles dataGridViewData.DragDrop
            ' Get the entire text object that has been dropped on us.
            Dim tmp As String = e.Data.GetData(DataFormats.Text).ToString()
            ' Tokenize the string into what (we hope) are Security strings
            Dim sep As Char() = {vbCrLf, vbTab}
            Dim words As String() = tmp.Split(sep)
            For Each sec As String In words
                If (sec.Trim().Length > 0) Then
                    If (sec.Trim().Contains(" ")) Then
                        ' add securities
                        dataGridViewData.Rows.Add(New Object() {sec.Trim()})
                    Else
                        ' add fields
                        dataGridViewData.Columns.Add(sec.Trim(), sec.Trim())
                    End If
                End If
            Next
            setControlStates()
        End Sub
        ''' <summary>
        ''' Mouse drag over grid
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub dataGridViewData_DragEnter(ByVal sender As Object, ByVal e As DragEventArgs) Handles dataGridViewData.DragEnter
            If (e.Data.GetDataPresent(DataFormats.Text)) Then
                e.Effect = DragDropEffects.Copy
            Else
                e.Effect = DragDropEffects.None
            End If
        End Sub
        ''' <summary>
        ''' Allow user to delete single field or security from grid
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub dataGridViewData_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles dataGridViewData.KeyDown
            Dim dataGrid As DataGridView = CType(sender, DataGridView)
            If (e.KeyData = Keys.Delete And dataGrid.SelectedCells.Count > 0 And Not d_isSubscribed) Then
                Dim rowIndex As Integer = dataGrid.SelectedCells(0).RowIndex
                Dim columnIndex As Integer = dataGrid.SelectedCells(0).ColumnIndex
                If (columnIndex > 0) Then
                    ' remove field
                    dataGrid.Columns.RemoveAt(columnIndex)
                Else
                    ' remove security
                    dataGrid.Rows.RemoveAt(rowIndex)
                End If
                If (Not IsNothing(dataGrid.DataSource)) Then
                    ' update data set
                    Dim data As DataSet = CType(dataGrid.DataSource, DataSet)
                    data.AcceptChanges()
                End If
                If (dataGrid.Columns.Count > columnIndex And columnIndex > 0) Then
                    ' keep column selection the same 
                    dataGrid.Rows(rowIndex).Cells(columnIndex).Selected = True
                Else
                    If (dataGrid.Columns.Count > 1 And dataGrid.Columns.Count = columnIndex) Then
                        ' no more columns after the deleted colunm, move forward one
                        dataGrid.Rows(rowIndex).Cells(columnIndex - 1).Selected = True
                    End If
                End If
            End If
        End Sub
        ''' <summary>
        ''' Subscribe to securities
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub buttonSendRequest_Click(ByVal sender As Object, ByVal e As EventArgs) Handles buttonSendRequest.Click
            clearData()
            ' create session
            If (Not createSession()) Then
                toolStripStatusLabel1.Text = "Failed to start session."
                Return
            End If
            ' open market data session
            If (Not d_session.OpenService("//blp/mktdata")) Then
                toolStripStatusLabel1.Text = "Failed to open //blp/mktdata"
                Return
            End If
            toolStripStatusLabel1.Text = "Connected sucessfully"
            Dim refDataService As Service = d_session.GetService("//blp/mktdata")
            Dim fields As List(Of String) = New List(Of String)()
            Dim options As List(Of String) = New List(Of String)()
            d_subscriptions = New List(Of Subscription)()
            ' populate fields
            For fieldIndex As Integer = 1 To dataGridViewData.Columns.Count - 1
                fields.Add(dataGridViewData.Columns(fieldIndex).Name)
            Next
            ' create subscriptions and add to list
            For Each secRow As DataGridViewRow In dataGridViewData.Rows
                options.Clear()
                If (textBoxInterval.Text.Length > 0 And Integer.Parse(textBoxInterval.Text) > 0) Then
                    options.Add("interval=" + textBoxInterval.Text)
                End If
                Dim security As String = secRow.Cells("security").Value.ToString()
                ' check for delay subscription
                If (security.Contains("[FD]")) Then
                    options.Add("delayed")
                    security = security.Replace("[FD]", "").Trim()
                End If
                d_subscriptions.Add(New Subscription(security, fields, options, New CorrelationID(secRow)))
            Next
            ' open output file
            openOutputFile()
            ' subscribe to securities
            d_session.Subscribe(d_subscriptions)
            d_isSubscribed = True
            setControlStates()

            toolStripStatusLabel1.Text = "Subscribed to securities."
        End Sub
        ''' <summary>
        ''' close output file on form close
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs)
            If (Not IsNothing(d_realtimeOutputFile)) Then
                d_realtimeOutputFile.Flush()
                d_realtimeOutputFile.Close()
                d_realtimeOutputFile = Nothing
            End If
        End Sub
#End Region

#Region "Bloomberg API Event and data processing"
        ''' <summary>
        ''' Data Event
        ''' </summary>
        ''' <param name="eventObj"></param>
        ''' <param name="session"></param>
        Private Sub processEvent(ByVal eventObj As [Event], ByVal session As Session)
            If (InvokeRequired) Then
                Invoke(New Bloomberglp.Blpapi.EventHandler(AddressOf processEvent), New Object() {eventObj, session})
            Else
                Try
                    Select Case (eventObj.Type)
                        Case [Event].EventType.SUBSCRIPTION_DATA
                            ' process subscription data
                            processRequestDataEvent(eventObj, session)
                        Case [Event].EventType.SUBSCRIPTION_STATUS
                            ' process subscription status
                            processRequestStatusEvent(eventObj, session)
                        Case Else
                            processMiscEvents(eventObj, session)
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
            'process message
            For Each msg As Message In eventObj
                ' get correlation id
                Dim dataRow As DataGridViewRow = CType(msg.CorrelationID.Object, DataGridViewRow)
                ' output to file
                If (checkBoxOutputFile.Checked) Then
                    d_realtimeOutputFile.WriteLine(msg.TopicName + ":\n" + msg.ToString())
                End If
                ' process market data
                If (msg.MessageType.Equals(Bloomberglp.Blpapi.Name.GetName("MarketDataEvents"))) Then
                    ' check for initial paint
                    If (msg.HasElement("SES_START")) Then
                        If (msg.GetElementAsBool("IS_DELAYED_STREAM")) Then
                            ' set to delay stream color
                            For Each cell As DataGridViewCell In dataRow.Cells
                                cell.Style.BackColor = Color.Yellow
                            Next
                        End If
                    End If
                    ' process tick data
                    For fieldIndex As Integer = 1 To dataGridViewData.ColumnCount - 1
                        Dim field As String = dataGridViewData.Columns(fieldIndex).Name
                        If (msg.HasElement(field)) Then
                            Dim fieldData As Element = msg.GetElement(field)
                            ' check element to see if it has null value
                            If (Not fieldData.IsNull) Then
                                dataRow.Cells(field).Value = msg.GetElementAsString(field)
                            End If
                        End If
                        ' allow application to update UI
                        Application.DoEvents()
                    Next
                End If
            Next
        End Sub
        ''' <summary>
        ''' Request status event
        ''' </summary>
        ''' <param name="eventObj"></param>
        ''' <param name="session"></param>
        Private Sub processRequestStatusEvent(ByVal eventObj As [Event], ByVal session As Session)
            Dim dataList As List(Of String) = New List(Of String)()

            For Each msg As Message In eventObj
                Dim dataRow As DataGridViewRow = CType(msg.CorrelationID.Object, DataGridViewRow)
                ' process status message
                If (msg.MessageType.Equals(Bloomberglp.Blpapi.Name.GetName("SubscriptionStarted"))) Then
                    ' set subscribed color
                    For Each cell As DataGridViewCell In dataRow.Cells
                        cell.Style.BackColor = Color.LightGreen
                    Next
                    Try
                        ' check for error
                        If (msg.HasElement("exceptions")) Then
                            ' subscription has error
                            Dim err As Element = msg.GetElement("exceptions")
                            Dim searchIndex As Integer = 0
                            For errorIndex As Integer = 0 To err.NumValues - 1
                                Dim errorException As Element = err.GetValueAsElement(errorIndex)
                                Dim field As String = errorException.GetElementAsString(FIELD_ID)
                                Dim cause As Element = errorException.GetElement(REASON)
                                Dim message As String = cause.GetElementAsString(DESCRIPTION)
                                Do While (searchIndex < dataGridViewData.ColumnCount - 1)
                                    If (field = dataGridViewData.Columns(searchIndex).Name) Then
                                        dataRow.Cells(searchIndex).Value = message
                                        Exit Do
                                    End If
                                    searchIndex += 1
                                Loop
                            Next
                        End If
                    Catch e As Exception
                        toolStripStatusLabel1.Text = e.Message.ToString()
                    End Try
                Else
                    ' check for subscription failure
                    If (msg.MessageType.Equals(Bloomberglp.Blpapi.Name.GetName("SubscriptionFailure"))) Then
                        If (msg.HasElement(REASON)) Then
                            Dim cause = msg.GetElement(REASON)
                            Dim message As String = cause.GetElementAsString(DESCRIPTION)
                            dataRow.Cells(1).Value = message
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
                    Case "SessionTerminated"
                        ' "Session Terminated"
                    Case "SessionStopped"
                        ' "Session Stopped"
                    Case "ServiceOpened"
                        ' "Reference Service Opened"
                    Case "RequestFailure"
                        Dim cause = msg.GetElement(REASON)
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