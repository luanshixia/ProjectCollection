<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Wizard
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Wizard))
        Me.DateTimePicker1 = New System.Windows.Forms.DateTimePicker
        Me.nudDaySize = New System.Windows.Forms.NumericUpDown
        Me.btnFinish = New System.Windows.Forms.Button
        Me.cmbStyle = New System.Windows.Forms.ComboBox
        Me.TabControl1 = New System.Windows.Forms.TabControl
        Me.TabPage1 = New System.Windows.Forms.TabPage
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.TabPage2 = New System.Windows.Forms.TabPage
        Me.Label8 = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.TabPage3 = New System.Windows.Forms.TabPage
        Me.TextBox1 = New System.Windows.Forms.TextBox
        Me.Label6 = New System.Windows.Forms.Label
        Me.TabPage4 = New System.Windows.Forms.TabPage
        Me.TextBox2 = New System.Windows.Forms.TextBox
        Me.Label9 = New System.Windows.Forms.Label
        Me.cmbLevel = New System.Windows.Forms.ComboBox
        Me.Label7 = New System.Windows.Forms.Label
        Me.TabPage5 = New System.Windows.Forms.TabPage
        Me.TextBox3 = New System.Windows.Forms.TextBox
        Me.Label10 = New System.Windows.Forms.Label
        Me.nudGroupSize = New System.Windows.Forms.NumericUpDown
        Me.TabPage6 = New System.Windows.Forms.TabPage
        Me.TextBox4 = New System.Windows.Forms.TextBox
        Me.Label11 = New System.Windows.Forms.Label
        Me.btnCancel = New System.Windows.Forms.Button
        Me.btnPrev = New System.Windows.Forms.Button
        Me.btnNext = New System.Windows.Forms.Button
        Me.PictureBox1 = New System.Windows.Forms.PictureBox
        CType(Me.nudDaySize, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        Me.TabPage4.SuspendLayout()
        Me.TabPage5.SuspendLayout()
        CType(Me.nudGroupSize, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPage6.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'DateTimePicker1
        '
        Me.DateTimePicker1.Location = New System.Drawing.Point(31, 165)
        Me.DateTimePicker1.Name = "DateTimePicker1"
        Me.DateTimePicker1.Size = New System.Drawing.Size(200, 21)
        Me.DateTimePicker1.TabIndex = 0
        '
        'nudDaySize
        '
        Me.nudDaySize.Increment = New Decimal(New Integer() {10, 0, 0, 0})
        Me.nudDaySize.Location = New System.Drawing.Point(27, 198)
        Me.nudDaySize.Maximum = New Decimal(New Integer() {500, 0, 0, 0})
        Me.nudDaySize.Minimum = New Decimal(New Integer() {100, 0, 0, 0})
        Me.nudDaySize.Name = "nudDaySize"
        Me.nudDaySize.Size = New System.Drawing.Size(130, 21)
        Me.nudDaySize.TabIndex = 1
        Me.nudDaySize.Value = New Decimal(New Integer() {200, 0, 0, 0})
        '
        'btnFinish
        '
        Me.btnFinish.Location = New System.Drawing.Point(475, 304)
        Me.btnFinish.Name = "btnFinish"
        Me.btnFinish.Size = New System.Drawing.Size(92, 26)
        Me.btnFinish.TabIndex = 2
        Me.btnFinish.Text = "完成"
        Me.btnFinish.UseVisualStyleBackColor = True
        '
        'cmbStyle
        '
        Me.cmbStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbStyle.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmbStyle.FormattingEnabled = True
        Me.cmbStyle.Items.AddRange(New Object() {"GRE", "TOEFL iBT", "IELTS", "GMAT", "LSAT", "SAT", "考研", "四六级"})
        Me.cmbStyle.Location = New System.Drawing.Point(21, 137)
        Me.cmbStyle.Name = "cmbStyle"
        Me.cmbStyle.Size = New System.Drawing.Size(214, 23)
        Me.cmbStyle.TabIndex = 6
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Controls.Add(Me.TabPage3)
        Me.TabControl1.Controls.Add(Me.TabPage4)
        Me.TabControl1.Controls.Add(Me.TabPage5)
        Me.TabControl1.Controls.Add(Me.TabPage6)
        Me.TabControl1.Location = New System.Drawing.Point(178, -19)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(411, 303)
        Me.TabControl1.TabIndex = 7
        '
        'TabPage1
        '
        Me.TabPage1.BackColor = System.Drawing.Color.White
        Me.TabPage1.Controls.Add(Me.Label4)
        Me.TabPage1.Controls.Add(Me.Label3)
        Me.TabPage1.Controls.Add(Me.Label2)
        Me.TabPage1.Controls.Add(Me.Label1)
        Me.TabPage1.Location = New System.Drawing.Point(4, 21)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(403, 278)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "TabPage1"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(15, 144)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(125, 12)
        Me.Label4.TabIndex = 3
        Me.Label4.Text = "单击『下一步』继续。"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(15, 120)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(353, 12)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "完成本向导，您将更有针对性和有效地征服以往难以背诵的单词。"
        '
        'Label2
        '
        Me.Label2.AllowDrop = True
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(15, 96)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(257, 12)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "本向导将为您量身定制个性化的单词记忆体验。"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("微软雅黑", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.Label1.Location = New System.Drawing.Point(13, 31)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(295, 25)
        Me.Label1.TabIndex = 100
        Me.Label1.Text = "欢迎进入iWord单词记忆设置向导"
        '
        'TabPage2
        '
        Me.TabPage2.BackColor = System.Drawing.Color.White
        Me.TabPage2.Controls.Add(Me.Label8)
        Me.TabPage2.Controls.Add(Me.Label5)
        Me.TabPage2.Controls.Add(Me.cmbStyle)
        Me.TabPage2.Location = New System.Drawing.Point(4, 21)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(403, 278)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "TabPage2"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(18, 88)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(281, 12)
        Me.Label8.TabIndex = 102
        Me.Label8.Text = "选择您参加的考试类型，然后单击『下一步』继续。"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("微软雅黑", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.Label5.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.Label5.Location = New System.Drawing.Point(13, 31)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(221, 25)
        Me.Label5.TabIndex = 101
        Me.Label5.Text = "您参加的是哪一种考试？"
        '
        'TabPage3
        '
        Me.TabPage3.BackColor = System.Drawing.Color.White
        Me.TabPage3.Controls.Add(Me.TextBox1)
        Me.TabPage3.Controls.Add(Me.Label6)
        Me.TabPage3.Controls.Add(Me.DateTimePicker1)
        Me.TabPage3.Location = New System.Drawing.Point(4, 21)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage3.Size = New System.Drawing.Size(403, 278)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "TabPage3"
        '
        'TextBox1
        '
        Me.TextBox1.BackColor = System.Drawing.SystemColors.Window
        Me.TextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox1.Location = New System.Drawing.Point(31, 90)
        Me.TextBox1.Multiline = True
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.ReadOnly = True
        Me.TextBox1.Size = New System.Drawing.Size(322, 37)
        Me.TextBox1.TabIndex = 103
        Me.TextBox1.TabStop = False
        Me.TextBox1.Text = "选择您的考试时间，以便我们为您制定合适的时间表，" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "并且帮助您打理您的复习计划。"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("微软雅黑", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.Label6.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.Label6.Location = New System.Drawing.Point(26, 28)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(164, 25)
        Me.Label6.TabIndex = 102
        Me.Label6.Text = "您什么时候考试？"
        '
        'TabPage4
        '
        Me.TabPage4.BackColor = System.Drawing.Color.White
        Me.TabPage4.Controls.Add(Me.TextBox2)
        Me.TabPage4.Controls.Add(Me.Label9)
        Me.TabPage4.Controls.Add(Me.cmbLevel)
        Me.TabPage4.Controls.Add(Me.Label7)
        Me.TabPage4.Controls.Add(Me.nudDaySize)
        Me.TabPage4.Location = New System.Drawing.Point(4, 21)
        Me.TabPage4.Name = "TabPage4"
        Me.TabPage4.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage4.Size = New System.Drawing.Size(403, 278)
        Me.TabPage4.TabIndex = 3
        Me.TabPage4.Text = "TabPage4"
        '
        'TextBox2
        '
        Me.TextBox2.BackColor = System.Drawing.SystemColors.Window
        Me.TextBox2.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox2.Location = New System.Drawing.Point(27, 154)
        Me.TextBox2.Multiline = True
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.ReadOnly = True
        Me.TextBox2.Size = New System.Drawing.Size(322, 37)
        Me.TextBox2.TabIndex = 105
        Me.TextBox2.TabStop = False
        Me.TextBox2.Text = "针对您的情况，我们建议您每天背诵如下数量的单词。您也可以在此基础上进行自定义。"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(24, 79)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(185, 12)
        Me.Label9.TabIndex = 104
        Me.Label9.Text = "根据您的英语基础情况作出选择。"
        '
        'cmbLevel
        '
        Me.cmbLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbLevel.FormattingEnabled = True
        Me.cmbLevel.Items.AddRange(New Object() {"基础薄弱", "中等级别", "英语高手"})
        Me.cmbLevel.Location = New System.Drawing.Point(27, 115)
        Me.cmbLevel.Name = "cmbLevel"
        Me.cmbLevel.Size = New System.Drawing.Size(130, 20)
        Me.cmbLevel.TabIndex = 103
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("微软雅黑", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.Label7.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.Label7.Location = New System.Drawing.Point(22, 28)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(202, 25)
        Me.Label7.TabIndex = 102
        Me.Label7.Text = "您每天打算进行多少？"
        '
        'TabPage5
        '
        Me.TabPage5.BackColor = System.Drawing.Color.White
        Me.TabPage5.Controls.Add(Me.TextBox3)
        Me.TabPage5.Controls.Add(Me.Label10)
        Me.TabPage5.Controls.Add(Me.nudGroupSize)
        Me.TabPage5.Location = New System.Drawing.Point(4, 21)
        Me.TabPage5.Name = "TabPage5"
        Me.TabPage5.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage5.Size = New System.Drawing.Size(403, 278)
        Me.TabPage5.TabIndex = 4
        Me.TabPage5.Text = "TabPage5"
        '
        'TextBox3
        '
        Me.TextBox3.BackColor = System.Drawing.SystemColors.Window
        Me.TextBox3.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox3.Location = New System.Drawing.Point(29, 98)
        Me.TextBox3.Multiline = True
        Me.TextBox3.Name = "TextBox3"
        Me.TextBox3.ReadOnly = True
        Me.TextBox3.Size = New System.Drawing.Size(322, 47)
        Me.TextBox3.TabIndex = 108
        Me.TextBox3.TabStop = False
        Me.TextBox3.Text = "为了有效背诵，根据记忆原理，以约5分钟为一个单元进行短时重复效果最为理想。根据您的情况，我们建议您每个记忆单元（每组）的容量如下。您也可在此基础上进行自定义。"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("微软雅黑", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.Label10.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.Label10.Location = New System.Drawing.Point(24, 31)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(194, 25)
        Me.Label10.TabIndex = 107
        Me.Label10.Text = "您5分钟能记多少词？"
        '
        'nudGroupSize
        '
        Me.nudGroupSize.Increment = New Decimal(New Integer() {10, 0, 0, 0})
        Me.nudGroupSize.Location = New System.Drawing.Point(29, 167)
        Me.nudGroupSize.Minimum = New Decimal(New Integer() {10, 0, 0, 0})
        Me.nudGroupSize.Name = "nudGroupSize"
        Me.nudGroupSize.Size = New System.Drawing.Size(130, 21)
        Me.nudGroupSize.TabIndex = 106
        Me.nudGroupSize.Value = New Decimal(New Integer() {10, 0, 0, 0})
        '
        'TabPage6
        '
        Me.TabPage6.BackColor = System.Drawing.Color.White
        Me.TabPage6.Controls.Add(Me.TextBox4)
        Me.TabPage6.Controls.Add(Me.Label11)
        Me.TabPage6.Location = New System.Drawing.Point(4, 21)
        Me.TabPage6.Name = "TabPage6"
        Me.TabPage6.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage6.Size = New System.Drawing.Size(403, 278)
        Me.TabPage6.TabIndex = 5
        Me.TabPage6.Text = "TabPage6"
        '
        'TextBox4
        '
        Me.TextBox4.BackColor = System.Drawing.SystemColors.Window
        Me.TextBox4.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox4.Location = New System.Drawing.Point(25, 102)
        Me.TextBox4.Multiline = True
        Me.TextBox4.Name = "TextBox4"
        Me.TextBox4.ReadOnly = True
        Me.TextBox4.Size = New System.Drawing.Size(322, 95)
        Me.TextBox4.TabIndex = 110
        Me.TextBox4.TabStop = False
        Me.TextBox4.Text = "您已完成本向导。在您准备开始学习之前，请牢记本软件设计宗旨：出国英语考试单词认知记忆自我评估管理。在出国考试中，大量单词的要求是认知，这一过程务必相对快速地多次重" & _
            "复，而每次持续天数不宜太久。坚持到最后就是胜利者。祝您成功。" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "单击『完成』结束本向导。"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("微软雅黑", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.Label11.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.Label11.Location = New System.Drawing.Point(20, 36)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(126, 25)
        Me.Label11.TabIndex = 109
        Me.Label11.Text = "准备开始旅程"
        '
        'btnCancel
        '
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(138, 304)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(87, 26)
        Me.btnCancel.TabIndex = 3
        Me.btnCancel.Text = "取消"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnPrev
        '
        Me.btnPrev.Enabled = False
        Me.btnPrev.Location = New System.Drawing.Point(245, 304)
        Me.btnPrev.Name = "btnPrev"
        Me.btnPrev.Size = New System.Drawing.Size(101, 26)
        Me.btnPrev.TabIndex = 1
        Me.btnPrev.Text = "上一步(&P)"
        Me.btnPrev.UseVisualStyleBackColor = True
        '
        'btnNext
        '
        Me.btnNext.Location = New System.Drawing.Point(352, 304)
        Me.btnNext.Name = "btnNext"
        Me.btnNext.Size = New System.Drawing.Size(104, 26)
        Me.btnNext.TabIndex = 0
        Me.btnNext.Text = "下一步(&N)"
        Me.btnNext.UseVisualStyleBackColor = True
        '
        'PictureBox1
        '
        Me.PictureBox1.BackgroundImage = CType(resources.GetObject("PictureBox1.BackgroundImage"), System.Drawing.Image)
        Me.PictureBox1.Location = New System.Drawing.Point(-3, 0)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(190, 282)
        Me.PictureBox1.TabIndex = 8
        Me.PictureBox1.TabStop = False
        '
        'Wizard
        '
        Me.AcceptButton = Me.btnNext
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(581, 359)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.btnNext)
        Me.Controls.Add(Me.btnPrev)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.btnFinish)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Wizard"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "设置向导"
        CType(Me.nudDaySize, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage1.PerformLayout()
        Me.TabPage2.ResumeLayout(False)
        Me.TabPage2.PerformLayout()
        Me.TabPage3.ResumeLayout(False)
        Me.TabPage3.PerformLayout()
        Me.TabPage4.ResumeLayout(False)
        Me.TabPage4.PerformLayout()
        Me.TabPage5.ResumeLayout(False)
        Me.TabPage5.PerformLayout()
        CType(Me.nudGroupSize, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPage6.ResumeLayout(False)
        Me.TabPage6.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents DateTimePicker1 As System.Windows.Forms.DateTimePicker
    Friend WithEvents nudDaySize As System.Windows.Forms.NumericUpDown
    Friend WithEvents btnFinish As System.Windows.Forms.Button
    Friend WithEvents cmbStyle As System.Windows.Forms.ComboBox
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnPrev As System.Windows.Forms.Button
    Friend WithEvents btnNext As System.Windows.Forms.Button
    Friend WithEvents TabPage4 As System.Windows.Forms.TabPage
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents cmbLevel As System.Windows.Forms.ComboBox
    Friend WithEvents TextBox2 As System.Windows.Forms.TextBox
    Friend WithEvents TabPage5 As System.Windows.Forms.TabPage
    Friend WithEvents TextBox3 As System.Windows.Forms.TextBox
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents nudGroupSize As System.Windows.Forms.NumericUpDown
    Friend WithEvents TabPage6 As System.Windows.Forms.TabPage
    Friend WithEvents TextBox4 As System.Windows.Forms.TextBox
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
End Class
