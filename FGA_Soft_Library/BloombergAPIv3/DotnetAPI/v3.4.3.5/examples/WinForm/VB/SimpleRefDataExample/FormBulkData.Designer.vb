Namespace Bloomberglp.Blpapi.Examples
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class FormBulkData
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormBulkData))
            Me.dataGridViewRDBulkData = New System.Windows.Forms.DataGridView
            Me.toolStripBDTool = New System.Windows.Forms.ToolStrip
            Me.toolStripButtonSave = New System.Windows.Forms.ToolStripButton
            Me.toolStripButtonBDClose = New System.Windows.Forms.ToolStripButton
            CType(Me.dataGridViewRDBulkData, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.toolStripBDTool.SuspendLayout()
            Me.SuspendLayout()
            '
            'dataGridViewRDBulkData
            '
            Me.dataGridViewRDBulkData.AllowUserToAddRows = False
            Me.dataGridViewRDBulkData.AllowUserToDeleteRows = False
            Me.dataGridViewRDBulkData.AllowUserToResizeRows = False
            Me.dataGridViewRDBulkData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dataGridViewRDBulkData.Dock = System.Windows.Forms.DockStyle.Fill
            Me.dataGridViewRDBulkData.Location = New System.Drawing.Point(0, 25)
            Me.dataGridViewRDBulkData.MultiSelect = False
            Me.dataGridViewRDBulkData.Name = "dataGridViewRDBulkData"
            Me.dataGridViewRDBulkData.ReadOnly = True
            Me.dataGridViewRDBulkData.RowHeadersVisible = False
            Me.dataGridViewRDBulkData.Size = New System.Drawing.Size(674, 471)
            Me.dataGridViewRDBulkData.TabIndex = 7
            '
            'toolStripBDTool
            '
            Me.toolStripBDTool.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.toolStripButtonSave, Me.toolStripButtonBDClose})
            Me.toolStripBDTool.Location = New System.Drawing.Point(0, 0)
            Me.toolStripBDTool.Name = "toolStripBDTool"
            Me.toolStripBDTool.Size = New System.Drawing.Size(674, 25)
            Me.toolStripBDTool.TabIndex = 6
            Me.toolStripBDTool.Text = "toolStrip1"
            '
            'toolStripButtonSave
            '
            Me.toolStripButtonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.toolStripButtonSave.Image = CType(resources.GetObject("toolStripButtonSave.Image"), System.Drawing.Image)
            Me.toolStripButtonSave.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.toolStripButtonSave.Name = "toolStripButtonSave"
            Me.toolStripButtonSave.Size = New System.Drawing.Size(23, 22)
            Me.toolStripButtonSave.Text = "Save data to text file"
            Me.toolStripButtonSave.ToolTipText = "Save data to text file"
            '
            'toolStripButtonBDClose
            '
            Me.toolStripButtonBDClose.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
            Me.toolStripButtonBDClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.toolStripButtonBDClose.Image = CType(resources.GetObject("toolStripButtonBDClose.Image"), System.Drawing.Image)
            Me.toolStripButtonBDClose.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.toolStripButtonBDClose.Name = "toolStripButtonBDClose"
            Me.toolStripButtonBDClose.Size = New System.Drawing.Size(23, 22)
            Me.toolStripButtonBDClose.Text = "Close"
            '
            'FormBulkData
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(674, 496)
            Me.ControlBox = False
            Me.Controls.Add(Me.dataGridViewRDBulkData)
            Me.Controls.Add(Me.toolStripBDTool)
            Me.MinimumSize = New System.Drawing.Size(300, 300)
            Me.Name = "FormBulkData"
            Me.Text = "FormBulkData"
            CType(Me.dataGridViewRDBulkData, System.ComponentModel.ISupportInitialize).EndInit()
            Me.toolStripBDTool.ResumeLayout(False)
            Me.toolStripBDTool.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents dataGridViewRDBulkData As System.Windows.Forms.DataGridView
        Private WithEvents toolStripBDTool As System.Windows.Forms.ToolStrip
        Private WithEvents toolStripButtonSave As System.Windows.Forms.ToolStripButton
        Private WithEvents toolStripButtonBDClose As System.Windows.Forms.ToolStripButton
    End Class
End Namespace