using PictureBookViewer.Code;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace PictureBookViewer
{
    /// <summary>
    /// MainWindow.xaml code behind.
    /// </summary>
    public partial class MainWindow : Window
    {
        private PageManager PageManager;
        private string LastPath;

        public static MainWindow Current { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            MainWindow.Current = this;
        }

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new FolderBrowserDialog
            {
                SelectedPath = this.LastPath,
                Description = "Choose a folder that contains your eBook.",
                ShowNewFolderButton = false
            };

            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.PageManager = new PageManager(folderDialog.SelectedPath);
                this.LastPath = folderDialog.SelectedPath;
                this.DisplayArea.Content = this.PageManager.GetContinuousLayout();
                this.DisplayArea.ScrollToTop();
            }

            Enumerable.Range(1, this.PageManager.PictureFiles.Count).ToList().ForEach(i =>
            {
                this.PageCombo.Items.Add(i);
            });

            this.PageCombo.DropDownClosed += this.PageCombo_SelectionChanged;
            this.TotalPageLabel.Content = $"(of {this.PageManager.PictureFiles.Count})";
        }

        void PageCombo_SelectionChanged(object sender, EventArgs e)
        {
            if (PageCombo.SelectedItem != null)
            {
                this.PageManager.GoToPage((int)PageCombo.SelectedItem);
            }
        }

        private void DisplayArea_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (this.PageManager != null)
            {
                //Point viewportCenter = new Point(150, DisplayArea.VerticalOffset + DisplayArea.ActualHeight / 2);
                this.PageManager.OnPageDisplay(this.DisplayArea.VerticalOffset);
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.PageManager = null;
            this.DisplayArea.Content = null;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void ActualSize_Click(object sender, RoutedEventArgs e)
        {
            this.PageManager.SetDisplayMode(PageDisplayMode.ActualSize);
        }

        private void FitPage_Click(object sender, RoutedEventArgs e)
        {
            this.PageManager.SetDisplayMode(PageDisplayMode.FitPage);
        }

        private void FitWidth_Click(object sender, RoutedEventArgs e)
        {
            this.PageManager.SetDisplayMode(PageDisplayMode.FitWidth);
        }

        private void FitHeight_Click(object sender, RoutedEventArgs e)
        {
            this.PageManager.SetDisplayMode(PageDisplayMode.FitHeight);
        }

        private void DisplayArea_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.PageManager != null)
            {
                this.PageManager.OnResize();
            }
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            this.PageManager.ZoomIn();
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            this.PageManager.ZoomOut();
        }

        private void ZoomTo_Click(object sender, RoutedEventArgs e)
        {
            var ztw = new ZoomToWindow();
            if (ztw.ShowDialog() == true)
            {
                this.PageManager.Zoom(ztw.Magnification);
            }
        }

        private void FirstPage_Click(object sender, RoutedEventArgs e)
        {
            this.PageManager.GoToPage(1);
        }

        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            this.PageManager.GoToPage(this.PageManager.CurrentPage - 1);
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            this.PageManager.GoToPage(this.PageManager.CurrentPage + 1);
        }

        private void LastPage_Click(object sender, RoutedEventArgs e)
        {
            this.PageManager.GoToPage(this.PageManager.PictureFiles.Count);
        }

        private void GoToPage_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
