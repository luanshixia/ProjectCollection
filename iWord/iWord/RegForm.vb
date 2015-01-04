Public Class RegForm

    Public login As LoginForm

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        If TextBox2.Text <> TextBox3.Text Then
            MessageBox.Show("密码不一致！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        ElseIf TextBox1.Text = "" Or TextBox2.Text = "" Then
            MessageBox.Show("用户名或密码不能为空！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Else
            If MessageBox.Show("将要覆盖现有的用户，继续吗？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = Windows.Forms.DialogResult.Yes Then
                xprocess = New XDocument
                xprocess.Add(New XElement("Process"))
                If xprocess.Element("Process").Element("user") Is Nothing Then
                    xprocess.Element("Process").Add(New XElement("user"))
                    xprocess.Element("Process").Element("user").Add(New XAttribute("username", TextBox1.Text.Trim))
                    xprocess.Element("Process").Element("user").Add(New XAttribute("password", TextBox2.Text.Trim))
                    xprocess.Element("Process").Element("user").Add(New XAttribute("level", ""))
                End If
                xprocess.Save("Process.xml")
                login.Button1.Enabled = True
                login.TextBox1.Text = Me.TextBox1.Text
                Me.Close()
                Dim wf As New Wizard
                wf.btnCancel.Enabled = False
                wf.ShowDialog()
            End If
        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Me.Close()
    End Sub

    Private Sub Button1_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.MouseEnter, Button2.MouseEnter
        sender.backgroundimage = My.Resources.over '用参数sender编程，不同按钮一同处理
    End Sub

    Private Sub Button1_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.MouseLeave, Button2.MouseLeave
        sender.backgroundimage = My.Resources.normal
    End Sub
End Class