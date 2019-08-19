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
            Me.components = New System.ComponentModel.Container
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
            Me.statusStrip1 = New System.Windows.Forms.StatusStrip
            Me.toolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel
            Me.toolTip1 = New System.Windows.Forms.ToolTip(Me.components)
            Me.listViewRTIBData = New System.Windows.Forms.ListView
            Me.columnHeaderSecurity = New System.Windows.Forms.ColumnHeader
            Me.columnHeaderTime = New System.Windows.Forms.ColumnHeader
            Me.columnHeaderOpen = New System.Windows.Forms.ColumnHeader
            Me.columnHeaderHigh = New System.Windows.Forms.ColumnHeader
            Me.columnHeaderLow = New System.Windows.Forms.ColumnHeader
            Me.columnHeaderClose = New System.Windows.Forms.ColumnHeader
            Me.columnHeaderVolume = New System.Windows.Forms.ColumnHeader
            Me.columnHeaderNumberOfTicks = New System.Windows.Forms.ColumnHeader
            Me.panelRealtimeIB = New System.Windows.Forms.Panel
            Me.buttonClearAll = New System.Windows.Forms.Button
            Me.buttonClearData = New System.Windows.Forms.Button
            Me.buttonStopSubscribe = New System.Windows.Forms.Button
            Me.buttonSendRequest = New System.Windows.Forms.Button
            Me.buttonAddSecurity = New System.Windows.Forms.Button
            Me.labelNotes = New System.Windows.Forms.Label
            Me.textBoxOutputFile = New System.Windows.Forms.TextBox
            Me.checkBoxOutputFile = New System.Windows.Forms.CheckBox
            Me.labelMinutes = New System.Windows.Forms.Label
            Me.numericUpDownIntervalSize = New System.Windows.Forms.NumericUpDown
            Me.labelIntervalSize = New System.Windows.Forms.Label
            Me.dateTimePickerEndTime = New System.Windows.Forms.DateTimePicker
            Me.labelEndTime = New System.Windows.Forms.Label
            Me.dateTimePickerStartTime = New System.Windows.Forms.DateTimePicker
            Me.labelStartTime = New System.Windows.Forms.Label
            Me.textBoxSecurity = New System.Windows.Forms.TextBox
            Me.labelSecurity = New System.Windows.Forms.Label
            Me.panelTimeMessage = New System.Windows.Forms.Panel
            Me.labelTitle = New System.Windows.Forms.Label
            Me.labelTime = New System.Windows.Forms.Label
            Me.statusStrip1.SuspendLayout()
            Me.panelRealtimeIB.SuspendLayout()
            CType(Me.numericUpDownIntervalSize, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.panelTimeMessage.SuspendLayout()
            Me.SuspendLayout()
            '
            'statusStrip1
            '
            Me.statusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.toolStripStatusLabel1})
            Me.statusStrip1.Location = New System.Drawing.Point(0, 479)
            Me.statusStrip1.Name = "statusStrip1"
            Me.statusStrip1.Size = New System.Drawing.Size(788, 22)
            Me.statusStrip1.TabIndex = 33
            Me.statusStrip1.Text = "statusStrip1"
            '
            'toolStripStatusLabel1
            '
            Me.toolStripStatusLabel1.Name = "toolStripStatusLabel1"
            Me.toolStripStatusLabel1.Size = New System.Drawing.Size(0, 17)
            '
            'toolTip1
            '
            Me.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info
            Me.toolTip1.ToolTipTitle = "Subscription String"
            '
            'listViewRTIBData
            '
            Me.listViewRTIBData.AllowDrop = True
            Me.listViewRTIBData.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.columnHeaderSecurity, Me.columnHeaderTime, Me.columnHeaderOpen, Me.columnHeaderHigh, Me.columnHeaderLow, Me.columnHeaderClose, Me.columnHeaderVolume, Me.columnHeaderNumberOfTicks})
            Me.listViewRTIBData.Dock = System.Windows.Forms.DockStyle.Fill
            Me.listViewRTIBData.FullRowSelect = True
            Me.listViewRTIBData.Location = New System.Drawing.Point(0, 116)
            Me.listViewRTIBData.MultiSelect = False
            Me.listViewRTIBData.Name = "listViewRTIBData"
            Me.listViewRTIBData.Size = New System.Drawing.Size(788, 363)
            Me.listViewRTIBData.TabIndex = 37
            Me.listViewRTIBData.TabStop = False
            Me.listViewRTIBData.UseCompatibleStateImageBehavior = False
            Me.listViewRTIBData.View = System.Windows.Forms.View.Details
            '
            'columnHeaderSecurity
            '
            Me.columnHeaderSecurity.Tag = "Security"
            Me.columnHeaderSecurity.Text = "Security"
            Me.columnHeaderSecurity.Width = 192
            '
            'columnHeaderTime
            '
            Me.columnHeaderTime.Tag = "TIME"
            Me.columnHeaderTime.Text = "Time"
            Me.columnHeaderTime.Width = 114
            '
            'columnHeaderOpen
            '
            Me.columnHeaderOpen.Tag = "OPEN"
            Me.columnHeaderOpen.Text = "Open"
            Me.columnHeaderOpen.Width = 65
            '
            'columnHeaderHigh
            '
            Me.columnHeaderHigh.Tag = "HIGH"
            Me.columnHeaderHigh.Text = "High"
            Me.columnHeaderHigh.Width = 65
            '
            'columnHeaderLow
            '
            Me.columnHeaderLow.Tag = "LOW"
            Me.columnHeaderLow.Text = "Low"
            Me.columnHeaderLow.Width = 65
            '
            'columnHeaderClose
            '
            Me.columnHeaderClose.Tag = "CLOSE"
            Me.columnHeaderClose.Text = "Close"
            Me.columnHeaderClose.Width = 65
            '
            'columnHeaderVolume
            '
            Me.columnHeaderVolume.Tag = "VOLUME"
            Me.columnHeaderVolume.Text = "Volume"
            Me.columnHeaderVolume.Width = 65
            '
            'columnHeaderNumberOfTicks
            '
            Me.columnHeaderNumberOfTicks.Tag = "NUMBER_OF_TICKS"
            Me.columnHeaderNumberOfTicks.Text = "Number Of Ticks"
            Me.columnHeaderNumberOfTicks.Width = 96
            '
            'panelRealtimeIB
            '
            Me.panelRealtimeIB.Controls.Add(Me.buttonClearAll)
            Me.panelRealtimeIB.Controls.Add(Me.buttonClearData)
            Me.panelRealtimeIB.Controls.Add(Me.buttonStopSubscribe)
            Me.panelRealtimeIB.Controls.Add(Me.buttonSendRequest)
            Me.panelRealtimeIB.Controls.Add(Me.buttonAddSecurity)
            Me.panelRealtimeIB.Controls.Add(Me.labelNotes)
            Me.panelRealtimeIB.Controls.Add(Me.textBoxOutputFile)
            Me.panelRealtimeIB.Controls.Add(Me.checkBoxOutputFile)
            Me.panelRealtimeIB.Controls.Add(Me.labelMinutes)
            Me.panelRealtimeIB.Controls.Add(Me.numericUpDownIntervalSize)
            Me.panelRealtimeIB.Controls.Add(Me.labelIntervalSize)
            Me.panelRealtimeIB.Controls.Add(Me.dateTimePickerEndTime)
            Me.panelRealtimeIB.Controls.Add(Me.labelEndTime)
            Me.panelRealtimeIB.Controls.Add(Me.dateTimePickerStartTime)
            Me.panelRealtimeIB.Controls.Add(Me.labelStartTime)
            Me.panelRealtimeIB.Controls.Add(Me.textBoxSecurity)
            Me.panelRealtimeIB.Controls.Add(Me.labelSecurity)
            Me.panelRealtimeIB.Dock = System.Windows.Forms.DockStyle.Top
            Me.panelRealtimeIB.Location = New System.Drawing.Point(0, 0)
            Me.panelRealtimeIB.Name = "panelRealtimeIB"
            Me.panelRealtimeIB.Size = New System.Drawing.Size(788, 116)
            Me.panelRealtimeIB.TabIndex = 36
            '
            'buttonClearAll
            '
            Me.buttonClearAll.Enabled = False
            Me.buttonClearAll.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.buttonClearAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.buttonClearAll.ImageIndex = 3
            Me.buttonClearAll.Location = New System.Drawing.Point(674, 3)
            Me.buttonClearAll.Name = "buttonClearAll"
            Me.buttonClearAll.Size = New System.Drawing.Size(81, 23)
            Me.buttonClearAll.TabIndex = 56
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
            Me.buttonClearData.Location = New System.Drawing.Point(587, 3)
            Me.buttonClearData.Name = "buttonClearData"
            Me.buttonClearData.Size = New System.Drawing.Size(81, 23)
            Me.buttonClearData.TabIndex = 55
            Me.buttonClearData.Tag = "RD"
            Me.buttonClearData.Text = "Clear Data"
            Me.buttonClearData.UseVisualStyleBackColor = True
            '
            'buttonStopSubscribe
            '
            Me.buttonStopSubscribe.Enabled = False
            Me.buttonStopSubscribe.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.buttonStopSubscribe.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.buttonStopSubscribe.Location = New System.Drawing.Point(498, 3)
            Me.buttonStopSubscribe.Name = "buttonStopSubscribe"
            Me.buttonStopSubscribe.Size = New System.Drawing.Size(81, 23)
            Me.buttonStopSubscribe.TabIndex = 54
            Me.buttonStopSubscribe.Tag = "RD"
            Me.buttonStopSubscribe.Text = "Stop"
            Me.buttonStopSubscribe.UseVisualStyleBackColor = True
            '
            'buttonSendRequest
            '
            Me.buttonSendRequest.Enabled = False
            Me.buttonSendRequest.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.buttonSendRequest.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.buttonSendRequest.Location = New System.Drawing.Point(411, 3)
            Me.buttonSendRequest.Name = "buttonSendRequest"
            Me.buttonSendRequest.Size = New System.Drawing.Size(81, 23)
            Me.buttonSendRequest.TabIndex = 53
            Me.buttonSendRequest.Tag = "RD"
            Me.buttonSendRequest.Text = "Subscribe"
            Me.buttonSendRequest.UseVisualStyleBackColor = True
            '
            'buttonAddSecurity
            '
            Me.buttonAddSecurity.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.buttonAddSecurity.ImageKey = "Symbol-Add.ico"
            Me.buttonAddSecurity.Location = New System.Drawing.Point(324, 3)
            Me.buttonAddSecurity.Name = "buttonAddSecurity"
            Me.buttonAddSecurity.Size = New System.Drawing.Size(81, 23)
            Me.buttonAddSecurity.TabIndex = 52
            Me.buttonAddSecurity.Tag = "RT"
            Me.buttonAddSecurity.Text = "Add"
            Me.buttonAddSecurity.UseVisualStyleBackColor = True
            '
            'labelNotes
            '
            Me.labelNotes.AccessibleRole = System.Windows.Forms.AccessibleRole.None
            Me.labelNotes.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.labelNotes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.labelNotes.Location = New System.Drawing.Point(9, 81)
            Me.labelNotes.Name = "labelNotes"
            Me.labelNotes.Size = New System.Drawing.Size(776, 29)
            Me.labelNotes.TabIndex = 51
            Me.labelNotes.Text = resources.GetString("labelNotes.Text")
            Me.labelNotes.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'textBoxOutputFile
            '
            Me.textBoxOutputFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.textBoxOutputFile.Location = New System.Drawing.Point(163, 55)
            Me.textBoxOutputFile.Name = "textBoxOutputFile"
            Me.textBoxOutputFile.ReadOnly = True
            Me.textBoxOutputFile.Size = New System.Drawing.Size(612, 20)
            Me.textBoxOutputFile.TabIndex = 12
            '
            'checkBoxOutputFile
            '
            Me.checkBoxOutputFile.AutoSize = True
            Me.checkBoxOutputFile.Location = New System.Drawing.Point(68, 57)
            Me.checkBoxOutputFile.Name = "checkBoxOutputFile"
            Me.checkBoxOutputFile.Size = New System.Drawing.Size(89, 17)
            Me.checkBoxOutputFile.TabIndex = 11
            Me.checkBoxOutputFile.Tag = "RT"
            Me.checkBoxOutputFile.Text = "Output to File"
            Me.checkBoxOutputFile.UseVisualStyleBackColor = True
            '
            'labelMinutes
            '
            Me.labelMinutes.AutoSize = True
            Me.labelMinutes.Location = New System.Drawing.Point(449, 35)
            Me.labelMinutes.Name = "labelMinutes"
            Me.labelMinutes.Size = New System.Drawing.Size(43, 13)
            Me.labelMinutes.TabIndex = 9
            Me.labelMinutes.Text = "minutes"
            '
            'numericUpDownIntervalSize
            '
            Me.numericUpDownIntervalSize.Location = New System.Drawing.Point(399, 31)
            Me.numericUpDownIntervalSize.Maximum = New Decimal(New Integer() {1440, 0, 0, 0})
            Me.numericUpDownIntervalSize.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
            Me.numericUpDownIntervalSize.Name = "numericUpDownIntervalSize"
            Me.numericUpDownIntervalSize.Size = New System.Drawing.Size(50, 20)
            Me.numericUpDownIntervalSize.TabIndex = 8
            Me.numericUpDownIntervalSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
            Me.numericUpDownIntervalSize.Value = New Decimal(New Integer() {1, 0, 0, 0})
            '
            'labelIntervalSize
            '
            Me.labelIntervalSize.AutoSize = True
            Me.labelIntervalSize.Location = New System.Drawing.Point(280, 35)
            Me.labelIntervalSize.Name = "labelIntervalSize"
            Me.labelIntervalSize.Size = New System.Drawing.Size(113, 13)
            Me.labelIntervalSize.TabIndex = 7
            Me.labelIntervalSize.Text = "Time Bar Interval Size:"
            '
            'dateTimePickerEndTime
            '
            Me.dateTimePickerEndTime.CustomFormat = "HH:mm"
            Me.dateTimePickerEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom
            Me.dateTimePickerEndTime.Location = New System.Drawing.Point(202, 31)
            Me.dateTimePickerEndTime.MinDate = New Date(1900, 1, 1, 0, 0, 0, 0)
            Me.dateTimePickerEndTime.Name = "dateTimePickerEndTime"
            Me.dateTimePickerEndTime.ShowCheckBox = True
            Me.dateTimePickerEndTime.ShowUpDown = True
            Me.dateTimePickerEndTime.Size = New System.Drawing.Size(71, 20)
            Me.dateTimePickerEndTime.TabIndex = 6
            Me.dateTimePickerEndTime.Tag = "IBAR"
            '
            'labelEndTime
            '
            Me.labelEndTime.AutoSize = True
            Me.labelEndTime.Location = New System.Drawing.Point(145, 35)
            Me.labelEndTime.Name = "labelEndTime"
            Me.labelEndTime.Size = New System.Drawing.Size(55, 13)
            Me.labelEndTime.TabIndex = 5
            Me.labelEndTime.Text = "End Time:"
            '
            'dateTimePickerStartTime
            '
            Me.dateTimePickerStartTime.CustomFormat = "HH:mm"
            Me.dateTimePickerStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom
            Me.dateTimePickerStartTime.Location = New System.Drawing.Point(68, 31)
            Me.dateTimePickerStartTime.MinDate = New Date(1900, 1, 1, 0, 0, 0, 0)
            Me.dateTimePickerStartTime.Name = "dateTimePickerStartTime"
            Me.dateTimePickerStartTime.ShowCheckBox = True
            Me.dateTimePickerStartTime.ShowUpDown = True
            Me.dateTimePickerStartTime.Size = New System.Drawing.Size(71, 20)
            Me.dateTimePickerStartTime.TabIndex = 4
            Me.dateTimePickerStartTime.Tag = "IBAR"
            '
            'labelStartTime
            '
            Me.labelStartTime.AutoSize = True
            Me.labelStartTime.Location = New System.Drawing.Point(8, 35)
            Me.labelStartTime.Name = "labelStartTime"
            Me.labelStartTime.Size = New System.Drawing.Size(58, 13)
            Me.labelStartTime.TabIndex = 3
            Me.labelStartTime.Text = "Start Time:"
            '
            'textBoxSecurity
            '
            Me.textBoxSecurity.Location = New System.Drawing.Point(68, 5)
            Me.textBoxSecurity.Name = "textBoxSecurity"
            Me.textBoxSecurity.Size = New System.Drawing.Size(246, 20)
            Me.textBoxSecurity.TabIndex = 1
            Me.textBoxSecurity.Tag = ""
            '
            'labelSecurity
            '
            Me.labelSecurity.AutoSize = True
            Me.labelSecurity.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.labelSecurity.Location = New System.Drawing.Point(18, 8)
            Me.labelSecurity.Name = "labelSecurity"
            Me.labelSecurity.Size = New System.Drawing.Size(48, 13)
            Me.labelSecurity.TabIndex = 0
            Me.labelSecurity.Text = "Security:"
            '
            'panelTimeMessage
            '
            Me.panelTimeMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.panelTimeMessage.Controls.Add(Me.labelTitle)
            Me.panelTimeMessage.Controls.Add(Me.labelTime)
            Me.panelTimeMessage.Location = New System.Drawing.Point(299, 163)
            Me.panelTimeMessage.Name = "panelTimeMessage"
            Me.panelTimeMessage.Size = New System.Drawing.Size(193, 85)
            Me.panelTimeMessage.TabIndex = 38
            '
            'labelTitle
            '
            Me.labelTitle.AutoSize = True
            Me.labelTitle.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.labelTitle.Location = New System.Drawing.Point(15, 4)
            Me.labelTitle.Name = "labelTitle"
            Me.labelTitle.Size = New System.Drawing.Size(58, 13)
            Me.labelTitle.TabIndex = 2
            Me.labelTitle.Text = "Warning:"
            '
            'labelTime
            '
            Me.labelTime.BackColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.labelTime.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.labelTime.Location = New System.Drawing.Point(15, 24)
            Me.labelTime.Name = "labelTime"
            Me.labelTime.Size = New System.Drawing.Size(162, 45)
            Me.labelTime.TabIndex = 1
            Me.labelTime.Text = "End time is earlier than start time. Please adjust start or end time before proce" & _
                "eding."
            '
            'Form1
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(788, 501)
            Me.Controls.Add(Me.panelTimeMessage)
            Me.Controls.Add(Me.listViewRTIBData)
            Me.Controls.Add(Me.panelRealtimeIB)
            Me.Controls.Add(Me.statusStrip1)
            Me.Name = "Form1"
            Me.Text = "Simple Intraday Bar Subscription Example"
            Me.statusStrip1.ResumeLayout(False)
            Me.statusStrip1.PerformLayout()
            Me.panelRealtimeIB.ResumeLayout(False)
            Me.panelRealtimeIB.PerformLayout()
            CType(Me.numericUpDownIntervalSize, System.ComponentModel.ISupportInitialize).EndInit()
            Me.panelTimeMessage.ResumeLayout(False)
            Me.panelTimeMessage.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
		Private WithEvents statusStrip1 As System.Windows.Forms.StatusStrip
		Private WithEvents toolStripStatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
        Private WithEvents toolTip1 As System.Windows.Forms.ToolTip
		Private WithEvents listViewRTIBData As System.Windows.Forms.ListView
		Private WithEvents columnHeaderSecurity As System.Windows.Forms.ColumnHeader
		Private WithEvents columnHeaderTime As System.Windows.Forms.ColumnHeader
		Private WithEvents columnHeaderOpen As System.Windows.Forms.ColumnHeader
		Private WithEvents columnHeaderHigh As System.Windows.Forms.ColumnHeader
		Private WithEvents columnHeaderLow As System.Windows.Forms.ColumnHeader
		Private WithEvents columnHeaderClose As System.Windows.Forms.ColumnHeader
		Private WithEvents columnHeaderVolume As System.Windows.Forms.ColumnHeader
		Private WithEvents columnHeaderNumberOfTicks As System.Windows.Forms.ColumnHeader
		Private WithEvents panelRealtimeIB As System.Windows.Forms.Panel
		Private WithEvents buttonClearAll As System.Windows.Forms.Button
		Private WithEvents buttonClearData As System.Windows.Forms.Button
		Private WithEvents buttonStopSubscribe As System.Windows.Forms.Button
		Private WithEvents buttonSendRequest As System.Windows.Forms.Button
		Private WithEvents buttonAddSecurity As System.Windows.Forms.Button
		Private WithEvents labelNotes As System.Windows.Forms.Label
		Private WithEvents textBoxOutputFile As System.Windows.Forms.TextBox
		Private WithEvents checkBoxOutputFile As System.Windows.Forms.CheckBox
		Private WithEvents labelMinutes As System.Windows.Forms.Label
		Private WithEvents numericUpDownIntervalSize As System.Windows.Forms.NumericUpDown
		Private WithEvents labelIntervalSize As System.Windows.Forms.Label
		Private WithEvents dateTimePickerEndTime As System.Windows.Forms.DateTimePicker
		Private WithEvents labelEndTime As System.Windows.Forms.Label
		Private WithEvents dateTimePickerStartTime As System.Windows.Forms.DateTimePicker
		Private WithEvents labelStartTime As System.Windows.Forms.Label
		Private WithEvents textBoxSecurity As System.Windows.Forms.TextBox
        Private WithEvents labelSecurity As System.Windows.Forms.Label
        Private WithEvents panelTimeMessage As System.Windows.Forms.Panel
        Private WithEvents labelTitle As System.Windows.Forms.Label
        Private WithEvents labelTime As System.Windows.Forms.Label

	End Class
End Namespace
