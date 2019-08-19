' ==========================================================
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT 
'  WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED,    
' INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES   
' OF MERCHANTABILITY AND/OR FITNESS FOR A  PARTICULAR   
' PURPOSE.								
' ==========================================================
' Purpose of this example:
' - Make asynchronous refdata request
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
Imports EventQueue = Bloomberglp.Blpapi.EventQueue
Imports CorrelationID = Bloomberglp.Blpapi.CorrelationID

Namespace Bloomberglp.Blpapi.Examples
    Public Class Form1
        Private ReadOnly SECURITY_DATA As Name = New Name("securityData")
        Private ReadOnly SECURITY As Name = New Name("security")
        Private ReadOnly FIELD_DATA As Name = New Name("fieldData")
        Private ReadOnly RESPONSE_ERROR As Name = New Name("responseError")
        Private ReadOnly SECURITY_ERROR As Name = New Name("securityError")
        Private ReadOnly FIELD_EXCEPTIONS As Name = New Name("fieldExceptions")
        Private ReadOnly FIELD_ID As Name = New Name("fieldId")
        Private ReadOnly ERROR_INFO As Name = New Name("errorInfo")
        Private ReadOnly CATEGORY As Name = New Name("category")
        Private ReadOnly MESSAGE As Name = New Name("message")

        Private Const NUMBER_OF_SCENARIO As Integer = 3

        Private d_host As String
        Private d_port As Integer
        Private d_session As Session
        Private d_service As Service
        Private d_scenarioIndexLookup As Dictionary(Of String, Integer)

        ''' <summary>
        ''' form load event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            ' init control values
            initialize()
            ' start API session
            If (startAPISession()) Then
                ' started sucessfully, enable security controls
                labelSecurity.Enabled = True
                textBoxSecurity.Enabled = True
                buttonGetData.Enabled = True
                buttonGetAllData.Enabled = True
                buttonClearData.Enabled = True
                buttonClearAllData.Enabled = True
            Else
                ' failed
                d_session = Nothing
            End If
        End Sub

#Region "Private functions"
        ''' <summary>
        '''  init form controls and variables
        ''' </summary>
        Private Sub initialize()
            ' variables
            d_host = "localhost"
            d_port = 8194
            d_scenarioIndexLookup = New Dictionary(Of String, Integer)()
            d_scenarioIndexLookup.Add("Scenario 1", 1)
            d_scenarioIndexLookup.Add("Scenario 2", 2)
            d_scenarioIndexLookup.Add("Scenario 3", 3)

            Dim inputFields As String() = New String() {"MTG_PREPAY_SPEED", "MTG_PREPAY_TYP", _
                "PREPAY_SPEED_VECTOR", "ALLOW_DYNAMIC_CASHFLOW_CALCS", "DEFAULT_PERCENT", _
                "DEFAULT_TYPE", "DEFAULT_SPEED_VECTOR", "LOSS_SEVERITY", _
                "RECOVERY_LAG"}

            dataGridViewAnalyticInput.Rows.Add(New Object() {"Prepay Speed", 100, 100, 100})
            dataGridViewAnalyticInput.Rows.Add(New Object() {"Prepay Type", "CPR", "CPR", "CPR"})
            dataGridViewAnalyticInput.Rows.Add(New Object() {"Prepay Vector", "15", "2 12 R 10", "2"})
            dataGridViewAnalyticInput.Rows.Add(New Object() {"Dynamics", "Y", "Y", "Y"})
            dataGridViewAnalyticInput.Rows.Add(New Object() {"Default Speed", 200, 100, 100})
            dataGridViewAnalyticInput.Rows.Add(New Object() {"Default Type", "CDR", "CDR", "CDR"})
            dataGridViewAnalyticInput.Rows.Add(New Object() {"Default Vector", "2 10 R 8", "2 4 6 8", "6 12 R 2"})
            dataGridViewAnalyticInput.Rows.Add(New Object() {"Severity Curve", "60 24 R 85", "60 12 R 85", "85 12 R 60"})
            dataGridViewAnalyticInput.Rows.Add(New Object() {"Recovery Lag", 0, 0, 0})
            ' set tag property
            For index As Integer = 0 To inputFields.Length - 1
                dataGridViewAnalyticInput.Rows(index).Tag = inputFields(index)
            Next index

            ' set vary price to skip 5
            comboBoxVaryPrice.SelectedIndex = 2
            ' create sub-items for listview
            For Each item As ListViewItem In listViewScenarios.Items
                item.SubItems.Add(String.Empty)
                item.SubItems.Add(String.Empty)
                item.SubItems.Add(String.Empty)
                item.SubItems.Add(String.Empty)
            Next

            Try
                ' load Vector tab information 
                '[note: Make sure this rtf file is in the same directory as the executable.]
                richTextBoxVectors.LoadFile("vectors.rtf")
            Catch
                richTextBoxVectors.Text = "Missing vectors.rtf file in the executable directory."
            End Try
        End Sub

        ''' <summary>
        ''' Create and start session. 
        ''' Open refdata service for reference data requests.
        ''' </summary>
        ''' <returns></returns>
        Private Function startAPISession() As Boolean
            Dim status As Boolean = False

            ' setup sessionOption
            Dim sessionOptions As SessionOptions = New SessionOptions()
            sessionOptions.ServerHost = d_host
            sessionOptions.ServerPort = d_port

            ' create new session
            d_session = New Session(sessionOptions, New Bloomberglp.Blpapi.EventHandler(AddressOf processEvent))
            ' start session
            If (Not d_session.Start()) Then
                MessageBox.Show("Failed to start session.", "Session")
            Else
                ' open refdata service
                If (Not d_session.OpenService("//blp/refdata")) Then
                    MessageBox.Show("Failed to open //blp/refdata", "Service")
                Else
                    ' sucess
                    status = True
                End If
            End If
            startAPISession = status
        End Function

        ''' <summary>
        ''' Clear tab page textbox
        ''' </summary>
        ''' <param name="tab"></param>
        Private Sub clearTextBox(ByVal tab As TabPage)
            ' loop through Data tab for field textbox to update data
            For Each control As Control In tab.Controls
                ' found GroupBox
                If (control.GetType() Is GetType(GroupBox)) Then
                    ' Loop through GroupBox control collection
                    For Each child As Control In control.Controls
                        ' textbox Tag property contain field name
                        If (child.GetType() Is GetType(TextBox)) Then
                            child.Text = "-"
                        End If
                    Next
                End If
            Next
        End Sub

        ''' <summary>
        ''' Clear scenarios data
        ''' </summary>
        Private Sub clearScenarios()
            For Each item As ListViewItem In listViewScenarios.Items
                For index As Integer = 1 To listViewScenarios.Columns.Count - 1
                    item.SubItems(index).Text = String.Empty
                Next
            Next
        End Sub

        ''' <summary>
        ''' Populate tab textbox data
        ''' </summary>
        ''' <param name="tab"></param>
        ''' <param name="field"></param>
        Private Sub populateTextBox(ByVal tab As TabPage, ByVal field As String, ByVal value As String)
            Dim foundTextbox As Boolean = False
            ' loop through Data tab for field textbox to update data
            For Each control As Control In tab.Controls
                ' found GroupBox
                If (control.GetType() Is GetType(GroupBox)) Then
                    ' Loop through GroupBox control collection
                    For Each child As Control In control.Controls
                        ' textbox Tag property contain field name
                        If (Not (child.Tag Is Nothing)) Then
                            If (child.Tag.ToString().Trim().Length > 0) Then
                                ' check if textbox control contain correct field
                                If (child.Tag.ToString() = field) Then
                                    ' Populate data
                                    child.Text = value
                                    foundTextbox = True
                                    Exit For
                                End If
                            End If
                        End If
                    Next
                End If
                If (foundTextbox) Then
                    ' no need to loop through controls
                    Exit For
                End If
            Next
        End Sub

        ''' <summary>
        ''' Populate listview control with scenario data
        ''' </summary>
        ''' <param name="scenario"></param>
        ''' <param name="priceIndex"></param>
        ''' <param name="returnFields"></param>
        Private Sub populateScenario(ByVal scenario As String, ByVal priceIndex As Integer, ByVal returnFields As Element)
            Dim currentPriceIndex As Integer = 0
            If (returnFields.NumElements > 0) Then
                ' get number of fields returned
                Dim numElements As Integer = returnFields.NumElements
                For j As Integer = 0 To numElements - 1
                    ' reset index
                    currentPriceIndex = -1
                    ' get field
                    Dim field As Element = returnFields.GetElement(j)
                    For Each item As ListViewItem In listViewScenarios.Items
                        ' look for correct price 
                        If (item.Text.Contains("Price")) Then
                            ' move to next price position
                            currentPriceIndex += 1
                        End If

                        ' check if in correct price slot
                        If (currentPriceIndex < priceIndex) Then
                            ' continue to search for correct price section
                            Continue For
                        End If

                        ' check if row updateable
                        If (Not (item.Tag Is Nothing)) Then
                            If (item.Tag.ToString().Trim().Length > 0) Then
                                ' check if correct row
                                If (item.Tag.ToString() = field.Name.ToString()) Then
                                    ' populate data
                                    Dim index As Integer = d_scenarioIndexLookup(scenario)
                                    item.SubItems(index).Text = field.GetValueAsString()
                                    Exit For
                                End If
                            End If
                        End If
                    Next
                Next
            End If
        End Sub

        ''' <summary>
        ''' Calculate scenarios average for each price
        ''' </summary>
        Private Sub calculateScenariosAverage()
            For Each item As ListViewItem In listViewScenarios.Items
                ' check if field can be average
                If (item.Checked) Then
                    Dim value As Double = 0
                    Dim sum As Double = 0
                    ' calculate average for this item
                    For Each scenario As KeyValuePair(Of String, Integer) In d_scenarioIndexLookup
                        Double.TryParse(item.SubItems(scenario.Value).Text, value)
                        sum += value
                    Next
                    ' get average
                    sum /= d_scenarioIndexLookup.Count
                    item.SubItems(4).Text = sum.ToString()
                End If
            Next
        End Sub

        ''' <summary>
        ''' Get security data
        ''' </summary>
        Private Sub getData()
            ' get reference data service
            If (d_service Is Nothing) Then
                d_service = d_session.GetService("//blp/refdata")
            End If
            ' create reference data request
            Dim request As Request = d_service.CreateRequest("ReferenceDataRequest")
            ' set security
            request.GetElement("securities").AppendValue(textBoxSecurity.Text.ToUpper().Trim() + " Mtge")
            ' set fields
            Dim Fields As Element = request.GetElement("fields")
            Fields.AppendValue("MTG_PL_CPR_1M")
            Fields.AppendValue("MTG_CDR_1M")
            Fields.AppendValue("MTG_VPR_1M")
            Fields.AppendValue("MTG_SEV_1M")
            Fields.AppendValue("CREDIT_MODELED_INDICATOR")
            Fields.AppendValue("MTG_AMT_OUT_FACE")
            Fields.AppendValue("MTG_FACTOR")
            Fields.AppendValue("CPN")
            Fields.AppendValue("MTG_TRANCHE_TYP_LONG")
            Fields.AppendValue("RTG_SP_INITIAL")
            Fields.AppendValue("RTG_SP")
            Fields.AppendValue("RTG_MDY_INITIAL")
            Fields.AppendValue("RTG_MOODY")
            Fields.AppendValue("ORIG_CREDIT_SUPPORT")
            Fields.AppendValue("CURR_CREDIT_SUPPORT")
            Fields.AppendValue("CREDIT_SUPPORT_COVERAGE")
            Fields.AppendValue("COLLAT_TYP")
            Fields.AppendValue("MTG_POOL_FACTOR")
            Fields.AppendValue("MTG_NUM_POOLS")
            Fields.AppendValue("MTG_WACPN")
            Fields.AppendValue("MTG_WHLN_WALA")
            Fields.AppendValue("MTG_WHLN_30DLQ")
            Fields.AppendValue("MTG_WHLN_60DLQ")
            Fields.AppendValue("MTG_WHLN_90DLQ")
            Fields.AppendValue("BANKRUPT_PCT")
            Fields.AppendValue("MTG_WHLN_FCLS")
            Fields.AppendValue("MTG_WHLN_REO")
            Fields.AppendValue("MTG_DELQ_60PLUS_CUR")
            Fields.AppendValue("MTG_DELQ_90PLUS_CUR")
            Fields.AppendValue("CURR_CUM_LOSS_AMT")

            ' create event queue for synchronus request
            Dim eventQueue As EventQueue = New EventQueue()
            ' send request
            d_session.SendRequest(request, eventQueue, Nothing)
            ' wait for data to come back
            While (True)
                Dim eventObj As [Event] = eventQueue.NextEvent()
                ' process data
                For Each msg As Message In eventObj
                    ' check for request error
                    If (msg.HasElement(RESPONSE_ERROR)) Then
                        MessageBox.Show("REQUEST FAILED: " + _
                            msg.GetElement(RESPONSE_ERROR).ToString(), "Request Error")
                        Continue For
                    End If

                    ' get securities
                    Dim securities As Element = msg.GetElement(SECURITY_DATA)
                    Dim numSecurities As Integer = securities.NumValues
                    For i As Integer = 0 To numSecurities - 1
                        ' get security
                        Dim securityElement As Element = securities.GetValueAsElement(i)
                        Dim ticker As String = securityElement.GetElementAsString(SECURITY)
                        ' check for security error
                        If (securityElement.HasElement("securityError")) Then
                            MessageBox.Show("SECURITY FAILED: " + _
                                securityElement.GetElement(SECURITY_ERROR).ToString(), "Security Error")
                            Continue For
                        End If
                        ' get fields
                        Dim returnFields As Element = securityElement.GetElement(FIELD_DATA)
                        If (returnFields.NumElements > 0) Then
                            ' get number of fields returned
                            Dim numElements As Integer = returnFields.NumElements
                            For j As Integer = 0 To numElements - 1
                                ' get field
                                Dim field As Element = returnFields.GetElement(j)
                                ' populate textbox on Data tab
                                populateTextBox(tabPageData, field.Name.ToString(), field.GetValueAsString())
                            Next j
                        End If
                    Next i
                Next

                If (eventObj.Type = [Event].EventType.RESPONSE) Then
                    ' all the data came back for this request
                    Exit While
                End If
            End While
        End Sub

        ''' <summary>
        ''' Get analytic data for prices and scenarios
        ''' </summary>
        Private Sub getAnalyticData()
            Dim price1 As Double = 0
            Dim price2 As Double = 0
            Dim price3 As Double = 0
            Dim prices As Double()
            Dim overridePriceYieldField As String = String.Empty
            Dim returnPriceYieldField As String = String.Empty

            ' get reference data service
            If (d_service Is Nothing) Then
                d_service = d_session.GetService("//blp/refdata")
            End If

            ' get prices
            Double.TryParse(textBoxPrice1.Text, price1)
            Double.TryParse(textBoxPrice2.Text, price2)
            Double.TryParse(textBoxPrice3.Text, price3)
            prices = New Double() {price1, price2, price3}

            ' set to either price or yield override
            If (radioButtonPrice.Checked) Then
                ' price
                overridePriceYieldField = radioButtonPrice.Tag.ToString()
                returnPriceYieldField = radioButtonYield.Tag.ToString()
            Else
                ' yield
                overridePriceYieldField = radioButtonYield.Tag.ToString()
                returnPriceYieldField = radioButtonPrice.Tag.ToString()
            End If
            ' run scenarios for each price
            Dim priceIndex As Integer = 0
            Dim correlationId As Long = 0
            For Each price As Double In prices
                ' max 3 scenario for each price
                For scenario As Integer = 1 To NUMBER_OF_SCENARIO
                    ' create reference data request
                    Dim request As Request = d_service.CreateRequest("ReferenceDataRequest")
                    ' set security
                    request.GetElement("securities").AppendValue(textBoxSecurity.Text.ToUpper().Trim() + " Mtge")
                    ' set fields
                    Dim fields As Element = request.GetElement("fields")
                    fields.AppendValue(returnPriceYieldField)
                    fields.AppendValue("MTG_WAL")
                    fields.AppendValue("MTG_STATIC_MOD_DUR")
                    fields.AppendValue("MTG_PRINC_WIN")
                    fields.AppendValue("I_SPRD_ASK")
                    fields.AppendValue("Z_SPRD_ASK")
                    fields.AppendValue("N_SPRD_ASK")
                    fields.AppendValue("E_SPRD_ASK")
                    fields.AppendValue("FIRST_LOSS_DATE")
                    fields.AppendValue("PROJ_BOND_CUM_LOSS_AMT")
                    fields.AppendValue("PROJ_COLL_CUM_LOSS_AMT")
                    fields.AppendValue("PROJ_BOND_CUM_LOSS_PCT")
                    fields.AppendValue("PROJ_COLL_CUM_LOSS_PCT")
                    fields.AppendValue("PROJ_BOND_WRITEDWN_PCT_CURR_FACE")
                    fields.AppendValue("PROJ_COLL_WRITEDWN_PCT_CURR_FACE")

                    ' set overrides
                    Dim overridefields As Element = request("overrides")
                    Dim overrideField As Element = Nothing
                    ' override field
                    overrideField = overridefields.AppendElement()
                    ' set fieldId 
                    overrideField.SetElement("fieldId", overridePriceYieldField)
                    ' set override value
                    overrideField.SetElement("value", price.ToString())

                    ' get scenario info
                    For Each row As DataGridViewRow In dataGridViewAnalyticInput.Rows
                        ' field name is in row tag property
                        If (Not row.Tag Is Nothing) Then
                            If (row.Tag.ToString().Trim().Length > 0) Then
                                Dim overrideValue As String = String.Empty
                                ' textbox by default
                                overrideValue = row.Cells(scenario).Value.ToString()

                                ' override field
                                overrideField = overridefields.AppendElement()
                                ' set fieldId 
                                overrideField.SetElement("fieldId", row.Tag.ToString())
                                ' set override value
                                overrideField.SetElement("value", overrideValue)
                            End If
                        End If
                    Next
                    ' make asynchronous request, data will come back in processEvent()
                    ' correlationId is use to map data to listview
                    d_session.SendRequest(request, New CorrelationID(correlationId))
                    correlationId += 1
                Next
                ' point to next price
                priceIndex += 1
            Next
        End Sub

        Private Sub processData(ByVal eventObj As [Event])
            ' process data
            For Each msg As Message In eventObj
                ' get correlation id
                Dim scenario As Integer = CInt(msg.CorrelationID.Value Mod NUMBER_OF_SCENARIO + 1)
                Dim priceIndex As Integer = CInt(msg.CorrelationID.Value \ NUMBER_OF_SCENARIO)

                ' check for request error
                If (msg.HasElement(RESPONSE_ERROR)) Then
                    MessageBox.Show("REQUEST FAILED: " + _
                        msg.GetElement(RESPONSE_ERROR).ToString(), "Request Error")
                    Continue For
                End If

                ' get securities
                Dim securities As Element = msg.GetElement(SECURITY_DATA)
                Dim numSecurities As Integer = securities.NumValues
                For i As Integer = 0 To numSecurities - 1
                    ' get security
                    Dim securityElement As Element = securities.GetValueAsElement(i)
                    Dim ticker As String = securityElement.GetElementAsString(SECURITY)
                    ' check for security error
                    If (securityElement.HasElement(SECURITY_ERROR)) Then
                        MessageBox.Show("SECURITY FAILED: " + _
                            securityElement.GetElement(SECURITY_ERROR).ToString(), "Security Error")
                        Continue For
                    End If
                    ' get fields
                    Dim returnFields As Element = securityElement.GetElement(FIELD_DATA)

                    ' populate scenario fields on Analytic tab
                    populateScenario(dataGridViewAnalyticInput.Columns(scenario).HeaderText, priceIndex, returnFields)
                Next
            Next
        End Sub
#End Region

#Region "Events"
        ''' <summary>
        ''' Asynchronous request data event
        ''' </summary>
        ''' <param name="eventObj"></param>
        ''' <param name="session"></param>
        Private Sub processEvent(ByVal eventObj As [Event], ByVal session As Session)
            If (InvokeRequired) Then
                '' make sure data get process in winform thread
                Invoke(New Bloomberglp.Blpapi.EventHandler(AddressOf processEvent), New Object() {eventObj, session})
            Else
                Try
                    Select Case (eventObj.Type)
                        Case [Event].EventType.PARTIAL_RESPONSE
                            ' process partial data
                            processData(eventObj)
                        Case [Event].EventType.RESPONSE
                            ' preocess return data
                            processData(eventObj)
                            ' calculate average
                            calculateScenariosAverage()
                    End Select
                Catch e As System.Exception
                    MessageBox.Show("Exception: " + e.Message, "Populate Data Exception")
                End Try
            End If
        End Sub

        ''' <summary>
        ''' Vary price for scenarios
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub comboBoxVaryPrice_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles comboBoxVaryPrice.SelectedIndexChanged
            Dim varyPrice As Integer = 0
            Dim price1 As Double = 0

            ' get vary price
            Integer.TryParse(comboBoxVaryPrice.SelectedItem.ToString(), varyPrice)
            ' get scenario 1 price
            Double.TryParse(textBoxPrice1.Text, price1)
            If (varyPrice > 0) Then
                ' vary price for scenario 2 and 3
                price1 += varyPrice
                textBoxPrice2.Text = price1.ToString()
                price1 += varyPrice
                textBoxPrice3.Text = price1.ToString()
            End If
        End Sub

        ''' <summary>
        ''' Only allow floating numeric value
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub textBoxPrice_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles textBoxPrice3.KeyPress, textBoxPrice2.KeyPress, textBoxPrice1.KeyPress
            If (Not Char.IsDigit(e.KeyChar) And e.KeyChar <> ChrW(Keys.Back)) Then
                If (e.KeyChar = ChrW(Keys.Decimal)) Then
                    Dim price As TextBox = CType(sender, TextBox)
                    If (price.Text.Contains(".")) Then
                        e.Handled = True
                    Else
                        e.Handled = True
                    End If
                End If
            End If
        End Sub

        ''' <summary>
        ''' Stop session on form closing
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs)
            If (Not d_session Is Nothing) Then
                d_session.Stop()
                d_session = Nothing
            End If
        End Sub

        ''' <summary>
        ''' Get data for selected tab
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub buttonGetData_Click(ByVal sender As Object, ByVal e As EventArgs) Handles buttonGetData.Click
            If (textBoxSecurity.Text.Trim().Length = 0) Then
                ' missing security
                MessageBox.Show("Please enter a security", "Security Input")
                Return
            End If
            ' change cursor to hourglass
            Cursor.Current = Cursors.WaitCursor
            Select Case (tabControlNonAgency.SelectedTab.Text)
                Case "Data"
                    ' clear data
                    clearTextBox(tabPageData)
                    ' request for data
                    getData()
                Case Else ' Analytic
                    ' clear scenarios
                    clearScenarios()
                    ' show data tab
                    tabControlNonAgency.SelectedTab = tabPageAnalyic
                    ' request for analytic data
                    getAnalyticData()
            End Select
            ' change cursor to normal
            Cursor.Current = Cursors.Default
        End Sub

        ''' <summary>
        ''' get both Data and Analytic data
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub buttonGetAllData_Click(ByVal sender As Object, ByVal e As EventArgs) Handles buttonGetAllData.Click
            If (textBoxSecurity.Text.Trim().Length = 0) Then
                ' missing security
                MessageBox.Show("Please enter a security", "Security Input")
                Return
            End If
            ' change cursor to hourglass
            Cursor.Current = Cursors.WaitCursor
            ' clear data
            clearTextBox(tabPageData)
            ' request for data
            getData()
            ' clear scenarios
            clearScenarios()
            ' request for analytic data
            getAnalyticData()
            ' calculate average
            calculateScenariosAverage()
            ' change cursor to normal
            Cursor.Current = Cursors.Default
        End Sub

        ''' <summary>
        ''' Clear tab data
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub buttonClearData_Click(ByVal sender As Object, ByVal e As EventArgs) Handles buttonClearData.Click
            Select Case (tabControlNonAgency.SelectedTab.Text)
                Case "Data"
                    ' clear data
                    clearTextBox(tabPageData)
                Case Else ' Analytic
                    ' clear scenarios
                    clearScenarios()
            End Select
        End Sub

        ''' <summary>
        ''' Clear all data
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub buttonClearAllData_Click(ByVal sender As Object, ByVal e As EventArgs) Handles buttonClearAllData.Click
            ' clear data
            clearTextBox(tabPageData)
            ' clear scenarios
            clearScenarios()
        End Sub


        ''' <summary>
        ''' Drag and drop content to textbox
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub textBox_DragDrop(ByVal sender As Object, ByVal e As DragEventArgs) Handles textBoxSecurity.DragDrop, textBoxPrice3.DragDrop, textBoxPrice2.DragDrop, textBoxPrice1.DragDrop
            ' Get the entire text object that has been dropped on us.
            Dim tmp As String = e.Data.GetData(DataFormats.Text).ToString()
            ' cast to textbox control
            Dim control As TextBox = CType(sender, TextBox)
            ' set text property
            control.Text = tmp.Trim()
        End Sub

        ''' <summary>
        ''' Drag content over textbox
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub textBox_DragEnter(ByVal sender As Object, ByVal e As DragEventArgs) Handles textBoxSecurity.DragEnter, textBoxPrice3.DragEnter, textBoxPrice2.DragEnter, textBoxPrice1.DragEnter
            If (e.Data.GetDataPresent(DataFormats.Text)) Then
                e.Effect = DragDropEffects.Copy
            Else
                e.Effect = DragDropEffects.None
            End If
        End Sub

        ''' <summary>
        ''' Change override between price and yield
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub radioButton_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles radioButtonYield.CheckedChanged, radioButtonPrice.CheckedChanged
            Dim displayName As String = String.Empty
            Dim fieldName As String = String.Empty
            Dim oldDisplayName As String = String.Empty
            Dim oldFieldName As String = String.Empty
            Dim control As RadioButton = CType(sender, RadioButton)

            Select Case (control.Text)
                Case "Price"
                    ' change display text to price
                    displayName = radioButtonPrice.Text
                    fieldName = radioButtonPrice.Tag.ToString()
                    oldDisplayName = radioButtonYield.Text
                    oldFieldName = radioButtonYield.Tag.ToString()
                Case "Yield"
                    ' change display text to yield
                    displayName = radioButtonYield.Text
                    fieldName = radioButtonYield.Tag.ToString()
                    oldDisplayName = radioButtonPrice.Text
                    oldFieldName = radioButtonPrice.Tag.ToString()
                Case Else
                    ' controls not initialized yet. 
                    Return
            End Select

            ' change lable name
            labelVaryPrice.Text = labelVaryPrice.Text.Replace(oldDisplayName, displayName)
            labelPrice1.Text = labelPrice1.Text.Replace(oldDisplayName, displayName)
            labelPrice2.Text = labelPrice2.Text.Replace(oldDisplayName, displayName)
            labelPrice3.Text = labelPrice3.Text.Replace(oldDisplayName, displayName)
            ' update listview text and fields
            For Each item As ListViewItem In listViewScenarios.Items
                If (item.Text = displayName) Then
                    item.Text = oldDisplayName
                    item.Tag = oldFieldName
                ElseIf (item.BackColor = Color.LightBlue) Then
                    item.Text = item.Text.Replace(oldDisplayName, displayName)
                End If
            Next
            ' clear scenarios
            clearScenarios()
        End Sub

        ''' <summary>
        ''' Convert alpha to upper case
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub dataGridViewAnalyticInput_CellEndEdit(ByVal sender As Object, ByVal e As DataGridViewCellEventArgs) Handles dataGridViewAnalyticInput.CellEndEdit
            ' make sure all alpha are in upper case
            dataGridViewAnalyticInput.Rows(e.RowIndex).Cells(e.ColumnIndex).Value = _
                dataGridViewAnalyticInput.Rows(e.RowIndex).Cells(e.ColumnIndex).Value.ToString().ToUpper()
        End Sub
#End Region
    End Class
End Namespace