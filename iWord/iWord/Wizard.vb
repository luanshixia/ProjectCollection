Public Class Wizard

    Private a As Integer

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFinish.Click
        Try
            Dim source = String.Format(Application.StartupPath & "\Word\{0}词汇.xml", cmbStyle.SelectedItem.ToString)
            IO.File.Copy(source, Application.StartupPath & "\Wordbank.xml", True)
        Catch ex As Exception
            MessageBox.Show("导入词库出错，请检查您的安装是否完整。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try
        Grouping()
        Dim xd As New XDocument
        Dim totalwords = xword.Element("Word").Elements.Count
        xd.Add(New XElement("Data"))
        xd.Element("Data").Add(New XElement("Style", cmbStyle.SelectedItem.ToString))
        xd.Element("Data").Add(New XElement("TestDate", DateTimePicker1.Value.ToShortDateString))
        xd.Element("Data").Add(New XElement("DaysToThat", (DateTimePicker1.Value - Date.Now).Days))
        xd.Element("Data").Add(New XElement("WordsPerDay", nudDaySize.Value))
        xd.Element("Data").Add(New XElement("WordsPerGroup", nudGroupSize.Value))
        xd.Element("Data").Add(New XElement("TotalDays", (totalwords - 1) \ nudDaySize.Value + 1))
        xd.Element("Data").Add(New XElement("TaskMode", "1"))
        xd.Save(Application.StartupPath & "\Preference.xml")
        a = 10
        Me.Close()
    End Sub

    Public Sub Grouping()
        xword = XDocument.Load(Application.StartupPath & "\Wordbank.xml")
        For Each i In xword.Element("Word").Elements
            Dim id = Convert.ToInt32(i.Attribute("i").Value)
            Dim group = (id - 1) \ nudDaySize.Value + 1
            If i.Element("s") Is Nothing Then
                i.Add(New XElement("s"))
                i.Element("s").Add(New XAttribute("s1", 0))
                i.Element("s").Add(New XAttribute("s2", group))
                i.Element("s").Add(New XAttribute("s3", 0))
                i.Element("s").Add(New XAttribute("s4", 0))
            Else
                i.Element("s").Attribute("s2").Value = group
            End If
        Next
        xword.Save(Application.StartupPath & "\Wordbank.xml")
    End Sub

    Private Sub Wizard_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        cmbStyle.SelectedIndex = 0
        cmbLevel.SelectedIndex = 1
    End Sub

    Private Sub btnNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNext.Click
        TabControl1.SelectedIndex += 1
        btnPrev.Enabled = True
        If TabControl1.SelectedIndex = 5 Then
            btnNext.Enabled = False
        End If
    End Sub

    Private Sub btnPrev_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrev.Click
        TabControl1.SelectedIndex -= 1
        btnNext.Enabled = True
        If TabControl1.SelectedIndex = 0 Then
            btnPrev.Enabled = False
        End If
    End Sub

    Private Sub cmbLevel_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbLevel.SelectedIndexChanged
        Select Case cmbLevel.SelectedIndex
            Case 0
                nudDaySize.Value = 100
            Case 1
                nudDaySize.Value = 200
            Case 2
                nudDaySize.Value = 300
        End Select
    End Sub

    Private Sub nudDaySize_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles nudDaySize.ValueChanged
        If nudDaySize.Value > 299 Then
            nudGroupSize.Value = 20
        Else
            nudGroupSize.Value = 10
        End If
    End Sub

    '重写基类ShowDialog()方法，当单击完成结束向导时返回10
    Public Overloads Function ShowDialog()
        Dim b = MyBase.ShowDialog()
        If a = 10 Then
            Return a
        Else
            Return b
        End If
    End Function
End Class