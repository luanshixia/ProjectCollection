using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DesktopClient
{
    /// <summary>
    /// QuestionSummary.xaml 的交互逻辑
    /// </summary>
    public partial class QuestionSummary : UserControl
    {
        public QuestionSummary()
        {
            InitializeComponent();
        }

        public string Points
        {
            get
            {
                return txtPoints.Text;
            }
            set
            {
                txtPoints.Text = value;
            }
        }

        public void Update(double max, double value)
        {
            //progressBar1.Maximum = max;
            //progressBar1.Foreground = FindResource("green_banner") as LinearGradientBrush;
            //progressBar1.Value = value;

            //StarBar starbar1 = new StarBar();
            starbar1.SetStar(value);

            txtPoints.Text = value.ToString("0.#");
        }
    }
}
