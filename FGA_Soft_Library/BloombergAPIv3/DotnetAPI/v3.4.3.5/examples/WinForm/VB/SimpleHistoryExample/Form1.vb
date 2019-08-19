' ==========================================================
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT 
'  WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED,    
' INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES   
' OF MERCHANTABILITY AND/OR FITNESS FOR A  PARTICULAR   
' PURPOSE.								
' ==========================================================
' Purpose of this example:
' - Make asynchronous and synchronous historical request
'   using //blp/refdata service.
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
        Private d_data As DataTable

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
        ''' <remarks></remarks>
        Private Sub initUI()
            Const security As String = "security"
            Const [date] As String = "date"

            textBoxCurrencyCode.Text = "USD"
            textBoxMaxPoints.Text = "1000"
            comboBoxPricing.SelectedIndex = 0
            comboBoxNonTradingDayMethod.SelectedIndex = 1
            comboBoxNonTradingDayValue.SelectedIndex = 2
            comboBoxPeriodicityAdjustment.SelectedIndex = 0
            comboBoxOverrideOption.SelectedIndex = 0
            dateTimePickerStart.Value = System.DateTime.Now.AddDays(-1)

            ' add columns to grid
            If IsNothing(d_data) Then
                d_data = New DataTable()
                d_data.Columns.Add(security)
                d_data.Columns.Add([date])
                dataGridViewData.DataSource = d_data
                dataGridViewData.Columns(security).SortMode = DataGridViewColumnSortMode.NotSortable
            End If
        End Sub
        ''' <summary>
        ''' Add fields
        ''' </summary>
        ''' <param name="fields"></param>
        Private Sub addFields(ByVal fields As String)
            ' Tokenize the string into what (we hope) are Security strings
            Dim sep As Char() = {vbCrLf, vbTab, ","c}
            Dim words As String() = fields.Split(sep)

            For Each field As String In words
                If (field.Trim().Length > 0) Then
                    ' add fields
                    If (Not d_data.Columns.Contains(field.Trim())) Then
                        d_data.Columns.Add(field.Trim())
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
            If (Not IsNothing(d_session)) Then
                d_session.Stop()
            End If
            toolStripStatusLabel1.Text = "Connecting..."
            If (radioButtonAsynch.Checked) Then
                ' create asynchronous session
                d_session = New Session(d_sessionOptions, New Bloomberglp.Blpapi.EventHandler(AddressOf processEvent))
            Else
                ' create synchronous session
                d_session = New Session(d_sessionOptions)
            End If
            Return d_session.Start()
        End Function
        ''' <summary>
        ''' Manage control states
        ''' </summary>
        Private Sub setControlStates()
            ' require at lease 4 characters to enable buttons
            If (textBoxSecurity.Text.Length > 4) Then
                labelField.Enabled = True
                buttonSendRequest.Enabled = (dataGridViewData.ColumnCount > 2)
                buttonClearFields.Enabled = buttonSendRequest.Enabled
            Else
                labelField.Enabled = False
                buttonSendRequest.Enabled = labelField.Enabled
                buttonClearFields.Enabled = buttonSendRequest.Enabled
            End If
            buttonClearData.Enabled = dataGridViewData.RowCount > 0
            textBoxField.Enabled = labelField.Enabled
            buttonAddFields.Enabled = labelField.Enabled
        End Sub
        ''' <summary>
        ''' Clear security data
        ''' </summary>
        Private Sub clearData()
            d_data.Rows.Clear()
            d_data.AcceptChanges()
            toolStripStatusLabel1.Text = String.Empty
        End Sub
#End Region

#Region "Control Events"
        ''' <summary>
        ''' Update control states
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub textBoxSecurity_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles textBoxSecurity.KeyDown
            setControlStates()
        End Sub
        ''' <summary>
        ''' dd field button
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub buttonAddFields_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles buttonAddFields.Click
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
                buttonAddFields_Click(sender, New EventArgs())
            End If
        End Sub
        ''' <summary>
        ''' Allow drag and drop of fields
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub dataGridViewData_DragDrop(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles dataGridViewData.DragDrop
            ' Get the entire text object that has been dropped on us.
            Dim tmp As String = e.Data.GetData(DataFormats.Text).ToString()
            addFields(tmp)
        End Sub
        ''' <summary>
        ''' Mouse drag over grid
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub dataGridViewData_DragEnter(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles dataGridViewData.DragEnter
            If (buttonAddFields.Enabled) Then
                If (e.Data.GetDataPresent(DataFormats.Text)) Then
                    e.Effect = DragDropEffects.Copy
                Else
                    e.Effect = DragDropEffects.None
                End If
            End If
        End Sub
        ''' <summary>
        ''' Allow user to delete single field or security from grid
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub dataGridViewData_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles dataGridViewData.KeyDown
            If (e.KeyData = Keys.Delete And dataGridViewData.SelectedCells.Count > 0) Then
                Dim rowIndex As Integer = dataGridViewData.SelectedCells(0).RowIndex
                Dim columnIndex As Integer = dataGridViewData.SelectedCells(0).ColumnIndex
                If (columnIndex > 1) Then
                    ' remove field
                    d_data.Columns.RemoveAt(columnIndex)
                    d_data.AcceptChanges()
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
        ''' Remove all fields button
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub buttonClearFields_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles buttonClearFields.Click
            For index As Integer = d_data.Columns.Count - 1 To 2 Step -1
                d_data.Columns.RemoveAt(index)
            Next
            clearData()
            d_data.AcceptChanges()
            setControlStates()
        End Sub
        ''' <summary>
        ''' Clear data button
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub buttonClearData_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles buttonClearData.Click
            clearData()
            setControlStates()
        End Sub
        ''' <summary>
        ''' Select periodicity adjustment and setup list of periodicity selection
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub comboBoxPeriodicityAdjustment_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles comboBoxPeriodicityAdjustment.SelectedIndexChanged
            Dim actualList As Object() = New Object() {"DAILY", "WEEKLY", "MONTHLY", "QUARTERLY", _
                "SEMI_ANNUALLY", "YEARLY"}
            Dim fiscalList As Object() = New Object() {"QUARTERLY", "SEMI_ANNUALLY", "YEARLY"}
            comboBoxPeriodicitySelection.Items.Clear()
            ' update list according to selection
            Select Case (comboBoxPeriodicityAdjustment.SelectedItem.ToString())
                Case "FISCAL"
                    comboBoxPeriodicitySelection.Items.AddRange(fiscalList)
                Case Else
                    comboBoxPeriodicitySelection.Items.AddRange(actualList)
            End Select
            comboBoxPeriodicitySelection.SelectedIndex = 0
        End Sub
        ''' <summary>
        ''' Send historical request
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub buttonSendRequest_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles buttonSendRequest.Click
            ' clear data before submitting request
            clearData()
            If (Not createSession()) Then
                toolStripStatusLabel1.Text = "Failed to start session."
                Return
            End If
            ' start reference data service
            If (Not d_session.OpenService("//blp/refdata")) Then
                toolStripStatusLabel1.Text = "Failed to open //blp/refdata"
                Return
            End If
            toolStripStatusLabel1.Text = "Connected sucessfully"
            ' get refdata service
            Dim refDataService As Service = d_session.GetService("//blp/refdata")
            ' create historical request
            Dim request As Request = refDataService.CreateRequest("HistoricalDataRequest")
            ' set security to request
            Dim securities As Element = request.GetElement("securities")
            securities.AppendValue(textBoxSecurity.Text)
            ' set fields to request
            Dim fields As Element = request.GetElement("fields")
            For fieldIndex As Integer = 2 To dataGridViewData.ColumnCount - 1
                fields.AppendValue(dataGridViewData.Columns(fieldIndex).Name)
            Next
            ' set historical request properties
            request.Set("periodicityAdjustment", comboBoxPeriodicityAdjustment.SelectedItem.ToString())
            request.Set("periodicitySelection", comboBoxPeriodicitySelection.SelectedItem.ToString())
            request.Set("currency", textBoxCurrencyCode.Text.Trim())
            If (tabControlDates.SelectedIndex = 0) Then
                request.Set("startDate", dateTimePickerStart.Value.ToString("yyyyMMdd"))
                request.Set("endDate", dateTimePickerEndDate.Value.ToString("yyyyMMdd"))

            Else
                request.Set("startDate", textBoxRelStartDate.Text.ToUpper().Trim())
                request.Set("endDate", textBoxRelEndDate.Text.ToUpper().Trim())
            End If
            Dim nonTradingDayValue As String = String.Empty
            Select Case (comboBoxNonTradingDayValue.SelectedIndex)
                Case 0
                    nonTradingDayValue = "NON_TRADING_WEEKDAYS"
                Case 1
                    nonTradingDayValue = "ALL_CALENDAR_DAYS"
                Case 2
                    nonTradingDayValue = "ACTIVE_DAYS_ONLY"
            End Select
            request.Set("nonTradingDayFillOption", nonTradingDayValue)
            Dim nonTradingDayMethod As String = String.Empty
            Select Case (comboBoxNonTradingDayMethod.SelectedIndex)
                Case 0
                    nonTradingDayMethod = "NIL_VALUE"
                Case 1
                    nonTradingDayMethod = "PREVIOUS_VALUE"
            End Select
            request.Set("nonTradingDayFillMethod", nonTradingDayMethod)
            Dim overrideOption As String = String.Empty
            Select Case (comboBoxOverrideOption.SelectedIndex)
                Case 0
                    overrideOption = "OVERRIDE_OPTION_CLOSE"
                Case 1
                    overrideOption = "OVERRIDE_OPTION_GPA"
            End Select
            request.Set("overrideOption", overrideOption)
            request.Set("maxDataPoints", textBoxMaxPoints.Text)
            request.Set("returnEids", True)
            ' send request
            d_session.SendRequest(request, Nothing)
            toolStripStatusLabel1.Text = "Submitted request. Waiting for response..."

            If (radioButtonSynch.Checked) Then
                ' synchronous request
                Application.DoEvents()
                Do While (True)
                    ' process data
                    Dim eventObj As [Event] = d_session.NextEvent()
                    toolStripStatusLabel1.Text = "Processing data..."
                    processEvent(eventObj, d_session)
                    If (eventObj.Type = [Event].EventType.RESPONSE) Then
                        ' response completed
                        Exit Do
                    End If
                Loop
                setControlStates()
                toolStripStatusLabel1.Text = "Completed"
            End If
        End Sub
#End Region

#Region "Bloomberg API Event and data processing"
        ''' <summary>
        ''' Bloomberg data event
        ''' </summary>
        ''' <param name="eventObj"></param>
        ''' <param name="session"></param>
        Private Sub processEvent(ByVal eventObj As [Event], ByVal session As Session)
            If (InvokeRequired) Then
                Invoke(New Bloomberglp.Blpapi.EventHandler(AddressOf processEvent), New Object() {eventObj, session})
            Else
                Try
                    Select Case (eventObj.Type)
                        Case [Event].EventType.RESPONSE
                            ' process data
                            processRequestDataEvent(eventObj, session)
                            setControlStates()
                            toolStripStatusLabel1.Text = "Completed"
                        Case [Event].EventType.PARTIAL_RESPONSE
                            ' process partial data
                            processRequestDataEvent(eventObj, session)
                        Case Else
                            ' process misc events
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
            Dim securityName As String = String.Empty
            Dim hasFieldError As Boolean = False
            ' clear column tag of field error message
            For Each col As DataGridViewColumn In dataGridViewData.Columns
                col.Tag = Nothing
            Next
            ' process message
            For Each msg As Message In eventObj
                If (msg.MessageType.Equals(Bloomberglp.Blpapi.Name.GetName("HistoricalDataResponse"))) Then
                    ' process errors
                    If (msg.HasElement(RESPONSE_ERROR)) Then
                        Dim cause As Element = msg.GetElement(RESPONSE_ERROR)
                        dataGridViewData.Rows.Add(New Object() {cause.GetElementAsString(MESSAGE)})
                    Else
                        Dim secDataArray As Element = msg.GetElement(SECURITY_DATA)
                        Dim numberOfSecurities As Integer = secDataArray.NumValues
                        ' security error
                        If (secDataArray.HasElement(SECURITY_ERROR)) Then
                            ' security error
                            Dim secError As Element = secDataArray.GetElement(SECURITY_ERROR)
                            dataGridViewData.Rows.Add(New Object() {secError.GetElementAsString(MESSAGE)})
                        End If
                        ' field error
                        If (secDataArray.HasElement(FIELD_EXCEPTIONS)) Then
                            ' field error
                            Dim cause = secDataArray.GetElement(FIELD_EXCEPTIONS)
                            For errorIndex As Integer = 0 To cause.NumValues - 1
                                Dim errorException As Element = cause.GetValueAsElement(errorIndex)
                                Dim field As String = errorException.GetElementAsString(FIELD_ID)
                                Dim errorInfo As Element = errorException.GetElement(ERROR_INFO)
                                Dim msge As String = errorInfo.GetElementAsString(MESSAGE)
                                dataGridViewData.Columns(field).Tag = msge
                                hasFieldError = True
                            Next
                        End If
                        ' process securities
                        For index As Integer = 0 To numberOfSecurities - 1
                            For Each secData As Element In secDataArray.Elements
                                Select Case (secData.Name.ToString())
                                    Case "eidsData"
                                        ' process security eid data here
                                    Case "security"
                                        ' security name
                                        securityName = secData.GetValueAsString()
                                    Case "fieldData"
                                        If (hasFieldError And secData.NumValues = 0) Then
                                            ' no data but have field error
                                            Dim dataValues(dataGridViewData.ColumnCount - 1) As Object
                                            dataValues(0) = securityName
                                            Dim fieldIndex As Integer = 0
                                            For Each col As DataGridViewColumn In dataGridViewData.Columns
                                                If (Not IsNothing(col.Tag)) Then
                                                    dataValues(fieldIndex) = col.Tag.ToString()
                                                End If
                                                fieldIndex += 1
                                            Next
                                            d_data.Rows.Add(dataValues)
                                        Else
                                            ' get field data
                                            d_data.BeginLoadData()
                                            For pointIndex As Integer = 0 To secData.NumValues - 1
                                                Dim fieldIndex As Integer = 0
                                                Dim dataValues As Object()
                                                ReDim dataValues(dataGridViewData.ColumnCount - 1)
                                                Dim fields As Element = secData.GetValueAsElement(pointIndex)
                                                For Each col As DataGridViewColumn In dataGridViewData.Columns
                                                    Try
                                                        If (col.Name = "security") Then
                                                            dataValues(fieldIndex) = securityName
                                                        Else
                                                            If (fields.HasElement(col.Name)) Then
                                                                Dim item As Element = fields.GetElement(col.Name)
                                                                If (item.IsArray) Then
                                                                    ' bulk field data
                                                                    dataValues(fieldIndex) = "Bulk Data"
                                                                Else
                                                                    ' field data
                                                                    dataValues(fieldIndex) = item.GetValueAsString()
                                                                End If
                                                            Else
                                                                ' no field value
                                                                If (col.Tag Is Nothing) Then
                                                                    dataValues(fieldIndex) = DBNull.Value
                                                                ElseIf (col.Tag.ToString().Length > 0) Then
                                                                    ' field has error
                                                                    dataValues(fieldIndex) = col.Tag.ToString()
                                                                Else
                                                                    dataValues(fieldIndex) = DBNull.Value
                                                                End If
                                                            End If
                                                        End If
                                                    Catch ex As Exception
                                                        ' display error message
                                                        dataValues(fieldIndex) = ex.Message
                                                    Finally
                                                        fieldIndex += 1
                                                    End Try
                                                Next
                                                ' add data to data table
                                                d_data.Rows.Add(dataValues)
                                            Next
                                            d_data.EndLoadData()
                                        End If
                                End Select
                            Next
                        Next
                    End If
                End If
            Next
        End Sub

        ''' <summary>
        ''' Request status event
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