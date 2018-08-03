using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PictureBookViewer.Code
{
    public class PageManager
    {
        public string FolderPath { get; private set; }
        public List<string> PictureFiles { get; private set; }
        public PageDisplayMode DisplayMode { get; private set; }
        public double ZoomScale { get; private set; }
        public int CurrentPage { get; private set; }

        private List<Image> _pageImages;
        private List<double> _pageHeights;
        private List<double> _pageWidths;
        private StackPanel _sp;

        //private const double DefaultPageWidth = 300;
        //private const double DefaultPageHeight = 300;

        public PageManager(string folderPath)
        {
            this.FolderPath = folderPath;
            this.PictureFiles = Directory
                .GetFiles(folderPath)
                .Where(file => PageManager.IsPicture(file))
                .ToList();

            _pageImages = new List<Image>();
            _pageHeights = new List<double>(); // PictureFiles.Select(x => DefaultPageHeight).ToList();
            _pageWidths = new List<double>(); // PictureFiles.Select(x => DefaultPageWidth).ToList();
            this.ZoomScale = 1;
            this.CurrentPage = 1;
        }

        private static bool IsPicture(string file)
        {
            string[] extensions = { ".jpg", ".png", "gif", "bmp", ".tif" };
            return extensions.Any(extension => file.ToLower().EndsWith(extension));
        }

        public FrameworkElement GetContinuousLayout()
        {
            _sp = new StackPanel();
            _pageImages.Clear();
            _pageWidths.Clear();
            _pageHeights.Clear();
            this.PictureFiles.ForEach(file =>
            {
                var frame = BitmapFrame.Create(
                    bitmapUri: new Uri(file, UriKind.Absolute),
                    createOptions: BitmapCreateOptions.DelayCreation,
                    cacheOption: BitmapCacheOption.None);

                var image = new Image
                {
                    Width = frame.PixelWidth,
                    Height = frame.PixelHeight,
                    Stretch = Stretch.None,
                    Tag = file
                };

                //Image image = new Image { Width = DefaultPageWidth, Height = DefaultPageHeight, Stretch = Stretch.None, Tag = f };
                _pageImages.Add(image);
                _pageWidths.Add(image.Width);
                _pageHeights.Add(image.Height);
                _sp.Children.Add(image);
            });
            return _sp;
        }

        public void OnPageDisplay(double offset)
        {
            int index = Enumerable
                .Range(0, _pageImages.Count)
                .First(i => _pageHeights.Take(i + 1).Sum() * ZoomScale > offset) + 1;

            for (int i = 0; i < _pageImages.Count; i++)
            {
                if (Math.Abs(index - i) < 4)
                {
                    this.LoadBitmap(i);
                }
                else
                {
                    this.UnloadBitmap(i);
                }
            }

            this.CurrentPage = index; // Save page position after scroll
            MainWindow.Current.PageCombo.SelectedItem = index;
        }

        //public void OnPageDisplay(Point viewportCenter)
        //{
        //    VisualTreeHelper.HitTest(
        //        _sp,
        //        PageFilter,
        //        PageDisplayCallback,
        //        new PointHitTestParameters(viewportCenter)
        //    );
        //}

        //private HitTestFilterBehavior PageFilter(DependencyObject o)
        //{
        //    if (o is Image)
        //    {
        //        return HitTestFilterBehavior.Continue;
        //    }
        //    else
        //    {
        //        return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
        //    }
        //}

        //private HitTestResultBehavior PageDisplayCallback(HitTestResult result)
        //{
        //    var image = result.VisualHit as Image;
        //    if (image == null)
        //    {
        //        return HitTestResultBehavior.Continue;
        //    }
        //    int index = _pageImages.IndexOf(image);
        //    for (int i = 0; i < _pageImages.Count; i++)
        //    {
        //        if (Math.Abs(index - i) < 4)
        //        {
        //            LoadBitmap(i);
        //        }
        //        else
        //        {
        //            UnloadBitmap(i);
        //        }
        //    }
        //    return HitTestResultBehavior.Stop;
        //}

        private void LoadBitmap(int i)
        {
            var image = _pageImages[i];
            if (image.Source == null)
            {
                var bitmap = new BitmapImage(new Uri(image.Tag.ToString(), UriKind.Absolute));
                image.Width = bitmap.PixelWidth;
                image.Height = bitmap.PixelHeight;
                image.Source = bitmap;
                _pageHeights[i] = image.Height;
                _pageWidths[i] = image.Width;
            }
        }

        private void UnloadBitmap(int i)
        {
            var image = _pageImages[i];
            image.Source = null;
        }

        public void SetDisplayMode(PageDisplayMode mode)
        {
            this.DisplayMode = mode;
            this.OnResize();
        }

        public void OnResize()
        {
            double scale = 1;
            if (DisplayMode.HasFlag(PageDisplayMode.ActualSize))
            {
                scale = 1;
            }
            else if (DisplayMode.HasFlag(PageDisplayMode.FitPage))
            {
                scale = Math.Min(MainWindow.Current.DisplayArea.ActualWidth / _pageWidths.Max(), MainWindow.Current.DisplayArea.ActualHeight / _pageHeights.Max());
            }
            else if (DisplayMode.HasFlag(PageDisplayMode.FitWidth))
            {
                scale = MainWindow.Current.DisplayArea.ActualWidth / _pageWidths.Max();
            }
            else if (DisplayMode.HasFlag(PageDisplayMode.FitHeight))
            {
                scale = MainWindow.Current.DisplayArea.ActualHeight / _pageHeights.Max();
            }

            this.Zoom(scale);
        }

        public void Zoom(double scale)
        {
            _sp.LayoutTransform = new ScaleTransform(scale, scale);
            double deltaScale = scale / ZoomScale;
            this.ZoomScale = scale;
            MainWindow.Current.DisplayArea.ScrollToVerticalOffset(deltaScale * MainWindow.Current.DisplayArea.VerticalOffset); // Ensure same position after zoom
        }

        public void ZoomIn()
        {
            double[] scales = { 0.1, 0.25, 0.5, 0.75, 1.0, 1.25, 1.5, 2.0, 4.0, 8.0, 16.0 };
            double scale = scales.Any(x => x > ZoomScale) ? scales.First(x => x > ZoomScale) : scales.Max();
            this.Zoom(scale);
        }

        public void ZoomOut()
        {
            double[] scales = { 0.1, 0.25, 0.5, 0.75, 1.0, 1.25, 1.5, 2.0, 4.0, 8.0, 16.0 };
            double scale = scales.Any(x => x < ZoomScale) ? scales.Last(x => x < ZoomScale) : scales.Min();
            this.Zoom(scale);
        }

        public void GoToPage(int page)
        {
            if (page < 1)
            {
                page = 1;
            }
            else if (page > PictureFiles.Count)
            {
                page = PictureFiles.Count;
            }

            double offset = _pageHeights.Take(page - 1).Sum() * ZoomScale;
            MainWindow.Current.DisplayArea.ScrollToVerticalOffset(offset);
            this.CurrentPage = page;
            MainWindow.Current.PageCombo.SelectedItem = page;
        }
    }

    [Flags]
    public enum PageDisplayMode
    {
        Unknown = 0x00,
        ActualSize = 0x01,
        FitPage = 0x02,
        FitWidth = 0x04,
        FitHeight = 0x08,
        SinglePage = 0x10,
        SingleContinuous = 0x20,
        TwoUp = 0x40,
        TwoUpContinuous = 0x80
    }
}
