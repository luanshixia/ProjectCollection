using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TongJi.City
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
            timer2.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();

            this.Hide();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Stop();

            Viewer vf = new Viewer();
            vf.ShowDialog();
            this.Close();
        }
    }
}
