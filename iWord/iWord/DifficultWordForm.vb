Public Class DifficultWordForm

    Public fset As DataSet
    Public grid As DataGridView

    Private Sub DifficultWordForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim q = From i In xword.Element("Word").Elements Select Convert.ToInt32(i.Attribute("i").Value)
        NumericUpDown2.Value = q.Max()
        ComboBox1.SelectedIndex = 0
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim dt As New DataTable("SelectedData")
        dt.Columns.Add(New DataColumn("ID", GetType(Int32)))
        dt.Columns.Add(New DataColumn("单词", GetType(String)))
        dt.Columns.Add(New DataColumn("词义", GetType(String)))
        Dim q
        Select Case ComboBox1.SelectedIndex
            Case 0
                q = From u In xword.Element("Word").Elements _
                    Where Convert.ToInt32(u.Element("s").Attribute("s4").Value) >= NumericUpDown3.Value _
                    And u.Element("s").Attribute("s1").Value = "2" _
                    And Convert.ToInt32(u.Attribute("i").Value) > NumericUpDown1.Value _
                    And Convert.ToInt32(u.Attribute("i").Value) <= NumericUpDown2.Value _
                    Select u
                For Each i In q
                    Dim row As DataRow = dt.NewRow
                    row(0) = i.Attribute("i").Value
                    row(1) = i.Attribute("w").Value
                    row(2) = i.Attribute("m").Value
                    dt.Rows.Add(row.ItemArray)
                Next
            Case 1
                q = From u In xword.Element("Word").Elements _
                    Where Convert.ToInt32(u.Element("s").Attribute("s4").Value) >= NumericUpDown3.Value _
                    And u.Element("s").Attribute("s1").Value = "1" _
                    And Convert.ToInt32(u.Attribute("i").Value) > NumericUpDown1.Value _
                    And Convert.ToInt32(u.Attribute("i").Value) <= NumericUpDown2.Value _
                    Select u
                For Each i In q
                    Dim row As DataRow = dt.NewRow
                    row(0) = i.Attribute("i").Value
                    row(1) = i.Attribute("w").Value
                    row(2) = i.Attribute("m").Value
                    dt.Rows.Add(row.ItemArray)
                Next
            Case Else
                q = From u In xword.Element("Word").Elements _
                    Where Convert.ToInt32(u.Element("s").Attribute("s4").Value) >= NumericUpDown3.Value _
                    And Convert.ToInt32(u.Attribute("i").Value) > NumericUpDown1.Value _
                    And Convert.ToInt32(u.Attribute("i").Value) <= NumericUpDown2.Value _
                    Select u
                For Each i In q
                    Dim row As DataRow = dt.NewRow
                    row(0) = i.Attribute("i").Value
                    row(1) = i.Attribute("w").Value
                    row(2) = i.Attribute("m").Value
                    dt.Rows.Add(row.ItemArray)
                Next
        End Select
        fset.Tables.Clear()
        fset.Tables.Add(dt)
        grid.DataSource = fset.Tables(0)
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub
End Class