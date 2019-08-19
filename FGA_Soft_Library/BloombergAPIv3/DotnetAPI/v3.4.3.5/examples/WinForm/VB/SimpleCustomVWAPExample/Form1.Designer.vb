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
            Me.labelOverrideNote = New System.Windows.Forms.Label
            Me.buttonStopSubscribe = New System.Windows.Forms.Button
            Me.buttonClearAll = New System.Windows.Forms.Button
            Me.splitContainerRDData = New System.Windows.Forms.SplitContainer
            Me.dataGridViewData = New System.Windows.Forms.DataGridView
            Me.listViewOverrides = New System.Windows.Forms.ListView
            Me.columnHeaderRDOvrFields = New System.Windows.Forms.ColumnHeader
            Me.columnHeaderRDOvrValues = New System.Windows.Forms.ColumnHeader
            Me.buttonAddOverride = New System.Windows.Forms.Button
            Me.textBoxOverride = New System.Windows.Forms.TextBox
            Me.labelOverride = New System.Windows.Forms.Label
            Me.buttonClearData = New System.Windows.Forms.Button
            Me.buttonClearFields = New System.Windows.Forms.Button
            Me.buttonAddField = New System.Windows.Forms.Button
            Me.textBoxField = New System.Windows.Forms.TextBox
            Me.labelField = New System.Windows.Forms.Label
            Me.buttonSendRequest = New System.Windows.Forms.Button
            Me.buttonAddSecurity = New System.Windows.Forms.Button
            Me.textBoxSecurity = New System.Windows.Forms.TextBox
            Me.labelSecurity = New System.Windows.Forms.Label
            Me.statusStrip1 = New System.Windows.Forms.StatusStrip
            Me.toolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel
            Me.labelUsageNote = New System.Windows.Forms.Label
            Me.splitContainerRDData.Panel1.SuspendLayout()
            Me.splitContainerRDData.Panel2.SuspendLayout()
            Me.splitContainerRDData.SuspendLayout()
            CType(Me.dataGridViewData, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.statusStrip1.SuspendLayout()
            Me.SuspendLayout()
            '
            'labelOverrideNote
            '
            Me.labelOverrideNote.AutoSize = True
            Me.labelOverrideNote.Location = New System.Drawing.Point(470, 69)
            Me.labelOverrideNote.Name = "labelOverrideNote"
            Me.labelOverrideNote.Size = New System.Drawing.Size(266, 13)
            Me.labelOverrideNote.TabIndex = 74
            Me.labelOverrideNote.Text = "(Note: Override example:  VWAP_START_TIME=9:30)"
            '
            'buttonStopSubscribe
            '
            Me.buttonStopSubscribe.Enabled = False
            Me.buttonStopSubscribe.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.buttonStopSubscribe.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.buttonStopSubscribe.Location = New System.Drawing.Point(551, 5)
            Me.buttonStopSubscribe.Name = "buttonStopSubscribe"
            Me.buttonStopSubscribe.Size = New System.Drawing.Size(81, 23)
            Me.buttonStopSubscribe.TabIndex = 69
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
            Me.buttonClearAll.Location = New System.Drawing.Point(638, 34)
            Me.buttonClearAll.Name = "buttonClearAll"
            Me.buttonClearAll.Size = New System.Drawing.Size(81, 23)
            Me.buttonClearAll.TabIndex = 72
            Me.buttonClearAll.Tag = "RD"
            Me.buttonClearAll.Text = "Clear All"
            Me.buttonClearAll.UseVisualStyleBackColor = True
            '
            'splitContainerRDData
            '
            Me.splitContainerRDData.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.splitContainerRDData.Location = New System.Drawing.Point(5, 124)
            Me.splitContainerRDData.Name = "splitContainerRDData"
            '
            'splitContainerRDData.Panel1
            '
            Me.splitContainerRDData.Panel1.Controls.Add(Me.dataGridViewData)
            '
            'splitContainerRDData.Panel2
            '
            Me.splitContainerRDData.Panel2.Controls.Add(Me.listViewOverrides)
            Me.splitContainerRDData.Size = New System.Drawing.Size(801, 350)
            Me.splitContainerRDData.SplitterDistance = 559
            Me.splitContainerRDData.TabIndex = 73
            '
            'dataGridViewData
            '
            Me.dataGridViewData.AllowDrop = True
            Me.dataGridViewData.AllowUserToAddRows = False
            Me.dataGridViewData.AllowUserToDeleteRows = False
            Me.dataGridViewData.AllowUserToOrderColumns = True
            Me.dataGridViewData.AllowUserToResizeRows = False
            Me.dataGridViewData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dataGridViewData.Dock = System.Windows.Forms.DockStyle.Fill
            Me.dataGridViewData.Location = New System.Drawing.Point(0, 0)
            Me.dataGridViewData.MultiSelect = False
            Me.dataGridViewData.Name = "dataGridViewData"
            Me.dataGridViewData.ReadOnly = True
            Me.dataGridViewData.RowHeadersVisible = False
            Me.dataGridViewData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
            Me.dataGridViewData.Size = New System.Drawing.Size(559, 350)
            Me.dataGridViewData.TabIndex = 16
            Me.dataGridViewData.Tag = "RD"
            '
            'listViewOverrides
            '
            Me.listViewOverrides.AllowDrop = True
            Me.listViewOverrides.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.columnHeaderRDOvrFields, Me.columnHeaderRDOvrValues})
            Me.listViewOverrides.Dock = System.Windows.Forms.DockStyle.Fill
            Me.listViewOverrides.FullRowSelect = True
            Me.listViewOverrides.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
            Me.listViewOverrides.Location = New System.Drawing.Point(0, 0)
            Me.listViewOverrides.Name = "listViewOverrides"
            Me.listViewOverrides.Size = New System.Drawing.Size(238, 350)
            Me.listViewOverrides.TabIndex = 17
            Me.listViewOverrides.Tag = "RD"
            Me.listViewOverrides.UseCompatibleStateImageBehavior = False
            Me.listViewOverrides.View = System.Windows.Forms.View.Details
            '
            'columnHeaderRDOvrFields
            '
            Me.columnHeaderRDOvrFields.Text = "Override Fields"
            Me.columnHeaderRDOvrFields.Width = 149
            '
            'columnHeaderRDOvrValues
            '
            Me.columnHeaderRDOvrValues.Text = "Values"
            Me.columnHeaderRDOvrValues.Width = 80
            '
            'buttonAddOverride
            '
            Me.buttonAddOverride.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.buttonAddOverride.Location = New System.Drawing.Point(374, 64)
            Me.buttonAddOverride.Name = "buttonAddOverride"
            Me.buttonAddOverride.Size = New System.Drawing.Size(81, 23)
            Me.buttonAddOverride.TabIndex = 67
            Me.buttonAddOverride.Tag = "RD"
            Me.buttonAddOverride.Text = "Add"
            Me.buttonAddOverride.UseVisualStyleBackColor = True
            '
            'textBoxOverride
            '
            Me.textBoxOverride.Location = New System.Drawing.Point(74, 66)
            Me.textBoxOverride.Name = "textBoxOverride"
            Me.textBoxOverride.Size = New System.Drawing.Size(294, 20)
            Me.textBoxOverride.TabIndex = 66
            Me.textBoxOverride.Tag = "RD"
            '
            'labelOverride
            '
            Me.labelOverride.AutoSize = True
            Me.labelOverride.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.labelOverride.Location = New System.Drawing.Point(22, 69)
            Me.labelOverride.Name = "labelOverride"
            Me.labelOverride.Size = New System.Drawing.Size(50, 13)
            Me.labelOverride.TabIndex = 65
            Me.labelOverride.Tag = "RD"
            Me.labelOverride.Text = "Override:"
            '
            'buttonClearData
            '
            Me.buttonClearData.Enabled = False
            Me.buttonClearData.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.buttonClearData.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.buttonClearData.ImageIndex = 3
            Me.buttonClearData.Location = New System.Drawing.Point(551, 34)
            Me.buttonClearData.Name = "buttonClearData"
            Me.buttonClearData.Size = New System.Drawing.Size(81, 23)
            Me.buttonClearData.TabIndex = 71
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
            Me.buttonClearFields.Location = New System.Drawing.Point(464, 34)
            Me.buttonClearFields.Name = "buttonClearFields"
            Me.buttonClearFields.Size = New System.Drawing.Size(81, 23)
            Me.buttonClearFields.TabIndex = 70
            Me.buttonClearFields.Tag = "RD"
            Me.buttonClearFields.Text = "Clear Fields"
            Me.buttonClearFields.UseVisualStyleBackColor = True
            '
            'buttonAddField
            '
            Me.buttonAddField.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.buttonAddField.Location = New System.Drawing.Point(374, 35)
            Me.buttonAddField.Name = "buttonAddField"
            Me.buttonAddField.Size = New System.Drawing.Size(81, 23)
            Me.buttonAddField.TabIndex = 64
            Me.buttonAddField.Tag = "RD"
            Me.buttonAddField.Text = "Add"
            Me.buttonAddField.UseVisualStyleBackColor = True
            '
            'textBoxField
            '
            Me.textBoxField.AllowDrop = True
            Me.textBoxField.Location = New System.Drawing.Point(74, 37)
            Me.textBoxField.Name = "textBoxField"
            Me.textBoxField.Size = New System.Drawing.Size(294, 20)
            Me.textBoxField.TabIndex = 63
            Me.textBoxField.Tag = "RD"
            '
            'labelField
            '
            Me.labelField.AutoSize = True
            Me.labelField.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.labelField.Location = New System.Drawing.Point(40, 40)
            Me.labelField.Name = "labelField"
            Me.labelField.Size = New System.Drawing.Size(32, 13)
            Me.labelField.TabIndex = 62
            Me.labelField.Text = "Field:"
            '
            'buttonSendRequest
            '
            Me.buttonSendRequest.Enabled = False
            Me.buttonSendRequest.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.buttonSendRequest.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.buttonSendRequest.Location = New System.Drawing.Point(464, 5)
            Me.buttonSendRequest.Name = "buttonSendRequest"
            Me.buttonSendRequest.Size = New System.Drawing.Size(81, 23)
            Me.buttonSendRequest.TabIndex = 68
            Me.buttonSendRequest.Tag = "RD"
            Me.buttonSendRequest.Text = "Submit"
            Me.buttonSendRequest.UseVisualStyleBackColor = True
            '
            'buttonAddSecurity
            '
            Me.buttonAddSecurity.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.buttonAddSecurity.Location = New System.Drawing.Point(374, 6)
            Me.buttonAddSecurity.Name = "buttonAddSecurity"
            Me.buttonAddSecurity.Size = New System.Drawing.Size(81, 23)
            Me.buttonAddSecurity.TabIndex = 61
            Me.buttonAddSecurity.Tag = "RD"
            Me.buttonAddSecurity.Text = "Add"
            Me.buttonAddSecurity.UseVisualStyleBackColor = True
            '
            'textBoxSecurity
            '
            Me.textBoxSecurity.AllowDrop = True
            Me.textBoxSecurity.Location = New System.Drawing.Point(74, 8)
            Me.textBoxSecurity.Name = "textBoxSecurity"
            Me.textBoxSecurity.Size = New System.Drawing.Size(294, 20)
            Me.textBoxSecurity.TabIndex = 60
            Me.textBoxSecurity.Tag = "RD"
            '
            'labelSecurity
            '
            Me.labelSecurity.AutoSize = True
            Me.labelSecurity.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.labelSecurity.Location = New System.Drawing.Point(24, 11)
            Me.labelSecurity.Name = "labelSecurity"
            Me.labelSecurity.Size = New System.Drawing.Size(48, 13)
            Me.labelSecurity.TabIndex = 59
            Me.labelSecurity.Text = "Security:"
            '
            'statusStrip1
            '
            Me.statusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.toolStripStatusLabel1})
            Me.statusStrip1.Location = New System.Drawing.Point(0, 481)
            Me.statusStrip1.Name = "statusStrip1"
            Me.statusStrip1.Size = New System.Drawing.Size(811, 22)
            Me.statusStrip1.TabIndex = 75
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
            Me.labelUsageNote.Location = New System.Drawing.Point(5, 92)
            Me.labelUsageNote.Name = "labelUsageNote"
            Me.labelUsageNote.Size = New System.Drawing.Size(800, 29)
            Me.labelUsageNote.TabIndex = 76
            Me.labelUsageNote.Text = "Note: User can delete field/security by selecting a cell within the field column " & _
                "or the security name and press the delete key.  Drag and drop securities from MO" & _
                "ST<GO> to add securities to grid."
            Me.labelUsageNote.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'Form1
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(811, 503)
            Me.Controls.Add(Me.labelUsageNote)
            Me.Controls.Add(Me.statusStrip1)
            Me.Controls.Add(Me.labelOverrideNote)
            Me.Controls.Add(Me.buttonStopSubscribe)
            Me.Controls.Add(Me.buttonClearAll)
            Me.Controls.Add(Me.splitContainerRDData)
            Me.Controls.Add(Me.buttonAddOverride)
            Me.Controls.Add(Me.textBoxOverride)
            Me.Controls.Add(Me.labelOverride)
            Me.Controls.Add(Me.buttonClearData)
            Me.Controls.Add(Me.buttonClearFields)
            Me.Controls.Add(Me.buttonAddField)
            Me.Controls.Add(Me.textBoxField)
            Me.Controls.Add(Me.labelField)
            Me.Controls.Add(Me.buttonSendRequest)
            Me.Controls.Add(Me.buttonAddSecurity)
            Me.Controls.Add(Me.textBoxSecurity)
            Me.Controls.Add(Me.labelSecurity)
            Me.MinimumSize = New System.Drawing.Size(819, 530)
            Me.Name = "Form1"
            Me.Text = "Form1"
            Me.splitContainerRDData.Panel1.ResumeLayout(False)
            Me.splitContainerRDData.Panel2.ResumeLayout(False)
            Me.splitContainerRDData.ResumeLayout(False)
            CType(Me.dataGridViewData, System.ComponentModel.ISupportInitialize).EndInit()
            Me.statusStrip1.ResumeLayout(False)
            Me.statusStrip1.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents labelOverrideNote As System.Windows.Forms.Label
        Private WithEvents buttonStopSubscribe As System.Windows.Forms.Button
        Private WithEvents buttonClearAll As System.Windows.Forms.Button
        Private WithEvents splitContainerRDData As System.Windows.Forms.SplitContainer
        Private WithEvents dataGridViewData As System.Windows.Forms.DataGridView
        Private WithEvents listViewOverrides As System.Windows.Forms.ListView
        Private WithEvents columnHeaderRDOvrFields As System.Windows.Forms.ColumnHeader
        Private WithEvents columnHeaderRDOvrValues As System.Windows.Forms.ColumnHeader
        Private WithEvents buttonAddOverride As System.Windows.Forms.Button
        Private WithEvents textBoxOverride As System.Windows.Forms.TextBox
        Private WithEvents labelOverride As System.Windows.Forms.Label
        Private WithEvents buttonClearData As System.Windows.Forms.Button
        Private WithEvents buttonClearFields As System.Windows.Forms.Button
        Private WithEvents buttonAddField As System.Windows.Forms.Button
        Private WithEvents textBoxField As System.Windows.Forms.TextBox
        Private WithEvents labelField As System.Windows.Forms.Label
        Private WithEvents buttonSendRequest As System.Windows.Forms.Button
        Private WithEvents buttonAddSecurity As System.Windows.Forms.Button
        Private WithEvents textBoxSecurity As System.Windows.Forms.TextBox
        Private WithEvents labelSecurity As System.Windows.Forms.Label
        Private WithEvents statusStrip1 As System.Windows.Forms.StatusStrip
        Private WithEvents toolStripStatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
        Private WithEvents labelUsageNote As System.Windows.Forms.Label

    End Class
End Namespace