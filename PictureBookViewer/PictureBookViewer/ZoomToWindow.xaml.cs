using System.Windows;

namespace PictureBookViewer
{
    /// <summary>
    /// ZoomToWindow.xaml code behind.
    /// </summary>
    public partial class ZoomToWindow : Window
    {
        public double Magnification
        {
            get
            {
                string source = this.InputTextBox.Text;
                if (source.EndsWith("%"))
                {
                    source = source.Remove(source.Length - 1);
                    source = source.Insert(source.Length - 2, ".");
                }

                if (double.TryParse(source, out double mag))
                {
                    return mag;
                }

                return 1;
            }
        }

        public ZoomToWindow()
        {
            this.InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
