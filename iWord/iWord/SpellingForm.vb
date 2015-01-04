Public Class SpellingForm
    Private today As Integer
    Private WorkingSet As DataSet
    Private WorkingBindingSource As BindingSource
    Private WithEvents workingbind As BindingManagerBase
    Private n As Integer
    Private nn = xword.Element("Word").Elements.Count
    Private status As Integer() = New Integer(nn + 1) {}
    Private inputs As String() = New String(nn + 1) {}
    Private time As Integer
    Public b As Integer

    Sub New(ByVal workingset As DataSet, ByVal today As Integer)
        Me.InitializeComponent()
        Me.WorkingSet = workingset
        Me.WorkingBindingSource = New BindingSource()
        Me.WorkingBindingSource.DataSource = workingset.Tables(0)
        Me.BindingNavigator1.BindingSource = Me.WorkingBindingSource
        Me.workingbind = Me.BindingContext(Me.WorkingBindingSource)
        Me.Label1.DataBindings.Add("Text", Me.WorkingBindingSource, "单词")
        Me.Label2.DataBindings.Add("Text", Me.WorkingBindingSource, "ID")
        Me.TextBox1.DataBindings.Add("Text", Me.WorkingBindingSource, "词义")
        Me.today = today
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
        Me.Text = "拼写测试"
        n = WorkingSet.Tables(0).Rows.Count
        If n = 0 Then
            b = 1
            Me.Close()
        End If
        CheckBox1.Checked = True
        Label4.Text = Hint(Label1.Text)
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick '计时器
        time += 1
    End Sub

    Private Sub Changed(ByVal sender As Object, ByVal e As System.EventArgs) Handles workingbind.PositionChanged
        Me.Text = "拼写测试"
        If CheckBox1.Checked Then
            Label4.Text = Hint(Label1.Text)
        Else
            Label4.Text = "Spelling Test..."
        End If
    End Sub

    Public Sub OneTime()
        Dim i As Integer
        Int32.TryParse(Label2.Text, i)
        inputs(i) = TextBox2.Text
        If Label1.Text.Trim.ToUpper = TextBox2.Text.Trim.ToUpper Then
            status(i) = 1
        Else
            status(i) = 2
        End If
    End Sub

    Public Sub GoOn()
        If workingbind.Position = workingbind.Count - 1 Then
            If MessageBox.Show("测验完成！现在交卷吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
                Submit()
            End If
        Else
            workingbind.Position += 1
        End If
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        If TextBox2.Text <> "Type your spelling here" Then
            OneTime()
            GoOn()
            TextBox2.Focus()
            TextBox2.Text = ""
        End If
    End Sub

    Public Sub Submit()
        Dim report As New IO.StringWriter
        Dim min = Int(time / 60)
        Dim sec = time Mod 60
        report.WriteLine("本次测试共用时 {0} 分 {1} 秒", min, sec)
        Dim wrong = status.Where(Function(x) x = 2).Count '终于体会到Linq to Object的强大用处了！
        Dim right = status.Where(Function(x) x = 1).Count
        Dim rate
        If wrong + right = 0 Then
            rate = 0
        Else
            rate = (1000 * right \ (wrong + right)) / 10
        End If
        report.WriteLine("正确个数 = {0}，错误个数 = {1}，正确率 = {2}%，继续加油哦！", right, wrong, rate)
        report.WriteLine()
        report.WriteLine("单词                您的错写            词义")
        report.WriteLine("---------------------------------------------------")
        For i = 1 To nn
            If status(i) = 2 Then
                Dim ii = i
                Dim w = xword.Element("Word").Elements.Single(Function(x) x.Attribute("i").Value = ii).Attribute("w").Value
                Dim m = xword.Element("Word").Elements.Single(Function(x) x.Attribute("i").Value = ii).Attribute("m").Value.Trim
                Dim s = w & Space(20 - w.Length) & inputs(ii) & Space(20 - inputs(ii).Length) & m
                report.WriteLine(s)
            End If
        Next
        Dim f As New Report
        f.message = report.ToString
        f.status = Me.status
        f.f = Me
        f.ShowDialog()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Submit()
    End Sub

    Private Sub TextBox2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles TextBox2.Click
        TextBox2.Text = ""
        TextBox2.ForeColor = Color.Black
    End Sub

    Public Function Hint(ByVal word As String) As String
        Dim len = word.Length
        Dim a = word.ToCharArray
        If len <= 3 Then
            '3个字母以内的单词，只提示首字母。
            For i = 1 To a.Length - 1
                a(i) = "."
            Next
        ElseIf len <= 6 Then
            '4-6个字母之间的单词，提示首尾两个字母。
            For i = 1 To a.Length - 2
                a(i) = "."
            Next
        Else
            '7个字母以上的单词，提示首字母和位于单词后半部分的随机连续3个字母（可能只是最后2个字母）。
            Dim rd As New Random
            Dim len2 = len / 2
            Dim ini As Integer = rd.NextDouble * len2 + len2 - 1
            For i = 1 To a.Length - 1
                If i - ini > 1 Or i - ini < -1 Then
                    a(i) = "."
                End If
            Next
        End If
        Dim s As String = ""
        For i = 0 To a.Length - 1
            s += a(i)
        Next
        Return s
    End Function

    Private Sub CheckBox1_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked Then
            Label4.Text = Hint(Label1.Text)
        Else
            Label4.Text = "Spelling Test..."
        End If
    End Sub

    Private Sub Button1_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.MouseEnter, Button2.MouseEnter
        sender.backgroundimage = My.Resources.over '用参数sender编程，不同按钮一同处理
    End Sub

    Private Sub Button1_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.MouseLeave, Button2.MouseLeave
        sender.backgroundimage = My.Resources.normal
    End Sub
End Class