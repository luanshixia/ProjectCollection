using System.Windows;

namespace Dreambuild.Gis.Display
{
    /// <summary>
    /// InputBox.xaml 的交互逻辑
    /// </summary>
    public partial class InputBox : Window
    {
        public string Value { get { return txtValue.Text; } }

        public InputBox()
        {
            InitializeComponent();
        }

        public InputBox(string defaultValue)
        {
            InitializeComponent();

            txtValue.Text = defaultValue;
        }

        public InputBox(string tip, string defaultValue)
        {
            InitializeComponent();

            this.Title = tip;
            txtValue.Text = defaultValue;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
