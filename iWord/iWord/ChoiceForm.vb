Public Class ChoiceForm
    Private WorkingSet As DataSet
    Private WorkingBindingSource As BindingSource
    Private WithEvents workingbind As BindingManagerBase
    Private n As Integer
    Private nn = xword.Element("Word").Elements.Count
    Private status As Integer() = New Integer(nn + 1) {}
    Private time As Integer
    Public rights, wrongs As Integer
    Public rate As Double
    Public IsCompelete As Boolean
    Private s As New System.Speech.Synthesis.SpeechSynthesizer
    Private thr As New System.Threading.Thread(AddressOf ReadAloud)
    Public b As Integer

    Private Enum Answer
        A
        B
        C
        D
    End Enum
    Private CorrectAnswer As Answer

    Sub New(ByVal workingset As DataSet)
        Me.InitializeComponent()
        Me.WorkingSet = workingset
        Me.WorkingBindingSource = New BindingSource()
        Me.WorkingBindingSource.DataSource = workingset.Tables(0)
        Me.BindingNavigator1.BindingSource = Me.WorkingBindingSource
        Me.workingbind = Me.BindingContext(Me.WorkingBindingSource)
        Me.Label1.DataBindings.Add("Text", Me.WorkingBindingSource, "单词")
        Me.Label2.DataBindings.Add("Text", Me.WorkingBindingSource, "ID")
        Me.TextBox1.DataBindings.Add("Text", Me.WorkingBindingSource, "词义")
    End Sub

    Private Sub Form2_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If b <> 1 Then
            Select Case MessageBox.Show("确实要关闭测试吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                Case Windows.Forms.DialogResult.Yes
                    Exit Select
                Case Windows.Forms.DialogResult.No
                    e.Cancel = True
            End Select
        End If
    End Sub

    Private Sub Form2_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'TODO: 这行代码将数据加载到表“GreDataSet1.GRE”中。您可以根据需要移动或移除它。
        LabelManipulate()
        n = WorkingSet.Tables(0).Rows.Count
        If n = 0 Then
            b = 1
            Me.Close()
        End If
        OneTime()
        Button1.Hide()
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick '计时器
        time += 1
    End Sub

    Private Sub Changed(ByVal sender As Object, ByVal e As System.EventArgs) Handles workingbind.PositionChanged
        LabelManipulate()
        If CheckBox1.Checked Then
            Read()
        End If
        OneTime()
        AButton.ForeColor = Color.Black
        BButton.ForeColor = Color.Black
        CButton.ForeColor = Color.Black
        DButton.ForeColor = Color.Black
    End Sub

    Public Sub SetCorrect()
        Dim rd As New Random
        Select Case Int(rd.NextDouble * 4)
            Case 0
                CorrectAnswer = Answer.A
            Case 1
                CorrectAnswer = Answer.B
            Case 2
                CorrectAnswer = Answer.C
            Case 3
                CorrectAnswer = Answer.D
        End Select
    End Sub

    Public Sub Display()
        Dim rd As New Random
        Dim s As New List(Of String)
        For i = 1 To 3
            Dim r As Integer = rd.NextDouble * nn
            s.Add(xword.Element("Word").Elements.Single(Function(x) x.Attribute("i").Value = r).Attribute("m").Value.Trim)
        Next
        Select Case CorrectAnswer
            Case Answer.A
                AButton.Text = TextBox1.Text
                BButton.Text = s(0)
                CButton.Text = s(1)
                DButton.Text = s(2)
            Case Answer.B
                BButton.Text = TextBox1.Text
                AButton.Text = s(0)
                CButton.Text = s(1)
                DButton.Text = s(2)
            Case Answer.C
                CButton.Text = TextBox1.Text
                AButton.Text = s(0)
                BButton.Text = s(1)
                DButton.Text = s(2)
            Case Answer.D
                DButton.Text = TextBox1.Text
                AButton.Text = s(0)
                BButton.Text = s(1)
                CButton.Text = s(2)
        End Select
    End Sub

    Public Sub OneTime()
        SetCorrect()
        Display()
    End Sub

    Public Sub GoOn()
        If workingbind.Position = workingbind.Count - 1 Then
            If MessageBox.Show("测验完成！现在交卷吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
                IsCompelete = True
                Submit()
            End If
        Else
            workingbind.Position += 1
        End If
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        GoOn()
    End Sub

    Public Sub Submit()
        Dim report As New IO.StringWriter
        Dim min = Int(time / 60)
        Dim sec = time Mod 60
        report.WriteLine("本次测试共用时 {0} 分 {1} 秒", min, sec)
        wrongs = status.Where(Function(x) x = 2).Count '终于体会到Linq to Object的强大用处了！
        rights = status.Where(Function(x) x = 1).Count
        If wrongs + rights = 0 Then
            rate = 0
        Else
            rate = (1000 * rights \ (wrongs + rights)) / 10
        End If
        report.WriteLine("正确个数 = {0}，错误个数 = {1}，正确率 = {2}%，继续加油哦！", rights, wrongs, rate)
        report.WriteLine()
        report.WriteLine("单词                词义")
        report.WriteLine("---------------------------------------------------")
        For i = 1 To nn
            If status(i) = 2 Then
                Dim ii = i
                Dim w = xword.Element("Word").Elements.Single(Function(x) x.Attribute("i").Value = ii).Attribute("w").Value
                Dim m = xword.Element("Word").Elements.Single(Function(x) x.Attribute("i").Value = ii).Attribute("m").Value.Trim
                Dim s = w & Space(20 - w.Length) & m
                report.WriteLine(s)
            End If
        Next
        Dim f As New Report
        f.message = report.ToString
        f.status = Me.status
        f.f = Me
        f.ShowDialog()
    End Sub

    Public Function Evaluate(ByVal myAnswer) As Boolean
        Return myAnswer = Me.CorrectAnswer
    End Function

    Private Sub RadioButton2_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton2.CheckedChanged
        Button1.Show()
    End Sub

    Private Sub RadioButton1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton1.CheckedChanged
        Button1.Hide()
    End Sub

    Private Sub AButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AButton.Click
        Dim id = Convert.ToInt32(Label2.Text)
        If RadioButton2.Checked Then
            If Evaluate(Answer.A) Then
                AButton.ForeColor = Color.Green
                status(id) = 1
            Else
                AButton.ForeColor = Color.Red
                Select Case CorrectAnswer
                    Case Answer.B
                        BButton.ForeColor = Color.Green
                    Case Answer.C
                        CButton.ForeColor = Color.Green
                    Case Answer.D
                        DButton.ForeColor = Color.Green
                End Select
                status(id) = 2
            End If
        Else
            If Evaluate(Answer.A) Then
                status(id) = 1
            Else
                status(id) = 2
            End If
            GoOn()
        End If
    End Sub

    Private Sub BButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BButton.Click
        Dim id = Convert.ToInt32(Label2.Text)
        If RadioButton2.Checked Then
            If Evaluate(Answer.B) Then
                BButton.ForeColor = Color.Green
                status(id) = 1
            Else
                BButton.ForeColor = Color.Red
                Select Case CorrectAnswer
                    Case Answer.A
                        AButton.ForeColor = Color.Green
                    Case Answer.C
                        CButton.ForeColor = Color.Green
                    Case Answer.D
                        DButton.ForeColor = Color.Green
                End Select
                status(id) = 2
            End If
        Else
            If Evaluate(Answer.B) Then
                status(id) = 1
            Else
                status(id) = 2
            End If
            GoOn()
        End If
    End Sub

    Private Sub CButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CButton.Click
        Dim id = Convert.ToInt32(Label2.Text)
        If RadioButton2.Checked Then
            If Evaluate(Answer.C) Then
                CButton.ForeColor = Color.Green
                status(id) = 1
            Else
                CButton.ForeColor = Color.Red
                Select Case CorrectAnswer
                    Case Answer.B
                        BButton.ForeColor = Color.Green
                    Case Answer.A
                        AButton.ForeColor = Color.Green
                    Case Answer.D
                        DButton.ForeColor = Color.Green
                End Select
                status(id) = 2
            End If
        Else
            If Evaluate(Answer.C) Then
                status(id) = 1
            Else
                status(id) = 2
            End If
            GoOn()
        End If
    End Sub

    Private Sub DButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DButton.Click
        Dim id = Convert.ToInt32(Label2.Text)
        If RadioButton2.Checked Then
            If Evaluate(Answer.D) Then
                DButton.ForeColor = Color.Green
                status(id) = 1
            Else
                DButton.ForeColor = Color.Red
                Select Case CorrectAnswer
                    Case Answer.B
                        BButton.ForeColor = Color.Green
                    Case Answer.C
                        CButton.ForeColor = Color.Green
                    Case Answer.A
                        AButton.ForeColor = Color.Green
                End Select
                status(id) = 2
            End If
        Else
            If Evaluate(Answer.D) Then
                status(id) = 1
            Else
                status(id) = 2
            End If
            GoOn()
        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Submit()
    End Sub

    Private Sub CheckBox1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox1.CheckedChanged
        LabelManipulate()
    End Sub

    Public Sub LabelManipulate()
        If CheckBox1.Checked Then
            LabelShow.Text = "Listen Carefully..."
        Else
            LabelShow.Text = Label1.Text
        End If
        Me.Text = LabelShow.Text.ToUpper & " - 词义测试"
    End Sub

    Private Sub Button1_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.MouseEnter, Button2.MouseEnter
        sender.backgroundimage = My.Resources.over '用参数sender编程，不同按钮一同处理
    End Sub

    Private Sub Button1_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.MouseLeave, Button2.MouseLeave
        sender.backgroundimage = My.Resources.normal
    End Sub

    Private Sub ToolStripButton1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton1.Click
        Read()
    End Sub

    Public Sub ReadAloud()
        s.Speak(Label1.Text)
    End Sub

    Public Sub Read()
        thr = New Threading.Thread(AddressOf ReadAloud)
        thr.Start()
    End Sub
End Class