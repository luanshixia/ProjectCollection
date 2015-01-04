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
    public partial class Form4 : Form
    {
        private int a, b, c;
        public Form4(int x,int y,int z)
        {
            InitializeComponent();
            a = x;
            b = y;
            c = z;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Fun f = new Fun();
            f.a = a;
            f.b = b;
            f.c = c;
            double m, n;
            Double.TryParse(textBox1.Text, out m);
            Double.TryParse(textBox2.Text, out n);
            if(m<n)
                f.MaxMin(m,n);
            else
                MessageBox.Show("请输入有效区间！","警告",MessageBoxButtons.OK,MessageBoxIcon.Warning);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
