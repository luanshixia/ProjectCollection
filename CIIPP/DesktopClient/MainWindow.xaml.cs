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

using System.Windows.Media.Animation;

using CIIPP;

namespace DesktopClient
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Current { get; private set; }
        public static LicenseState LicenseState { get; private set; }

        public static string ImagePath;

        public MainWindow()
        {
            DocumentManager.New();
            InitializeComponent();
            Skin();

            Current = this;
            //NavigationManager.Navigate("RichMainPage.xaml");
        }

        private void Skin()
        {
            try { if (OptionsManager.Options["Imagepath"] != "") { ImagePath = OptionsManager.Options["Imagepath"]; } }
            catch { ImagePath = "Resources/"; }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Back();
        }

        private void BtnForward_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Forward();
        }

        private void BtnHome_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Navigate("RichMainPage.xaml");
        }

        private void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            //NavigationManager.Navigate("TestPage.xaml");
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (LicenseState == LicenseState.Illegal)
            {
                return;
            }
            if (MessageBox.Show("请确认您已经保存对当前文档的更改。退出程序？", "确认", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            NavFrame.Refresh();
        }

        public void RefreshPage()
        {
            NavFrame.Refresh();
        }

        private void BtnHelp_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Navigate("TestPage.xaml");
        }

        private void BtnAbout_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Navigate("RichMainPage.xaml");
        }

        public void SetTitle()
        {
            string fileName = DocumentManager.CurrentDocument.FileName == string.Empty ? "New document" : DocumentManager.CurrentDocument.FileName;
            this.Title = string.Format("{0}{1} - {2}", DesktopClient.Properties.Resources.AppName, LicenseState == CIIPP.LicenseState.Trial ? "(试用版)" : string.Empty, fileName);
        }

        private void NavFrame_Navigated(object sender, NavigationEventArgs e)
        {
            var fe = NavFrame;
            ScaleTransform st = new ScaleTransform(10, 10, fe.ActualWidth / 2, fe.ActualHeight / 2);
            fe.RenderTransform = st;
            DoubleAnimation da1 = new DoubleAnimation(10, 1, new Duration(TimeSpan.Parse("0:0:0.5")));
            st.BeginAnimation(ScaleTransform.ScaleXProperty, da1);
            st.BeginAnimation(ScaleTransform.ScaleYProperty, da1);

            DoubleAnimation da2 = new DoubleAnimation(0, 1, new Duration(TimeSpan.Parse("0:0:0.5")));
            fe.BeginAnimation(FrameworkElement.OpacityProperty, da2);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            NavFrame.Navigated += NavFrame_Navigated;

            LicenseState = Security.CheckLicense(ActivationWindow.LicenseFile);
            if (LicenseState == LicenseState.Illegal)
            {
                MessageBox.Show("软件许可出现非法状态。程序将马上退出。", "软件许可", MessageBoxButton.OK, MessageBoxImage.Stop);
                App.Current.Shutdown();
            }
            else if (LicenseState == LicenseState.Trial)
            {
                ActivationWindow aw = new ActivationWindow { Owner = this };
                aw.ShowDialog();
            }

            this.SetTitle();
        }

        #region Fullscreen Mode

        private double _left;
        private double _top;
        private double _width;
        private double _height;
        private bool _isFullscreen;

        public bool IsFullscreen { get { return _isFullscreen; } }

        public void Fullscreen()
        {
            if (!_isFullscreen)
            {
                _left = this.Left;
                _top = this.Top;
                _width = this.ActualWidth;
                _height = this.ActualHeight;
                _isFullscreen = true;

                this.WindowState = System.Windows.WindowState.Normal;
                this.WindowStyle = System.Windows.WindowStyle.None;
                this.ResizeMode = System.Windows.ResizeMode.NoResize;
                //this.Topmost = true;
                this.Left = 0;
                this.Top = 0;
                this.Width = SystemParameters.PrimaryScreenWidth;
                this.Height = SystemParameters.PrimaryScreenHeight;
            }
        }

        public void ExitFullscreen()
        {
            _isFullscreen = false;

            this.WindowState = System.Windows.WindowState.Normal;
            this.WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
            this.ResizeMode = System.Windows.ResizeMode.CanResize;
            this.Topmost = false;
            this.Left = _left;
            this.Top = _top;
            this.Width = _width;
            this.Height = _height;
        }

        private bool _isUglyTheme = true;
        public bool IsUglyTheme { get { return _isUglyTheme; } }

        public void ToggleFullscreen()
        {
            if (ActivationWindow.CheckLicense() == false)
            {
                return;
            }

            if (!IsFullscreen)
            {
                Fullscreen();
            }
            else
            {
                ExitFullscreen();
            }
        }

        public string FullscreenToolTip
        {
            get
            {
                if (MainWindow.Current.IsFullscreen)
                {
                    return "退出全屏演示";
                }
                else
                {
                    return "全屏演示";
                }
            }
        }

        #endregion

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (_isFullscreen)
                {
                    this.ExitFullscreen();
                }
            }
            else if (e.Key == Key.B)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    NavigationManager.Back();
                }
            }
            else if (e.Key == Key.F)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    NavigationManager.Forward();
                }
            }
            else if (e.Key == Key.T)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    _isUglyTheme = !_isUglyTheme;
                    if (_isUglyTheme)
                    {
                        NavigationManager.Navigate("TileMainPage.xaml");
                    }
                    else
                    {
                        NavigationManager.Navigate("RichMainPage.xaml");
                    }
                }
            }
            else if (e.Key == Key.P)
            {
                if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    TryPrint();
                }
            }
            else if (e.Key == Key.C)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    MainWindow.ImagePath = "Resources/green/";
                    NavigationManager.Navigate("TileMainPage.xaml");
                }
            }
            else if (e.Key == Key.V)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    MainWindow.ImagePath = "Resources/";
                    NavigationManager.Navigate("TileMainPage.xaml");
                }
            }
            else if (e.Key == Key.S)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    Commands.SaveAs();
                }
            }
            else if (e.Key == Key.O)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    Commands.Open();
                }
            }
        }

        public void TryPrint()
        {
            if (ActivationWindow.CheckLicense() == false)
            {
                return;
            }

            if (NavFrame.CurrentSource.ToString().Contains("TileContentPage"))
            {
                TileContentPage.Current.PrintContent();
            }
            else
            {
                MessageBox.Show("当前没有可以打印的内容。", DesktopClient.Properties.Resources.AppName, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    public static class Commands
    {
        public static void New()
        {
            if (MessageBox.Show("请确认您已经保存对当前文档的更改。新建文档？", "确认", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                DocumentManager.New();
                MainWindow.Current.SetTitle();
                NavigationManager.Navigate("TileMainPage.xaml");
                TileMainPage.Current.FileBasedUpdate();
            }
        }

        public static void Open()
        {
            if (MessageBox.Show("请确认您已经保存对当前文档的更改。打开文档？", "确认", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                ofd.Filter = "CIIPP Document (*.ciipp)|*.ciipp|原版Excel (*.xls)|*.xls|数据交换版Excel (*.xlsx)|*.xlsx";
                if (ofd.ShowDialog() == true)
                {
                    if (ofd.FileName.Substring(ofd.FileName.LastIndexOf('.') + 1).ToLower() == "ciipp")
                    {
                        DocumentManager.Open(ofd.FileName);
                        MainWindow.Current.SetTitle();
                        NavigationManager.Navigate("TileMainPage.xaml");
                        TileMainPage.Current.FileBasedUpdate();
                    }
                    else if (ofd.FileName.Substring(ofd.FileName.LastIndexOf('.') + 1).ToLower() == "xlsx")
                    {
                        DocumentManager.New();
                        //LoadExcelApplication.LoadExcelTable1(ofd.FileName);
                        try
                        {
                            LoadExcelOpenXml.LoadExcelTable(ofd.FileName);
                            MessageBox.Show("导入成功！", "提示");
                        }
                        catch (Exception ex)
                        {
                            //MessageBox.Show("请先关闭此Excel文件,再进行导入操作！", "提示");
                            MessageBox.Show(ex.Message);
                        }

                    }
                    else if (ofd.FileName.Substring(ofd.FileName.LastIndexOf('.') + 1).ToLower() == "xls")
                    {
                        DocumentManager.New();
                        try
                        {
                            GetCiippExcel.GetCIIPPExcelTabel(ofd.FileName);
                            MessageBox.Show("导入成功！", "提示");
                        }
                        catch
                        {
                            MessageBox.Show("未安装Excel，无法进行导入！", "提示");
                        }
                    }
                }
            }
        }

        public static void Save()
        {
            if (ActivationWindow.CheckLicense() == false)
            {
                return;
            }

            if (DocumentManager.CurrentDocument.FileName == string.Empty)
            {
                SaveAs();
            }
            else
            {
                DocumentManager.CurrentDocument.Save();
                MessageBox.Show("保存成功。", "提示");
            }
        }

        public static void SaveAs()
        {
            if (ActivationWindow.CheckLicense() == false)
            {
                return;
            }

            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Filter = "CIIPP Document (*.ciipp)|*.ciipp|数据交换版Excel (*.xlsx)|*.xlsx";
            if (sfd.ShowDialog() == true)
            {
                if (sfd.FileName.Substring(sfd.FileName.LastIndexOf('.') + 1).ToLower() == "ciipp")
                {
                    DocumentManager.CurrentDocument.SaveAs(sfd.FileName);
                    MainWindow.Current.SetTitle();
                }
                else if (sfd.FileName.Substring(sfd.FileName.LastIndexOf('.') + 1).ToLower() == "xlsx")
                {
                    ExportExcel.ExportExcelT(sfd.FileName);
                    MessageBox.Show("已保存！", "提示");
                }
            }
        }
    }
}
