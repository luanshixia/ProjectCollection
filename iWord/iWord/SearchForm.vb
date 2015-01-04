Public Class SearchForm
    Public fset As DataSet
    Public grid As DataGridView

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim s = TextBox1.Text.Trim
        Dim q = From i In xword.Element("Word").Elements _
                Where i.Attribute("w").Value.IndexOf(s) <> -1 Or i.Attribute("m").Value.IndexOf(s) <> -1 _
                Select i
        Dim dt As New DataTable("SelectedData")
        dt.Columns.Add(New DataColumn("ID", GetType(Int32)))
        dt.Columns.Add(New DataColumn("单词", GetType(String)))
        dt.Columns.Add(New DataColumn("词义", GetType(String)))
        For Each i In q
            Dim row As DataRow = dt.NewRow
            row(0) = i.Attribute("i").Value
            row(1) = i.Attribute("w").Value
            row(2) = i.Attribute("m").Value
            dt.Rows.Add(row.ItemArray)
        Next
        fset.Tables.Clear()
        fset.Tables.Add(dt)
        grid.DataSource = fset.Tables(0)
        Me.Label1.Text = String.Format("已找到 {0} 个单词", q.Count)
    End Sub

    Private Sub TextBox1_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles TextBox1.MouseClick
        If TextBox1.Text = "输入中英文" Then
            TextBox1.Text = ""
            TextBox1.ForeColor = System.Drawing.SystemColors.WindowText
            Dim ft = New Font("Tahoma", 9)
            TextBox1.Font = ft
        End If
    End Sub
End Class