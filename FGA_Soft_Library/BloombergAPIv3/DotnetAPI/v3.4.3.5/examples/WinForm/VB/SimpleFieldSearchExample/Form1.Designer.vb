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
            Me.panelFieldSearchTop = New System.Windows.Forms.Panel
            Me.labelSearchType = New System.Windows.Forms.Label
            Me.comboBoxSearchType = New System.Windows.Forms.ComboBox
            Me.buttonSubmitSearch = New System.Windows.Forms.Button
            Me.groupBoxExcludeOptions = New System.Windows.Forms.GroupBox
            Me.comboBoxExcludeFieldType = New System.Windows.Forms.ComboBox
            Me.labelFieldType = New System.Windows.Forms.Label
            Me.comboBoxExcludeProductType = New System.Windows.Forms.ComboBox
            Me.labelExcludeProductType = New System.Windows.Forms.Label
            Me.groupBoxIncludOption = New System.Windows.Forms.GroupBox
            Me.comboBoxIncludeFieldType = New System.Windows.Forms.ComboBox
            Me.labelIncludeFieldType = New System.Windows.Forms.Label
            Me.comboBoxIncludeProductType = New System.Windows.Forms.ComboBox
            Me.labelIncludeProductType = New System.Windows.Forms.Label
            Me.textBoxSearchSpec = New System.Windows.Forms.TextBox
            Me.labelSearchSpec = New System.Windows.Forms.Label
            Me.dataGridViewDataView = New System.Windows.Forms.DataGridView
            Me.statusStrip1 = New System.Windows.Forms.StatusStrip
            Me.toolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel
            Me.splitContainerInfo = New System.Windows.Forms.SplitContainer
            Me.labelDocumentation = New System.Windows.Forms.Label
            Me.richTextBoxDocumentation = New System.Windows.Forms.RichTextBox
            Me.labelOverrides = New System.Windows.Forms.Label
            Me.dataGridViewOverrides = New System.Windows.Forms.DataGridView
            Me.panelFieldSearchTop.SuspendLayout()
            Me.groupBoxExcludeOptions.SuspendLayout()
            Me.groupBoxIncludOption.SuspendLayout()
            CType(Me.dataGridViewDataView, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.statusStrip1.SuspendLayout()
            Me.splitContainerInfo.Panel1.SuspendLayout()
            Me.splitContainerInfo.Panel2.SuspendLayout()
            Me.splitContainerInfo.SuspendLayout()
            CType(Me.dataGridViewOverrides, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'panelFieldSearchTop
            '
            Me.panelFieldSearchTop.Controls.Add(Me.labelSearchType)
            Me.panelFieldSearchTop.Controls.Add(Me.comboBoxSearchType)
            Me.panelFieldSearchTop.Controls.Add(Me.buttonSubmitSearch)
            Me.panelFieldSearchTop.Controls.Add(Me.groupBoxExcludeOptions)
            Me.panelFieldSearchTop.Controls.Add(Me.groupBoxIncludOption)
            Me.panelFieldSearchTop.Controls.Add(Me.textBoxSearchSpec)
            Me.panelFieldSearchTop.Controls.Add(Me.labelSearchSpec)
            Me.panelFieldSearchTop.Dock = System.Windows.Forms.DockStyle.Top
            Me.panelFieldSearchTop.Location = New System.Drawing.Point(0, 0)
            Me.panelFieldSearchTop.Name = "panelFieldSearchTop"
            Me.panelFieldSearchTop.Size = New System.Drawing.Size(870, 87)
            Me.panelFieldSearchTop.TabIndex = 2
            '
            'labelSearchType
            '
            Me.labelSearchType.AutoSize = True
            Me.labelSearchType.Location = New System.Drawing.Point(416, 9)
            Me.labelSearchType.Name = "labelSearchType"
            Me.labelSearchType.Size = New System.Drawing.Size(71, 13)
            Me.labelSearchType.TabIndex = 2
            Me.labelSearchType.Text = "Search Type:"
            '
            'comboBoxSearchType
            '
            Me.comboBoxSearchType.FormattingEnabled = True
            Me.comboBoxSearchType.ItemHeight = 13
            Me.comboBoxSearchType.Items.AddRange(New Object() {"Field Search", "Category Field Search"})
            Me.comboBoxSearchType.Location = New System.Drawing.Point(489, 6)
            Me.comboBoxSearchType.Name = "comboBoxSearchType"
            Me.comboBoxSearchType.Size = New System.Drawing.Size(229, 21)
            Me.comboBoxSearchType.TabIndex = 3
            '
            'buttonSubmitSearch
            '
            Me.buttonSubmitSearch.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.buttonSubmitSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.buttonSubmitSearch.ImageIndex = 7
            Me.buttonSubmitSearch.Location = New System.Drawing.Point(724, 4)
            Me.buttonSubmitSearch.Name = "buttonSubmitSearch"
            Me.buttonSubmitSearch.Size = New System.Drawing.Size(73, 23)
            Me.buttonSubmitSearch.TabIndex = 4
            Me.buttonSubmitSearch.Tag = "FS"
            Me.buttonSubmitSearch.Text = "Search"
            Me.buttonSubmitSearch.UseVisualStyleBackColor = True
            '
            'groupBoxExcludeOptions
            '
            Me.groupBoxExcludeOptions.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.groupBoxExcludeOptions.Controls.Add(Me.comboBoxExcludeFieldType)
            Me.groupBoxExcludeOptions.Controls.Add(Me.labelFieldType)
            Me.groupBoxExcludeOptions.Controls.Add(Me.comboBoxExcludeProductType)
            Me.groupBoxExcludeOptions.Controls.Add(Me.labelExcludeProductType)
            Me.groupBoxExcludeOptions.Location = New System.Drawing.Point(385, 30)
            Me.groupBoxExcludeOptions.MinimumSize = New System.Drawing.Size(262, 50)
            Me.groupBoxExcludeOptions.Name = "groupBoxExcludeOptions"
            Me.groupBoxExcludeOptions.Size = New System.Drawing.Size(482, 50)
            Me.groupBoxExcludeOptions.TabIndex = 6
            Me.groupBoxExcludeOptions.TabStop = False
            Me.groupBoxExcludeOptions.Text = "Exclude Options"
            '
            'comboBoxExcludeFieldType
            '
            Me.comboBoxExcludeFieldType.FormattingEnabled = True
            Me.comboBoxExcludeFieldType.Items.AddRange(New Object() {"None", "All", "RealTime", "Static"})
            Me.comboBoxExcludeFieldType.Location = New System.Drawing.Point(254, 19)
            Me.comboBoxExcludeFieldType.Name = "comboBoxExcludeFieldType"
            Me.comboBoxExcludeFieldType.Size = New System.Drawing.Size(107, 21)
            Me.comboBoxExcludeFieldType.TabIndex = 11
            '
            'labelFieldType
            '
            Me.labelFieldType.AutoSize = True
            Me.labelFieldType.Location = New System.Drawing.Point(193, 22)
            Me.labelFieldType.Name = "labelFieldType"
            Me.labelFieldType.Size = New System.Drawing.Size(59, 13)
            Me.labelFieldType.TabIndex = 10
            Me.labelFieldType.Text = "Field Type:"
            '
            'comboBoxExcludeProductType
            '
            Me.comboBoxExcludeProductType.FormattingEnabled = True
            Me.comboBoxExcludeProductType.Items.AddRange(New Object() {"None", "All", "Govt", "Corp", "Mtge", "M-Mkt", "Muni", "Pfd", "Equity", "Cmdty", "Index", "Curncy"})
            Me.comboBoxExcludeProductType.Location = New System.Drawing.Point(82, 19)
            Me.comboBoxExcludeProductType.Name = "comboBoxExcludeProductType"
            Me.comboBoxExcludeProductType.Size = New System.Drawing.Size(107, 21)
            Me.comboBoxExcludeProductType.TabIndex = 9
            '
            'labelExcludeProductType
            '
            Me.labelExcludeProductType.AutoSize = True
            Me.labelExcludeProductType.Location = New System.Drawing.Point(6, 22)
            Me.labelExcludeProductType.Name = "labelExcludeProductType"
            Me.labelExcludeProductType.Size = New System.Drawing.Size(74, 13)
            Me.labelExcludeProductType.TabIndex = 8
            Me.labelExcludeProductType.Text = "Product Type:"
            '
            'groupBoxIncludOption
            '
            Me.groupBoxIncludOption.Controls.Add(Me.comboBoxIncludeFieldType)
            Me.groupBoxIncludOption.Controls.Add(Me.labelIncludeFieldType)
            Me.groupBoxIncludOption.Controls.Add(Me.comboBoxIncludeProductType)
            Me.groupBoxIncludOption.Controls.Add(Me.labelIncludeProductType)
            Me.groupBoxIncludOption.Location = New System.Drawing.Point(6, 30)
            Me.groupBoxIncludOption.Name = "groupBoxIncludOption"
            Me.groupBoxIncludOption.Size = New System.Drawing.Size(373, 50)
            Me.groupBoxIncludOption.TabIndex = 5
            Me.groupBoxIncludOption.TabStop = False
            Me.groupBoxIncludOption.Text = "Include Options"
            '
            'comboBoxIncludeFieldType
            '
            Me.comboBoxIncludeFieldType.FormattingEnabled = True
            Me.comboBoxIncludeFieldType.Items.AddRange(New Object() {"None", "All", "RealTime", "Static"})
            Me.comboBoxIncludeFieldType.Location = New System.Drawing.Point(253, 19)
            Me.comboBoxIncludeFieldType.Name = "comboBoxIncludeFieldType"
            Me.comboBoxIncludeFieldType.Size = New System.Drawing.Size(107, 21)
            Me.comboBoxIncludeFieldType.TabIndex = 8
            '
            'labelIncludeFieldType
            '
            Me.labelIncludeFieldType.AutoSize = True
            Me.labelIncludeFieldType.Location = New System.Drawing.Point(192, 22)
            Me.labelIncludeFieldType.Name = "labelIncludeFieldType"
            Me.labelIncludeFieldType.Size = New System.Drawing.Size(59, 13)
            Me.labelIncludeFieldType.TabIndex = 7
            Me.labelIncludeFieldType.Text = "Field Type:"
            '
            'comboBoxIncludeProductType
            '
            Me.comboBoxIncludeProductType.FormattingEnabled = True
            Me.comboBoxIncludeProductType.Items.AddRange(New Object() {"None", "All", "Govt", "Corp", "Mtge", "M-Mkt", "Muni", "Pfd", "Equity", "Cmdty", "Index", "Curncy"})
            Me.comboBoxIncludeProductType.Location = New System.Drawing.Point(83, 19)
            Me.comboBoxIncludeProductType.Name = "comboBoxIncludeProductType"
            Me.comboBoxIncludeProductType.Size = New System.Drawing.Size(107, 21)
            Me.comboBoxIncludeProductType.TabIndex = 6
            '
            'labelIncludeProductType
            '
            Me.labelIncludeProductType.AutoSize = True
            Me.labelIncludeProductType.Location = New System.Drawing.Point(7, 22)
            Me.labelIncludeProductType.Name = "labelIncludeProductType"
            Me.labelIncludeProductType.Size = New System.Drawing.Size(74, 13)
            Me.labelIncludeProductType.TabIndex = 5
            Me.labelIncludeProductType.Text = "Product Type:"
            '
            'textBoxSearchSpec
            '
            Me.textBoxSearchSpec.Location = New System.Drawing.Point(78, 6)
            Me.textBoxSearchSpec.Name = "textBoxSearchSpec"
            Me.textBoxSearchSpec.Size = New System.Drawing.Size(333, 20)
            Me.textBoxSearchSpec.TabIndex = 1
            '
            'labelSearchSpec
            '
            Me.labelSearchSpec.AutoSize = True
            Me.labelSearchSpec.Location = New System.Drawing.Point(4, 9)
            Me.labelSearchSpec.Name = "labelSearchSpec"
            Me.labelSearchSpec.Size = New System.Drawing.Size(72, 13)
            Me.labelSearchSpec.TabIndex = 0
            Me.labelSearchSpec.Text = "Search Spec:"
            '
            'dataGridViewDataView
            '
            Me.dataGridViewDataView.AllowUserToAddRows = False
            Me.dataGridViewDataView.AllowUserToDeleteRows = False
            Me.dataGridViewDataView.AllowUserToResizeRows = False
            Me.dataGridViewDataView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dataGridViewDataView.Dock = System.Windows.Forms.DockStyle.Top
            Me.dataGridViewDataView.Location = New System.Drawing.Point(0, 87)
            Me.dataGridViewDataView.Name = "dataGridViewDataView"
            Me.dataGridViewDataView.ReadOnly = True
            Me.dataGridViewDataView.RowHeadersVisible = False
            Me.dataGridViewDataView.Size = New System.Drawing.Size(870, 265)
            Me.dataGridViewDataView.TabIndex = 3
            '
            'statusStrip1
            '
            Me.statusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.toolStripStatusLabel1})
            Me.statusStrip1.Location = New System.Drawing.Point(0, 504)
            Me.statusStrip1.Name = "statusStrip1"
            Me.statusStrip1.Size = New System.Drawing.Size(870, 22)
            Me.statusStrip1.TabIndex = 5
            Me.statusStrip1.Text = "statusStrip1"
            '
            'toolStripStatusLabel1
            '
            Me.toolStripStatusLabel1.Name = "toolStripStatusLabel1"
            Me.toolStripStatusLabel1.Size = New System.Drawing.Size(109, 17)
            Me.toolStripStatusLabel1.Text = "toolStripStatusLabel1"
            '
            'splitContainerInfo
            '
            Me.splitContainerInfo.Dock = System.Windows.Forms.DockStyle.Fill
            Me.splitContainerInfo.Location = New System.Drawing.Point(0, 352)
            Me.splitContainerInfo.Name = "splitContainerInfo"
            '
            'splitContainerInfo.Panel1
            '
            Me.splitContainerInfo.Panel1.Controls.Add(Me.labelDocumentation)
            Me.splitContainerInfo.Panel1.Controls.Add(Me.richTextBoxDocumentation)
            '
            'splitContainerInfo.Panel2
            '
            Me.splitContainerInfo.Panel2.Controls.Add(Me.labelOverrides)
            Me.splitContainerInfo.Panel2.Controls.Add(Me.dataGridViewOverrides)
            Me.splitContainerInfo.Size = New System.Drawing.Size(870, 152)
            Me.splitContainerInfo.SplitterDistance = 393
            Me.splitContainerInfo.TabIndex = 6
            '
            'labelDocumentation
            '
            Me.labelDocumentation.AutoSize = True
            Me.labelDocumentation.Location = New System.Drawing.Point(0, 3)
            Me.labelDocumentation.Name = "labelDocumentation"
            Me.labelDocumentation.Size = New System.Drawing.Size(82, 13)
            Me.labelDocumentation.TabIndex = 1
            Me.labelDocumentation.Text = "Documentation:"
            '
            'richTextBoxDocumentation
            '
            Me.richTextBoxDocumentation.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.richTextBoxDocumentation.Location = New System.Drawing.Point(3, 18)
            Me.richTextBoxDocumentation.Name = "richTextBoxDocumentation"
            Me.richTextBoxDocumentation.Size = New System.Drawing.Size(387, 131)
            Me.richTextBoxDocumentation.TabIndex = 0
            Me.richTextBoxDocumentation.Tag = "0"
            Me.richTextBoxDocumentation.Text = ""
            '
            'labelOverrides
            '
            Me.labelOverrides.AutoSize = True
            Me.labelOverrides.Location = New System.Drawing.Point(3, 3)
            Me.labelOverrides.Name = "labelOverrides"
            Me.labelOverrides.Size = New System.Drawing.Size(55, 13)
            Me.labelOverrides.TabIndex = 2
            Me.labelOverrides.Text = "Overrides:"
            '
            'dataGridViewOverrides
            '
            Me.dataGridViewOverrides.AllowUserToAddRows = False
            Me.dataGridViewOverrides.AllowUserToDeleteRows = False
            Me.dataGridViewOverrides.AllowUserToOrderColumns = True
            Me.dataGridViewOverrides.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.dataGridViewOverrides.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dataGridViewOverrides.Location = New System.Drawing.Point(6, 18)
            Me.dataGridViewOverrides.Name = "dataGridViewOverrides"
            Me.dataGridViewOverrides.ReadOnly = True
            Me.dataGridViewOverrides.RowHeadersVisible = False
            Me.dataGridViewOverrides.Size = New System.Drawing.Size(464, 131)
            Me.dataGridViewOverrides.TabIndex = 0
            '
            'Form1
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(870, 526)
            Me.Controls.Add(Me.splitContainerInfo)
            Me.Controls.Add(Me.statusStrip1)
            Me.Controls.Add(Me.dataGridViewDataView)
            Me.Controls.Add(Me.panelFieldSearchTop)
            Me.MinimumSize = New System.Drawing.Size(878, 553)
            Me.Name = "Form1"
            Me.Text = "Simple Field Search Example"
            Me.panelFieldSearchTop.ResumeLayout(False)
            Me.panelFieldSearchTop.PerformLayout()
            Me.groupBoxExcludeOptions.ResumeLayout(False)
            Me.groupBoxExcludeOptions.PerformLayout()
            Me.groupBoxIncludOption.ResumeLayout(False)
            Me.groupBoxIncludOption.PerformLayout()
            CType(Me.dataGridViewDataView, System.ComponentModel.ISupportInitialize).EndInit()
            Me.statusStrip1.ResumeLayout(False)
            Me.statusStrip1.PerformLayout()
            Me.splitContainerInfo.Panel1.ResumeLayout(False)
            Me.splitContainerInfo.Panel1.PerformLayout()
            Me.splitContainerInfo.Panel2.ResumeLayout(False)
            Me.splitContainerInfo.Panel2.PerformLayout()
            Me.splitContainerInfo.ResumeLayout(False)
            CType(Me.dataGridViewOverrides, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents panelFieldSearchTop As System.Windows.Forms.Panel
        Private WithEvents labelSearchType As System.Windows.Forms.Label
        Private WithEvents comboBoxSearchType As System.Windows.Forms.ComboBox
        Private WithEvents buttonSubmitSearch As System.Windows.Forms.Button
        Private WithEvents groupBoxExcludeOptions As System.Windows.Forms.GroupBox
        Private WithEvents comboBoxExcludeFieldType As System.Windows.Forms.ComboBox
        Private WithEvents labelFieldType As System.Windows.Forms.Label
        Private WithEvents comboBoxExcludeProductType As System.Windows.Forms.ComboBox
        Private WithEvents labelExcludeProductType As System.Windows.Forms.Label
        Private WithEvents groupBoxIncludOption As System.Windows.Forms.GroupBox
        Private WithEvents comboBoxIncludeFieldType As System.Windows.Forms.ComboBox
        Private WithEvents labelIncludeFieldType As System.Windows.Forms.Label
        Private WithEvents comboBoxIncludeProductType As System.Windows.Forms.ComboBox
        Private WithEvents labelIncludeProductType As System.Windows.Forms.Label
        Private WithEvents textBoxSearchSpec As System.Windows.Forms.TextBox
        Private WithEvents labelSearchSpec As System.Windows.Forms.Label
        Private WithEvents dataGridViewDataView As System.Windows.Forms.DataGridView
        Private WithEvents statusStrip1 As System.Windows.Forms.StatusStrip
        Private WithEvents toolStripStatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
        Private WithEvents splitContainerInfo As System.Windows.Forms.SplitContainer
        Private WithEvents labelDocumentation As System.Windows.Forms.Label
        Private WithEvents richTextBoxDocumentation As System.Windows.Forms.RichTextBox
        Private WithEvents labelOverrides As System.Windows.Forms.Label
        Private WithEvents dataGridViewOverrides As System.Windows.Forms.DataGridView

End Class
End Namespace