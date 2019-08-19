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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
            Me.textBoxOutputFile = New System.Windows.Forms.TextBox
            Me.buttonStopSubscribe = New System.Windows.Forms.Button
            Me.buttonClearAll = New System.Windows.Forms.Button
            Me.buttonClearData = New System.Windows.Forms.Button
            Me.buttonClearFields = New System.Windows.Forms.Button
            Me.buttonSendRequest = New System.Windows.Forms.Button
            Me.checkBoxForceDelay = New System.Windows.Forms.CheckBox
            Me.checkBoxOutputFile = New System.Windows.Forms.CheckBox
            Me.textBoxInterval = New System.Windows.Forms.TextBox
            Me.labelInterval = New System.Windows.Forms.Label
            Me.dataGridViewData = New System.Windows.Forms.DataGridView
            Me.buttonAddField = New System.Windows.Forms.Button
            Me.textBoxField = New System.Windows.Forms.TextBox
            Me.labelField = New System.Windows.Forms.Label
            Me.buttonAddSecurity = New System.Windows.Forms.Button
            Me.textBoxSecurity = New System.Windows.Forms.TextBox
            Me.labelSecurity = New System.Windows.Forms.Label
            Me.statusStrip1 = New System.Windows.Forms.StatusStrip
            Me.toolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel
            Me.labelUsageNote = New System.Windows.Forms.Label
            CType(Me.dataGridViewData, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.statusStrip1.SuspendLayout()
            Me.SuspendLayout()
            '
            'textBoxOutputFile
            '
            Me.textBoxOutputFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.textBoxOutputFile.Location = New System.Drawing.Point(351, 60)
            Me.textBoxOutputFile.Name = "textBoxOutputFile"
            Me.textBoxOutputFile.ReadOnly = True
            Me.textBoxOutputFile.Size = New System.Drawing.Size(393, 20)
            Me.textBoxOutputFile.TabIndex = 46
            Me.textBoxOutputFile.Visible = False
            Me.textBoxOutputFile.WordWrap = False
            '
            'buttonStopSubscribe
            '
            Me.buttonStopSubscribe.Enabled = False
            Me.buttonStopSubscribe.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.buttonStopSubscribe.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.buttonStopSubscribe.Location = New System.Drawing.Point(544, 3)
            Me.buttonStopSubscribe.Name = "buttonStopSubscribe"
            Me.buttonStopSubscribe.Size = New System.Drawing.Size(81, 23)
            Me.buttonStopSubscribe.TabIndex = 41
            Me.buttonStopSubscribe.Tag = "RD"
            Me.buttonStopSubscribe.Text = "Stop"
            Me.buttonStopSubscribe.UseVisualStyleBackColor = True
            '
            'buttonClearAll
            '
            Me.buttonClearAll.Enabled = False
            Me.buttonClearAll.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.buttonClearAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.buttonClearAll.ImageIndex = 3
            Me.buttonClearAll.Location = New System.Drawing.Point(631, 32)
            Me.buttonClearAll.Name = "buttonClearAll"
            Me.buttonClearAll.Size = New System.Drawing.Size(81, 23)
            Me.buttonClearAll.TabIndex = 44
            Me.buttonClearAll.Tag = "RD"
            Me.buttonClearAll.Text = "Clear All"
            Me.buttonClearAll.UseVisualStyleBackColor = True
            '
            'buttonClearData
            '
            Me.buttonClearData.Enabled = False
            Me.buttonClearData.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.buttonClearData.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.buttonClearData.ImageIndex = 3
            Me.buttonClearData.Location = New System.Drawing.Point(544, 32)
            Me.buttonClearData.Name = "buttonClearData"
            Me.buttonClearData.Size = New System.Drawing.Size(81, 23)
            Me.buttonClearData.TabIndex = 43
            Me.buttonClearData.Tag = "RD"
            Me.buttonClearData.Text = "Clear Data"
            Me.buttonClearData.UseVisualStyleBackColor = True
            '
            'buttonClearFields
            '
            Me.buttonClearFields.Enabled = False
            Me.buttonClearFields.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.buttonClearFields.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.buttonClearFields.ImageIndex = 2
            Me.buttonClearFields.Location = New System.Drawing.Point(457, 32)
            Me.buttonClearFields.Name = "buttonClearFields"
            Me.buttonClearFields.Size = New System.Drawing.Size(81, 23)
            Me.buttonClearFields.TabIndex = 42
            Me.buttonClearFields.Tag = "RD"
            Me.buttonClearFields.Text = "Clear Fields"
            Me.buttonClearFields.UseVisualStyleBackColor = True
            '
            'buttonSendRequest
            '
            Me.buttonSendRequest.Enabled = False
            Me.buttonSendRequest.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.buttonSendRequest.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.buttonSendRequest.Location = New System.Drawing.Point(458, 3)
            Me.buttonSendRequest.Name = "buttonSendRequest"
            Me.buttonSendRequest.Size = New System.Drawing.Size(81, 23)
            Me.buttonSendRequest.TabIndex = 40
            Me.buttonSendRequest.Tag = "RD"
            Me.buttonSendRequest.Text = "Subscribe"
            Me.buttonSendRequest.UseVisualStyleBackColor = True
            '
            'checkBoxForceDelay
            '
            Me.checkBoxForceDelay.AutoSize = True
            Me.checkBoxForceDelay.Location = New System.Drawing.Point(131, 62)
            Me.checkBoxForceDelay.Name = "checkBoxForceDelay"
            Me.checkBoxForceDelay.Size = New System.Drawing.Size(119, 17)
            Me.checkBoxForceDelay.TabIndex = 38
            Me.checkBoxForceDelay.Tag = "RT"
            Me.checkBoxForceDelay.Text = "Force Delay Stream"
            Me.checkBoxForceDelay.UseVisualStyleBackColor = True
            '
            'checkBoxOutputFile
            '
            Me.checkBoxOutputFile.AutoSize = True
            Me.checkBoxOutputFile.Location = New System.Drawing.Point(256, 62)
            Me.checkBoxOutputFile.Name = "checkBoxOutputFile"
            Me.checkBoxOutputFile.Size = New System.Drawing.Size(89, 17)
            Me.checkBoxOutputFile.TabIndex = 39
            Me.checkBoxOutputFile.Tag = "RT"
            Me.checkBoxOutputFile.Text = "Output to File"
            Me.checkBoxOutputFile.UseVisualStyleBackColor = True
            '
            'textBoxInterval
            '
            Me.textBoxInterval.Location = New System.Drawing.Point(71, 60)
            Me.textBoxInterval.MaxLength = 5
            Me.textBoxInterval.Name = "textBoxInterval"
            Me.textBoxInterval.Size = New System.Drawing.Size(54, 20)
            Me.textBoxInterval.TabIndex = 37
            Me.textBoxInterval.Tag = "RT"
            Me.textBoxInterval.Text = "0"
            '
            'labelInterval
            '
            Me.labelInterval.AutoSize = True
            Me.labelInterval.Location = New System.Drawing.Point(-1, 63)
            Me.labelInterval.Name = "labelInterval"
            Me.labelInterval.Size = New System.Drawing.Size(71, 13)
            Me.labelInterval.TabIndex = 36
            Me.labelInterval.Text = "Interval (sec):"
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
            Me.dataGridViewData.CausesValidation = False
            Me.dataGridViewData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dataGridViewData.EnableHeadersVisualStyles = False
            Me.dataGridViewData.Location = New System.Drawing.Point(2, 118)
            Me.dataGridViewData.MultiSelect = False
            Me.dataGridViewData.Name = "dataGridViewData"
            Me.dataGridViewData.ReadOnly = True
            Me.dataGridViewData.RowHeadersVisible = False
            Me.dataGridViewData.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
            Me.dataGridViewData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
            Me.dataGridViewData.ShowCellErrors = False
            Me.dataGridViewData.ShowCellToolTips = False
            Me.dataGridViewData.ShowEditingIcon = False
            Me.dataGridViewData.ShowRowErrors = False
            Me.dataGridViewData.Size = New System.Drawing.Size(742, 404)
            Me.dataGridViewData.TabIndex = 45
            Me.dataGridViewData.Tag = "RT"
            '
            'buttonAddField
            '
            Me.buttonAddField.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.buttonAddField.ImageKey = "Symbol-Add.ico"
            Me.buttonAddField.Location = New System.Drawing.Point(371, 32)
            Me.buttonAddField.Name = "buttonAddField"
            Me.buttonAddField.Size = New System.Drawing.Size(81, 23)
            Me.buttonAddField.TabIndex = 35
            Me.buttonAddField.Tag = "RT"
            Me.buttonAddField.Text = "Add"
            Me.buttonAddField.UseVisualStyleBackColor = True
            '
            'textBoxField
            '
            Me.textBoxField.AllowDrop = True
            Me.textBoxField.Location = New System.Drawing.Point(71, 34)
            Me.textBoxField.Name = "textBoxField"
            Me.textBoxField.Size = New System.Drawing.Size(294, 20)
            Me.textBoxField.TabIndex = 34
            Me.textBoxField.Tag = "RT"
            '
            'labelField
            '
            Me.labelField.AutoSize = True
            Me.labelField.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.labelField.Location = New System.Drawing.Point(37, 37)
            Me.labelField.Name = "labelField"
            Me.labelField.Size = New System.Drawing.Size(32, 13)
            Me.labelField.TabIndex = 33
            Me.labelField.Text = "Field:"
            '
            'buttonAddSecurity
            '
            Me.buttonAddSecurity.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.buttonAddSecurity.ImageKey = "Symbol-Add.ico"
            Me.buttonAddSecurity.Location = New System.Drawing.Point(371, 3)
            Me.buttonAddSecurity.Name = "buttonAddSecurity"
            Me.buttonAddSecurity.Size = New System.Drawing.Size(81, 23)
            Me.buttonAddSecurity.TabIndex = 32
            Me.buttonAddSecurity.Tag = "RT"
            Me.buttonAddSecurity.Text = "Add"
            Me.buttonAddSecurity.UseVisualStyleBackColor = True
            '
            'textBoxSecurity
            '
            Me.textBoxSecurity.AllowDrop = True
            Me.textBoxSecurity.Location = New System.Drawing.Point(71, 6)
            Me.textBoxSecurity.Name = "textBoxSecurity"
            Me.textBoxSecurity.Size = New System.Drawing.Size(294, 20)
            Me.textBoxSecurity.TabIndex = 31
            Me.textBoxSecurity.Tag = "RT"
            '
            'labelSecurity
            '
            Me.labelSecurity.AutoSize = True
            Me.labelSecurity.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.labelSecurity.Location = New System.Drawing.Point(21, 9)
            Me.labelSecurity.Name = "labelSecurity"
            Me.labelSecurity.Size = New System.Drawing.Size(48, 13)
            Me.labelSecurity.TabIndex = 30
            Me.labelSecurity.Text = "Security:"
            '
            'statusStrip1
            '
            Me.statusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.toolStripStatusLabel1})
            Me.statusStrip1.Location = New System.Drawing.Point(0, 525)
            Me.statusStrip1.Name = "statusStrip1"
            Me.statusStrip1.Size = New System.Drawing.Size(747, 22)
            Me.statusStrip1.TabIndex = 47
            Me.statusStrip1.Text = "statusStrip1"
            '
            'toolStripStatusLabel1
            '
            Me.toolStripStatusLabel1.Name = "toolStripStatusLabel1"
            Me.toolStripStatusLabel1.Size = New System.Drawing.Size(0, 17)
            '
            'labelUsageNote
            '
            Me.labelUsageNote.AccessibleRole = System.Windows.Forms.AccessibleRole.None
            Me.labelUsageNote.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.labelUsageNote.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.labelUsageNote.Location = New System.Drawing.Point(2, 86)
            Me.labelUsageNote.Name = "labelUsageNote"
            Me.labelUsageNote.Size = New System.Drawing.Size(742, 29)
            Me.labelUsageNote.TabIndex = 48
            Me.labelUsageNote.Text = resources.GetString("labelUsageNote.Text")
            Me.labelUsageNote.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'Form1
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(747, 547)
            Me.Controls.Add(Me.labelUsageNote)
            Me.Controls.Add(Me.statusStrip1)
            Me.Controls.Add(Me.textBoxOutputFile)
            Me.Controls.Add(Me.buttonStopSubscribe)
            Me.Controls.Add(Me.buttonClearAll)
            Me.Controls.Add(Me.buttonClearData)
            Me.Controls.Add(Me.buttonClearFields)
            Me.Controls.Add(Me.buttonSendRequest)
            Me.Controls.Add(Me.checkBoxForceDelay)
            Me.Controls.Add(Me.checkBoxOutputFile)
            Me.Controls.Add(Me.textBoxInterval)
            Me.Controls.Add(Me.labelInterval)
            Me.Controls.Add(Me.dataGridViewData)
            Me.Controls.Add(Me.buttonAddField)
            Me.Controls.Add(Me.textBoxField)
            Me.Controls.Add(Me.labelField)
            Me.Controls.Add(Me.buttonAddSecurity)
            Me.Controls.Add(Me.textBoxSecurity)
            Me.Controls.Add(Me.labelSecurity)
            Me.MinimumSize = New System.Drawing.Size(755, 574)
            Me.Name = "Form1"
            Me.Text = "Simple Subscription Example"
            CType(Me.dataGridViewData, System.ComponentModel.ISupportInitialize).EndInit()
            Me.statusStrip1.ResumeLayout(False)
            Me.statusStrip1.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents textBoxOutputFile As System.Windows.Forms.TextBox
        Private WithEvents buttonStopSubscribe As System.Windows.Forms.Button
        Private WithEvents buttonClearAll As System.Windows.Forms.Button
        Private WithEvents buttonClearData As System.Windows.Forms.Button
        Private WithEvents buttonClearFields As System.Windows.Forms.Button
        Private WithEvents buttonSendRequest As System.Windows.Forms.Button
        Private WithEvents checkBoxForceDelay As System.Windows.Forms.CheckBox
        Private WithEvents checkBoxOutputFile As System.Windows.Forms.CheckBox
        Private WithEvents textBoxInterval As System.Windows.Forms.TextBox
        Private WithEvents labelInterval As System.Windows.Forms.Label
        Private WithEvents dataGridViewData As System.Windows.Forms.DataGridView
        Private WithEvents buttonAddField As System.Windows.Forms.Button
        Private WithEvents textBoxField As System.Windows.Forms.TextBox
        Private WithEvents labelField As System.Windows.Forms.Label
        Private WithEvents buttonAddSecurity As System.Windows.Forms.Button
        Private WithEvents textBoxSecurity As System.Windows.Forms.TextBox
        Private WithEvents labelSecurity As System.Windows.Forms.Label
        Private WithEvents statusStrip1 As System.Windows.Forms.StatusStrip
        Private WithEvents toolStripStatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
        Private WithEvents labelUsageNote As System.Windows.Forms.Label
    End Class
End Namespace