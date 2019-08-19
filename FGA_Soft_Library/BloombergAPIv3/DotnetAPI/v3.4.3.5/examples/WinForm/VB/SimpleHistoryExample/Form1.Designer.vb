Namespace Bloomberglp.Blpapi.Examples
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class Form1
        Inherits System.Windows.Forms.Form

        'Form overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.statusStrip1 = New System.Windows.Forms.StatusStrip
            Me.toolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel
            Me.buttonClearData = New System.Windows.Forms.Button
            Me.radioButtonSynch = New System.Windows.Forms.RadioButton
            Me.radioButtonAsynch = New System.Windows.Forms.RadioButton
            Me.buttonClearFields = New System.Windows.Forms.Button
            Me.buttonSendRequest = New System.Windows.Forms.Button
            Me.dataGridViewData = New System.Windows.Forms.DataGridView
            Me.groupBox1 = New System.Windows.Forms.GroupBox
            Me.comboBoxNonTradingDayValue = New System.Windows.Forms.ComboBox
            Me.labelNonTradingDayValue = New System.Windows.Forms.Label
            Me.comboBoxOverrideOption = New System.Windows.Forms.ComboBox
            Me.labelOverrideOption = New System.Windows.Forms.Label
            Me.comboBoxPricing = New System.Windows.Forms.ComboBox
            Me.labelPricing = New System.Windows.Forms.Label
            Me.tabControlDates = New System.Windows.Forms.TabControl
            Me.tabPageDate = New System.Windows.Forms.TabPage
            Me.dateTimePickerEndDate = New System.Windows.Forms.DateTimePicker
            Me.labelEndDate = New System.Windows.Forms.Label
            Me.dateTimePickerStart = New System.Windows.Forms.DateTimePicker
            Me.labelStartDate = New System.Windows.Forms.Label
            Me.tabPageRelativeDate = New System.Windows.Forms.TabPage
            Me.textBoxRelEndDate = New System.Windows.Forms.TextBox
            Me.textBoxRelStartDate = New System.Windows.Forms.TextBox
            Me.labelRelEndDate = New System.Windows.Forms.Label
            Me.labelRelStartDate = New System.Windows.Forms.Label
            Me.comboBoxPeriodicitySelection = New System.Windows.Forms.ComboBox
            Me.labelPeriodicitySelection = New System.Windows.Forms.Label
            Me.comboBoxNonTradingDayMethod = New System.Windows.Forms.ComboBox
            Me.labelNonTradingDayMethod = New System.Windows.Forms.Label
            Me.comboBoxPeriodicityAdjustment = New System.Windows.Forms.ComboBox
            Me.labelPeridicityAdjustment = New System.Windows.Forms.Label
            Me.textBoxMaxPoints = New System.Windows.Forms.TextBox
            Me.labelMaxPoint = New System.Windows.Forms.Label
            Me.textBoxCurrencyCode = New System.Windows.Forms.TextBox
            Me.labelCurrencyCode = New System.Windows.Forms.Label
            Me.buttonAddFields = New System.Windows.Forms.Button
            Me.textBoxField = New System.Windows.Forms.TextBox
            Me.labelField = New System.Windows.Forms.Label
            Me.textBoxSecurity = New System.Windows.Forms.TextBox
            Me.labelSecurity = New System.Windows.Forms.Label
            Me.labelUsageNote = New System.Windows.Forms.Label
            Me.statusStrip1.SuspendLayout()
            CType(Me.dataGridViewData, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.groupBox1.SuspendLayout()
            Me.tabControlDates.SuspendLayout()
            Me.tabPageDate.SuspendLayout()
            Me.tabPageRelativeDate.SuspendLayout()
            Me.SuspendLayout()
            '
            'statusStrip1
            '
            Me.statusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.toolStripStatusLabel1})
            Me.statusStrip1.Location = New System.Drawing.Point(0, 448)
            Me.statusStrip1.Name = "statusStrip1"
            Me.statusStrip1.Size = New System.Drawing.Size(759, 22)
            Me.statusStrip1.TabIndex = 47
            Me.statusStrip1.Text = "statusStrip1"
            '
            'toolStripStatusLabel1
            '
            Me.toolStripStatusLabel1.Name = "toolStripStatusLabel1"
            Me.toolStripStatusLabel1.Size = New System.Drawing.Size(0, 17)
            '
            'buttonClearData
            '
            Me.buttonClearData.Enabled = False
            Me.buttonClearData.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.buttonClearData.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.buttonClearData.ImageIndex = 2
            Me.buttonClearData.Location = New System.Drawing.Point(534, 6)
            Me.buttonClearData.Name = "buttonClearData"
            Me.buttonClearData.Size = New System.Drawing.Size(81, 23)
            Me.buttonClearData.TabIndex = 50
            Me.buttonClearData.Tag = "HD"
            Me.buttonClearData.Text = "Clear Data"
            Me.buttonClearData.UseVisualStyleBackColor = True
            '
            'radioButtonSynch
            '
            Me.radioButtonSynch.AutoSize = True
            Me.radioButtonSynch.Location = New System.Drawing.Point(547, 37)
            Me.radioButtonSynch.Name = "radioButtonSynch"
            Me.radioButtonSynch.Size = New System.Drawing.Size(87, 17)
            Me.radioButtonSynch.TabIndex = 53
            Me.radioButtonSynch.Text = "Synchronous"
            Me.radioButtonSynch.UseVisualStyleBackColor = True
            '
            'radioButtonAsynch
            '
            Me.radioButtonAsynch.AutoSize = True
            Me.radioButtonAsynch.Checked = True
            Me.radioButtonAsynch.Location = New System.Drawing.Point(449, 37)
            Me.radioButtonAsynch.Name = "radioButtonAsynch"
            Me.radioButtonAsynch.Size = New System.Drawing.Size(92, 17)
            Me.radioButtonAsynch.TabIndex = 51
            Me.radioButtonAsynch.TabStop = True
            Me.radioButtonAsynch.Text = "Asynchronous"
            Me.radioButtonAsynch.UseVisualStyleBackColor = True
            '
            'buttonClearFields
            '
            Me.buttonClearFields.Enabled = False
            Me.buttonClearFields.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.buttonClearFields.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.buttonClearFields.ImageIndex = 2
            Me.buttonClearFields.Location = New System.Drawing.Point(447, 6)
            Me.buttonClearFields.Name = "buttonClearFields"
            Me.buttonClearFields.Size = New System.Drawing.Size(81, 23)
            Me.buttonClearFields.TabIndex = 49
            Me.buttonClearFields.Tag = "HD"
            Me.buttonClearFields.Text = "Clear Fields"
            Me.buttonClearFields.UseVisualStyleBackColor = True
            '
            'buttonSendRequest
            '
            Me.buttonSendRequest.Enabled = False
            Me.buttonSendRequest.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.buttonSendRequest.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.buttonSendRequest.Location = New System.Drawing.Point(360, 6)
            Me.buttonSendRequest.Name = "buttonSendRequest"
            Me.buttonSendRequest.Size = New System.Drawing.Size(81, 23)
            Me.buttonSendRequest.TabIndex = 48
            Me.buttonSendRequest.Tag = "HD"
            Me.buttonSendRequest.Text = "Submit"
            Me.buttonSendRequest.UseVisualStyleBackColor = True
            '
            'dataGridViewData
            '
            Me.dataGridViewData.AllowDrop = True
            Me.dataGridViewData.AllowUserToAddRows = False
            Me.dataGridViewData.AllowUserToDeleteRows = False
            Me.dataGridViewData.AllowUserToOrderColumns = True
            Me.dataGridViewData.AllowUserToResizeRows = False
            Me.dataGridViewData.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.dataGridViewData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dataGridViewData.Location = New System.Drawing.Point(249, 92)
            Me.dataGridViewData.MultiSelect = False
            Me.dataGridViewData.Name = "dataGridViewData"
            Me.dataGridViewData.ReadOnly = True
            Me.dataGridViewData.RowHeadersVisible = False
            Me.dataGridViewData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
            Me.dataGridViewData.Size = New System.Drawing.Size(498, 348)
            Me.dataGridViewData.TabIndex = 52
            Me.dataGridViewData.Tag = "HD"
            '
            'groupBox1
            '
            Me.groupBox1.Controls.Add(Me.comboBoxNonTradingDayValue)
            Me.groupBox1.Controls.Add(Me.labelNonTradingDayValue)
            Me.groupBox1.Controls.Add(Me.comboBoxOverrideOption)
            Me.groupBox1.Controls.Add(Me.labelOverrideOption)
            Me.groupBox1.Controls.Add(Me.comboBoxPricing)
            Me.groupBox1.Controls.Add(Me.labelPricing)
            Me.groupBox1.Controls.Add(Me.tabControlDates)
            Me.groupBox1.Controls.Add(Me.comboBoxPeriodicitySelection)
            Me.groupBox1.Controls.Add(Me.labelPeriodicitySelection)
            Me.groupBox1.Controls.Add(Me.comboBoxNonTradingDayMethod)
            Me.groupBox1.Controls.Add(Me.labelNonTradingDayMethod)
            Me.groupBox1.Controls.Add(Me.comboBoxPeriodicityAdjustment)
            Me.groupBox1.Controls.Add(Me.labelPeridicityAdjustment)
            Me.groupBox1.Controls.Add(Me.textBoxMaxPoints)
            Me.groupBox1.Controls.Add(Me.labelMaxPoint)
            Me.groupBox1.Controls.Add(Me.textBoxCurrencyCode)
            Me.groupBox1.Controls.Add(Me.labelCurrencyCode)
            Me.groupBox1.Location = New System.Drawing.Point(12, 62)
            Me.groupBox1.Name = "groupBox1"
            Me.groupBox1.Size = New System.Drawing.Size(231, 378)
            Me.groupBox1.TabIndex = 46
            Me.groupBox1.TabStop = False
            Me.groupBox1.Text = "Options"
            '
            'comboBoxNonTradingDayValue
            '
            Me.comboBoxNonTradingDayValue.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.comboBoxNonTradingDayValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.comboBoxNonTradingDayValue.FormattingEnabled = True
            Me.comboBoxNonTradingDayValue.Items.AddRange(New Object() {"Weekdays", "ALL Calender Days", "Active Days Only"})
            Me.comboBoxNonTradingDayValue.Location = New System.Drawing.Point(98, 115)
            Me.comboBoxNonTradingDayValue.Name = "comboBoxNonTradingDayValue"
            Me.comboBoxNonTradingDayValue.Size = New System.Drawing.Size(125, 21)
            Me.comboBoxNonTradingDayValue.TabIndex = 37
            Me.comboBoxNonTradingDayValue.Tag = "HD"
            '
            'labelNonTradingDayValue
            '
            Me.labelNonTradingDayValue.Location = New System.Drawing.Point(21, 111)
            Me.labelNonTradingDayValue.Name = "labelNonTradingDayValue"
            Me.labelNonTradingDayValue.Size = New System.Drawing.Size(67, 28)
            Me.labelNonTradingDayValue.TabIndex = 36
            Me.labelNonTradingDayValue.Text = "Non Trading Day Value:"
            '
            'comboBoxOverrideOption
            '
            Me.comboBoxOverrideOption.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.comboBoxOverrideOption.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.comboBoxOverrideOption.FormattingEnabled = True
            Me.comboBoxOverrideOption.Items.AddRange(New Object() {"CLOSE", "GPA"})
            Me.comboBoxOverrideOption.Location = New System.Drawing.Point(96, 196)
            Me.comboBoxOverrideOption.Name = "comboBoxOverrideOption"
            Me.comboBoxOverrideOption.Size = New System.Drawing.Size(125, 21)
            Me.comboBoxOverrideOption.TabIndex = 43
            Me.comboBoxOverrideOption.Tag = "HD"
            '
            'labelOverrideOption
            '
            Me.labelOverrideOption.AutoSize = True
            Me.labelOverrideOption.Location = New System.Drawing.Point(7, 199)
            Me.labelOverrideOption.Name = "labelOverrideOption"
            Me.labelOverrideOption.Size = New System.Drawing.Size(84, 13)
            Me.labelOverrideOption.TabIndex = 42
            Me.labelOverrideOption.Text = "Override Option:"
            '
            'comboBoxPricing
            '
            Me.comboBoxPricing.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.comboBoxPricing.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.comboBoxPricing.FormattingEnabled = True
            Me.comboBoxPricing.Items.AddRange(New Object() {"PRICE", "YIELD"})
            Me.comboBoxPricing.Location = New System.Drawing.Point(97, 169)
            Me.comboBoxPricing.Name = "comboBoxPricing"
            Me.comboBoxPricing.Size = New System.Drawing.Size(125, 21)
            Me.comboBoxPricing.TabIndex = 41
            Me.comboBoxPricing.Tag = "HD"
            '
            'labelPricing
            '
            Me.labelPricing.AutoSize = True
            Me.labelPricing.Location = New System.Drawing.Point(49, 172)
            Me.labelPricing.Name = "labelPricing"
            Me.labelPricing.Size = New System.Drawing.Size(42, 13)
            Me.labelPricing.TabIndex = 40
            Me.labelPricing.Text = "Pricing:"
            '
            'tabControlDates
            '
            Me.tabControlDates.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.tabControlDates.Controls.Add(Me.tabPageDate)
            Me.tabControlDates.Controls.Add(Me.tabPageRelativeDate)
            Me.tabControlDates.Location = New System.Drawing.Point(11, 227)
            Me.tabControlDates.Name = "tabControlDates"
            Me.tabControlDates.SelectedIndex = 0
            Me.tabControlDates.Size = New System.Drawing.Size(212, 140)
            Me.tabControlDates.TabIndex = 44
            '
            'tabPageDate
            '
            Me.tabPageDate.Controls.Add(Me.dateTimePickerEndDate)
            Me.tabPageDate.Controls.Add(Me.labelEndDate)
            Me.tabPageDate.Controls.Add(Me.dateTimePickerStart)
            Me.tabPageDate.Controls.Add(Me.labelStartDate)
            Me.tabPageDate.Location = New System.Drawing.Point(4, 22)
            Me.tabPageDate.Name = "tabPageDate"
            Me.tabPageDate.Padding = New System.Windows.Forms.Padding(3)
            Me.tabPageDate.Size = New System.Drawing.Size(204, 114)
            Me.tabPageDate.TabIndex = 0
            Me.tabPageDate.Text = "Actual Dates"
            Me.tabPageDate.UseVisualStyleBackColor = True
            '
            'dateTimePickerEndDate
            '
            Me.dateTimePickerEndDate.Location = New System.Drawing.Point(9, 70)
            Me.dateTimePickerEndDate.MinDate = New Date(1900, 1, 1, 0, 0, 0, 0)
            Me.dateTimePickerEndDate.Name = "dateTimePickerEndDate"
            Me.dateTimePickerEndDate.Size = New System.Drawing.Size(187, 20)
            Me.dateTimePickerEndDate.TabIndex = 31
            Me.dateTimePickerEndDate.Tag = "HD"
            '
            'labelEndDate
            '
            Me.labelEndDate.AutoSize = True
            Me.labelEndDate.Location = New System.Drawing.Point(6, 54)
            Me.labelEndDate.Name = "labelEndDate"
            Me.labelEndDate.Size = New System.Drawing.Size(55, 13)
            Me.labelEndDate.TabIndex = 30
            Me.labelEndDate.Text = "End Date:"
            '
            'dateTimePickerStart
            '
            Me.dateTimePickerStart.Location = New System.Drawing.Point(9, 28)
            Me.dateTimePickerStart.MinDate = New Date(1900, 1, 1, 0, 0, 0, 0)
            Me.dateTimePickerStart.Name = "dateTimePickerStart"
            Me.dateTimePickerStart.Size = New System.Drawing.Size(187, 20)
            Me.dateTimePickerStart.TabIndex = 29
            Me.dateTimePickerStart.Tag = "HD"
            '
            'labelStartDate
            '
            Me.labelStartDate.AutoSize = True
            Me.labelStartDate.Location = New System.Drawing.Point(6, 12)
            Me.labelStartDate.Name = "labelStartDate"
            Me.labelStartDate.Size = New System.Drawing.Size(58, 13)
            Me.labelStartDate.TabIndex = 28
            Me.labelStartDate.Text = "Start Date:"
            '
            'tabPageRelativeDate
            '
            Me.tabPageRelativeDate.Controls.Add(Me.textBoxRelEndDate)
            Me.tabPageRelativeDate.Controls.Add(Me.textBoxRelStartDate)
            Me.tabPageRelativeDate.Controls.Add(Me.labelRelEndDate)
            Me.tabPageRelativeDate.Controls.Add(Me.labelRelStartDate)
            Me.tabPageRelativeDate.Location = New System.Drawing.Point(4, 22)
            Me.tabPageRelativeDate.Name = "tabPageRelativeDate"
            Me.tabPageRelativeDate.Padding = New System.Windows.Forms.Padding(3)
            Me.tabPageRelativeDate.Size = New System.Drawing.Size(204, 114)
            Me.tabPageRelativeDate.TabIndex = 1
            Me.tabPageRelativeDate.Text = "Relative Dates"
            Me.tabPageRelativeDate.UseVisualStyleBackColor = True
            '
            'textBoxRelEndDate
            '
            Me.textBoxRelEndDate.Location = New System.Drawing.Point(12, 70)
            Me.textBoxRelEndDate.Name = "textBoxRelEndDate"
            Me.textBoxRelEndDate.Size = New System.Drawing.Size(184, 20)
            Me.textBoxRelEndDate.TabIndex = 35
            Me.textBoxRelEndDate.Tag = "HD"
            Me.textBoxRelEndDate.Text = "-1CQ"
            '
            'textBoxRelStartDate
            '
            Me.textBoxRelStartDate.Location = New System.Drawing.Point(9, 31)
            Me.textBoxRelStartDate.Name = "textBoxRelStartDate"
            Me.textBoxRelStartDate.Size = New System.Drawing.Size(187, 20)
            Me.textBoxRelStartDate.TabIndex = 33
            Me.textBoxRelStartDate.Tag = "HD"
            Me.textBoxRelStartDate.Text = "ED-6CQ"
            '
            'labelRelEndDate
            '
            Me.labelRelEndDate.AutoSize = True
            Me.labelRelEndDate.Location = New System.Drawing.Point(6, 54)
            Me.labelRelEndDate.Name = "labelRelEndDate"
            Me.labelRelEndDate.Size = New System.Drawing.Size(55, 13)
            Me.labelRelEndDate.TabIndex = 34
            Me.labelRelEndDate.Text = "End Date:"
            '
            'labelRelStartDate
            '
            Me.labelRelStartDate.AutoSize = True
            Me.labelRelStartDate.Location = New System.Drawing.Point(6, 12)
            Me.labelRelStartDate.Name = "labelRelStartDate"
            Me.labelRelStartDate.Size = New System.Drawing.Size(58, 13)
            Me.labelRelStartDate.TabIndex = 32
            Me.labelRelStartDate.Text = "Start Date:"
            '
            'comboBoxPeriodicitySelection
            '
            Me.comboBoxPeriodicitySelection.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.comboBoxPeriodicitySelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.comboBoxPeriodicitySelection.FormattingEnabled = True
            Me.comboBoxPeriodicitySelection.Location = New System.Drawing.Point(98, 88)
            Me.comboBoxPeriodicitySelection.Name = "comboBoxPeriodicitySelection"
            Me.comboBoxPeriodicitySelection.Size = New System.Drawing.Size(125, 21)
            Me.comboBoxPeriodicitySelection.TabIndex = 35
            Me.comboBoxPeriodicitySelection.Tag = "HD"
            '
            'labelPeriodicitySelection
            '
            Me.labelPeriodicitySelection.AutoSize = True
            Me.labelPeriodicitySelection.Location = New System.Drawing.Point(13, 91)
            Me.labelPeriodicitySelection.Name = "labelPeriodicitySelection"
            Me.labelPeriodicitySelection.Size = New System.Drawing.Size(79, 13)
            Me.labelPeriodicitySelection.TabIndex = 34
            Me.labelPeriodicitySelection.Text = "Periodicity Sel.:"
            '
            'comboBoxNonTradingDayMethod
            '
            Me.comboBoxNonTradingDayMethod.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.comboBoxNonTradingDayMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.comboBoxNonTradingDayMethod.FormattingEnabled = True
            Me.comboBoxNonTradingDayMethod.Items.AddRange(New Object() {"Blank", "Previous"})
            Me.comboBoxNonTradingDayMethod.Location = New System.Drawing.Point(98, 142)
            Me.comboBoxNonTradingDayMethod.Name = "comboBoxNonTradingDayMethod"
            Me.comboBoxNonTradingDayMethod.Size = New System.Drawing.Size(125, 21)
            Me.comboBoxNonTradingDayMethod.TabIndex = 39
            Me.comboBoxNonTradingDayMethod.Tag = "HD"
            '
            'labelNonTradingDayMethod
            '
            Me.labelNonTradingDayMethod.Location = New System.Drawing.Point(21, 139)
            Me.labelNonTradingDayMethod.Name = "labelNonTradingDayMethod"
            Me.labelNonTradingDayMethod.Size = New System.Drawing.Size(75, 28)
            Me.labelNonTradingDayMethod.TabIndex = 38
            Me.labelNonTradingDayMethod.Text = "Non Trading Day Method:"
            '
            'comboBoxPeriodicityAdjustment
            '
            Me.comboBoxPeriodicityAdjustment.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.comboBoxPeriodicityAdjustment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.comboBoxPeriodicityAdjustment.FormattingEnabled = True
            Me.comboBoxPeriodicityAdjustment.Items.AddRange(New Object() {"ACTUAL", "CALENDAR", "FISCAL"})
            Me.comboBoxPeriodicityAdjustment.Location = New System.Drawing.Point(98, 61)
            Me.comboBoxPeriodicityAdjustment.Name = "comboBoxPeriodicityAdjustment"
            Me.comboBoxPeriodicityAdjustment.Size = New System.Drawing.Size(125, 21)
            Me.comboBoxPeriodicityAdjustment.TabIndex = 33
            Me.comboBoxPeriodicityAdjustment.Tag = "HD"
            '
            'labelPeridicityAdjustment
            '
            Me.labelPeridicityAdjustment.AutoSize = True
            Me.labelPeridicityAdjustment.Location = New System.Drawing.Point(13, 64)
            Me.labelPeridicityAdjustment.Name = "labelPeridicityAdjustment"
            Me.labelPeridicityAdjustment.Size = New System.Drawing.Size(79, 13)
            Me.labelPeridicityAdjustment.TabIndex = 32
            Me.labelPeridicityAdjustment.Text = "Periodicity Adj.:"
            '
            'textBoxMaxPoints
            '
            Me.textBoxMaxPoints.Location = New System.Drawing.Point(98, 36)
            Me.textBoxMaxPoints.MaxLength = 5
            Me.textBoxMaxPoints.Name = "textBoxMaxPoints"
            Me.textBoxMaxPoints.Size = New System.Drawing.Size(48, 20)
            Me.textBoxMaxPoints.TabIndex = 31
            Me.textBoxMaxPoints.Tag = "HD"
            '
            'labelMaxPoint
            '
            Me.labelMaxPoint.AutoSize = True
            Me.labelMaxPoint.Location = New System.Drawing.Point(27, 39)
            Me.labelMaxPoint.Name = "labelMaxPoint"
            Me.labelMaxPoint.Size = New System.Drawing.Size(65, 13)
            Me.labelMaxPoint.TabIndex = 29
            Me.labelMaxPoint.Text = "Max. Points:"
            '
            'textBoxCurrencyCode
            '
            Me.textBoxCurrencyCode.Location = New System.Drawing.Point(98, 11)
            Me.textBoxCurrencyCode.MaxLength = 3
            Me.textBoxCurrencyCode.Name = "textBoxCurrencyCode"
            Me.textBoxCurrencyCode.Size = New System.Drawing.Size(48, 20)
            Me.textBoxCurrencyCode.TabIndex = 30
            Me.textBoxCurrencyCode.Tag = "HD"
            '
            'labelCurrencyCode
            '
            Me.labelCurrencyCode.AutoSize = True
            Me.labelCurrencyCode.Location = New System.Drawing.Point(12, 14)
            Me.labelCurrencyCode.Name = "labelCurrencyCode"
            Me.labelCurrencyCode.Size = New System.Drawing.Size(80, 13)
            Me.labelCurrencyCode.TabIndex = 28
            Me.labelCurrencyCode.Text = "Currency Code:"
            '
            'buttonAddFields
            '
            Me.buttonAddFields.Enabled = False
            Me.buttonAddFields.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.buttonAddFields.Location = New System.Drawing.Point(360, 34)
            Me.buttonAddFields.Name = "buttonAddFields"
            Me.buttonAddFields.Size = New System.Drawing.Size(81, 23)
            Me.buttonAddFields.TabIndex = 45
            Me.buttonAddFields.Tag = "HD"
            Me.buttonAddFields.Text = "Add"
            Me.buttonAddFields.UseVisualStyleBackColor = True
            '
            'textBoxField
            '
            Me.textBoxField.Enabled = False
            Me.textBoxField.Location = New System.Drawing.Point(60, 36)
            Me.textBoxField.Name = "textBoxField"
            Me.textBoxField.Size = New System.Drawing.Size(294, 20)
            Me.textBoxField.TabIndex = 44
            Me.textBoxField.Tag = "HD"
            '
            'labelField
            '
            Me.labelField.AutoSize = True
            Me.labelField.Enabled = False
            Me.labelField.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.labelField.Location = New System.Drawing.Point(26, 39)
            Me.labelField.Name = "labelField"
            Me.labelField.Size = New System.Drawing.Size(32, 13)
            Me.labelField.TabIndex = 43
            Me.labelField.Text = "Field:"
            '
            'textBoxSecurity
            '
            Me.textBoxSecurity.AllowDrop = True
            Me.textBoxSecurity.Location = New System.Drawing.Point(60, 8)
            Me.textBoxSecurity.Name = "textBoxSecurity"
            Me.textBoxSecurity.Size = New System.Drawing.Size(294, 20)
            Me.textBoxSecurity.TabIndex = 42
            Me.textBoxSecurity.Tag = "HD"
            '
            'labelSecurity
            '
            Me.labelSecurity.AutoSize = True
            Me.labelSecurity.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.labelSecurity.Location = New System.Drawing.Point(10, 11)
            Me.labelSecurity.Name = "labelSecurity"
            Me.labelSecurity.Size = New System.Drawing.Size(48, 13)
            Me.labelSecurity.TabIndex = 41
            Me.labelSecurity.Text = "Security:"
            '
            'labelUsageNote
            '
            Me.labelUsageNote.AccessibleRole = System.Windows.Forms.AccessibleRole.None
            Me.labelUsageNote.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.labelUsageNote.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.labelUsageNote.Location = New System.Drawing.Point(249, 66)
            Me.labelUsageNote.Name = "labelUsageNote"
            Me.labelUsageNote.Size = New System.Drawing.Size(498, 23)
            Me.labelUsageNote.TabIndex = 54
            Me.labelUsageNote.Text = "Note: User can delete field by selecting a cell within the field column and press" & _
                " the delete key. "
            Me.labelUsageNote.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'Form1
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(759, 470)
            Me.Controls.Add(Me.labelUsageNote)
            Me.Controls.Add(Me.statusStrip1)
            Me.Controls.Add(Me.buttonClearData)
            Me.Controls.Add(Me.radioButtonSynch)
            Me.Controls.Add(Me.radioButtonAsynch)
            Me.Controls.Add(Me.buttonClearFields)
            Me.Controls.Add(Me.buttonSendRequest)
            Me.Controls.Add(Me.dataGridViewData)
            Me.Controls.Add(Me.groupBox1)
            Me.Controls.Add(Me.buttonAddFields)
            Me.Controls.Add(Me.textBoxField)
            Me.Controls.Add(Me.labelField)
            Me.Controls.Add(Me.textBoxSecurity)
            Me.Controls.Add(Me.labelSecurity)
            Me.MinimumSize = New System.Drawing.Size(767, 497)
            Me.Name = "Form1"
            Me.Text = "Simple History Example"
            Me.statusStrip1.ResumeLayout(False)
            Me.statusStrip1.PerformLayout()
            CType(Me.dataGridViewData, System.ComponentModel.ISupportInitialize).EndInit()
            Me.groupBox1.ResumeLayout(False)
            Me.groupBox1.PerformLayout()
            Me.tabControlDates.ResumeLayout(False)
            Me.tabPageDate.ResumeLayout(False)
            Me.tabPageDate.PerformLayout()
            Me.tabPageRelativeDate.ResumeLayout(False)
            Me.tabPageRelativeDate.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents statusStrip1 As System.Windows.Forms.StatusStrip
        Private WithEvents toolStripStatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
        Private WithEvents buttonClearData As System.Windows.Forms.Button
        Private WithEvents radioButtonSynch As System.Windows.Forms.RadioButton
        Private WithEvents radioButtonAsynch As System.Windows.Forms.RadioButton
        Private WithEvents buttonClearFields As System.Windows.Forms.Button
        Private WithEvents buttonSendRequest As System.Windows.Forms.Button
        Private WithEvents dataGridViewData As System.Windows.Forms.DataGridView
        Private WithEvents groupBox1 As System.Windows.Forms.GroupBox
        Private WithEvents comboBoxNonTradingDayValue As System.Windows.Forms.ComboBox
        Private WithEvents labelNonTradingDayValue As System.Windows.Forms.Label
        Private WithEvents comboBoxOverrideOption As System.Windows.Forms.ComboBox
        Private WithEvents labelOverrideOption As System.Windows.Forms.Label
        Private WithEvents comboBoxPricing As System.Windows.Forms.ComboBox
        Private WithEvents labelPricing As System.Windows.Forms.Label
        Private WithEvents tabControlDates As System.Windows.Forms.TabControl
        Private WithEvents tabPageDate As System.Windows.Forms.TabPage
        Private WithEvents dateTimePickerEndDate As System.Windows.Forms.DateTimePicker
        Private WithEvents labelEndDate As System.Windows.Forms.Label
        Private WithEvents dateTimePickerStart As System.Windows.Forms.DateTimePicker
        Private WithEvents labelStartDate As System.Windows.Forms.Label
        Private WithEvents tabPageRelativeDate As System.Windows.Forms.TabPage
        Private WithEvents textBoxRelEndDate As System.Windows.Forms.TextBox
        Private WithEvents textBoxRelStartDate As System.Windows.Forms.TextBox
        Private WithEvents labelRelEndDate As System.Windows.Forms.Label
        Private WithEvents labelRelStartDate As System.Windows.Forms.Label
        Private WithEvents comboBoxPeriodicitySelection As System.Windows.Forms.ComboBox
        Private WithEvents labelPeriodicitySelection As System.Windows.Forms.Label
        Private WithEvents comboBoxNonTradingDayMethod As System.Windows.Forms.ComboBox
        Private WithEvents labelNonTradingDayMethod As System.Windows.Forms.Label
        Private WithEvents comboBoxPeriodicityAdjustment As System.Windows.Forms.ComboBox
        Private WithEvents labelPeridicityAdjustment As System.Windows.Forms.Label
        Private WithEvents textBoxMaxPoints As System.Windows.Forms.TextBox
        Private WithEvents labelMaxPoint As System.Windows.Forms.Label
        Private WithEvents textBoxCurrencyCode As System.Windows.Forms.TextBox
        Private WithEvents labelCurrencyCode As System.Windows.Forms.Label
        Private WithEvents buttonAddFields As System.Windows.Forms.Button
        Private WithEvents textBoxField As System.Windows.Forms.TextBox
        Private WithEvents labelField As System.Windows.Forms.Label
        Private WithEvents textBoxSecurity As System.Windows.Forms.TextBox
        Private WithEvents labelSecurity As System.Windows.Forms.Label
        Private WithEvents labelUsageNote As System.Windows.Forms.Label

    End Class
End Namespace