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
    public partial class Form2 : Form
    {
        //private int a, b, c;
        private TextBox temp1 = new TextBox();
        private TextBox temp2 = new TextBox();
        private TextBox temp3 = new TextBox();
        public Form2(TextBox box1,TextBox box2,TextBox box3)
        {
            InitializeComponent();
            textBox1.Text = box1.Text;
            textBox2.Text = box2.Text;
            textBox3.Text = box3.Text;
            this.temp1.Text = box1.Text;
            this.temp1 = box1;
            this.temp2.Text = box2.Text;
            this.temp2 = box2;
            this.temp3.Text = box3.Text;
            this.temp3 = box3;
        }
        //private int refer(ref int m)
        //{
        //    int n=m;
        //    return n;
        //}

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.temp1.Text = textBox1.Text;
            this.temp2.Text = textBox2.Text;
            this.temp3.Text = textBox3.Text;
            //Int32.TryParse(textBox1.Text, out a);
            //Int32.TryParse(textBox2.Text, out b);
            //Int32.TryParse(textBox3.Text, out c);
            this.Close();
        }

    }
}
