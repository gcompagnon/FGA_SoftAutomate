' ==========================================================
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT 
'  WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED,    
' INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES   
' OF MERCHANTABILITY AND/OR FITNESS FOR A  PARTICULAR   
' PURPOSE.								
' ==========================================================
' Purpose of this example:
' - Make asynchronous and synchronous historical request
'   using //blp/apiflds service.
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
Imports InvalidRequestException = Bloomberglp.Blpapi.InvalidRequestException

Namespace Bloomberglp.Blpapi.Examples
    Public Class Form1
        Private ReadOnly REASON As Name = New Name("reason")
        Private ReadOnly CATEGORY As Name = New Name("category")
        Private ReadOnly DESCRIPTION As Name = New Name("description")
        Private ReadOnly ERROR_CODE As Name = New Name("errorCode")
        Private ReadOnly SOURCE As Name = New Name("source")

        Private d_sessionOptions As SessionOptions
        Private d_session As Session
        Private d_service As Service
        Private d_data As DataSet
        Private d_overrideDataTemp As DataTable
        Private d_fieldIds As List(Of String) = Nothing

#Region "properties"
        Private Property overrideFieldsTempTable() As DataTable
            Get
                Return d_overrideDataTemp
            End Get

            Set(ByVal value As DataTable)
                d_overrideDataTemp = value
            End Set
        End Property

        Private ReadOnly Property fieldTable() As DataTable
            Get
                Return d_data.Tables("fieldData")
            End Get
        End Property

        Private ReadOnly Property propertyTable() As DataTable
            Get
                Return d_data.Tables("fieldPropertyData")
            End Get
        End Property

        Private ReadOnly Property overrideTable() As DataTable
            Get
                Return d_data.Tables("fieldOverrideData")
            End Get
        End Property
#End Region

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
            ' init control
            comboBoxSearchType.SelectedIndex = 0
            comboBoxIncludeFieldType.SelectedIndex = 0
            comboBoxIncludeProductType.SelectedIndex = 0
            comboBoxExcludeFieldType.SelectedIndex = 0
            comboBoxExcludeProductType.SelectedIndex = 0

            ' add columns to grid
            If IsNothing(d_data) Then
                initDataTable()
            End If
            dataGridViewDataView.DataSource = d_data
            dataGridViewDataView.DataMember = "fieldData"
            dataGridViewDataView.Columns(2).Width = 300
            dataGridViewDataView.Columns(3).Width = 500

            ' start connection
            If (createSession()) Then
                toolStripStatusLabel1.Text = "Session started"
            Else
                buttonSubmitSearch.Enabled = False
                toolStripStatusLabel1.Text = "Session failed to start. Please close example and try again."
            End If
        End Sub

        ''' <summary>
        ''' Create data tables
        ''' </summary>
        Public Sub initDataTable()
            If (IsNothing(d_data)) Then
                d_data = New DataSet()
            End If

            d_data.Tables.Add("fieldData")
            Dim col As DataColumn = fieldTable.Columns.Add("Id")
            col = fieldTable.Columns.Add("mnemonic")
            col = fieldTable.Columns.Add("description")
            col = fieldTable.Columns.Add("categoryName")

            d_data.Tables.Add("fieldPropertyData")
            col = propertyTable.Columns.Add("id")
            col = propertyTable.Columns.Add("documentation")
            col.MaxLength = 10000

            d_data.Tables.Add("fieldOverrideData")
            col = overrideTable.Columns.Add("parentId")
            col = overrideTable.Columns.Add("id")
            col = overrideTable.Columns.Add("mnemonic")
            col = overrideTable.Columns.Add("description")

            d_data.AcceptChanges()
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
            ' create asynchronous session
                d_session = New Session(d_sessionOptions, New Bloomberglp.Blpapi.EventHandler(AddressOf processEvent))
            Return d_session.Start()
        End Function

        ''' <summary>
        ''' Clear security data
        ''' </summary>
        Private Sub clearData()
            fieldTable.Clear()
            propertyTable.Clear()
            If (Not IsNothing(overrideFieldsTempTable)) Then
                overrideFieldsTempTable.Clear()
            End If
            overrideTable.Clear()
            d_data.AcceptChanges()
        End Sub

        ''' <summary>
        ''' Build and submit field search request
        ''' </summary>
        ''' <param name="searchSpec"></param>
        ''' <param name="searchType"></param>
        ''' <param name="includeOptions"></param>
        ''' <param name="excludeOptions"></param>
        Private Sub fieldSearch(ByVal searchSpec As String, ByVal searchType As Integer, _
                ByVal includeOptions As FieldSearchOptions, ByVal excludeOptions As FieldSearchOptions)
            Dim request As Request = Nothing
            Dim cId As CorrelationID

            ' get auth service
            If (IsNothing(d_service)) Then
                If (d_session.OpenService("//blp/apiflds")) Then
                    d_service = d_session.GetService("//blp/apiflds")
                End If
            End If

            ' clear dataset
            clearData()

            If (searchType = 0) Then
                '  set field search correlationID to 1000
                cId = New CorrelationID(1000)
                request = d_service.CreateRequest("FieldSearchRequest")

                If (Not IsNothing(includeOptions)) Then
                    ' include options
                    Dim include As Element = request.GetElement("include")
                    If (Not includeOptions.ProductType = "None") Then
                        include.SetElement("productType", includeOptions.ProductType)
                    End If
                    If (Not includeOptions.FieldType = "None") Then
                        include.SetElement("fieldType", includeOptions.FieldType)
                    End If
                End If
            Else
                ' set category field search correlationID to 2000
                cId = New CorrelationID(2000)
                request = d_service.CreateRequest("CategorizedFieldSearchRequest")
            End If
            ' set search string
            request.Set("searchSpec", searchSpec)
            ' return field documentation
            request.Set("returnFieldDocumentation", True)

            If (Not IsNothing(excludeOptions)) Then
                ' exclude options
                Dim exclude As Element = request.GetElement("exclude")
                If (Not excludeOptions.ProductType = "None") Then
                    exclude.SetElement("productType", excludeOptions.ProductType)
                End If
                If (Not excludeOptions.FieldType = "None") Then
                    exclude.SetElement("fieldType", excludeOptions.FieldType)
                End If
            End If

            ' cancel previous pending request
            d_session.Cancel(cId)
            d_session.SendRequest(request, cId)
            toolStripStatusLabel1.Text = "Request sent."
        End Sub

        ''' <summary>
        ''' Get override field informations
        ''' </summary>
        ''' <param name="ids"></param>
        Private Sub getFieldInformation(ByVal ids As List(Of String))
            ' get auth service
            If (IsNothing(d_service)) Then
                If (d_session.OpenService("//blp/apiflds")) Then
                    d_service = d_session.GetService("//blp/apiflds")
                End If
            End If

            Dim request As Request = d_service.CreateRequest("FieldInfoRequest")
            request.Set("returnFieldDocumentation", True)

            For Each id As String In ids
                request.Append("id", id)
            Next

            ' set field info request correlationID to 3000
            Dim cId As CorrelationID = New CorrelationID(3000)
            d_session.Cancel(cId)
            d_session.SendRequest(request, cId)
        End Sub

        ''' <summary>
        ''' Get field documentation information
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        Private Function getFieldDocumentation(ByVal id As String) As String
            Dim doc As String = String.Empty
            ' get field documentation
            Dim rows As DataRow() = propertyTable.Select("id = '" + id + "'")
            If (rows.Length > 0) Then
                doc = rows(0)("documentation").ToString()
            End If
            Return doc
        End Function

        ''' <summary>
        ''' Get field overrides for field id
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        Private Function getFieldOverrides(ByVal id As String) As DataTable
            If (IsNothing(d_fieldIds)) Then
                d_fieldIds = New List(Of String)()
            Else
                d_fieldIds.Clear()
            End If

            If (IsNothing(overrideFieldsTempTable)) Then
                ' create table
                overrideFieldsTempTable = fieldTable.Clone()
                overrideFieldsTempTable.Columns.Remove("categoryName")
            Else
                ' clear talbe
                overrideFieldsTempTable.Clear()
                overrideFieldsTempTable.AcceptChanges()
            End If

            ' get override fields
            Dim rows As DataRow() = overrideTable.Select("parentId = '" + id + "'")
            For Each row As DataRow In rows
                Dim fieldId As String = row("id").ToString()
                If (row("mnemonic").ToString().Length > 0) Then
                    ' populate override field information
                    overrideFieldsTempTable.Rows.Add(New Object() {fieldId, row("mnemonic").ToString(), _
                            row("description").ToString()})
                Else
                    ' override field does not have information
                    d_fieldIds.Add(fieldId)
                    overrideFieldsTempTable.Rows.Add(New Object() {fieldId, DBNull.Value, _
                            DBNull.Value})
                End If
            Next

            ' get override field informations
            If (d_fieldIds.Count > 0) Then
                getFieldInformation(d_fieldIds)
            End If

            overrideFieldsTempTable.AcceptChanges()
            Return overrideFieldsTempTable
        End Function
#End Region 'end methods

#Region "Control Events"
        ''' <summary>
        ''' Field search event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub buttonSubmitSearch_Click(ByVal sender As Object, ByVal e As EventArgs) _
                Handles buttonSubmitSearch.Click
            Dim include As FieldSearchOptions = Nothing
            Dim exclude As FieldSearchOptions = Nothing

            If (comboBoxSearchType.SelectedIndex = 0) Then
                ' field search
                If (comboBoxIncludeProductType.SelectedIndex > 0 Or _
                    comboBoxIncludeFieldType.SelectedIndex > 0) Then
                    ' field search, add include filters
                    include = New FieldSearchOptions()
                    include.ProductType = comboBoxIncludeProductType.Text
                    include.FieldType = comboBoxIncludeFieldType.Text
                End If
            End If

            If (comboBoxExcludeProductType.SelectedIndex > 0 Or _
                comboBoxExcludeFieldType.SelectedIndex > 0) Then
                ' add exclude filters
                exclude = New FieldSearchOptions()
                exclude.ProductType = comboBoxExcludeProductType.Text
                exclude.FieldType = comboBoxExcludeFieldType.Text
            End If
            ' build field search request
            fieldSearch(textBoxSearchSpec.Text.Trim(), comboBoxSearchType.SelectedIndex, _
                    include, exclude)
            ' clear data tables
            fieldTable.Rows.Clear()
            overrideTable.Rows.Clear()
            propertyTable.Rows.Clear()
            d_data.AcceptChanges()
            richTextBoxDocumentation.Text = String.Empty
        End Sub

        ''' <summary>
        ''' Field select change
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub dataGridViewDataView_RowEnter(ByVal sender As Object, _
                ByVal e As DataGridViewCellEventArgs) Handles dataGridViewDataView.RowEnter
            ' get field documentation
            Dim id As String = dataGridViewDataView.Rows(e.RowIndex).Cells("id").Value.ToString()
            If (Not richTextBoxDocumentation.Tag.ToString() = id) Then
                richTextBoxDocumentation.Tag = id
                richTextBoxDocumentation.Text = getFieldDocumentation(id)
                dataGridViewOverrides.DataSource = getFieldOverrides(id)
                dataGridViewOverrides.Columns("description").Width = 350
            End If
        End Sub

        ''' <summary>
        ''' Field search type change 
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub comboBoxSearchType_SelectedIndexChanged(ByVal sender As Object, _
                ByVal e As EventArgs) Handles comboBoxSearchType.SelectedIndexChanged
            If (comboBoxSearchType.SelectedIndex = 1) Then
                ' disable include option for category search
                groupBoxIncludOption.Enabled = False
            Else
                ' enable include option for field search
                groupBoxIncludOption.Enabled = True
            End If
        End Sub
#End Region

#Region "Bloomberg API Events"
        ''' <summary>
        ''' Bloomberg data event
        ''' </summary>
        ''' <param name="eventObj"></param>
        ''' <param name="session"></param>
        Private Sub processEvent(ByVal eventObj As [Event], ByVal session As Session)
            If (InvokeRequired) Then
                Invoke(New EventHandler(AddressOf processEvent), New Object() {eventObj, session})
            Else
                Try
                    Select Case (eventObj.Type)
                        Case [Event].EventType.RESPONSE
                            ' process data
                            processResponse(eventObj, session)
                            toolStripStatusLabel1.Text = "Completed"
                        Case [Event].EventType.PARTIAL_RESPONSE
                            ' process partial data
                            processResponse(eventObj, session)
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
        ''' Process API response event
        ''' </summary>
        ''' <param name="eventObj"></param>
        ''' <param name="session"></param>
        Private Sub processResponse(ByVal eventObj As [Event], ByVal session As Session)
            For Each msg As Message In eventObj
                Dim cId As Integer = Convert.ToInt32(msg.CorrelationID.Value)
                Select Case (cId)
                    Case 1000
                        ' process field search response
                        processFieldData(msg)
                    Case 2000
                        ' process categorized field search response
                        processCategorizedFieldData(msg)
                    Case 3000
                        ' process field info response
                        processOverrides(msg)
                End Select
            Next
        End Sub

        ''' <summary>
        ''' Process field search response
        ''' </summary>
        ''' <param name="msg"></param>
        ''' <remarks></remarks>
        Private Sub processFieldData(ByVal msg As Bloomberglp.Blpapi.Message)
            Dim message As String = String.Empty
            Dim elementList As String() = New String() {"mnemonic", "description", _
                "categoryName", "documentation", "overrides"}
            Dim fieldDataValues As Object() = Nothing

            ' process message
            toolStripStatusLabel1.Text = "Processing data..."

            If (msg.MessageType.Equals(Bloomberglp.Blpapi.Name.GetName("fieldResponse"))) Then
                If (msg.HasElement("fieldSearchError")) Then
                    ' process field search error
                    Dim reason As Element = msg.GetElement("fieldSearchError")
                    message = String.Concat("Error: Source-", reason.GetElementAsString("source"), _
                        ", Code-", reason.GetElementAsString("code"), ", category-", _
                        reason.GetElementAsString("category"), _
                        ", desc-", reason.GetElementAsString("message"))
                Else
                    ' process field data
                    Dim fieldDataArray As Element = msg.GetElement("fieldData")
                    Dim numberOfFields As Integer = fieldDataArray.NumValues
                    ' start table update
                    fieldTable.BeginLoadData()
                    propertyTable.BeginLoadData()
                    overrideTable.BeginLoadData()
                    For index As Integer = 0 To numberOfFields - 1 Step 1
                        ' get field element
                        fieldDataValues = New Object(3) {}
                        Dim fieldElement As Element = fieldDataArray.GetValueAsElement(index)
                        Dim fieldData As Element = fieldElement.GetElement("fieldInfo")
                        ' get field id
                        Dim fieldId As String = fieldElement.GetElementAsString("id")
                        fieldDataValues(0) = fieldId
                        Try
                            Dim dataValue As String = String.Empty
                            Dim dataIndex As Integer = 1
                            For Each item As String In elementList
                                ' get field property
                                Dim dataElement As Element = fieldData.GetElement(item)
                                If (dataElement.IsArray) Then
                                    ' process array data
                                    Select Case (item)
                                        Case "categoryName"
                                            fieldDataValues(dataIndex) = dataElement.GetValueAsString().Trim()
                                        Case "overrides"
                                            For overrideIndex As Integer = 0 To dataElement.NumValues - 1 Step 1
                                                overrideTable.Rows.Add(New Object() {fieldId, dataElement(overrideIndex).ToString(), _
                                                        DBNull.Value, DBNull.Value})
                                            Next
                                    End Select
                                Else
                                    ' process element data
                                    Select Case (item)
                                        Case "documentation"
                                            ' add documentation row
                                            propertyTable.Rows.Add(New Object() {fieldId, dataElement.GetValue()})
                                        Case Else
                                            fieldDataValues(dataIndex) = dataElement.GetValueAsString()
                                    End Select
                                End If
                                dataIndex += 1
                            Next
                            ' add field to table
                            fieldTable.Rows.Add(fieldDataValues)
                        Catch
                            ' field property not in response
                        End Try
                    Next
                    ' end of table update
                    fieldTable.EndLoadData()
                    propertyTable.EndLoadData()
                    overrideTable.EndLoadData()
                    d_data.AcceptChanges()
                    If (fieldTable.Rows.Count > 0) Then
                        richTextBoxDocumentation.Tag = String.Empty
                        ' trigger event to update override grid
                        dataGridViewDataView_RowEnter(Me, New DataGridViewCellEventArgs(1, 0))
                    End If
                End If
            End If
            If (Not (message = String.Empty)) Then
                toolStripStatusLabel1.Text = "Request error: " + message
            End If
        End Sub

        ''' <summary>
        ''' Process categorized field search response
        ''' </summary>
        ''' <param name="msg"></param>
        Private Sub processCategorizedFieldData(ByVal msg As Bloomberglp.Blpapi.Message)
            Dim message As String = String.Empty
            Dim elementList As String() = New String() {"mnemonic", "description", _
                "categoryName", "documentation", "overrides"}
            Dim fieldDataValues As Object() = Nothing

            ' process message
            toolStripStatusLabel1.Text = "Processing data..."

            If (msg.MessageType.Equals(Bloomberglp.Blpapi.Name.GetName("categorizedFieldResponse"))) Then
                If (msg.HasElement("categorizedFieldSearchError")) Then
                    ' process field search error
                    Dim errorMessage As Element = msg.GetElement("categorizedFieldSearchError")
                    message = String.Concat("Error: Source-", errorMessage.GetElementAsString("source"), _
                        ", Code-", errorMessage.GetElementAsString("code"), ", category-", _
                        errorMessage.GetElementAsString("category"), _
                        ", desc-", errorMessage.GetElementAsString("message"))
                Else
                    If (msg.HasElement("category")) Then
                        ' process category
                        Dim categories As Element = msg.GetElement("category")
                        Dim numberOfCategories As Integer = categories.NumValues
                        For categoryIndex As Integer = 0 To numberOfCategories - 1 Step 1
                            ' process category data
                            Dim category As Element = categories.GetValueAsElement(categoryIndex)
                            Dim fieldDataArray As Element = category.GetElement("fieldData")
                            Dim numberOfFields As Integer = fieldDataArray.NumValues
                            ' start table update
                            fieldTable.BeginLoadData()
                            propertyTable.BeginLoadData()
                            overrideTable.BeginLoadData()
                            For index As Integer = 0 To numberOfFields - 1 Step 1
                                ' process field data
                                fieldDataValues = New Object(3) {}
                                Dim fieldElement As Element = fieldDataArray.GetValueAsElement(index)
                                Dim fieldData As Element = fieldElement.GetElement("fieldInfo")
                                ' get field id
                                Dim fieldId As String = fieldElement.GetElementAsString("id")
                                fieldDataValues(0) = fieldId
                                Try
                                    Dim dataValue As String = String.Empty
                                    Dim dataIndex As Integer = 1
                                    For Each item As String In elementList
                                        ' get field property
                                        Dim dataElement As Element = fieldData.GetElement(item)
                                        If (dataElement.IsArray) Then
                                            ' process array data
                                            Select Case (item)
                                                Case "categoryName"
                                                    fieldDataValues(dataIndex) = category.GetElementAsString("categoryName")
                                                Case "overrides"
                                                    For overrideIndex As Integer = 0 To dataElement.NumValues - 1 Step 1
                                                        If (overrideTable.Select("parentId = '" + fieldId + "' AND id = '" + _
                                                                dataElement(overrideIndex).ToString() + "'").Length = 0) Then
                                                            overrideTable.Rows.Add(New Object() {fieldId, dataElement(overrideIndex).ToString(), _
                                                                    DBNull.Value, DBNull.Value})
                                                        End If
                                                    Next
                                            End Select
                                        Else
                                            ' process element data
                                            Select Case (item)
                                                Case "documentation"
                                                    ' add documentation row
                                                    propertyTable.Rows.Add(New Object() {fieldId, dataElement.GetValue()})
                                                Case Else
                                                    fieldDataValues(dataIndex) = dataElement.GetValueAsString()
                                            End Select
                                        End If
                                        dataIndex += 1
                                    Next
                                    ' add field to table
                                    fieldTable.Rows.Add(fieldDataValues)
                                Catch
                                    ' field property not in response
                                End Try
                            Next
                        Next
                    End If
                    ' end of table update
                    fieldTable.EndLoadData()
                    propertyTable.EndLoadData()
                    overrideTable.EndLoadData()
                    d_data.AcceptChanges()
                    If (fieldTable.Rows.Count > 0) Then
                        richTextBoxDocumentation.Tag = String.Empty
                        ' trigger event to update override grid
                        dataGridViewDataView_RowEnter(Me, New DataGridViewCellEventArgs(1, 0))
                    End If
                End If
            End If
            If (Not (message = String.Empty)) Then
                toolStripStatusLabel1.Text = "Request error: " + message
            End If
        End Sub

        ''' <summary>
        ''' Process override field data returned
        ''' </summary>
        ''' <param name="msg"></param>
        Private Sub processOverrides(ByVal msg As Bloomberglp.Blpapi.Message)
            Dim elementList As String() = New String() {"mnemonic", "description", "categoryName"}
            Dim fieldDataValues As Object() = Nothing

            If (msg.MessageType.Equals(Bloomberglp.Blpapi.Name.GetName("fieldResponse"))) Then
                ' process response
                Dim fieldDataArray As Element = msg.GetElement("fieldData")
                Dim numberOfFields As Integer = fieldDataArray.NumValues
                ' start table update
                overrideTable.BeginLoadData()
                overrideFieldsTempTable.BeginLoadData()
                For index As Integer = 0 To numberOfFields - 1 Step 1
                    ' process field element
                    fieldDataValues = New Object(3) {}
                    Dim fieldElement As Element = fieldDataArray.GetValueAsElement(index)
                    Dim fieldData As Element = fieldElement.GetElement("fieldInfo")
                    Try
                        ' get field id
                        Dim fieldId = d_fieldIds(index)
                        ' get override for field id
                        Dim selectCriteria As String = "id = '" + fieldId + "'"
                        Dim ovrRows As DataRow() = overrideTable.Select(selectCriteria)
                        Dim fields As DataRow() = overrideFieldsTempTable.Select(selectCriteria)
                        If (fields.Length > 0) Then
                            ' process override fields
                            Dim dataIndex As Integer = 1
                            For Each item As String In elementList
                                ' field property
                                Dim dataElement As Element = fieldData.GetElement(item)
                                If (dataElement.IsArray) Then
                                    ' process array data
                                    Select Case (item)
                                        Case "categoryName"
                                            ' process categoryName here
                                        Case "overrides"
                                            ' process categoryName here
                                    End Select
                                Else
                                    ' process element data
                                    Select Case (item)
                                        Case "documentation"
                                            ' process documentation here
                                        Case Else
                                            fields(0)(dataIndex) = dataElement.GetValueAsString()
                                    End Select
                                End If
                                dataIndex += 1
                            Next

                            If (ovrRows.Length > 0) Then
                                ' update override field properties
                                For Each ovrRow As DataRow In ovrRows
                                    ovrRow("mnemonic") = fields(0)("mnemonic")
                                    ovrRow("description") = fields(0)("description")
                                Next
                            End If
                        End If
                    Catch
                        ' field property not in response
                    End Try
                Next
                ' end of table update
                overrideTable.BeginLoadData()
                overrideTable.AcceptChanges()
                overrideFieldsTempTable.BeginLoadData()
                overrideFieldsTempTable.AcceptChanges()
            End If
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
                        ' "Session stoped"
                    Case "ServiceOpened"
                        ' "Reference Service Opened"
                    Case "RequestFailure"
                        Dim errorMessage As Element = msg.GetElement(REASON)
                        Dim message As String = String.Concat("Error: Source-", errorMessage.GetElementAsString(SOURCE), _
                            ", Code-", errorMessage.GetElementAsString(ERROR_CODE), ", category-", _
                            errorMessage.GetElementAsString(CATEGORY), _
                            ", desc-", errorMessage.GetElementAsString(DESCRIPTION))
                        toolStripStatusLabel1.Text = message
                    Case Else
                        toolStripStatusLabel1.Text = msg.MessageType.ToString()
                End Select
            Next
        End Sub
#End Region
    End Class

#Region "FieldSearchOptions Class"
    ''' <summary>
    ''' Field search option class
    ''' </summary>
    Class FieldSearchOptions
        Private d_productType As String = String.Empty
        Private d_fieldType As String = String.Empty

        Public Property ProductType() As String
            Get
                Return d_productType
            End Get
            Set(ByVal value As String)
                d_productType = value
            End Set
        End Property

        Public Property FieldType() As String
            Get
                Return d_fieldType
            End Get
            Set(ByVal value As String)
                d_fieldType = value
            End Set
        End Property

        Public Sub FieldSearchOptions()

        End Sub
    End Class
#End Region 'FieldSearchOptions Class

#Region "FieldInformation Class"
    ''' <summary>
    ''' Field information class
    ''' </summary>
    Class FieldInformation
        Private d_id As String = String.Empty
        Private d_mnemonic As String = String.Empty
        Private d_dataType As String = String.Empty
        Private d_description As String = String.Empty
        Private d_documentation As String = String.Empty
        Private d_categoryName As String = String.Empty

        Public Property Id() As String
            Get
                Return d_id
            End Get
            Private Set(ByVal value As String)
                d_id = value
            End Set
        End Property

        Public Property Mnemonic() As String
            Get
                Return d_mnemonic
            End Get
            Set(ByVal value As String)
                d_mnemonic = value
            End Set
        End Property

        Public Property DataType() As String
            Get
                Return d_dataType
            End Get
            Set(ByVal value As String)
                d_dataType = value
            End Set
        End Property

        Public Property Description() As String
            Get
                Return d_description
            End Get
            Set(ByVal value As String)
                d_description = value
            End Set
        End Property

        Public Property Documentation() As String
            Get
                Return d_documentation
            End Get
            Set(ByVal value As String)
                d_documentation = value
            End Set
        End Property

        Public Property CategoryName() As String
            Get
                Return d_categoryName
            End Get
            Set(ByVal value As String)
                d_categoryName = value
            End Set
        End Property

        Public Sub FieldInformation(ByVal fieldId As String, ByVal fieldMnemonic As String, ByVal fieldDataType As String, _
            ByVal fieldDescription As String, ByVal fieldDocumentation As String, ByVal fieldCategoryName As String)
            Id = fieldId
            Mnemonic = fieldMnemonic
            DataType = fieldDataType
            Description = fieldDescription
            Documentation = fieldDocumentation
            CategoryName = fieldCategoryName
        End Sub

        Public Function GetObjectArray() As Object()
            Dim data As Object() = New Object() {Id, Mnemonic, DataType, Description, Documentation, CategoryName}
            Return data
        End Function
    End Class
#End Region 'FieldInformation Class
End Namespace

