' ==========================================================
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT 
'  WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED,    
' INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES   
' OF MERCHANTABILITY AND/OR FITNESS FOR A  PARTICULAR   
' PURPOSE.								
' ==========================================================
' Purpose of this example:
' - Subscribe to Custom VWAP, Bloomberg VWAP, and 
'   Market VWAP using //blp/mktvwap service.
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

        Private d_sessionOptions As SessionOptions
        Private d_session As Session
        Private d_subscriptions As List(Of Subscription)
        Private d_isSubscribed As Boolean = False

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
            ' default with VWap fields
            textBoxField.Text = "VWAP,RT_VWAP_VOLUME,VWAP_TURNOVER_RT,VWAP_NUM_TRADES_RT,VWAP_STANDARD_DEV_RT"
            ' default with Market VWap fields
            'textBoxField.Text = "MARKET_DEFINED_VWAP_REALTIME,RT_MKT_VWAP_VOLUME," & _
            '        "MKT_DEF_VWAP_TURNOVER_RT,MKT_DEF_VWAP_NUM_TRADES_RT," & _
            '        "MKT_DEF_VWAP_STANDARD_DEV_RT"
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
                If (security.Trim().Length > 0) Then
                    ' add security
                    dataGridViewData.Rows.Add(security.Trim())
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
        End Sub
        ''' <summary>
        ''' Create session
        ''' </summary>
        ''' <returns></returns>
        Private Function createSession() As Boolean
            If IsNothing(d_session) Then
                toolStripStatusLabel1.Text = "Connecting..."
                d_session = New Session(d_sessionOptions, New Bloomberglp.Blpapi.EventHandler(AddressOf processEvent))
            End If
            Return d_session.Start()
        End Function
        ''' <summary>
        ''' Manage control states
        ''' </summary>
        Private Sub setControlStates()
            buttonSendRequest.Enabled = dataGridViewData.Rows.Count > 0 And _
                                        dataGridViewData.Columns.Count > 1 And _
                                        Not d_isSubscribed
            buttonClearFields.Enabled = dataGridViewData.Columns.Count > 1 And Not d_isSubscribed
            buttonClearData.Enabled = buttonSendRequest.Enabled
            buttonClearAll.Enabled = (dataGridViewData.Rows.Count > 0 Or dataGridViewData.Columns.Count > 1) And Not d_isSubscribed
            buttonStopSubscribe.Enabled = d_isSubscribed
            labelSecurity.Enabled = Not d_isSubscribed
            textBoxSecurity.Enabled = Not d_isSubscribed
            buttonAddSecurity.Enabled = Not d_isSubscribed
            labelField.Enabled = Not d_isSubscribed
            textBoxField.Enabled = Not d_isSubscribed
            buttonAddField.Enabled = Not d_isSubscribed
            labelOverride.Enabled = Not d_isSubscribed
            textBoxOverride.Enabled = labelOverride.Enabled
            buttonAddOverride.Enabled = labelOverride.Enabled
            listViewOverrides.Enabled = labelOverride.Enabled
            labelOverrideNote.Enabled = labelOverride.Enabled
        End Sub
        ''' <summary>
        ''' Clear security data
        ''' </summary>
        Private Sub clearData()
            If (dataGridViewData.Rows.Count > 0) Then
                For Each row As DataGridViewRow In dataGridViewData.Rows
                    For index As Integer = 1 To dataGridViewData.Columns.Count - 1
                        row.Cells(index).Value = String.Empty
                    Next index
                Next row
            End If
            toolStripStatusLabel1.Text = String.Empty
        End Sub
        ''' <summary>
        ''' Remove all fields from grid
        ''' </summary>
        Private Sub clearFields()
            For index As Integer = dataGridViewData.Columns.Count - 1 To 1 Step -1
                dataGridViewData.Columns.RemoveAt(index)
            Next index
            toolStripStatusLabel1.Text = String.Empty
        End Sub
        ''' <summary>
        ''' Remove all securities and fields from grid
        ''' </summary>
        Private Sub clearAll()
            d_isSubscribed = False
            dataGridViewData.Rows.Clear()
            clearFields()
            listViewOverrides.Items.Clear()
            setControlStates()
        End Sub
        ''' <summary>
        ''' Stop all subscriptions
        ''' </summary>
        Public Sub StopSubscription()
            If (Not IsNothing(d_subscriptions) And d_isSubscribed) Then
                d_session.Unsubscribe(d_subscriptions)
                toolStripStatusLabel1.Text = "Subscription stopped."
                For Each row As DataGridViewRow In dataGridViewData.Rows
                    For Each cell As DataGridViewCell In row.Cells
                        cell.Style.BackColor = Color.White
                    Next
                Next
            End If
            d_isSubscribed = False
        End Sub
#End Region

#Region "Events"
        ''' <summary>
        ''' Add security button
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub buttonAddSecurity_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles buttonAddSecurity.Click
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
        ''' <remarks></remarks>
        Private Sub textBoxSecurity_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles textBoxSecurity.KeyDown
            If (e.KeyCode = Keys.Return) Then
                buttonAddSecurity_Click(sender, New EventArgs())
            End If
        End Sub
        ''' <summary>
        ''' Add field button
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub buttonAddField_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles buttonAddField.Click
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
        ''' <remarks></remarks>
        Private Sub textBoxField_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles textBoxField.KeyDown
            If (e.KeyCode = Keys.Return) Then
                buttonAddField_Click(sender, New EventArgs())
            End If
        End Sub
        ''' <summary>
        ''' Stop subscription button
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub buttonStopSubscribe_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles buttonStopSubscribe.Click
            StopSubscription()
            setControlStates()
        End Sub
        ''' <summary>
        ''' Clear fields button
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub buttonClearFields_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles buttonClearFields.Click
            clearFields()
        End Sub
        ''' <summary>
        ''' Remove securities and fields button
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub buttonClearAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles buttonClearAll.Click
            clearAll()
            setControlStates()
        End Sub
        ''' <summary>
        ''' Clear data
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub buttonClearData_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles buttonClearData.Click
            clearData()
        End Sub
        ''' <summary>
        ''' Allow drag and drop of securities and fields
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub dataGridViewData_DragDrop(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles dataGridViewData.DragDrop
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
        ''' <remarks></remarks>
        Private Sub dataGridViewData_DragEnter(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles dataGridViewData.DragEnter
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
        ''' <remarks></remarks>
        Private Sub dataGridViewData_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles dataGridViewData.KeyDown
            If (e.KeyData = Keys.Delete And dataGridViewData.SelectedCells.Count > 0 And Not d_isSubscribed) Then
                Dim rowIndex As Integer = dataGridViewData.SelectedCells(0).RowIndex
                Dim columnIndex As Integer = dataGridViewData.SelectedCells(0).ColumnIndex
                If (columnIndex > 0) Then
                    ' remove field
                    dataGridViewData.Columns.RemoveAt(columnIndex)
                Else
                    ' remove security
                    dataGridViewData.Rows.RemoveAt(rowIndex)
                    If (Not IsNothing(dataGridViewData.DataSource)) Then
                        Dim data As DataSet = CType(dataGridViewData.DataSource, DataSet)
                        data.AcceptChanges()
                    End If
                    If (dataGridViewData.Columns.Count > columnIndex And columnIndex > 0) Then
                        dataGridViewData.Rows(rowIndex).Cells(columnIndex).Selected = True
                    Else
                        If (dataGridViewData.Columns.Count > 1 And dataGridViewData.Columns.Count = columnIndex) Then
                            dataGridViewData.Rows(rowIndex).Cells(columnIndex - 1).Selected = True
                        End If
                    End If
                End If
            End If
        End Sub
        ''' <summary>
        ''' Drag and drop override fields and values
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub listViewOverrides_DragDrop(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles listViewOverrides.DragDrop
            ' Get the entire text object that has been dropped on us.
            Dim tmp As String = e.Data.GetData(DataFormats.Text).ToString()
            Dim values As List(Of String) = New List(Of String)()
            Dim fields As List(Of String) = New List(Of String)()
            ' Tokenize the string into what (we hope) are Security strings
            Dim sep As Char() = {vbCrLf, vbTab}
            Dim words As String() = tmp.Split(sep)
            For Each sec As String In words
                If (sec.Contains("=")) Then
                    Dim ovr As String() = sec.Split(New Char() {"="})
                    If (ovr(0).Trim().Length > 0) Then
                        Dim item As ListViewItem = listViewOverrides.Items.Add(ovr(0).Trim())
                        item.SubItems.Add(ovr(1).Trim())
                    End If
                End If
            Next
        End Sub
        ''' <summary>
        ''' Mouse drag over override listView
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub listViewOverrides_DragEnter(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles listViewOverrides.DragEnter
            If (e.Data.GetDataPresent(DataFormats.Text)) Then
                e.Effect = DragDropEffects.Copy
            Else
                e.Effect = DragDropEffects.None
            End If
        End Sub
        ''' <summary>
        ''' Allow user to delete single override field
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub listViewOverrides_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles listViewOverrides.KeyDown
            If (e.KeyData = Keys.Delete And listViewOverrides.SelectedItems.Count > 0) Then
                For Each item As ListViewItem In listViewOverrides.SelectedItems
                    item.Remove()
                Next
            End If
        End Sub
        ''' <summary>
        ''' Enter key pressed to add override field
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub textBoxOverride_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles textBoxOverride.KeyDown
            If (e.KeyCode = Keys.Return) Then
                buttonAddOverride_Click(sender, New EventArgs())
            End If
        End Sub
        ''' <summary>
        ''' Add override field to list
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub buttonAddOverride_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles buttonAddOverride.Click
            If (textBoxOverride.Text.Length = 0 Or Not textBoxOverride.Text.Contains("=")) Then
                MessageBox.Show("Missing field or missing '=' seperator between field and value", "Add Field", MessageBoxButtons.OK, MessageBoxIcon.Information)
                textBoxOverride.Focus()
            Else
                Dim input As String() = textBoxOverride.Text.Split(New Char() {","})
                For Each overrideItem As String In input
                    If (overrideItem.Trim().Length > 0 And overrideItem.Contains("=")) Then
                        Dim ovr As String() = overrideItem.Split(New Char() {"="})
                        Dim item As ListViewItem = listViewOverrides.Items.Add(ovr(0).Trim())
                        item.SubItems.Add(ovr(1).Trim())
                    End If
                Next
                textBoxOverride.Text = String.Empty
            End If
            setControlStates()
        End Sub
        ''' <summary>
        ''' Subscribe to custom VWAP
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub buttonSendRequest_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles buttonSendRequest.Click
            Const prefix As String = "//blp/mktvwap/ticker/"
            ' clear data cells
            clearData()
            ' create session
            If (Not createSession()) Then
                toolStripStatusLabel1.Text = "Failed to start session."
                Return
            End If
            ' open service
            If (Not d_session.OpenService("//blp/mktvwap")) Then
                toolStripStatusLabel1.Text = "Failed to open //blp/mktvwap"
                Return
            End If
            toolStripStatusLabel1.Text = "Connected sucessfully"

            Dim fields As List(Of String) = New List(Of String)()
            Dim options As List(Of String) = New List(Of String)()
            d_subscriptions = New List(Of Subscription)()
            ' populate fields
            For fieldIndex As Integer = 1 To dataGridViewData.Columns.Count - 1
                fields.Add(dataGridViewData.Columns(fieldIndex).Name)
            Next
            ' add overrides
            For Each item As ListViewItem In listViewOverrides.Items
                options.Add(item.Text + "=" + item.SubItems(1).Text)
            Next
            ' create subscription 
            For Each secRow As DataGridViewRow In dataGridViewData.Rows
                Dim security As String = secRow.Cells("security").Value.ToString()
                d_subscriptions.Add(New Subscription(prefix + security, fields, options, New CorrelationID(secRow)))
            Next
            ' subscribe to securities
            d_session.Subscribe(d_subscriptions)
            d_isSubscribed = True
            setControlStates()
            toolStripStatusLabel1.Text = "Subscribed to securities."
        End Sub
#End Region

#Region "Bloomberg API Event and data processing"
        ' <summary>
        ' Data event
        ' </summary>
        ' <param name="eventObj"></param>
        ' <param name="session"></param>
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
            For Each msg As Message In eventObj
                Dim dataRow As DataGridViewRow = CType(msg.CorrelationID.Object, DataGridViewRow)
                If (msg.MessageType.Equals(Bloomberglp.Blpapi.Name.GetName("MarketDataEvents"))) Then
                    ' check for initial paint
                    If (msg.HasElement("SES_START")) Then
                        If (msg.GetElementAsBool("IS_DELAYED_STREAM")) Then
                            ' set to delay stream color
                            For Each cell As DataGridViewCell In dataRow.Cells
                                cell.Style.BackColor = Color.LightSkyBlue
                            Next
                        End If
                    End If
                End If
                ' process tick data
                For fieldIndex As Integer = 1 To dataGridViewData.ColumnCount - 1
                    Dim field As String = dataGridViewData.Columns(fieldIndex).Name
                    If (msg.HasElement(field)) Then
                        dataRow.Cells(field).Value = msg.GetElementAsString(field)
                    End If
                    ' allow application to update UI
                    Application.DoEvents()
                Next
            Next
        End Sub
        ' <summary>
        ' Request status event
        ' </summary>
        ' <param name="eventObj"></param>
        ' <param name="session"></param>
        Private Sub processRequestStatusEvent(ByVal eventObj As [Event], ByVal session As Session)
            Dim dataList As List(Of String) = New List(Of String)()
            ' process messages
            For Each msg As Message In eventObj
                Dim dataRow As DataGridViewRow = CType(msg.CorrelationID.Object, DataGridViewRow)
                If (msg.MessageType.Equals(Bloomberglp.Blpapi.Name.GetName("SubscriptionStarted"))) Then
                    ' set subscribed color
                    For Each cell As DataGridViewCell In dataRow.Cells
                        cell.Style.BackColor = Color.LightGreen
                    Next
                    Try
                        ' check for exceptions
                        If (msg.HasElement("exceptions")) Then
                            Dim err As Element = msg.GetElement("exceptions")
                            For errorIndex As Integer = 0 To err.NumValues - 1
                                Dim errorException As Element = err.GetValueAsElement(errorIndex)
                                Dim field As String = errorException.GetElementAsString(FIELD_ID)
                                Dim cause As Element = errorException.GetElement(REASON)
                                Dim message As String = cause.GetElementAsString(DESCRIPTION)
                                If (dataGridViewData.Columns.Contains(field)) Then
                                    dataRow.Cells(field).Value = message
                                End If
                            Next
                        End If
                    Catch
                    End Try
                Else
                    If (msg.MessageType.Equals(Bloomberglp.Blpapi.Name.GetName("SubscriptionFailure"))) Then
                        If (msg.HasElement(REASON)) Then
                            Dim cause As Element = msg.GetElement(REASON)
                            Dim message As String = cause.GetElementAsString(DESCRIPTION)
                            dataRow.Cells(1).Value = message
                        End If
                    End If
                End If
            Next
        End Sub
        ' <summary>
        ' Process miscellaneous events
        ' </summary>
        ' <param name="eventObj"></param>
        ' <param name="session"></param>
        Private Sub processMiscEvents(ByVal eventObj As [Event], ByVal session As Session)
            For Each msg As Message In eventObj
                Select Case (msg.MessageType.ToString())
                    Case "SessionStarted"
                        ' "Session Started"
                    Case "SessionTerminated"
                        ' "Session Terminated"
                    Case "SessionStopped"
                        ' "Session stopped"
                    Case "ServiceOpened"
                        ' "Service Opened"
                    Case "RequestFailure"
                        Dim cause As Element = msg.GetElement(REASON)
                        Dim msge As String = String.Concat("Error: Source-", cause.GetElementAsString(SOURCE), _
                            ", Code-", cause.GetElementAsString(ERROR_CODE), ", category-", cause.GetElementAsString(CATEGORY), _
                            ", desc-", cause.GetElementAsString(DESCRIPTION))
                        toolStripStatusLabel1.Text = msge
                    Case Else
                        toolStripStatusLabel1.Text = msg.MessageType.ToString()
                End Select
            Next
        End Sub
#End Region
    End Class
End Namespace