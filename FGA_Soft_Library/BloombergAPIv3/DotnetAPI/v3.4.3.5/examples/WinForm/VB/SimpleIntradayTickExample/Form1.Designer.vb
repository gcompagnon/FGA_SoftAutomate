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
            Me.splitContainerIRView = New System.Windows.Forms.SplitContainer
            Me.listBoxSecurities = New System.Windows.Forms.ListBox
            Me.dataGridViewData = New System.Windows.Forms.DataGridView
            Me.statusStrip1 = New System.Windows.Forms.StatusStrip
            Me.toolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel
            Me.panelIRTopView = New System.Windows.Forms.Panel
            Me.radioButtonSynch = New System.Windows.Forms.RadioButton
            Me.radioButtonAsynch = New System.Windows.Forms.RadioButton
            Me.buttonClearAll = New System.Windows.Forms.Button
            Me.buttonSendRequest = New System.Windows.Forms.Button
            Me.checkBoxIncludeExchangeCode = New System.Windows.Forms.CheckBox
            Me.labelEventTypes = New System.Windows.Forms.Label
            Me.checkedListBoxEventTypes = New System.Windows.Forms.CheckedListBox
            Me.checkBoxIncludeConditionCode = New System.Windows.Forms.CheckBox
            Me.dateTimePickerEndDate = New System.Windows.Forms.DateTimePicker
            Me.labelIREndDate = New System.Windows.Forms.Label
            Me.dateTimePickerStartDate = New System.Windows.Forms.DateTimePicker
            Me.labelIRStartDate = New System.Windows.Forms.Label
            Me.textBoxSecurity = New System.Windows.Forms.TextBox
            Me.labelIRSecurity = New System.Windows.Forms.Label
            Me.splitContainerIRView.Panel1.SuspendLayout()
            Me.splitContainerIRView.Panel2.SuspendLayout()
            Me.splitContainerIRView.SuspendLayout()
            CType(Me.dataGridViewData, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.statusStrip1.SuspendLayout()
            Me.panelIRTopView.SuspendLayout()
            Me.SuspendLayout()
            '
            'splitContainerIRView
            '
            Me.splitContainerIRView.Dock = System.Windows.Forms.DockStyle.Fill
            Me.splitContainerIRView.Location = New System.Drawing.Point(0, 139)
            Me.splitContainerIRView.Name = "splitContainerIRView"
            '
            'splitContainerIRView.Panel1
            '
            Me.splitContainerIRView.Panel1.Controls.Add(Me.listBoxSecurities)
            '
            'splitContainerIRView.Panel2
            '
            Me.splitContainerIRView.Panel2.Controls.Add(Me.dataGridViewData)
            Me.splitContainerIRView.Size = New System.Drawing.Size(806, 309)
            Me.splitContainerIRView.SplitterDistance = 190
            Me.splitContainerIRView.TabIndex = 34
            '
            'listBoxSecurities
            '
            Me.listBoxSecurities.Dock = System.Windows.Forms.DockStyle.Fill
            Me.listBoxSecurities.FormattingEnabled = True
            Me.listBoxSecurities.Location = New System.Drawing.Point(0, 0)
            Me.listBoxSecurities.Name = "listBoxSecurities"
            Me.listBoxSecurities.Size = New System.Drawing.Size(190, 303)
            Me.listBoxSecurities.TabIndex = 14
            '
            'dataGridViewData
            '
            Me.dataGridViewData.AllowUserToAddRows = False
            Me.dataGridViewData.AllowUserToDeleteRows = False
            Me.dataGridViewData.AllowUserToResizeRows = False
            Me.dataGridViewData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dataGridViewData.Dock = System.Windows.Forms.DockStyle.Fill
            Me.dataGridViewData.Location = New System.Drawing.Point(0, 0)
            Me.dataGridViewData.MultiSelect = False
            Me.dataGridViewData.Name = "dataGridViewData"
            Me.dataGridViewData.ReadOnly = True
            Me.dataGridViewData.RowHeadersVisible = False
            Me.dataGridViewData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
            Me.dataGridViewData.Size = New System.Drawing.Size(612, 309)
            Me.dataGridViewData.TabIndex = 18
            Me.dataGridViewData.TabStop = False
            Me.dataGridViewData.Tag = "HD"
            '
            'statusStrip1
            '
            Me.statusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.toolStripStatusLabel1})
            Me.statusStrip1.Location = New System.Drawing.Point(0, 448)
            Me.statusStrip1.Name = "statusStrip1"
            Me.statusStrip1.Size = New System.Drawing.Size(806, 22)
            Me.statusStrip1.TabIndex = 33
            Me.statusStrip1.Text = "statusStrip1"
            '
            'toolStripStatusLabel1
            '
            Me.toolStripStatusLabel1.Name = "toolStripStatusLabel1"
            Me.toolStripStatusLabel1.Size = New System.Drawing.Size(0, 17)
            '
            'panelIRTopView
            '
            Me.panelIRTopView.Controls.Add(Me.radioButtonSynch)
            Me.panelIRTopView.Controls.Add(Me.radioButtonAsynch)
            Me.panelIRTopView.Controls.Add(Me.buttonClearAll)
            Me.panelIRTopView.Controls.Add(Me.buttonSendRequest)
            Me.panelIRTopView.Controls.Add(Me.checkBoxIncludeExchangeCode)
            Me.panelIRTopView.Controls.Add(Me.labelEventTypes)
            Me.panelIRTopView.Controls.Add(Me.checkedListBoxEventTypes)
            Me.panelIRTopView.Controls.Add(Me.checkBoxIncludeConditionCode)
            Me.panelIRTopView.Controls.Add(Me.dateTimePickerEndDate)
            Me.panelIRTopView.Controls.Add(Me.labelIREndDate)
            Me.panelIRTopView.Controls.Add(Me.dateTimePickerStartDate)
            Me.panelIRTopView.Controls.Add(Me.labelIRStartDate)
            Me.panelIRTopView.Controls.Add(Me.textBoxSecurity)
            Me.panelIRTopView.Controls.Add(Me.labelIRSecurity)
            Me.panelIRTopView.Dock = System.Windows.Forms.DockStyle.Top
            Me.panelIRTopView.Location = New System.Drawing.Point(0, 0)
            Me.panelIRTopView.Name = "panelIRTopView"
            Me.panelIRTopView.Size = New System.Drawing.Size(806, 139)
            Me.panelIRTopView.TabIndex = 32
            '
            'radioButtonSynch
            '
            Me.radioButtonSynch.AutoSize = True
            Me.radioButtonSynch.Location = New System.Drawing.Point(481, 32)
            Me.radioButtonSynch.Name = "radioButtonSynch"
            Me.radioButtonSynch.Size = New System.Drawing.Size(87, 17)
            Me.radioButtonSynch.TabIndex = 13
            Me.radioButtonSynch.Text = "Synchronous"
            Me.radioButtonSynch.UseVisualStyleBackColor = True
            '
            'radioButtonAsynch
            '
            Me.radioButtonAsynch.AutoSize = True
            Me.radioButtonAsynch.Checked = True
            Me.radioButtonAsynch.Location = New System.Drawing.Point(383, 32)
            Me.radioButtonAsynch.Name = "radioButtonAsynch"
            Me.radioButtonAsynch.Size = New System.Drawing.Size(92, 17)
            Me.radioButtonAsynch.TabIndex = 12
            Me.radioButtonAsynch.TabStop = True
            Me.radioButtonAsynch.Text = "Asynchronous"
            Me.radioButtonAsynch.UseVisualStyleBackColor = True
            '
            'buttonClearAll
            '
            Me.buttonClearAll.Enabled = False
            Me.buttonClearAll.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.buttonClearAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.buttonClearAll.ImageIndex = 3
            Me.buttonClearAll.Location = New System.Drawing.Point(471, 3)
            Me.buttonClearAll.Name = "buttonClearAll"
            Me.buttonClearAll.Size = New System.Drawing.Size(81, 23)
            Me.buttonClearAll.TabIndex = 11
            Me.buttonClearAll.Tag = "RD"
            Me.buttonClearAll.Text = "Clear All"
            Me.buttonClearAll.UseVisualStyleBackColor = True
            '
            'buttonSendRequest
            '
            Me.buttonSendRequest.Enabled = False
            Me.buttonSendRequest.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.buttonSendRequest.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.buttonSendRequest.Location = New System.Drawing.Point(384, 3)
            Me.buttonSendRequest.Name = "buttonSendRequest"
            Me.buttonSendRequest.Size = New System.Drawing.Size(81, 23)
            Me.buttonSendRequest.TabIndex = 10
            Me.buttonSendRequest.Tag = "RD"
            Me.buttonSendRequest.Text = "Submit"
            Me.buttonSendRequest.UseVisualStyleBackColor = True
            '
            'checkBoxIncludeExchangeCode
            '
            Me.checkBoxIncludeExchangeCode.AutoSize = True
            Me.checkBoxIncludeExchangeCode.Location = New System.Drawing.Point(462, 109)
            Me.checkBoxIncludeExchangeCode.Name = "checkBoxIncludeExchangeCode"
            Me.checkBoxIncludeExchangeCode.Size = New System.Drawing.Size(145, 17)
            Me.checkBoxIncludeExchangeCode.TabIndex = 9
            Me.checkBoxIncludeExchangeCode.Text = "Include Exchange Codes"
            Me.checkBoxIncludeExchangeCode.UseVisualStyleBackColor = True
            '
            'labelEventTypes
            '
            Me.labelEventTypes.AutoSize = True
            Me.labelEventTypes.Location = New System.Drawing.Point(6, 85)
            Me.labelEventTypes.Name = "labelEventTypes"
            Me.labelEventTypes.Size = New System.Drawing.Size(70, 13)
            Me.labelEventTypes.TabIndex = 6
            Me.labelEventTypes.Text = "Event Types:"
            '
            'checkedListBoxEventTypes
            '
            Me.checkedListBoxEventTypes.BackColor = System.Drawing.SystemColors.Control
            Me.checkedListBoxEventTypes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.checkedListBoxEventTypes.CheckOnClick = True
            Me.checkedListBoxEventTypes.FormattingEnabled = True
            Me.checkedListBoxEventTypes.Items.AddRange(New Object() {"TRADE", "BID", "ASK", "BID_BEST", "ASK_BEST", "BID_YIELD", "ASK_YIELD", "MID_PRICE", "AT_TRADE"})
            Me.checkedListBoxEventTypes.Location = New System.Drawing.Point(77, 85)
            Me.checkedListBoxEventTypes.MultiColumn = True
            Me.checkedListBoxEventTypes.Name = "checkedListBoxEventTypes"
            Me.checkedListBoxEventTypes.Size = New System.Drawing.Size(376, 47)
            Me.checkedListBoxEventTypes.TabIndex = 7
            Me.checkedListBoxEventTypes.ThreeDCheckBoxes = True
            '
            'checkBoxIncludeConditionCode
            '
            Me.checkBoxIncludeConditionCode.AutoSize = True
            Me.checkBoxIncludeConditionCode.Location = New System.Drawing.Point(462, 86)
            Me.checkBoxIncludeConditionCode.Name = "checkBoxIncludeConditionCode"
            Me.checkBoxIncludeConditionCode.Size = New System.Drawing.Size(141, 17)
            Me.checkBoxIncludeConditionCode.TabIndex = 8
            Me.checkBoxIncludeConditionCode.Text = "Include Condition Codes"
            Me.checkBoxIncludeConditionCode.UseVisualStyleBackColor = True
            '
            'dateTimePickerEndDate
            '
            Me.dateTimePickerEndDate.CustomFormat = "dddd, MMMM dd, yyyy - HH:mm:ss"
            Me.dateTimePickerEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom
            Me.dateTimePickerEndDate.Location = New System.Drawing.Point(78, 59)
            Me.dateTimePickerEndDate.MinDate = New Date(1900, 1, 1, 0, 0, 0, 0)
            Me.dateTimePickerEndDate.Name = "dateTimePickerEndDate"
            Me.dateTimePickerEndDate.Size = New System.Drawing.Size(294, 20)
            Me.dateTimePickerEndDate.TabIndex = 5
            Me.dateTimePickerEndDate.Tag = "IBAR"
            '
            'labelIREndDate
            '
            Me.labelIREndDate.AutoSize = True
            Me.labelIREndDate.Location = New System.Drawing.Point(21, 63)
            Me.labelIREndDate.Name = "labelIREndDate"
            Me.labelIREndDate.Size = New System.Drawing.Size(55, 13)
            Me.labelIREndDate.TabIndex = 4
            Me.labelIREndDate.Text = "End Date:"
            '
            'dateTimePickerStartDate
            '
            Me.dateTimePickerStartDate.CustomFormat = "dddd, MMMM dd, yyyy - HH:mm:ss"
            Me.dateTimePickerStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom
            Me.dateTimePickerStartDate.Location = New System.Drawing.Point(78, 31)
            Me.dateTimePickerStartDate.MinDate = New Date(1900, 1, 1, 0, 0, 0, 0)
            Me.dateTimePickerStartDate.Name = "dateTimePickerStartDate"
            Me.dateTimePickerStartDate.Size = New System.Drawing.Size(294, 20)
            Me.dateTimePickerStartDate.TabIndex = 3
            Me.dateTimePickerStartDate.Tag = "IBAR"
            '
            'labelIRStartDate
            '
            Me.labelIRStartDate.AutoSize = True
            Me.labelIRStartDate.Location = New System.Drawing.Point(18, 35)
            Me.labelIRStartDate.Name = "labelIRStartDate"
            Me.labelIRStartDate.Size = New System.Drawing.Size(58, 13)
            Me.labelIRStartDate.TabIndex = 2
            Me.labelIRStartDate.Text = "Start Date:"
            '
            'textBoxSecurity
            '
            Me.textBoxSecurity.Location = New System.Drawing.Point(78, 5)
            Me.textBoxSecurity.Name = "textBoxSecurity"
            Me.textBoxSecurity.Size = New System.Drawing.Size(294, 20)
            Me.textBoxSecurity.TabIndex = 1
            Me.textBoxSecurity.Tag = "IBAR"
            '
            'labelIRSecurity
            '
            Me.labelIRSecurity.AutoSize = True
            Me.labelIRSecurity.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.labelIRSecurity.Location = New System.Drawing.Point(28, 8)
            Me.labelIRSecurity.Name = "labelIRSecurity"
            Me.labelIRSecurity.Size = New System.Drawing.Size(48, 13)
            Me.labelIRSecurity.TabIndex = 0
            Me.labelIRSecurity.Text = "Security:"
            '
            'Form1
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(806, 470)
            Me.Controls.Add(Me.splitContainerIRView)
            Me.Controls.Add(Me.statusStrip1)
            Me.Controls.Add(Me.panelIRTopView)
            Me.MinimumSize = New System.Drawing.Size(814, 497)
            Me.Name = "Form1"
            Me.Text = "Simple Intraday Tick Example"
            Me.splitContainerIRView.Panel1.ResumeLayout(False)
            Me.splitContainerIRView.Panel2.ResumeLayout(False)
            Me.splitContainerIRView.ResumeLayout(False)
            CType(Me.dataGridViewData, System.ComponentModel.ISupportInitialize).EndInit()
            Me.statusStrip1.ResumeLayout(False)
            Me.statusStrip1.PerformLayout()
            Me.panelIRTopView.ResumeLayout(False)
            Me.panelIRTopView.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents splitContainerIRView As System.Windows.Forms.SplitContainer
        Private WithEvents listBoxSecurities As System.Windows.Forms.ListBox
        Private WithEvents dataGridViewData As System.Windows.Forms.DataGridView
        Private WithEvents statusStrip1 As System.Windows.Forms.StatusStrip
        Private WithEvents toolStripStatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
        Private WithEvents panelIRTopView As System.Windows.Forms.Panel
        Private WithEvents radioButtonSynch As System.Windows.Forms.RadioButton
        Private WithEvents radioButtonAsynch As System.Windows.Forms.RadioButton
        Private WithEvents buttonClearAll As System.Windows.Forms.Button
        Private WithEvents buttonSendRequest As System.Windows.Forms.Button
        Private WithEvents checkBoxIncludeExchangeCode As System.Windows.Forms.CheckBox
        Private WithEvents labelEventTypes As System.Windows.Forms.Label
        Private WithEvents checkedListBoxEventTypes As System.Windows.Forms.CheckedListBox
        Private WithEvents checkBoxIncludeConditionCode As System.Windows.Forms.CheckBox
        Private WithEvents dateTimePickerEndDate As System.Windows.Forms.DateTimePicker
        Private WithEvents labelIREndDate As System.Windows.Forms.Label
        Private WithEvents dateTimePickerStartDate As System.Windows.Forms.DateTimePicker
        Private WithEvents labelIRStartDate As System.Windows.Forms.Label
        Private WithEvents textBoxSecurity As System.Windows.Forms.TextBox
        Private WithEvents labelIRSecurity As System.Windows.Forms.Label

    End Class
End Namespace