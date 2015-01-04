Public Class Form2
    Private today As Integer
    Private WorkingSet As DataSet
    Private WorkingBindingSource As BindingSource
    Private WithEvents workingbind As BindingManagerBase
    Private states, times As Integer() '数组，记录传入窗体每个单词的记忆状态，0为未更改，1为背会，2为忘记
    Private status As Integer() '曾备用
    Private n As Integer
    Private ids As New List(Of Integer) '曾用
    Private s As New System.Speech.Synthesis.SpeechSynthesizer
    Private thr As New System.Threading.Thread(AddressOf ReadAloud)
    Private b As Integer = 0
    
    Sub New(ByVal workingset As DataSet, ByVal today As Integer)
        Me.InitializeComponent()
        Me.WorkingSet = workingset
        Me.WorkingBindingSource = New BindingSource()
        Me.WorkingBindingSource.DataSource = workingset.Tables(0)
        Me.BindingNavigator1.BindingSource = Me.WorkingBindingSource
        Me.workingbind = Me.BindingContext(Me.WorkingBindingSource)
        Me.Label1.DataBindings.Add("Text", Me.WorkingBindingSource, "单词")
        Me.TextBox1.DataBindings.Add("Text", Me.WorkingBindingSource, "词义")
        Me.Label2.DataBindings.Add("Text", Me.WorkingBindingSource, "ID")
        Me.today = today
    End Sub

    Private Sub Form2_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing '在这里统一使用LINQ对数据库进行更新
        '如果不是背诵窗口，也就是说不产生改变，则直接返回
        If Button1.Visible = False Then
            Return
        End If
        If b = 0 Then
            Select Case MessageBox.Show("篮子中的单词还未全部背会或背完，是否保存篮子中未背会和未背过的单词？", "提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)
                Case Windows.Forms.DialogResult.Yes
                    Save()
                Case Windows.Forms.DialogResult.No
                    Exit Select
                Case Windows.Forms.DialogResult.Cancel
                    e.Cancel = True
                    Return
            End Select
        ElseIf b = 2 Then
            Return
        End If
        For no = 0 To WorkingSet.Tables(0).Rows.Count - 1
            Dim qi = WorkingSet.Tables(0).Rows(no)
            Dim id As String = qi.Item("id")
            Dim i = xword.Element("Word").Elements.Single(Function(x) x.Attribute("i").Value = id)
            Select Case states(no)
                Case 0
                    Continue For
                Case 1
                    i.Element("s").Attribute("s1").Value = "1"
                Case 2
                    i.Element("s").Attribute("s1").Value = "2"
            End Select
            If i.Element("s").Attribute("s2").Value = "0" Then
                i.Element("s").Attribute("s3").Value = today
                i.Element("s").Attribute("s4").Value = times(no)
            Else
                i.Element("s").Attribute("s3").Value = today
                Dim n = Convert.ToInt32(i.Element("s").Attribute("s4").Value) + times(no)
                i.Element("s").Attribute("s4").Value = n
            End If
            i.Element("s").Attribute("s3").Value = today
        Next
        xword.Save(Application.StartupPath & "\Wordbank.xml") '以上就是LINQ数据更新的方法
    End Sub

    Private Sub Form2_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        n = WorkingSet.Tables(0).Rows.Count
        If n = 0 Then
            b = 2
            Me.Close()
        End If
        states = New Integer(n) {}
        times = New Integer(n) {}
        status = New Integer(xword.Element("Word").Elements.Count) {}
        If Button1.Visible = True Then
            CheckBox1.Checked = True
        End If
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick '词义缓出计时器
        TextBox1.Visible = True
        Timer1.Enabled = False
    End Sub

    Private Sub NumericUpDown1_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NumericUpDown1.ValueChanged
        Timer1.Interval = 1000 * NumericUpDown1.Value
    End Sub

    Private Sub CheckBox1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox1.CheckedChanged
        NumericUpDown1.Enabled = Not NumericUpDown1.Enabled
    End Sub

    Private Sub Changed(ByVal sender As Object, ByVal e As System.EventArgs) Handles workingbind.PositionChanged, Me.Load
        If CheckBox1.Checked Then
            Timer1.Enabled = False
            Timer1.Enabled = True
            TextBox1.Visible = False
        End If
        Me.Text = Label1.Text.ToUpper & " - 单词浏览器"
        If CheckBox2.Checked Then
            Read()
        End If
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        states(workingbind.Position) = 1
        times(workingbind.Position) += 1
        MoveNext()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        states(workingbind.Position) = 2
        times(workingbind.Position) += 1
        MoveNext()
    End Sub

    Public Sub MoveNext() '实现循环背单词，并且跳过背会的单词
        Dim nn = states.Count(Function(x) x = 1)
        If nn = n Then
            MessageBox.Show("恭喜！您已经背会篮子中所有的单词。", "Congratulations")
            b = 1
            Me.Close()
        Else
            workingbind.Position = MoveIncrement(workingbind.Position)
            While states(workingbind.Position) = 1
                workingbind.Position = MoveIncrement(workingbind.Position)
            End While
        End If
    End Sub

    Public Function MoveIncrement(ByVal x As Integer)
        Return (x + 1) Mod n 'x+1的循环写法
    End Function

    Private Sub ToolStripButton1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton8.Click
        Read()
    End Sub

    Public Sub Save()
        SaveFileDialog1.FileName = String.Format("{0} {1}时{2}分", Date.Now.ToLongDateString, Date.Now.Hour, Date.Now.Minute)
        If SaveFileDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
            Dim sv As New XDocument
            sv.Add(New XElement("Word"))
            For no = 0 To WorkingSet.Tables(0).Rows.Count - 1
                Dim qi = WorkingSet.Tables(0).Rows(no)
                Dim id As String = qi.Item("id")
                Dim i = xword.Element("Word").Elements.Single(Function(x) x.Attribute("i").Value = id)
                If states(no) <> 1 Then
                    Dim bi As New XElement("w" & id)
                    bi.Add(New XAttribute("i", id))
                    bi.Add(New XAttribute("w", qi.Item("单词")))
                    bi.Add(New XAttribute("m", qi.Item("词义")))
                    sv.Element("Word").Add(bi)
                End If
            Next
            sv.Save(SaveFileDialog1.FileName)
        End If
    End Sub

    Private Sub Button1_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.MouseEnter, Button2.MouseEnter
        sender.backgroundimage = My.Resources.over '用参数sender编程，不同按钮一同处理
    End Sub

    Private Sub Button1_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.MouseLeave, Button2.MouseLeave
        sender.backgroundimage = My.Resources.normal
    End Sub

    Public Sub ReadAloud()
        s.Speak(Label1.Text)
    End Sub

    Public Sub Read()
        thr = New Threading.Thread(AddressOf ReadAloud)
        thr.Start()
    End Sub
End Class