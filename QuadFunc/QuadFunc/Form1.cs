using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 二次函数
{
    public partial class Form1 : Form
    {
        private Fun function = new Fun();
        private string s;
        private bool ShowAxis;


        public Form1()
        {
            InitializeComponent();
            ShowAxis = true;
        }

        public void DrawGraph(Color clr)
        {
            Graphics paper = pictureBox1.CreateGraphics();
            Pen penpoint = new Pen(clr);
            double x, y,x1,y1;
            int px, py,px1,py1;
            for (px = 0; px <= pictureBox1.Width; px++)
            {                
                x = Tx(px);
                y = function.FunValue(x);
                py = Ty(y);
                px1 = px + 1;
                x1 = Tx(px1);
                y1 = function.FunValue(x1);
                py1= Ty(y1);
                paper.DrawLine(penpoint,px,py,px1,py1);       //实践证明DrawEllipse方法不行，成散点了。 
            }
            float a;
            a = 100+(float)function.axis*pictureBox1.Width/10;
            if (function.a != 0)
            {
                if (clr != Color.White&&ShowAxis==true)
                    paper.DrawLine(Pens.GreenYellow, a, 0, a, pictureBox1.Height);
                else
                    paper.DrawLine(Pens.White, a, 0, a, pictureBox1.Height);
            }
        }

        public double Tx(int px)
        {
            double x;
            x = ((double)px - 100) *10/ pictureBox1.Width;
            return x;
        }
        public int Ty(double y)
        {
            int py;
            py = (int)((5 - y) * pictureBox1.Height / 10);
            return py;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(Pens.LightGray, 0, 100, 200, 100);
            e.Graphics.DrawLine(Pens.LightGray, 100, 0, 100, 200);

            //System.Drawing.Drawing2D.GraphicsPath graphPath = new System.Drawing.Drawing2D.GraphicsPath();
            //graphPath.AddEllipse(0, 0, 200, 100);            
            //e.Graphics.DrawPath(Pens.Brown, graphPath);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2(textBox1, textBox2, textBox3);
            f2.ShowDialog();
            Int32.TryParse(textBox1.Text, out function.a);
            Int32.TryParse(textBox2.Text, out function.b);
            Int32.TryParse(textBox3.Text, out function.c);
            s = function.FunForm;
            //s = "y = "+(textBox1.Text=="1"?"":textBox1.Text)+"x^ + "+(textBox2.Text=="1"?"":textBox2.Text)+"x + "+textBox3.Text;
            //s=string.Format("{0} {1} {2}",a,b,c);
            label1.Text = s;
        }

        private void 退出QToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void 新建NToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2(textBox1, textBox2, textBox3);
            f2.ShowDialog();
            Int32.TryParse(textBox1.Text, out function.a);
            Int32.TryParse(textBox2.Text, out function.b);
            Int32.TryParse(textBox3.Text, out function.c);
            s = function.FunForm;
            label1.Text = s;
        }

        private void y0的根ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            function.Solve();
        }

        private void yk的根ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 f3 = new Form3(function.a, function.b, function.c);
            f3.ShowDialog();
        }

        private void 两点式ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (function.HaveRealRoot == false)
                MessageBox.Show("抛物线与 x 轴无交点。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                label1.Text = function.InterceptForm;
        }

        private void 顶点式ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label1.Text = function.PeakForm;
        }

        private void 一般式ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label1.Text = function.FunForm;
        }

        private void 数值跟踪ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;           
            double x=(double)numericUpDown1.Value;            
            label3.Text = "y = "+function.FunValue(x).ToString();
            if (function.IsThisSecondOrder == true)
            {
                double subx = (double)(-function.b / function.a) - x;
                label4.Text = "x' = " + subx.ToString();
            }
            else
                label4.Text = "";
            //trackBar1.Value = function.x1;
            //trackBar2.Value = function.x2;
            //trackBar3.Value = function.FunValue;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            double x = (double)numericUpDown1.Value;
            label3.Text = "y = " + function.FunValue(x).ToString();
            if (function.IsThisSecondOrder == true)
            {
                double subx = (double)(-function.b / function.a) - x;
                label4.Text = "x' = " + subx.ToString();
            }
            else
                label4.Text = "";
            trackBar1.Value = (int)numericUpDown1.Value;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            numericUpDown1.Value = trackBar1.Value;
        }

        private void 函数FToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.两点式ToolStripMenuItem.Enabled = function.IsThisSecondOrder;
            this.顶点式ToolStripMenuItem.Enabled = function.IsThisSecondOrder;
            this.顶点ToolStripMenuItem.Enabled = function.IsThisSecondOrder;
            this.对称轴ToolStripMenuItem.Enabled = function.IsThisSecondOrder;
            this.判别式ToolStripMenuItem.Enabled = function.IsThisSecondOrder;
            this.极值ToolStripMenuItem.Enabled = function.IsThisSecondOrder;
        }

        private void 范围最值ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form4 f4 = new Form4(function.a, function.b, function.c);
            f4.ShowDialog();
        }

        private void 判别式ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s;
            s = "Δ = "+function.delta.ToString();
            if (function.delta > 0)
                s += "，方程 y = 0 有两个不相等实根，抛物线与 x 轴交于两个点。";
            else if (function.delta == 0)
                s += "，方程 y = 0 有两个相等实根，抛物线与 x 轴切于一个点。";
            else
                s += "，方程 y = 0 有两个共轭虚根，抛物线与 x 轴没有公共点。";
            MessageBox.Show(s, "判别式", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void 已知三点求函数ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("这是一个三元线性方程组求解的问题，请使用\n”草根数学套件：线性方程组求解器“。", "提示");
        }

        private void 对称轴ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s;
            s = "x = " + function.axis.ToString();
            MessageBox.Show(s, "对称轴", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void 顶点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s;
            s = "( " + function.axis.ToString()+" , "+function.extremum.ToString()+" )";
            MessageBox.Show(s, "顶点", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void 极值ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s;
            s = (function.a>0?"极小值为 ":"极大值为 ") + function.extremum.ToString();
            MessageBox.Show(s, "极值", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void 绘制DToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
            DrawGraph(Color.Black);
        }

        private void 清除上次绘制LToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Graphics paper = pictureBox1.CreateGraphics();
            DrawGraph(Color.White);
            paper.DrawLine(Pens.LightGray, 0, 100, 200, 100);
            paper.DrawLine(Pens.LightGray, 100, 0, 100, 200);
        }

        private void 清除已绘制函数图象ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Graphics paper = pictureBox1.CreateGraphics();
            paper.Clear(Color.White);
            paper.DrawLine(Pens.LightGray, 0, 100, 200, 100);
            paper.DrawLine(Pens.LightGray, 100, 0, 100, 200);
        }

        private void 显示对称轴ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.显示对称轴ToolStripMenuItem.Checked=!(this.显示对称轴ToolStripMenuItem.Checked);
            ShowAxis=!ShowAxis;
        }

        private void 图形ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            
        }

        private void 关于AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"二次函数精灵
草根数学套件成员
版本 1.00.0425
2008王阳软件股份版权所有", "关于");
        }

        private void 帮助主题SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("help.txt");
            }
            catch
            {
                MessageBox.Show("系统找不到帮助文件。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
    public class Fun
    {
        public int a, b, c;
        public int delta
        {
            get
            {
                return b * b - 4 * a * c;
            }
            set
            {
                delta = value;
            }
        }
        public double x1
        {
            get
            {
                if (delta >= 0)
                    return (-b + System.Math.Sqrt(delta)) / (2 * a);
                else
                    return 0;
            }
            set
            {
                x1 = value;
            }
        }
        public double x2
        {
            get
            {
                if (delta >= 0)
                    return (-b - System.Math.Sqrt(delta)) / (2 * a);
                else
                    return 0;
            }
            set
            {
                x1 = value;
            }
        }
        public double re
        {
            get
            {
                return -(double)b / (2 * (double)a);
            }
        }
        public double im
        {
            get
            {
                return (System.Math.Sqrt(System.Math.Abs(delta))) / (2 * a);
            }
        }
        public string ix1
        {
            get
            {
                string s;
                s = "x1 = " + re.ToString();
                if (im > 0)
                    s += " + ";
                s += im.ToString()+" i";
                return s;
            }
        }
        public string ix2
        {
            get
            {
                string s;
                s = "x2 = " + re.ToString();
                if (im > 0)
                {
                    s += " - ";
                    s += im.ToString() + " i";
                }
                else
                {
                    s += " + ";
                    s += (-im).ToString()+" i";
                }
                return s;
            }
        }
        public double axis
        {
            get
            {
                return re;
            }
        }
        public double extremum
        {
            get
            {
                return -im * im;
            }
        }
        public string FunForm
        {
            get
            {
                string s;
                s = "y = ";
                if (a != 1 && a != 0 && a != -1)
                    s += a.ToString();
                if (a == -1)
                    s += " - ";
                if (a != 0)
                    s += "x^";
                if (b > 0 && a != 0 && !(b == 0 && c == 0))
                    s += " + ";
                if (b != 1 && b != 0 && b != -1)
                    s += b.ToString();
                if (b == -1)
                    s += " - ";
                if (b != 0)
                    s += "x";
                if (!(a == 0 && b == 0) && c > 0)
                    s += " + ";
                if (c != 0)
                    s += c.ToString();
                if (a ==0&& b ==0&& c == 0)
                    s += "0";
                return s;
            }
            set
            {
                FunForm = value;
            }
        }
        public string PeakForm
        {
            get
            {
                string s;
                s = "y = ";
                if (a != 1)
                    s += a.ToString();
                s += " ( x ";
                s += "+ " + (-re).ToString() + " )^ ";
                s += "+ " + extremum.ToString();
                return s;
            }
        }
        public string InterceptForm
        {
            get
            {
                string s;
                s = "y = ";
                if (a != 1)
                    s += a.ToString();
                s += " ( x + " + ((float)(-x1)).ToString() + " ) ( x + " + ((float)(-x2)).ToString() + " ) ";
                return s;
            }
        }
        public bool IsThisSecondOrder
        {
            get
            {
                if (a != 0)
                    return true;
                else
                    return false;
            }
        }
        public bool HaveRealRoot
        {
            get
            {
                if (delta >= 0)
                    return true;
                else
                    return false;
            }
        }
        public void RealRoot()
        {
            MessageBox.Show("x1 = "+x1.ToString()+"\nx2 = "+x2.ToString()+(delta==0?"\n两实根相等。":""),"实根",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }
        public void ImRoot()
        {
            MessageBox.Show(ix1 + "\n" + ix2, "虚根", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public void OnlyRoot()
        {
            if (b == 0)
                MessageBox.Show("方程无解，请保证二次项系数与一次项系数至少有一个非零！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show("一元一次方程，根为 " + (-(double)c / (double)b).ToString() + "。", "独根", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public void Solve()
        {
            if (a == 0)
                OnlyRoot();
            else if (delta >= 0)
                RealRoot();
            else
                ImRoot();
        }
        public void SolveK()
        {
            
        }
        public double FunValue(double x)
        {
            double y;
            y = (double)a * x * x + (double)b * x + (double)c;
            return y;
        }
        public void MaxMin(double m, double n)
        {
            double max, min, left, right, middle;
            left = FunValue(m);
            right = FunValue(n);
            middle = FunValue(axis);
            if (a == 0 && b > 0 || a > 0 && m > axis || a < 0 && n < axis)
            {
                max = right;
                min = left;
            }
            else if (a == 0 && b < 0 || a < 0 && m > axis || a > 0 && n < axis)
            {
                max = left;
                min = right;
            }
            else if (a > 0)
            {
                min = middle;
                max = left > right ? left : right;
            }
            else if (a < 0)
            {
                max = middle;
                min = left < right ? left : right;
            }
            else
            {
                max = c;
                min = c;
            }
            MessageBox.Show("闭区间 [ "+m.ToString()+" , "+n.ToString()+" ] 上的最大值是 "+max.ToString()+"，最小值是 "+min.ToString()+"。", "最值", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


    }
}
