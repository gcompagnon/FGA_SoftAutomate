' ==========================================================
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT 
'  WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED,    
' INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES   
' OF MERCHANTABILITY AND/OR FITNESS FOR A  PARTICULAR   
' PURPOSE.								
' ==========================================================
' Purpose of this example:
' - Make asynchronous and synchronous reference data
'   request using //blp/refdata service.
' - Set request override fields.
' - Retrieve bulk data.
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
        Private d_bulkData As DataSet
        Private d_numberOfReturnedSecurities As Integer = 0

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
            ' add columns to data table
            If (IsNothing(d_data)) Then
                d_data = New DataTable()
                d_data.Columns.Add("security")
                d_data.AcceptChanges()
                ' set grid data source
                dataGridViewData.DataSource = d_data
            End If
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
                    ' add fields
                    d_data.Rows.Add(security.Trim())
                End If
            Next
            setControlStates()
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
                '// create synchronous session
                d_session = New Session(d_sessionOptions)
            End If
            Return d_session.Start()
        End Function
        ''' <summary>
        ''' Manage control states
        ''' </summary>
        Private Sub setControlStates()
            buttonSendRequest.Enabled = d_data.Rows.Count > 0 And d_data.Columns.Count > 1
            buttonClearFields.Enabled = d_data.Columns.Count > 1
            buttonClearData.Enabled = buttonSendRequest.Enabled
            buttonClearAll.Enabled = d_data.Rows.Count > 0 Or d_data.Columns.Count > 1
        End Sub
        ''' <summary>
        ''' Clear security data
        ''' </summary>
        Private Sub clearData()
            ' clear security count
            d_numberOfReturnedSecurities = 0
            ' clear bulk data
            If (Not IsNothing(d_bulkData)) Then
                d_bulkData.Clear()
                d_bulkData.AcceptChanges()
            End If
            If (Not IsNothing(d_data)) Then
                For Each row As DataRow In d_data.Rows
                    For index As Integer = 1 To d_data.Columns.Count - 1
                        row(index) = DBNull.Value
                    Next
                Next
                d_data.AcceptChanges()
            End If
            toolStripStatusLabel1.Text = String.Empty
        End Sub
        ''' <summary>
        ''' Remove all fields from grid
        ''' </summary>
        Private Sub clearFields()
            For index As Integer = d_data.Columns.Count - 1 To 1 Step -1
                d_data.Columns.RemoveAt(index)
            Next
            d_data.AcceptChanges()
            toolStripStatusLabel1.Text = String.Empty
        End Sub
        ''' <summary>
        ''' Remove all securities and fields from grid
        ''' </summary>
        Private Sub clearAll()
            If (Not IsNothing(d_bulkData)) Then
                d_bulkData.Clear()
                d_bulkData.AcceptChanges()
            End If
            clearFields()
            d_data.Rows.Clear()
            d_data.AcceptChanges()
            listViewOverrides.Items.Clear()
            setControlStates()
        End Sub
#End Region

#Region "Control Events"
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
        ''' Enter key pressed to add override field
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub textBoxOverride_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles textBoxOverride.KeyDown
            If (e.KeyCode = Keys.Return) Then
                buttonAddOverride_Click(sender, New EventArgs())
            End If
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
            For Each row As DataRow In d_data.Rows
                For index As Integer = 1 To d_data.Columns.Count - 1
                    row(index) = DBNull.Value
                Next
            Next
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
        ''' Add override field to list
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub buttonAddOverride_Click(ByVal sender As Object, ByVal e As EventArgs) Handles buttonAddOverride.Click
            If (textBoxOverride.Text.Length = 0 Or Not textBoxOverride.Text.Contains("=")) Then
                MessageBox.Show("Missing field or missing '=' seperator between field and value", "Add Field", MessageBoxButtons.OK, MessageBoxIcon.Information)
                textBoxOverride.Focus()
            Else
                Dim input As String() = textBoxOverride.Text.Split(New Char() {","c})
                For Each overrideItem As String In input
                    ' only accept field with value
                    If (overrideItem.Trim().Length > 0 And overrideItem.Contains("=")) Then
                        Dim ovr As String() = overrideItem.Split(New Char() {"="c})
                        Dim item As ListViewItem = listViewOverrides.Items.Add(ovr(0).Trim())
                        item.SubItems.Add(ovr(1).Trim())
                    End If
                Next
                textBoxOverride.Text = String.Empty
            End If
        End Sub
        ''' <summary>
        ''' Drag and drop override fields and values
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub listViewOverrides_DragDrop(ByVal sender As Object, ByVal e As DragEventArgs) Handles listViewOverrides.DragDrop
            ' Get the entire text object that has been dropped on us.
            Dim tmp As String = e.Data.GetData(DataFormats.Text).ToString()
            Dim values As List(Of String) = New List(Of String)()
            Dim fields As List(Of String) = New List(Of String)()
            ' Tokenize the string into what (we hope) are Security strings
            Dim sep As Char() = {vbCrLf, vbTab}
            Dim words As String() = tmp.Split(sep)
            For Each sec As String In words
                If (sec.Contains("=")) Then
                    Dim ovr As String() = sec.Split(New Char() {"="c})
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
        Private Sub listViewOverrides_DragEnter(ByVal sender As Object, ByVal e As DragEventArgs) Handles listViewOverrides.DragEnter
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
        Private Sub listViewOverrides_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles listViewOverrides.KeyDown
            If (e.KeyData = Keys.Delete And listViewOverrides.SelectedItems.Count > 0) Then
                For Each item As ListViewItem In listViewOverrides.SelectedItems
                    item.Remove()
                Next
            End If
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
                        d_data.Rows.Add(New Object() {sec.Trim()})
                    Else
                        ' add fields
                        d_data.Columns.Add(sec.Trim())
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
                e.Effect = DragDropEffects.Scroll
            End If
        End Sub
        ''' <summary>
        ''' Display bulk data
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub dataGridViewData_CellMouseClick(ByVal sender As Object, ByVal e As DataGridViewCellMouseEventArgs) Handles dataGridViewData.CellMouseClick
            Try
                Dim security As String = dataGridViewData.Rows(e.RowIndex).Cells("security").Value.ToString()
                Dim field As String = dataGridViewData.Columns(e.ColumnIndex).Name.ToString()
                Dim cellData = dataGridViewData.Rows(e.RowIndex).Cells(field).Value.ToString()
                If (Not cellData = "Bulk Data...") Then
                    Return
                End If
                ' create bulk data table for display
                Dim bulkTable As DataTable = d_bulkData.Tables(field).Clone()
                bulkTable.TableName = "BulkData"
                ' Get bulk data
                Dim rows As DataRow() = d_bulkData.Tables(field).Select("security = '" + security + "'")
                For Each row As DataRow In rows
                    bulkTable.ImportRow(row)
                Next
                ' Display data
                Dim bulkData As FormBulkData = New FormBulkData(bulkTable)
                bulkData.ShowDialog(Me)
            Catch ex As Exception
                toolStripStatusLabel1.Text = ex.Message.ToString()
            End Try
        End Sub
        ''' <summary>
        ''' Allow user to delete single field or security from grid
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub dataGridViewData_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles dataGridViewData.KeyDown
            Dim dataGrid As DataGridView = CType(sender, DataGridView)
            If (e.KeyData = Keys.Delete And dataGrid.SelectedCells.Count > 0) Then
                Dim rowIndex As Integer = dataGrid.SelectedCells(0).RowIndex
                Dim columnIndex As Integer = dataGrid.SelectedCells(0).ColumnIndex
                If (columnIndex > 0) Then
                    ' remove field
                    d_data.Columns.RemoveAt(columnIndex)
                Else
                    ' remove security
                    d_data.Rows.RemoveAt(rowIndex)
                End If
                ' update data
                d_data.AcceptChanges()
                If (dataGrid.Columns.Count > columnIndex And columnIndex > 0) Then
                    dataGrid.Rows(rowIndex).Cells(columnIndex).Selected = True
                Else
                    If (dataGrid.Columns.Count > 1 And dataGrid.Columns.Count = columnIndex) Then
                        dataGrid.Rows(rowIndex).Cells(columnIndex - 1).Selected = True
                    End If
                End If
            End If
        End Sub
        ''' <summary>
        ''' Send reference data request
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
            ' open reference data service
            If (Not d_session.OpenService("//blp/refdata")) Then
                toolStripStatusLabel1.Text = "Failed to open //blp/refdata"
                Return
            End If
            toolStripStatusLabel1.Text = "Connected sucessfully"
            Dim refDataService As Service = d_session.GetService("//blp/refdata")
            ' create reference data request
            Dim request As Request = refDataService.CreateRequest("ReferenceDataRequest")
            ' set request parameters
            Dim securities As Element = request.GetElement("securities")
            Dim fields As Element = request.GetElement("fields")
            Dim requestOverrides As Element = request.GetElement("overrides")
            request.Set("returnEids", True)
            ' populate security
            For Each secRow As DataRow In d_data.Rows
                securities.AppendValue(secRow("security").ToString())
            Next
            ' populate fields
            For fieldIndex As Integer = 1 To d_data.Columns.Count - 1
                fields.AppendValue(d_data.Columns(fieldIndex).ColumnName)
            Next
            If (listViewOverrides.Items.Count > 0) Then
                ' populate overrides
                For Each item As ListViewItem In listViewOverrides.Items
                    Dim ovr As Element = requestOverrides.AppendElement()
                    ovr.SetElement(FIELD_ID, item.Text)
                    ovr.SetElement("value", item.SubItems(1).Text)
                Next
            End If
            ' create correlation id
            Dim cID As CorrelationID = New CorrelationID(1)
            d_session.Cancel(cID)
            ' send request
            d_session.SendRequest(request, cID)
            toolStripStatusLabel1.Text = "Submitted request. Waiting for response..."
            If (radioButtonSynch.Checked) Then
                ' Allow UI to update
                Application.DoEvents()
                ' Synchronous mode. Wait for reply before proceeding.
                Do
                    Dim eventObj As [Event] = d_session.NextEvent()
                    toolStripStatusLabel1.Text = "Processing data..."
                    ' process data
                    processEvent(eventObj, d_session)
                    If (eventObj.Type = [Event].EventType.RESPONSE) Then
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
                            setControlStates()
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
            If (d_numberOfReturnedSecurities = 0) Then
                toolStripStatusLabel1.Text = "Processing data..."
            End If
            d_data.BeginLoadData()
            ' process message
            For Each msg As Message In eventObj
                ' get message correlation id
                Dim cId As Integer = CType(msg.CorrelationID.Value, Integer)
                If (msg.MessageType.Equals(Bloomberglp.Blpapi.Name.GetName("ReferenceDataResponse"))) Then
                    ' process security data
                    Dim secDataArray As Element = msg.GetElement(SECURITY_DATA)
                    Dim numberOfSecurities As Integer = secDataArray.NumValues
                    For index As Integer = 0 To numberOfSecurities - 1
                        Dim secData As Element = secDataArray.GetValueAsElement(index)
                        Dim fieldData As Element = secData.GetElement("fieldData")
                        d_numberOfReturnedSecurities += 1
                        ' get security index
                        Dim rowIndex As Integer = secData.GetElementAsInt32("sequenceNumber")
                        If (d_data.Rows.Count > rowIndex) Then
                            ' get security record
                            Dim row As DataRow = d_data.Rows(rowIndex)
                            ' check for field error
                            If (secData.HasElement(FIELD_EXCEPTIONS)) Then
                                ' process error
                                Dim err As Element = secData.GetElement(FIELD_EXCEPTIONS)
                                For errorIndex As Integer = 0 To err.NumValues - 1
                                    Dim errorException As Element = err.GetValueAsElement(errorIndex)
                                    Dim field As String = errorException.GetElementAsString(FIELD_ID)
                                    Dim errorInfo As Element = errorException.GetElement(ERROR_INFO)
                                    Dim msge As String = errorInfo.GetElementAsString(MESSAGE)
                                    row(field) = msge
                                Next
                            End If
                            ' check for security error
                            If (secData.HasElement(SECURITY_ERROR)) Then
                                Dim err As Element = secData.GetElement(SECURITY_ERROR)
                                Dim errorMessage As String = err.GetElementAsString(MESSAGE)
                                row(1) = errorMessage
                            End If
                            ' process data
                            For Each col As DataColumn In d_data.Columns
                                Dim dataValue As String = String.Empty
                                If (fieldData.HasElement(col.ColumnName)) Then
                                    Dim item As Element = fieldData.GetElement(col.ColumnName)
                                    If (item.IsArray) Then
                                        ' bulk field
                                        dataValue = "Bulk Data..."
                                        processBulkData(secData.GetElementAsString("security"), item)
                                    Else
                                        dataValue = item.GetValueAsString()
                                    End If
                                    row(col.ColumnName) = dataValue
                                End If
                            Next
                        End If
                    Next
                End If
            Next
            d_data.EndLoadData()
            ' check if we are done
            If (d_numberOfReturnedSecurities >= d_data.Rows.Count) Then
                toolStripStatusLabel1.Text = "Completed"
            End If
        End Sub
        ''' <summary>
        ''' Process bulk data
        ''' </summary>
        ''' <param name="security"></param>
        ''' <param name="data"></param>
        Private Sub processBulkData(ByVal security As String, ByVal data As Element)
            Dim bulkTable As DataTable = Nothing
            ' bulk data dataset
            If (IsNothing(d_bulkData)) Then
                d_bulkData = New DataSet()
            End If
            ' get bulk data
            Dim bulk As Element = data.GetValueAsElement(0)
            If (d_bulkData.Tables.Contains(bulk.Name.ToString())) Then
                ' get existing bulk data table
                bulkTable = d_bulkData.Tables(bulk.Name.ToString())
            Else
                ' create new bulk data table
                bulkTable = d_bulkData.Tables.Add(bulk.Name.ToString())
            End If
            ' create column if not already exist
            If (Not bulkTable.Columns.Contains("security")) Then
                bulkTable.Columns.Add(New DataColumn("security", GetType(String)))
                bulkTable.Columns.Add(New DataColumn("Id", GetType(Integer)))
            End If
            ' create columns in data table for bulk data
            For Each item As Element In bulk.Elements
                If (Not bulkTable.Columns.Contains(item.Name.ToString())) Then
                    bulkTable.Columns.Add(New DataColumn(item.Name.ToString(), GetType(String)))
                End If
            Next
            ' populate bulk
            Dim count As Integer = 0
            For index As Integer = 0 To data.NumValues - 1
                bulk = data.GetValueAsElement(index)
                Dim dataArray() As Object
                ReDim dataArray(bulk.NumElements + 1)
                dataArray(0) = security
                dataArray(1) = count
                Dim dataIndex As Integer = 2
                For Each item As Element In bulk.Elements
                    dataArray(dataIndex) = item.GetValueAsString()
                    dataIndex += 1
                Next
                bulkTable.Rows.Add(dataArray)
                count += 1
            Next
        End Sub

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