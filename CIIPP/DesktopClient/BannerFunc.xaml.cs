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

using CIIPP;

namespace DesktopClient
{
    /// <summary>
    /// BannerFunc.xaml 的交互逻辑
    /// </summary>
    public partial class BannerFunc : UserControl
    {
        public BannerFunc()
        {
            InitializeComponent();
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            Banner.Current.CloseAllPopups();
            if (MessageBox.Show("请确认您已经保存对当前文档的更改。新建文档？", "确认", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                DocumentManager.New();
                MainWindow.Current.SetTitle();
                NavigationManager.Navigate("RichMainPage.xaml");
                //MainPage.Current.FileBasedUpdate();
                RichMainPage.Current.FileBasedUpdate();
            }
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            Banner.Current.CloseAllPopups();
            if (MessageBox.Show("请确认您已经保存对当前文档的更改。打开文档？", "确认", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                ofd.Filter = "CIIPP Document (*.ciipp)|*.ciipp";
                if (ofd.ShowDialog() == true)
                {
                    DocumentManager.Open(ofd.FileName);
                    MainWindow.Current.SetTitle();
                    NavigationManager.Navigate("RichMainPage.xaml");
                    //MainPage.Current.FileBasedUpdate();
                    RichMainPage.Current.FileBasedUpdate();
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Banner.Current.CloseAllPopups();
            if (DocumentManager.CurrentDocument.FileName == string.Empty)
            {
                btnSaveAs_Click(sender, e);
            }
            else
            {
                DocumentManager.CurrentDocument.Save();
                MessageBox.Show("保存成功。", "提示");
            }
        }

        private void btnSaveAs_Click(object sender, RoutedEventArgs e)
        {
            Banner.Current.CloseAllPopups();
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Filter = "CIIPP Document (*.ciipp)|*.ciipp";
            if (sfd.ShowDialog() == true)
            {
                DocumentManager.CurrentDocument.SaveAs(sfd.FileName);
                MainWindow.Current.SetTitle();
            }
        }

        private void btnNewProj_Click(object sender, RoutedEventArgs e)
        {
            Banner.Current.CloseAllPopups();
            NewProj np = new NewProj { Owner = MainWindow.Current };
            if (np.ShowDialog() == true)
            {
                DocumentManager.CurrentDocument.Projects.Add(new ProjectStatistics(np.txtProjName.Text, np.txtProjLocation.Text));
                //RichMainPage.Current.UpdateProjList();
            }
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            Banner.Current.CloseAllPopups();
            NavigationManager.Navigate("TestPage.xaml");
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            Banner.Current.CloseAllPopups();
        }

        private void btnFullscreen_Click(object sender, RoutedEventArgs e)
        {
            Banner.Current.CloseAllPopups();
            MainWindow.Current.Fullscreen();
        }
    }
}
