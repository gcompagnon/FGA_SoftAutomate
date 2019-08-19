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
            Me.buttonClearAll = New System.Windows.Forms.Button
            Me.radioButtonSynch = New System.Windows.Forms.RadioButton
            Me.radioButtonAsynch = New System.Windows.Forms.RadioButton
            Me.statusStrip1 = New System.Windows.Forms.StatusStrip
            Me.toolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel
            Me.labelOverrideNote = New System.Windows.Forms.Label
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
            Me.labelUsageNote = New System.Windows.Forms.Label
            Me.statusStrip1.SuspendLayout()
            Me.splitContainerRDData.Panel1.SuspendLayout()
            Me.splitContainerRDData.Panel2.SuspendLayout()
            Me.splitContainerRDData.SuspendLayout()
            CType(Me.dataGridViewData, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'buttonClearAll
            '
            Me.buttonClearAll.Enabled = False
            Me.buttonClearAll.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.buttonClearAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.buttonClearAll.ImageIndex = 3
            Me.buttonClearAll.Location = New System.Drawing.Point(723, 2)
            Me.buttonClearAll.Name = "buttonClearAll"
            Me.buttonClearAll.Size = New System.Drawing.Size(81, 23)
            Me.buttonClearAll.TabIndex = 53
            Me.buttonClearAll.Tag = "RD"
            Me.buttonClearAll.Text = "Clear All"
            Me.buttonClearAll.UseVisualStyleBackColor = True
            '
            'radioButtonSynch
            '
            Me.radioButtonSynch.AutoSize = True
            Me.radioButtonSynch.Location = New System.Drawing.Point(563, 33)
            Me.radioButtonSynch.Name = "radioButtonSynch"
            Me.radioButtonSynch.Size = New System.Drawing.Size(87, 17)
            Me.radioButtonSynch.TabIndex = 55
            Me.radioButtonSynch.Text = "Synchronous"
            Me.radioButtonSynch.UseVisualStyleBackColor = True
            '
            'radioButtonAsynch
            '
            Me.radioButtonAsynch.AutoSize = True
            Me.radioButtonAsynch.Checked = True
            Me.radioButtonAsynch.Location = New System.Drawing.Point(465, 33)
            Me.radioButtonAsynch.Name = "radioButtonAsynch"
            Me.radioButtonAsynch.Size = New System.Drawing.Size(92, 17)
            Me.radioButtonAsynch.TabIndex = 54
            Me.radioButtonAsynch.TabStop = True
            Me.radioButtonAsynch.Text = "Asynchronous"
            Me.radioButtonAsynch.UseVisualStyleBackColor = True
            '
            'statusStrip1
            '
            Me.statusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.toolStripStatusLabel1})
            Me.statusStrip1.Location = New System.Drawing.Point(0, 481)
            Me.statusStrip1.Name = "statusStrip1"
            Me.statusStrip1.Size = New System.Drawing.Size(811, 22)
            Me.statusStrip1.TabIndex = 57
            '
            'toolStripStatusLabel1
            '
            Me.toolStripStatusLabel1.Name = "toolStripStatusLabel1"
            Me.toolStripStatusLabel1.Size = New System.Drawing.Size(0, 17)
            '
            'labelOverrideNote
            '
            Me.labelOverrideNote.AutoSize = True
            Me.labelOverrideNote.Location = New System.Drawing.Point(461, 64)
            Me.labelOverrideNote.Name = "labelOverrideNote"
            Me.labelOverrideNote.Size = New System.Drawing.Size(249, 13)
            Me.labelOverrideNote.TabIndex = 56
            Me.labelOverrideNote.Text = "(Note: Override example:  SETTLE_DT=20081008)"
            '
            'splitContainerRDData
            '
            Me.splitContainerRDData.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.splitContainerRDData.Location = New System.Drawing.Point(6, 118)
            Me.splitContainerRDData.Name = "splitContainerRDData"
            '
            'splitContainerRDData.Panel1
            '
            Me.splitContainerRDData.Panel1.Controls.Add(Me.dataGridViewData)
            '
            'splitContainerRDData.Panel2
            '
            Me.splitContainerRDData.Panel2.Controls.Add(Me.listViewOverrides)
            Me.splitContainerRDData.Size = New System.Drawing.Size(801, 351)
            Me.splitContainerRDData.SplitterDistance = 559
            Me.splitContainerRDData.TabIndex = 58
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
            Me.dataGridViewData.Size = New System.Drawing.Size(559, 351)
            Me.dataGridViewData.TabIndex = 14
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
            Me.listViewOverrides.Size = New System.Drawing.Size(238, 351)
            Me.listViewOverrides.TabIndex = 15
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
            Me.buttonAddOverride.Location = New System.Drawing.Point(375, 59)
            Me.buttonAddOverride.Name = "buttonAddOverride"
            Me.buttonAddOverride.Size = New System.Drawing.Size(81, 23)
            Me.buttonAddOverride.TabIndex = 49
            Me.buttonAddOverride.Tag = "RD"
            Me.buttonAddOverride.Text = "Add"
            Me.buttonAddOverride.UseVisualStyleBackColor = True
            '
            'textBoxOverride
            '
            Me.textBoxOverride.Location = New System.Drawing.Point(75, 61)
            Me.textBoxOverride.Name = "textBoxOverride"
            Me.textBoxOverride.Size = New System.Drawing.Size(294, 20)
            Me.textBoxOverride.TabIndex = 48
            Me.textBoxOverride.Tag = "RD"
            '
            'labelOverride
            '
            Me.labelOverride.AutoSize = True
            Me.labelOverride.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.labelOverride.Location = New System.Drawing.Point(23, 64)
            Me.labelOverride.Name = "labelOverride"
            Me.labelOverride.Size = New System.Drawing.Size(50, 13)
            Me.labelOverride.TabIndex = 47
            Me.labelOverride.Tag = "RD"
            Me.labelOverride.Text = "Override:"
            '
            'buttonClearData
            '
            Me.buttonClearData.Enabled = False
            Me.buttonClearData.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.buttonClearData.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.buttonClearData.ImageIndex = 3
            Me.buttonClearData.Location = New System.Drawing.Point(636, 2)
            Me.buttonClearData.Name = "buttonClearData"
            Me.buttonClearData.Size = New System.Drawing.Size(81, 23)
            Me.buttonClearData.TabIndex = 52
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
            Me.buttonClearFields.Location = New System.Drawing.Point(549, 2)
            Me.buttonClearFields.Name = "buttonClearFields"
            Me.buttonClearFields.Size = New System.Drawing.Size(81, 23)
            Me.buttonClearFields.TabIndex = 51
            Me.buttonClearFields.Tag = "RD"
            Me.buttonClearFields.Text = "Clear Fields"
            Me.buttonClearFields.UseVisualStyleBackColor = True
            '
            'buttonAddField
            '
            Me.buttonAddField.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.buttonAddField.Location = New System.Drawing.Point(375, 30)
            Me.buttonAddField.Name = "buttonAddField"
            Me.buttonAddField.Size = New System.Drawing.Size(81, 23)
            Me.buttonAddField.TabIndex = 46
            Me.buttonAddField.Tag = "RD"
            Me.buttonAddField.Text = "Add"
            Me.buttonAddField.UseVisualStyleBackColor = True
            '
            'textBoxField
            '
            Me.textBoxField.AllowDrop = True
            Me.textBoxField.Location = New System.Drawing.Point(75, 32)
            Me.textBoxField.Name = "textBoxField"
            Me.textBoxField.Size = New System.Drawing.Size(294, 20)
            Me.textBoxField.TabIndex = 45
            Me.textBoxField.Tag = "RD"
            '
            'labelField
            '
            Me.labelField.AutoSize = True
            Me.labelField.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.labelField.Location = New System.Drawing.Point(41, 35)
            Me.labelField.Name = "labelField"
            Me.labelField.Size = New System.Drawing.Size(32, 13)
            Me.labelField.TabIndex = 44
            Me.labelField.Text = "Field:"
            '
            'buttonSendRequest
            '
            Me.buttonSendRequest.Enabled = False
            Me.buttonSendRequest.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.buttonSendRequest.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.buttonSendRequest.Location = New System.Drawing.Point(462, 2)
            Me.buttonSendRequest.Name = "buttonSendRequest"
            Me.buttonSendRequest.Size = New System.Drawing.Size(81, 23)
            Me.buttonSendRequest.TabIndex = 50
            Me.buttonSendRequest.Tag = "RD"
            Me.buttonSendRequest.Text = "Submit"
            Me.buttonSendRequest.UseVisualStyleBackColor = True
            '
            'buttonAddSecurity
            '
            Me.buttonAddSecurity.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.buttonAddSecurity.Location = New System.Drawing.Point(375, 2)
            Me.buttonAddSecurity.Name = "buttonAddSecurity"
            Me.buttonAddSecurity.Size = New System.Drawing.Size(81, 23)
            Me.buttonAddSecurity.TabIndex = 43
            Me.buttonAddSecurity.Tag = "RD"
            Me.buttonAddSecurity.Text = "Add"
            Me.buttonAddSecurity.UseVisualStyleBackColor = True
            '
            'textBoxSecurity
            '
            Me.textBoxSecurity.AllowDrop = True
            Me.textBoxSecurity.Location = New System.Drawing.Point(75, 4)
            Me.textBoxSecurity.Name = "textBoxSecurity"
            Me.textBoxSecurity.Size = New System.Drawing.Size(294, 20)
            Me.textBoxSecurity.TabIndex = 42
            Me.textBoxSecurity.Tag = "RD"
            '
            'labelSecurity
            '
            Me.labelSecurity.AutoSize = True
            Me.labelSecurity.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.labelSecurity.Location = New System.Drawing.Point(25, 7)
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
            Me.labelUsageNote.Location = New System.Drawing.Point(6, 86)
            Me.labelUsageNote.Name = "labelUsageNote"
            Me.labelUsageNote.Size = New System.Drawing.Size(800, 29)
            Me.labelUsageNote.TabIndex = 59
            Me.labelUsageNote.Text = resources.GetString("labelUsageNote.Text")
            Me.labelUsageNote.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'Form1
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(811, 503)
            Me.Controls.Add(Me.labelUsageNote)
            Me.Controls.Add(Me.buttonClearAll)
            Me.Controls.Add(Me.radioButtonSynch)
            Me.Controls.Add(Me.radioButtonAsynch)
            Me.Controls.Add(Me.statusStrip1)
            Me.Controls.Add(Me.labelOverrideNote)
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
            Me.Text = "Simple Reference Data with Override Example"
            Me.statusStrip1.ResumeLayout(False)
            Me.statusStrip1.PerformLayout()
            Me.splitContainerRDData.Panel1.ResumeLayout(False)
            Me.splitContainerRDData.Panel2.ResumeLayout(False)
            Me.splitContainerRDData.ResumeLayout(False)
            CType(Me.dataGridViewData, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents buttonClearAll As System.Windows.Forms.Button
        Private WithEvents radioButtonSynch As System.Windows.Forms.RadioButton
        Private WithEvents radioButtonAsynch As System.Windows.Forms.RadioButton
        Private WithEvents statusStrip1 As System.Windows.Forms.StatusStrip
        Private WithEvents toolStripStatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
        Private WithEvents labelOverrideNote As System.Windows.Forms.Label
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
        Private WithEvents labelUsageNote As System.Windows.Forms.Label

    End Class
End Namespace