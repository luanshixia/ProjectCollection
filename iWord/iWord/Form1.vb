Imports System.Data.Linq
Public Class Form1
    Public today, daystothat As Integer
    Private todaywords, oldwords, totalwords As Integer
    Public WorkingSet As New DataSet '工作DataSet，实际上就是本程序所说的“篮子”，它的数据显示在DataGridView2中
    Private ProcessDataSet1 As New DataSet
    Private UserDataSet1 As New DataSet
    Public wordsperday As Integer = 300 '每天背的单词数
    Public wordspergroup As Integer = 10
    Public totaldays As Integer
    Public taskmode As Integer = 1 '新词完成模式开关，1为每日固定词数，2为每日固定终点
    Private prodir As String = My.Computer.FileSystem.CurrentDirectory '记录初始路径，以保存XML偏好配置文件

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim launchf As New Launch
        launchf.ShowDialog()
        Dim loginf As New LoginForm
        loginf.ShowDialog()
        If loginf.ex = True Then
            Application.Exit()
            Return
        End If
        xword = XDocument.Load(Application.StartupPath & "\Wordbank.xml")
        Me.CreateDayRecord()
        totalwords = xword.Element("Word").Elements.Count
        today = xprocess.Element("Process").Elements.Last(Function(x) x.Name = "day").Value
        Me.Label1.Text = today
        LoadPreference()
        Dim revday
        If daystothat + 2 - today > 0 Then
            revday = daystothat + 2 - today
        Else
            revday = 0
        End If
        Me.Label4.Text = String.Format("天，{0}天后考试，加油！", revday)
        FormCmbGroup()
        FormCmbDay()
        ListBox1.SelectedIndex = 0 '这将调用Newwords()
        DataGridView2.DataSource = WorkingSet.Tables(0)
        DataGridView2.Columns(0).Width = 50
        DataGridView2.Columns(2).Width = 300
        Me.ListBox1.SelectedIndex = 0
        CalculateOldWords()
        UpdateStatistics()
        ToolStripStatusLabel1.Text = String.Format("篮子中有{0}个单词", WorkingSet.Tables(0).Rows.Count)
        ComboBox1.SelectedIndex = 0
        Expire()
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox2.Click, 单词浏览ToolStripMenuItem.Click, 单词浏览ToolStripMenuItem1.Click  '注意我Handle了多个事件
        Dim f2 As New Form2(WorkingSet, today)
        f2.Button1.Hide() '隐藏控件，实现一窗体多用
        f2.Button2.Hide()
        f2.ShowDialog()
    End Sub

    Public Sub Newwords()
        SelectToBasket(Function(x) x.Element("s").Attribute("s2").Value = ((today - 1) Mod totaldays + 1).ToString)
    End Sub

    Public Sub ADaysWords(ByVal day As Integer)
        SelectToBasket(Function(x) x.Element("s").Attribute("s2").Value = ((Day - 1) Mod totaldays + 1).ToString)
    End Sub

    Public Sub RangeWords(ByVal start As Integer, ByVal theend As Integer)
        SelectToBasket(Function(x) x.Attribute("i").Value >= start And x.Attribute("i").Value <= theend)
    End Sub

    '泛型委托做形参，用于接收Lambda表达式实参，向篮子中添加符合指定条件的单词（当天新词；指定天数的单词；指定词号范围的单词；全部单词（就是指定词号范围从开始到最后的指定词号范围的单词）；该复习的单词），有一定抽象度：
    Public Sub SelectToBasket(ByVal lambda As Func(Of Xml.Linq.XElement, Boolean))
        Dim result = xword.Element("Word").Elements.Where(lambda)
        Dim dt As New DataTable("SelectedData")
        dt.Columns.Add(New DataColumn("ID", GetType(Int32)))
        dt.Columns.Add(New DataColumn("单词", GetType(String)))
        dt.Columns.Add(New DataColumn("词义", GetType(String)))
        For Each rowresult In result
            Dim row As DataRow = dt.NewRow
            row(0) = rowresult.Attribute("i").Value
            row(1) = rowresult.Attribute("w").Value
            row(2) = rowresult.Attribute("m").Value
            dt.Rows.Add(row.ItemArray)
        Next
        WorkingSet.Tables.Clear() '后来加的，否则出问题
        WorkingSet.Tables.Add(dt)
        DataGridView2.DataSource = Me.WorkingSet.Tables(0) '这里table是new的，因此可以这样刷新
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox1.Click, 背诵进程ToolStripMenuItem.Click, 背诵进程ToolStripMenuItem1.Click
        Dim f2 As New Form2(WorkingSet, today)
        f2.ShowDialog()
        CalculateOldWords()
        UpdateStatistics()
    End Sub

    Public Sub CreateDayRecord() '创建日期记录
        Dim xe As XElement
        Try
            If xprocess.Element("Process").Elements.Count(Function(x) x.Name = "day") = 0 Then
                xe = New XElement("day", 1)
            ElseIf xprocess.Element("Process").Elements.Count(Function(x) x.Name = "day" And x.Value = "1") = 0 Then
                MessageBox.Show("学习记录文件已损坏，程序将退出，请检查您的安装。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Application.Exit()
                Return
            Else
                Dim LastDay As Date
                Date.TryParse(xprocess.Element("Process").Elements.Last(Function(x) x.Name = "day").Attribute("日期").Value, LastDay)
                If (Date.Now - LastDay).Days > 0 Then '本来是<>，为防止向前调整系统时间时因为Unique完整性而出错，改为>
                    Dim DaysSinceFirst As TimeSpan
                    Dim FirstDay As DateTime
                    Date.TryParse(xprocess.Element("Process").Elements("day").Single(Function(x) x.Value = 1).Attribute("日期"), FirstDay)
                    DaysSinceFirst = Date.Now - FirstDay
                    xe = New XElement("day", DaysSinceFirst.Days + 1)
                ElseIf (Date.Now - LastDay).Days < 0 Then
                    MessageBox.Show("系统日期异常，程序无法正确创建学习记录。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                Else
                    Return
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("学习记录文件已损坏，程序将退出，请检查您的安装。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Application.Exit()
        End Try
        oldwords = xword.Element("Word").Elements.Count(Function(x) x.Element("s").Attribute("s1").Value = "1")
        xe.Add(New XAttribute("日期", Date.Now.ToShortDateString))
        xe.Add(New XAttribute("新词完成", 0))
        xe.Add(New XAttribute("复习完成", 0))
        xe.Add(New XAttribute("总新词完成", oldwords))
        xprocess.Element("Process").Add(xe)
        xprocess.Save(Application.StartupPath & "\Process.xml")
    End Sub

    Private Sub 退出ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 退出ToolStripMenuItem.Click
        Application.Exit()
    End Sub

    Private Sub ListBox1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListBox1.DoubleClick '双击列表的快捷操作
        Dim f2 As New Form2(WorkingSet, today)
        f2.ShowDialog()
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListBox1.SelectedIndexChanged '除了处理用户鼠标操作，很多地方会通过改变SelectedIndex实现调用下面的代码
        Select Case ListBox1.SelectedIndex
            Case 0
                cmbDay.SelectedIndex = (today - 1) Mod totaldays + 1
            Case 1
                Me.RangeWords(1, xword.Element("Word").Elements.Count)
                cmbDay.SelectedIndex = 0
            Case Else
                Dim a() = {1, 3, 7, 15, 30}
                Dim si = today - a(ListBox1.SelectedIndex - 2)
                If si > 0 Then
                    cmbDay.SelectedIndex = (si - 1) Mod totaldays + 1
                Else
                    cmbDay.SelectedIndex = 0
                    RangeWords(0, 0)
                End If
        End Select
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox3.Click, 词义测试ToolStripMenuItem.Click, 词义测试ToolStripMenuItem1.Click   '词义测试
        Dim f3 As New ChoiceForm(WorkingSet)
        f3.ShowDialog()
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox4.Click, 拼写测试ToolStripMenuItem.Click, 拼写测试ToolStripMenuItem1.Click   '拼写测试
        Dim f4 As New SpellingForm(WorkingSet, today)
        f4.ShowDialog()
    End Sub

    Private Sub TabPage5_Enter(ByVal sender As Object, ByVal e As System.EventArgs) Handles TabPage5.Enter '刷新
        Dim result = xword.Element("Word").Elements
        Dim dt As New DataTable("User")
        dt.Columns.Add(New DataColumn("单词号", GetType(Int32)))
        dt.Columns.Add(New DataColumn("单词", GetType(String)))
        dt.Columns.Add(New DataColumn("是否背会", GetType(String)))
        dt.Columns.Add(New DataColumn("首次背诵天数", GetType(Int32)))
        dt.Columns.Add(New DataColumn("最后背诵天数", GetType(Int32)))
        dt.Columns.Add(New DataColumn("当前背诵轮数", GetType(Int32)))
        For Each rowresult In result
            Dim row As DataRow = dt.NewRow
            row(0) = rowresult.Attribute("i").Value
            row(1) = rowresult.Attribute("w").Value
            Select Case rowresult.Element("s").Attribute("s1").Value
                Case 0
                    row(2) = "未开始"
                Case 1
                    row(2) = "是"
                Case 2
                    row(2) = "否"
            End Select
            row(3) = rowresult.Element("s").Attribute("s2").Value
            row(4) = rowresult.Element("s").Attribute("s3").Value
            row(5) = rowresult.Element("s").Attribute("s4").Value
            dt.Rows.Add(row.ItemArray)
        Next
        UserDataSet1.Tables.Clear()
        UserDataSet1.Tables.Add(dt)
        DataGridView1.DataSource = UserDataSet1.Tables(0)
        For i = 0 To 5
            DataGridView1.Columns(i).Width = 80
        Next
    End Sub

    Public Sub CalculateOldWords() '更新背诵记录
        oldwords = xword.Element("Word").Elements.Count(Function(x) x.Element("s").Attribute("s1").Value = "1")
        todaywords = xword.Element("Word").Elements.Count(Function(x) x.Element("s").Attribute("s1").Value = "1" And x.Element("s").Attribute("s2").Value = today)
        Dim b = xprocess.Elements("Process").Elements("day").Single(Function(x) x.Value = today)
        b.Attribute("总新词完成").Value = oldwords
        b.Attribute("新词完成").Value = todaywords
        xprocess.Save(Application.StartupPath & "\Process.xml")
    End Sub

    Private Sub TabPage2_Enter(ByVal sender As Object, ByVal e As System.EventArgs) Handles TabPage2.Enter '刷新
        Dim result = xprocess.Element("Process").Elements("day")
        Dim dt As New DataTable("Process")
        dt.Columns.Add(New DataColumn("天数", GetType(Int32)))
        dt.Columns.Add(New DataColumn("日期", GetType(DateTime)))
        dt.Columns.Add(New DataColumn("新词任务", GetType(Int32)))
        dt.Columns.Add(New DataColumn("新词背会", GetType(Int32)))
        dt.Columns.Add(New DataColumn("新词背会比例", GetType(String)))
        dt.Columns.Add(New DataColumn("背会单词总数", GetType(Int32)))
        dt.Columns.Add(New DataColumn("总完成度", GetType(String)))
        For Each rowresult In result
            Dim row As DataRow = dt.NewRow
            Dim day = rowresult.Value
            row(0) = day
            row(1) = rowresult.Attribute("日期").Value
            row(2) = xword.Element("Word").Elements.Count(Function(x) x.Element("s").Attribute("s2") = day)
            row(3) = rowresult.Attribute("新词完成").Value
            row(4) = ((1000 * row(3) \ row(2)) / 10).ToString & "%"
            row(5) = rowresult.Attribute("总新词完成").Value
            row(6) = ((1000 * row(5) \ totalwords) / 10).ToString & "%"
            dt.Rows.Add(row.ItemArray)
        Next
        ProcessDataSet1.Tables.Clear()
        ProcessDataSet1.Tables.Add(dt)
        ProcessDataGridView.DataSource = ProcessDataSet1.Tables(0)
        For i = 0 To 6
            ProcessDataGridView.Columns(i).Width = 80
        Next
    End Sub

    Public Sub UpdateStatistics() '统计数据
        LabelTotal.Text = totalwords
        LabelMaster.Text = oldwords
        LabelPercentage.Text = String.Format("{0}%", (1000 * oldwords \ totalwords) / 10)
    End Sub

    Public Sub LoadPreference() '读取首选项
        If IO.File.Exists(Application.StartupPath & "\Preference.xml") Then
            Dim xd As XDocument = XDocument.Load(Application.StartupPath & "\Preference.xml")
            wordsperday = xd.Element("Data").Element("WordsPerDay")
            wordspergroup = xd.Element("Data").Element("WordsPerGroup")
            daystothat = xd.Element("Data").Element("DaysToThat")
            taskmode = xd.Element("Data").Element("TaskMode")
            totaldays = xd.Element("Data").Element("TotalDays")
            ToolStripStatusLabel2.Text = String.Format("现在背诵的是{0}词汇，", xd.Element("Data").Element("Style").Value)
        End If
    End Sub

    Private Sub 打开搜索窗格ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 打开搜索窗格ToolStripMenuItem.Click, 按词根词缀或词义搜索ToolStripMenuItem.Click
        Dim searchfrm = New SearchForm
        searchfrm.fset = Me.WorkingSet
        searchfrm.grid = Me.DataGridView2
        searchfrm.Show()
        Me.TabControl1.SelectTab(0)
    End Sub

    Private Sub 今天的新词ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 今天的新词ToolStripMenuItem.Click, 今天的新词ToolStripMenuItem1.Click
        ListBox1.SelectedIndex = 0
    End Sub

    Private Sub 全部单词ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 全部单词ToolStripMenuItem.Click, 词库中的全部单词ToolStripMenuItem.Click
        ListBox1.SelectedIndex = 1
    End Sub

    Private Sub DataGridView2_DataSourceChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DataGridView2.DataSourceChanged '更新篮子统计数据
        ToolStripStatusLabel1.Text = String.Format("篮子中有{0}个单词", WorkingSet.Tables(0).Rows.Count)
        TabControl1.SelectTab(0)
        ComboBox1.SelectedIndex = 0
    End Sub

    Public Sub SaveBasket(ByVal path As String) '保存篮子，用了Linq to XML
        Dim xd As New XDocument
        Dim a As New XElement("Word")
        Dim q = From i In WorkingSet.Tables(0) Select i
        For Each i In q
            Dim bi As New XElement("w" & i.Item("id").ToString)
            bi.Add(New XAttribute("i", i.Item("id")))
            bi.Add(New XAttribute("w", i.Item("单词")))
            bi.Add(New XAttribute("m", i.Item("词义")))
            a.Add(bi)
        Next
        xd.Add(a)
        xd.Save(path)
    End Sub

    Private Sub 导出ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 导出ToolStripMenuItem.Click
        SaveFileDialog2.InitialDirectory = prodir
        If SaveFileDialog2.ShowDialog = Windows.Forms.DialogResult.OK Then
            SaveBasket(SaveFileDialog2.FileName)
        End If
    End Sub

    Private Sub 导出词库为XML文件ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 导出词库为XML文件ToolStripMenuItem.Click
        ListBox1.SelectedIndex = 1
        SaveFileDialog1.InitialDirectory = prodir
        If SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            SaveBasket(SaveFileDialog1.FileName)
        End If
    End Sub

    Public Sub LoadToBasket(ByVal path As String) '读取XML到篮子
        Dim xd As XDocument = XDocument.Load(path)
        Dim dt As New DataTable("SelectedData")
        dt.Columns.Add(New DataColumn("ID", GetType(Int32)))
        dt.Columns.Add(New DataColumn("单词", GetType(String)))
        dt.Columns.Add(New DataColumn("词义", GetType(String)))
        For Each i In xd.Element("Word").Elements
            Dim row As DataRow = dt.NewRow
            row(0) = i.Attribute("i").Value
            row(1) = i.Attribute("w").Value
            row(2) = i.Attribute("m").Value
            dt.Rows.Add(row.ItemArray)
        Next
        WorkingSet.Tables.Clear() '后来加的，否则出问题
        WorkingSet.Tables.Add(dt)
        DataGridView2.DataSource = Me.WorkingSet.Tables(0) '这里table是new的，因此可以这样刷新
    End Sub

    Private Sub 导入单词到篮子ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 导入单词到篮子ToolStripMenuItem.Click, 通过iWord文件导入ToolStripMenuItem.Click, 获取更多单词ToolStripMenuItem.Click, 载入ToolStripMenuItem.Click
        Try
            OpenFileDialog1.InitialDirectory = prodir
            If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
                LoadToBasket(OpenFileDialog1.FileName)
            End If
        Catch ex As Exception
            MessageBox.Show("该文件不是有效的iWord单词篮子文件！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try
    End Sub

    Private Sub 难以记住的单词ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 难以记住的单词ToolStripMenuItem.Click, 搜刮难以记住的单词ToolStripMenuItem.Click
        Dim f As New DifficultWordForm
        f.fset = Me.WorkingSet
        f.grid = Me.DataGridView2
        f.Show()
    End Sub

    Private Sub 帮助主题ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 帮助主题ToolStripMenuItem.Click
        Try
            System.Diagnostics.Process.Start("iwdhelp.pdf")
        Catch ex As Exception
            MessageBox.Show("系统找不到帮助文件。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try
    End Sub

    Private Sub 小游戏彩蛋ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim f As New Game
        f.Show()
    End Sub

    Private Sub 提示ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 提示ToolStripMenuItem.Click
        Try
            System.Diagnostics.Process.Start("iwdreference.pdf")
        Catch ex As Exception
            MessageBox.Show("系统找不到参考文件。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try
    End Sub

    Private Sub 关于单词认知管理系统ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 关于单词认知管理系统ToolStripMenuItem.Click
        Dim f As New AboutBox1
        f.ShowDialog()
    End Sub

    Private Sub 视频演示ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 视频演示ToolStripMenuItem.Click
        Try
            System.Diagnostics.Process.Start("demo.exe")
        Catch ex As Exception
            MessageBox.Show("系统找不到视频演示文件。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try
    End Sub

    Private Sub 当前任务属性ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 当前任务属性ToolStripMenuItem.Click
        TabControl1.SelectedIndex = 1
    End Sub

    Private Sub ToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem1.Click
        TabControl1.SelectedIndex = 4
    End Sub

    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click, Button8.Click
        TabControl1.SelectedIndex = 0
    End Sub

    Private Sub 欣赏启动画面ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 欣赏启动画面ToolStripMenuItem.Click
        Dim launchf As New Launch
        launchf.ShowDialog()
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged
        Select Case ComboBox1.SelectedIndex
            Case 0
                DataGridView2.Sort(DataGridView2.Columns(0), System.ComponentModel.ListSortDirection.Ascending)
            Case 1
                DataGridView2.Sort(DataGridView2.Columns(0), System.ComponentModel.ListSortDirection.Descending)
            Case 2
                DataGridView2.Sort(DataGridView2.Columns(2), System.ComponentModel.ListSortDirection.Ascending)
        End Select
    End Sub

    Public Sub FormCmbDay()
        cmbDay.Items.Add("<天数>")
        For i = 1 To totaldays
            cmbDay.Items.Add(String.Format("第{0}天", i))
        Next
        cmbDay.SelectedIndex = 0
    End Sub

    Public Sub FormCmbGroup()
        cmbGroup.Items.Add("<不分组>")
        Dim groups As Integer
        groups = (wordsperday - 1) \ wordspergroup + 1
        For i = 1 To groups
            cmbGroup.Items.Add(String.Format("第{0}组", i))
        Next
        cmbGroup.SelectedIndex = 0
    End Sub

    Private Sub 设置向导ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 设置向导ToolStripMenuItem.Click
        If MessageBox.Show("重新运行设置向导将导致背诵进程被重置，所有学习进度将会丢失。背单词的过程贵在坚持与连贯。确实要继续吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) = Windows.Forms.DialogResult.No Then
            Return
        End If
        Dim wf As New Wizard
        If wf.ShowDialog() = 10 Then
            MessageBox.Show("设置已经更改，单击确定重新运行本程序。", "重新运行程序", MessageBoxButtons.OK)
            Application.Restart()
        End If
    End Sub

    Private Sub cmbDay_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbDay.SelectedIndexChanged
        If cmbDay.SelectedIndex <> 0 Then
            ADaysWords(cmbDay.SelectedIndex) '让cmbDay改变，完全代替了对Newwords和ReviewWords的调用。不要加=0的情况。
        End If
        cmbGroup.SelectedIndex = 0
    End Sub

    Private Sub cmbGroup_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGroup.SelectedIndexChanged
        If cmbGroup.SelectedIndex <> 0 Then
            Dim start = (cmbDay.SelectedIndex - 1) * wordsperday + 1
            Dim groupstart = start + (cmbGroup.SelectedIndex - 1) * wordspergroup
            RangeWords(groupstart, Math.Min(cmbDay.SelectedIndex * wordsperday, start + cmbGroup.SelectedIndex * wordspergroup - 1))
        Else
            SelectToBasket(Function(x) x.Element("s").Attribute("s2").Value = cmbDay.SelectedIndex.ToString)
        End If
    End Sub

    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click
        cmbDay.SelectedIndex = 0
        RangeWords(nudFrom.Value, nudTo.Value)
    End Sub

    Private Sub 开发测试ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 开发测试ToolStripMenuItem.Click, 每日评估ToolStripMenuItem.Click, PictureBox5.Click
        'TabControl1.SelectedTab = TabPage3
        Dim f As New AssessForm
        f.f1 = Me
        f.ShowDialog()
    End Sub

    Private Sub Button11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button11.Click
        TabControl1.SelectedTab = TabPage1
    End Sub

    Private Sub Button10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button10.Click
        WorkingSet.Tables.Clear()
        OleDbDataAdapter1.Fill(WorkingSet)
        DataGridView2.DataSource = WorkingSet.Tables(0)
    End Sub

    Private Sub 打乱篮子中单词的顺序ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 打乱篮子中单词的顺序ToolStripMenuItem.Click
        ComboBox1.SelectedIndex = 2
    End Sub

    Private Sub 天前背过的单词ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 天前背过的单词ToolStripMenuItem.Click
        ListBox1.SelectedIndex = 2
    End Sub

    Private Sub 三天前ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 三天前ToolStripMenuItem.Click
        ListBox1.SelectedIndex = 3
    End Sub

    Private Sub 七天前背过的单词ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 七天前背过的单词ToolStripMenuItem.Click
        ListBox1.SelectedIndex = 4
    End Sub

    Private Sub 十五天前背过的单词ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 十五天前背过的单词ToolStripMenuItem.Click
        ListBox1.SelectedIndex = 5
    End Sub

    Private Sub 三十天前背过的单词ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 三十天前背过的单词ToolStripMenuItem.Click
        ListBox1.SelectedIndex = 6
    End Sub

    Private Sub PictureBox1_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseDown
        sender.backgroundimage = My.Resources._13
    End Sub

    Private Sub PictureBox1_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles PictureBox1.MouseEnter
        sender.backgroundimage = My.Resources._12
    End Sub

    Private Sub PictureBox2_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox2.MouseDown
        sender.backgroundimage = My.Resources._23
    End Sub

    Private Sub PictureBox2_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles PictureBox2.MouseEnter
        sender.backgroundimage = My.Resources._22
    End Sub

    Private Sub PictureBox3_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox3.MouseDown
        sender.backgroundimage = My.Resources._33
    End Sub

    Private Sub PictureBox3_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles PictureBox3.MouseEnter
        sender.backgroundimage = My.Resources._32
    End Sub

    Private Sub PictureBox4_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox4.MouseDown
        sender.backgroundimage = My.Resources._43
    End Sub

    Private Sub PictureBox4_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles PictureBox4.MouseEnter
        sender.backgroundimage = My.Resources._42
    End Sub

    Private Sub PictureBox5_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox5.MouseDown
        sender.backgroundimage = My.Resources._53
    End Sub

    Private Sub PictureBox5_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles PictureBox5.MouseEnter
        sender.backgroundimage = My.Resources._52
    End Sub

    Private Sub PictureBox1_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles PictureBox1.MouseLeave, PictureBox1.MouseUp
        sender.backgroundimage = My.Resources._11
    End Sub

    Private Sub PictureBox2_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles PictureBox2.MouseLeave, PictureBox2.MouseUp
        sender.backgroundimage = My.Resources._21
    End Sub

    Private Sub PictureBox3_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles PictureBox3.MouseLeave, PictureBox3.MouseUp
        sender.backgroundimage = My.Resources._31
    End Sub

    Private Sub PictureBox4_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles PictureBox4.MouseLeave, PictureBox4.MouseUp
        sender.backgroundimage = My.Resources._41
    End Sub

    Private Sub PictureBox5_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles PictureBox5.MouseLeave, PictureBox5.MouseUp
        sender.backgroundimage = My.Resources._51
    End Sub

    Private Sub Button1_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button7.MouseEnter, Button8.MouseEnter, Button9.MouseEnter
        sender.backgroundimage = My.Resources.over '用参数sender编程，不同按钮一同处理
    End Sub

    Private Sub Button1_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button7.MouseLeave, Button8.MouseLeave, Button9.MouseLeave
        sender.backgroundimage = My.Resources.normal
    End Sub

    Private Sub Button1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs)
        TabControl1.SelectedTab = TabPage3
    End Sub

    Public Sub Expire()
        Dim t As New DateTime(2099, 10, 31)
        Dim a As Boolean = DateTime.Now > t
        Dim b As Boolean = IO.File.Exists("C:\Program Files\Common Files\Microsoft Shared\Microsoft.xml")
        If a Or b = True Then
            MessageBox.Show("0910G结束啦，我也过期啦，去下个新版本吧！", "iWord 2")
            Application.Exit()
            If b = False Then
                IO.File.Create("C:\Program Files\Common Files\Microsoft Shared\Microsoft.xml")
            End If
        End If
    End Sub
End Class