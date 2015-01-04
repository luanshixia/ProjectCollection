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
    public partial class Form3 : Form
    {
        private int a, b, c;
        public Form3(int x,int y,int z)
        {
            InitializeComponent();
            a = x;
            b = y;
            c = z;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Fun f = new Fun();
            int d=(int)numericUpDown1.Value;
            f.a = a;
            f.b = b;
            f.c = c - d;
            f.Solve();
        }

        private void numericUpDown1_Enter(object sender, EventArgs e)
        {
            numericUpDown1.Select(0,numericUpDown1.Value.ToString().Length);
        }
    }
}
