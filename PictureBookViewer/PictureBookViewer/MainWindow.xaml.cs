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

using PictureBookViewer.Code;

namespace PictureBookViewer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private PageManager _pm;
        private string _lastPath;
        public static MainWindow Current { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            Current = this;
        }

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.SelectedPath = _lastPath;
            fbd.Description = "Choose the folder that contains your eBook.";
            fbd.ShowNewFolderButton = false;
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _pm = new PageManager(fbd.SelectedPath);
                _lastPath = fbd.SelectedPath;
                DisplayArea.Content = _pm.GetContinuousLayout();
                DisplayArea.ScrollToTop();
            }
            Enumerable.Range(1, _pm.PictureFiles.Count).ToList().ForEach(i =>
            {
                PageCombo.Items.Add(i);
            });
            PageCombo.DropDownClosed += PageCombo_SelectionChanged;
            TotalPageLabel.Content = string.Format("(of {0})", _pm.PictureFiles.Count);
        }

        void PageCombo_SelectionChanged(object sender, EventArgs e)
        {
            if (PageCombo.SelectedItem != null)
            {
                _pm.GoToPage((int)PageCombo.SelectedItem);
            }
        }

        private void DisplayArea_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (_pm != null)
            {
                //Point viewportCenter = new Point(150, DisplayArea.VerticalOffset + DisplayArea.ActualHeight / 2);
                _pm.OnPageDisplay(DisplayArea.VerticalOffset);
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            _pm = null;
            DisplayArea.Content = null;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void ActualSize_Click(object sender, RoutedEventArgs e)
        {
            _pm.SetDisplayMode(PageDisplayMode.ActualSize);
        }

        private void FitPage_Click(object sender, RoutedEventArgs e)
        {
            _pm.SetDisplayMode(PageDisplayMode.FitPage);
        }

        private void FitWidth_Click(object sender, RoutedEventArgs e)
        {
            _pm.SetDisplayMode(PageDisplayMode.FitWidth);
        }

        private void FitHeight_Click(object sender, RoutedEventArgs e)
        {
            _pm.SetDisplayMode(PageDisplayMode.FitHeight);
        }

        private void DisplayArea_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_pm != null)
            {
                _pm.OnResize();
            }
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            _pm.ZoomIn();
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            _pm.ZoomOut();
        }

        private void ZoomTo_Click(object sender, RoutedEventArgs e)
        {
            ZoomToWindow ztw = new ZoomToWindow();
            if (ztw.ShowDialog() == true)
            {
                _pm.Zoom(ztw.Magnification);
            }
        }

        private void FirstPage_Click(object sender, RoutedEventArgs e)
        {
            _pm.GoToPage(1);
        }

        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            _pm.GoToPage(_pm.CurrentPage - 1);
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            _pm.GoToPage(_pm.CurrentPage + 1);
        }

        private void LastPage_Click(object sender, RoutedEventArgs e)
        {
            _pm.GoToPage(_pm.PictureFiles.Count);
        }

        private void GoToPage_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
