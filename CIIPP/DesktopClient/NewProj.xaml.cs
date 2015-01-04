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

namespace DesktopClient
{
    /// <summary>
    /// NewProj.xaml 的交互逻辑
    /// </summary>
    public partial class NewProj : Window
    {
        public NewProj()
        {
            InitializeComponent();
            SetBackgroundImage();
            imageProjId.SelectedIndex = 0;
        }

        private void SetBackgroundImage()
        {
            int icount = ProjIconManager.Icons.Count;
            if (icount != 0)
            {
                var images = new FrameworkElement();
                for (int i = 0; i < icount; i++)
                {
                    string imagepath = ProjIconManager.Icons[i];
                    string imagename = ProjIconManager.GetName(i);
                    images = ComboBoxItems(imagepath, imagename);
                    imageProjId.Items.Add(images);
                }
            }
            else
            {
                imageProjId.Text = "没有可以选择的图片";
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (txtProjLocation.Text.Length > 0 && txtProjName.Text.Length > 0)
            {
                DialogResult = true;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void cb1_Click(object sender, RoutedEventArgs e)
        {
            UpdateOkButton();
        }

        private void cb2_Click(object sender, RoutedEventArgs e)
        {
            UpdateOkButton();
        }

        private void UpdateOkButton()
        {
            if (cb1.IsChecked == true && cb2.IsChecked == true)
            {
                btnOk.IsEnabled = true;
            }
            else
            {
                btnOk.IsEnabled = false;
            }
        }

        public FrameworkElement ComboBoxItems(string imgspath, string name)
        {
            var cbi = new StackPanel { Orientation = Orientation.Horizontal };
            var img = new Image { Source = new BitmapImage(new Uri(imgspath)), Height = 60, Width = 80, Margin = new Thickness(2) };
            var txtname = new TextBlock { Text = name, FontSize = 14, VerticalAlignment = System.Windows.VerticalAlignment.Center };
            cbi.Children.Add(img);
            cbi.Children.Add(txtname);
            return cbi;
        }
    }
}
