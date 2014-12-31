using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dreambuild.Games.BraveBoom
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private BoomMachine _machine = new BoomMachine();

        public MainWindow()
        {
            InitializeComponent();
            btn1.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.btn1_MouseLeftButtonDown), true);
            btn1.AddHandler(Button.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.btn1_MouseLeftButtonUp), true);

            _machine.Updated += Update;
        }

        private void btn1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _machine.Start();
        }

        private void btn1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _machine.Stop();
        }

        private void btn2_Click(object sender, RoutedEventArgs e)
        {
            _machine.Reset();
            Update();
        }

        private void Update()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                txt1.Text = _machine.MaxValue.ToString();
                txt2.Text = _machine.BoomValue.ToString();
                txt3.Text = _machine.Count.ToString();
            }));            
        }
    }
}
