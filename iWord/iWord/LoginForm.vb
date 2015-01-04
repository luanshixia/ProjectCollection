Public Class LoginForm

    Private cl As Boolean
    Public ex As Boolean

    Private Sub LoginForm_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If cl = False Then
            e.Cancel = True
        End If
    End Sub

    Private Sub LoginForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim qname
        If IO.File.Exists("Process.xml") = False Then
            qname = ""
        Else
            xprocess = XDocument.Load("Process.xml")
            qname = xprocess.Element("Process").Element("user").Attribute("username").Value.Trim
        End If
        TextBox1.Text = qname
        Button1.Enabled = TextBox1.Text <> ""
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        If TextBox1.Text = "" Then
            Dim f As New RegForm
            f.login = Me
            f.ShowDialog()
        Else
            Dim q = xprocess.Element("Process").Element("user").Attribute("password").Value.Trim
            If q = TextBox2.Text Then
                Dim f As New RegForm
                f.login = Me
                f.ShowDialog()
            Else
                MessageBox.Show("密码错误！为了保证他人学习数据的安全，进入系统或新建进程都必须键入正确的密码。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
        End If
        TextBox2.Clear()
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim q = xprocess.Element("Process").Element("user").Attribute("password").Value.Trim
        If q = TextBox2.Text Then
            cl = True
            Me.Close()
        Else
            MessageBox.Show("密码错误！为了保证他人学习数据的安全，进入系统或新建进程都必须键入正确的密码。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        cl = True
        ex = True
        Me.Close()
    End Sub

    Private Sub Button1_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.MouseEnter, Button2.MouseEnter, Button3.MouseEnter
        sender.backgroundimage = My.Resources.over '用参数sender编程，不同按钮一同处理
    End Sub

    Private Sub Button1_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.MouseLeave, Button2.MouseLeave, Button3.MouseLeave
        sender.backgroundimage = My.Resources.normal
    End Sub
End Class