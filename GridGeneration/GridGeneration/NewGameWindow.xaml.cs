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
using System.Windows.Shapes;

namespace GridGeneration
{
    /// <summary>
    /// NewGameWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NewGameWindow : Window
    {
        public int ColCount
        {
            get
            {
                int n = 10;
                int.TryParse(NCol.Text, out n);
                return n > 0 ? n : 10;
            }
        }

        public int RowCount
        {
            get
            {
                int n = 10;
                int.TryParse(NRow.Text, out n);
                return n > 0 ? n : 10;
            }
        }

        public NewGameWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
