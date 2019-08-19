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
Imports CorrelationID = Bloomberglp.Blpapi.CorrelationID
Imports BDateTime = Bloomberglp.Blpapi.Datetime
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
        Private d_requestSecurity As String = String.Empty

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
            ' add columns to table
            If (IsNothing(d_data)) Then
                d_data = New DataTable()
            End If
            d_data.Columns.Add("security")
            d_data.Columns.Add("time")
            d_data.Columns.Add("type")
            d_data.Columns.Add("value")
            d_data.Columns.Add("size")
            d_data.Columns.Add("conditionCodes")
            d_data.Columns.Add("exchangeCode")
            d_data.AcceptChanges()
            ' set grid data source
            dataGridViewData.DataSource = d_data
            ' default to TRADE
            checkedListBoxEventTypes.SetItemChecked(0, True)
            ' Intraday request need the time to be in GMT.
            dateTimePickerStartDate.Value = System.DateTime.Now.ToUniversalTime().AddMinutes(-5)
            dateTimePickerEndDate.Value = System.DateTime.Now.ToUniversalTime()
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
            ' require at lease 4 characters, event type checked and valid time period to enable button
            buttonSendRequest.Enabled = textBoxSecurity.Text.Length > 4 And _
                                        checkedListBoxEventTypes.CheckedItems.Count > 0 And _
                                        dateTimePickerEndDate.Value > dateTimePickerStartDate.Value
            buttonClearAll.Enabled = listBoxSecurities.Items.Count > 0 Or d_data.Rows.Count > 0
        End Sub
        ''' <summary>
        ''' Clear security data
        ''' </summary>
        Private Sub clearSecurityData(ByVal sec As String)
            Dim rows As DataRow() = d_data.Select("security = '" + sec + "'")
            For Each row As DataRow In rows
                row.Delete()
            Next
            toolStripStatusLabel1.Text = String.Empty
        End Sub
        ''' <summary>
        ''' Remove all securities and fields from grid
        ''' </summary>
        Private Sub clearAll()
            d_data.Rows.Clear()
            d_data.AcceptChanges()
            listBoxSecurities.Items.Clear()
            dataGridViewData.DataSource = Nothing
            setControlStates()
            toolStripStatusLabel1.Text = String.Empty
        End Sub
#End Region

#Region "Control Events"
        ''' <summary>
        ''' Selected security to display data 
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub listBoxSecurities_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles listBoxSecurities.SelectedIndexChanged
            If (Not listBoxSecurities.SelectedIndex = -1) Then
                ' create table for security's intraday data
                Dim table As DataTable = d_data.Clone()
                ' get data for security
                Dim rows As DataRow() = d_data.Select("security = '" + listBoxSecurities.SelectedItem.ToString() + "'")
                For Each row As DataRow In rows
                    table.Rows.Add(row.ItemArray)
                Next
                ' display data
                dataGridViewData.DataSource = table
                For Each col As DataGridViewColumn In dataGridViewData.Columns
                    If (col.Name = "security") Then
                        col.Visible = False
                        col.SortMode = DataGridViewColumnSortMode.NotSortable
                    End If
                    dataGridViewData.Refresh()
                Next
            End If
        End Sub
        ''' <summary>
        ''' Validate security text length and send request
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub textBoxSecurity_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles textBoxSecurity.KeyDown
            setControlStates()
            If (e.KeyCode = Keys.Return And buttonSendRequest.Enabled) Then
                buttonSendRequest_Click(sender, New EventArgs())
            End If
        End Sub
        ''' <summary>
        ''' Select event type
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub checkedListBoxEventTypes_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles checkedListBoxEventTypes.SelectedIndexChanged
            setControlStates()
        End Sub
        ''' <summary>
        ''' date/time changed
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub dateTimePickerEndDate_ValueChanged(ByVal sender As Object, ByVal e As EventArgs) Handles dateTimePickerStartDate.ValueChanged, dateTimePickerEndDate.ValueChanged
            setControlStates()
        End Sub
        ''' <summary>
        ''' Remove securities and fields button
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub buttonClearAll_Click(ByVal sender As Object, ByVal e As EventArgs) Handles buttonClearAll.Click
            clearAll()
        End Sub
        ''' <summary>
        ''' Submit Intraday Tick Request
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub buttonSendRequest_Click(ByVal sender As Object, ByVal e As EventArgs) Handles buttonSendRequest.Click
            Dim startDate As System.DateTime = dateTimePickerStartDate.Value
            Dim endDate As System.DateTime = dateTimePickerEndDate.Value
            ' check time period size
            If (endDate.Subtract(startDate).Minutes > 30) Then
                If (MessageBox.Show("You have selected a large time period for the intraday tick request. Request may " + vbCrLf + _
                                    "take a long period of time to process depending on the amount data." + _
                                    vbCrLf + vbCrLf + "Do you wish to continue?", "Intraday Tick Request", _
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
                                    MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.No) Then
                    Return
                End If
            End If
            ' clear security data if requested before
            d_requestSecurity = textBoxSecurity.Text.Trim()
            clearSecurityData(d_requestSecurity)
            ' create session
            If (Not createSession()) Then
                toolStripStatusLabel1.Text = "Failed to start session."
                Return
            End If
            ' open reference data service
            If (Not d_session.OpenService("//blp/refdata")) Then
                toolStripStatusLabel1.Text = "Failed to open //blp/refdata"
                Return
            End If
            toolStripStatusLabel1.Text = "Connected sucessfully"
            Dim refDataService As Service = d_session.GetService("//blp/refdata")
            ' create intraday tick request
            Dim request As Request = refDataService.CreateRequest("IntradayTickRequest")
            ' set request parameters
            request.Set("includeConditionCodes", checkBoxIncludeConditionCode.Checked)
            request.Set("includeExchangeCodes", checkBoxIncludeExchangeCode.Checked)
            Dim eventTypes As Element = request.GetElement("eventTypes")
            For Each item As Object In checkedListBoxEventTypes.CheckedItems
                eventTypes.AppendValue(item.ToString())
            Next
            request.Set("security", d_requestSecurity)
            request.Set("startDateTime", New BDateTime(startDate.Year, startDate.Month, startDate.Day, _
                    startDate.Hour, startDate.Minute, startDate.Second, 0))
            request.Set("endDateTime", New BDateTime(endDate.Year, endDate.Month, endDate.Day, _
                endDate.Hour, endDate.Minute, endDate.Second, 0))
            ' create correlation id
            Dim cID As CorrelationID = New CorrelationID(1)
            d_session.Cancel(cID)
            ' send request
            d_session.SendRequest(request, cID)
            toolStripStatusLabel1.Text = "Submitted request. Waiting for response..."
            If (radioButtonSynch.Checked) Then
                ' allow UI to update
                Application.DoEvents()
                ' synchronous mode. Wait for reply before proceeding.
                Do
                    Dim eventObj As [Event] = d_session.NextEvent()
                    toolStripStatusLabel1.Text = "Processing data..."
                    Application.DoEvents()
                    processEvent(eventObj, d_session)
                    If (eventObj.Type = [Event].EventType.RESPONSE) Then
                        Exit Do
                    End If
                Loop
                ' select requested security in listbox to disply data
                setControlStates()
                Dim itemIndex As Integer = -1
                If (Not listBoxSecurities.Items.Contains(d_requestSecurity)) Then
                    itemIndex = listBoxSecurities.Items.Add(d_requestSecurity)
                Else
                    listBoxSecurities.SelectedIndex = -1
                    itemIndex = listBoxSecurities.Items.IndexOf(d_requestSecurity)
                End If
                listBoxSecurities.SelectedIndex = itemIndex
                toolStripStatusLabel1.Text = "Completed"
            End If
        End Sub
#End Region

#Region "Bloomberg API Event and data processing"
        ''' <summary>
        ''' Data event
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
                            ' process final respose for request
                            processRequestDataEvent(eventObj, session)
                            ' display data
                            setControlStates()
                            Dim itemIndex As Integer = -1
                            If (Not listBoxSecurities.Items.Contains(d_requestSecurity)) Then
                                itemIndex = listBoxSecurities.Items.Add(d_requestSecurity)
                            Else
                                listBoxSecurities.SelectedIndex = -1
                                itemIndex = listBoxSecurities.Items.IndexOf(d_requestSecurity)
                            End If
                            listBoxSecurities.SelectedIndex = itemIndex
                            toolStripStatusLabel1.Text = "Completed"
                        Case [Event].EventType.PARTIAL_RESPONSE
                            ' process partial response
                            processRequestDataEvent(eventObj, session)
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
            d_data.BeginLoadData()
            ' process message
            For Each msg As Message In eventObj
                If (msg.MessageType.Equals(Bloomberglp.Blpapi.Name.GetName("IntradayTickResponse"))) Then
                    If (msg.HasElement(RESPONSE_ERROR)) Then
                        ' process error
                        Dim err As Element = msg.GetElement(RESPONSE_ERROR)
                        If (msg.NumElements = 1) Then
                            d_data.Rows.Add(New Object() {d_requestSecurity, err.GetElementAsString(MESSAGE)})
                            Return
                        End If
                    End If
                    ' process tick data
                    Dim tickDataArray As Element = msg.GetElement("tickData")
                    Dim numberOfTicks As Integer = tickDataArray.NumValues
                    For Each tickData As Element In tickDataArray.Elements
                        If (tickData.Name.ToString() = "tickData") Then
                            For pointIndex As Integer = 0 To tickData.NumValues - 1
                                Dim fieldIndex As Integer = 0
                                Dim dataValues As Object()
                                ReDim dataValues(d_data.Columns.Count - 1)
                                Dim fields As Element = tickData.GetValueAsElement(pointIndex)
                                For Each col As DataColumn In d_data.Columns
                                    If (fields.HasElement(col.ColumnName)) Then
                                        ' tick field data
                                        Dim item As Element = fields.GetElement(col.ColumnName)
                                        dataValues(fieldIndex) = item.GetValueAsString()
                                    Else
                                        If (col.ColumnName = "security") Then
                                            dataValues(fieldIndex) = d_requestSecurity
                                        Else
                                            dataValues(fieldIndex) = DBNull.Value
                                        End If
                                    End If
                                    fieldIndex += 1
                                Next
                                ' save tick data
                                d_data.Rows.Add(dataValues)
                            Next
                        End If
                    Next
                End If
            Next
            d_data.EndLoadData()
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