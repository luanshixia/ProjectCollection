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

namespace DesktopClient
{
    /// <summary>
    /// NavPoints.xaml 的交互逻辑
    /// </summary>
    public partial class NavPoints : UserControl
    {
        public int PointCount { get; set; }

        // newly 20120307
        public List<FrameworkElement> CustomPoints { get; set; }
        public List<string> PointDescripions { get; set; }

        protected int _currentPoint;
        public int CurrentPoint
        {
            get
            {
                return _currentPoint;
            }
            set
            {
                _currentPoint = value;
                OnCurrentPointChanged(new EventArgs());
            }
        }

        public double PointSize { get; set; }

        public event EventHandler CurrentPointChanged;

        public virtual void OnCurrentPointChanged(EventArgs e)
        {
            ReadyControl();

            if (CurrentPointChanged != null)
            {
                CurrentPointChanged(this, e);
            }
        }

        public NavPoints()
        {
            InitializeComponent();

            PointSize = 20;

            Enumerable.Range(0, 5).ToList().ForEach(x =>
            {
                LayoutRoot.Children.Add(RegularPoint(x));
            });
        }

        protected void ReadyControl()
        {
            //PointCount = pointCount;
            LayoutRoot.Children.Clear();
            if (PointCount < 2)
            {
                return;
            }
            Enumerable.Range(0, PointCount).ToList().ForEach(x =>
            {
                FrameworkElement point;
                if (CustomPoints != null && x < CustomPoints.Count)
                {
                    point = CustomPoints[x];
                }
                else
                {
                    if (x == CurrentPoint)
                    {
                        point = RegularPointHightlighted(x);
                    }
                    else
                    {
                        point = RegularPoint(x);
                    }
                }
                if (PointDescripions != null && x < PointDescripions.Count)
                {
                    point.ToolTip = PointDescripions[x];
                }
                point.MouseLeftButtonUp += (sender, e) => CurrentPoint = x;
                point.MouseEnter += (sender, e) => Cursor = Cursors.Hand;
                point.MouseLeave += (sender, e) => Cursor = Cursors.Arrow;
                LayoutRoot.Children.Add(point);
            });
        }

        protected Brush GrayBrush()
        {
            return new RadialGradientBrush(Colors.Silver, Colors.Gray) { Center = new Point(0.5, 1), RadiusX = 1, RadiusY = 1 };
        }

        protected Brush HightlightBrush()
        {
            return new RadialGradientBrush(Colors.Gold, Colors.Peru) { Center = new Point(0.5, 0), RadiusX = 1, RadiusY = 1 };
        }

        public FrameworkElement RegularPoint(int number)
        {
            var grid = new Grid { Width = PointSize, Height = PointSize, Margin = new Thickness(PointSize / 2, PointSize * 2, PointSize / 2, PointSize * 2) };
            var ellipse = new Ellipse { Width = PointSize, Height = PointSize, Fill = GrayBrush() };
            var tb = new TextBlock { Text = (number + 1).ToString(), Foreground = Brushes.Black, HorizontalAlignment = System.Windows.HorizontalAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center };
            grid.Children.Add(ellipse);
            grid.Children.Add(tb);
            return grid;
        }

        public FrameworkElement RegularPointHightlighted(int number)
        {
            var grid = new Grid { Width = PointSize, Height = PointSize, Margin = new Thickness(PointSize / 2, PointSize * 2, PointSize / 2, PointSize * 2) };
            var ellipse = new Ellipse { Width = PointSize, Height = PointSize, Fill = HightlightBrush() };
            grid.Children.Add(ellipse);
            return grid;
        }
    }
}
