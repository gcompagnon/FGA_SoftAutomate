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
            Me.buttonGetService = New System.Windows.Forms.Button
            Me.textBoxService = New System.Windows.Forms.TextBox
            Me.splitContainerMain = New System.Windows.Forms.SplitContainer
            Me.listBoxServices = New System.Windows.Forms.ListBox
            Me.label1 = New System.Windows.Forms.Label
            Me.splitContainerSchema = New System.Windows.Forms.SplitContainer
            Me.treeViewSchema = New System.Windows.Forms.TreeView
            Me.splitContainerProperties = New System.Windows.Forms.SplitContainer
            Me.listViewProperties = New System.Windows.Forms.ListView
            Me.columnHeaderProperties = New System.Windows.Forms.ColumnHeader
            Me.columnHeaderValues = New System.Windows.Forms.ColumnHeader
            Me.richTextBoxDescription = New System.Windows.Forms.RichTextBox
            Me.labelDescription = New System.Windows.Forms.Label
            Me.labelServices = New System.Windows.Forms.Label
            Me.splitContainerMain.Panel1.SuspendLayout()
            Me.splitContainerMain.Panel2.SuspendLayout()
            Me.splitContainerMain.SuspendLayout()
            Me.splitContainerSchema.Panel1.SuspendLayout()
            Me.splitContainerSchema.Panel2.SuspendLayout()
            Me.splitContainerSchema.SuspendLayout()
            Me.splitContainerProperties.Panel1.SuspendLayout()
            Me.splitContainerProperties.Panel2.SuspendLayout()
            Me.splitContainerProperties.SuspendLayout()
            Me.SuspendLayout()
            '
            'buttonGetService
            '
            Me.buttonGetService.Location = New System.Drawing.Point(287, 2)
            Me.buttonGetService.Name = "buttonGetService"
            Me.buttonGetService.Size = New System.Drawing.Size(75, 23)
            Me.buttonGetService.TabIndex = 10
            Me.buttonGetService.Text = "Get Service"
            Me.buttonGetService.UseVisualStyleBackColor = True
            '
            'textBoxService
            '
            Me.textBoxService.Location = New System.Drawing.Point(57, 4)
            Me.textBoxService.Name = "textBoxService"
            Me.textBoxService.Size = New System.Drawing.Size(224, 20)
            Me.textBoxService.TabIndex = 9
            '
            'splitContainerMain
            '
            Me.splitContainerMain.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.splitContainerMain.Location = New System.Drawing.Point(3, 31)
            Me.splitContainerMain.Name = "splitContainerMain"
            '
            'splitContainerMain.Panel1
            '
            Me.splitContainerMain.Panel1.Controls.Add(Me.listBoxServices)
            Me.splitContainerMain.Panel1.Controls.Add(Me.label1)
            '
            'splitContainerMain.Panel2
            '
            Me.splitContainerMain.Panel2.Controls.Add(Me.splitContainerSchema)
            Me.splitContainerMain.Size = New System.Drawing.Size(731, 400)
            Me.splitContainerMain.SplitterDistance = 182
            Me.splitContainerMain.TabIndex = 11
            '
            'listBoxServices
            '
            Me.listBoxServices.Dock = System.Windows.Forms.DockStyle.Fill
            Me.listBoxServices.FormattingEnabled = True
            Me.listBoxServices.Location = New System.Drawing.Point(0, 15)
            Me.listBoxServices.Name = "listBoxServices"
            Me.listBoxServices.Size = New System.Drawing.Size(182, 381)
            Me.listBoxServices.TabIndex = 4
            '
            'label1
            '
            Me.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.label1.Dock = System.Windows.Forms.DockStyle.Top
            Me.label1.FlatStyle = System.Windows.Forms.FlatStyle.Popup
            Me.label1.Location = New System.Drawing.Point(0, 0)
            Me.label1.Name = "label1"
            Me.label1.Size = New System.Drawing.Size(182, 15)
            Me.label1.TabIndex = 3
            Me.label1.Text = "Services"
            Me.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            '
            'splitContainerSchema
            '
            Me.splitContainerSchema.Dock = System.Windows.Forms.DockStyle.Fill
            Me.splitContainerSchema.Location = New System.Drawing.Point(0, 0)
            Me.splitContainerSchema.Name = "splitContainerSchema"
            '
            'splitContainerSchema.Panel1
            '
            Me.splitContainerSchema.Panel1.Controls.Add(Me.treeViewSchema)
            '
            'splitContainerSchema.Panel2
            '
            Me.splitContainerSchema.Panel2.Controls.Add(Me.splitContainerProperties)
            Me.splitContainerSchema.Size = New System.Drawing.Size(545, 400)
            Me.splitContainerSchema.SplitterDistance = 305
            Me.splitContainerSchema.TabIndex = 0
            '
            'treeViewSchema
            '
            Me.treeViewSchema.Dock = System.Windows.Forms.DockStyle.Fill
            Me.treeViewSchema.Location = New System.Drawing.Point(0, 0)
            Me.treeViewSchema.Name = "treeViewSchema"
            Me.treeViewSchema.Size = New System.Drawing.Size(305, 400)
            Me.treeViewSchema.TabIndex = 5
            '
            'splitContainerProperties
            '
            Me.splitContainerProperties.Dock = System.Windows.Forms.DockStyle.Fill
            Me.splitContainerProperties.Location = New System.Drawing.Point(0, 0)
            Me.splitContainerProperties.Name = "splitContainerProperties"
            Me.splitContainerProperties.Orientation = System.Windows.Forms.Orientation.Horizontal
            '
            'splitContainerProperties.Panel1
            '
            Me.splitContainerProperties.Panel1.Controls.Add(Me.listViewProperties)
            '
            'splitContainerProperties.Panel2
            '
            Me.splitContainerProperties.Panel2.Controls.Add(Me.richTextBoxDescription)
            Me.splitContainerProperties.Panel2.Controls.Add(Me.labelDescription)
            Me.splitContainerProperties.Size = New System.Drawing.Size(236, 400)
            Me.splitContainerProperties.SplitterDistance = 324
            Me.splitContainerProperties.TabIndex = 0
            '
            'listViewProperties
            '
            Me.listViewProperties.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.columnHeaderProperties, Me.columnHeaderValues})
            Me.listViewProperties.Dock = System.Windows.Forms.DockStyle.Fill
            Me.listViewProperties.Location = New System.Drawing.Point(0, 0)
            Me.listViewProperties.Name = "listViewProperties"
            Me.listViewProperties.Size = New System.Drawing.Size(236, 324)
            Me.listViewProperties.TabIndex = 6
            Me.listViewProperties.UseCompatibleStateImageBehavior = False
            Me.listViewProperties.View = System.Windows.Forms.View.Details
            '
            'columnHeaderProperties
            '
            Me.columnHeaderProperties.Text = "Properties"
            Me.columnHeaderProperties.Width = 116
            '
            'columnHeaderValues
            '
            Me.columnHeaderValues.Text = "Value"
            Me.columnHeaderValues.Width = 140
            '
            'richTextBoxDescription
            '
            Me.richTextBoxDescription.Dock = System.Windows.Forms.DockStyle.Fill
            Me.richTextBoxDescription.Location = New System.Drawing.Point(0, 13)
            Me.richTextBoxDescription.Name = "richTextBoxDescription"
            Me.richTextBoxDescription.Size = New System.Drawing.Size(236, 59)
            Me.richTextBoxDescription.TabIndex = 7
            Me.richTextBoxDescription.Text = ""
            '
            'labelDescription
            '
            Me.labelDescription.Dock = System.Windows.Forms.DockStyle.Top
            Me.labelDescription.Location = New System.Drawing.Point(0, 0)
            Me.labelDescription.Name = "labelDescription"
            Me.labelDescription.Size = New System.Drawing.Size(236, 13)
            Me.labelDescription.TabIndex = 0
            Me.labelDescription.Text = "Description"
            Me.labelDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'labelServices
            '
            Me.labelServices.AutoSize = True
            Me.labelServices.Location = New System.Drawing.Point(5, 7)
            Me.labelServices.Name = "labelServices"
            Me.labelServices.Size = New System.Drawing.Size(46, 13)
            Me.labelServices.TabIndex = 8
            Me.labelServices.Text = "Service:"
            '
            'Form1
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(738, 435)
            Me.Controls.Add(Me.buttonGetService)
            Me.Controls.Add(Me.textBoxService)
            Me.Controls.Add(Me.splitContainerMain)
            Me.Controls.Add(Me.labelServices)
            Me.MinimumSize = New System.Drawing.Size(400, 400)
            Me.Name = "Form1"
            Me.Text = "Simple Schema Example"
            Me.splitContainerMain.Panel1.ResumeLayout(False)
            Me.splitContainerMain.Panel2.ResumeLayout(False)
            Me.splitContainerMain.ResumeLayout(False)
            Me.splitContainerSchema.Panel1.ResumeLayout(False)
            Me.splitContainerSchema.Panel2.ResumeLayout(False)
            Me.splitContainerSchema.ResumeLayout(False)
            Me.splitContainerProperties.Panel1.ResumeLayout(False)
            Me.splitContainerProperties.Panel2.ResumeLayout(False)
            Me.splitContainerProperties.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents buttonGetService As System.Windows.Forms.Button
        Private WithEvents textBoxService As System.Windows.Forms.TextBox
        Private WithEvents splitContainerMain As System.Windows.Forms.SplitContainer
        Private WithEvents listBoxServices As System.Windows.Forms.ListBox
        Private WithEvents label1 As System.Windows.Forms.Label
        Private WithEvents splitContainerSchema As System.Windows.Forms.SplitContainer
        Private WithEvents treeViewSchema As System.Windows.Forms.TreeView
        Private WithEvents splitContainerProperties As System.Windows.Forms.SplitContainer
        Private WithEvents listViewProperties As System.Windows.Forms.ListView
        Private WithEvents columnHeaderProperties As System.Windows.Forms.ColumnHeader
        Private WithEvents columnHeaderValues As System.Windows.Forms.ColumnHeader
        Private WithEvents richTextBoxDescription As System.Windows.Forms.RichTextBox
        Private WithEvents labelDescription As System.Windows.Forms.Label
        Private WithEvents labelServices As System.Windows.Forms.Label

    End Class
End Namespace
