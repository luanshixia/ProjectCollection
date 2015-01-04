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

namespace DesktopClient
{
    /// <summary>
    /// PagerPresenter.xaml 的交互逻辑
    /// </summary>
    public partial class PagerPresenter : SlidePresenter
    {
        public double ColWidth { get; set; }
        public double ControlWidth { get; set; }
        protected Grid _panel;

        public Dock PointsLocation
        {
            get
            {
                return DockPanel.GetDock(NavBarContainer);
            }
            set
            {
                if (value == Dock.Top || value == Dock.Bottom)
                {
                    DockPanel.SetDock(NavBarContainer, value);
                }
            }
        }

        public PagerPresenter()
        {
            InitializeComponent();

            Slides = new List<FrameworkElement>();
            ColWidth = 964;
            ControlWidth = 964;
        }

        public override void OnSlideChanged(EventArgs e)
        {
            ThicknessAnimation ta = new ThicknessAnimation(_panel.Margin, new Thickness(-CurrentSlideNumber * ColWidth, 0, 0, 0), new Duration(TimeSpan.Parse("0:0:0.3")));
            _panel.BeginAnimation(Grid.MarginProperty, ta);

            base.OnSlideChanged(e);
        }

        public void ReadyControl(int slidePos = 0)
        {
            if (Slides.Count > 0)
            {
                //_colWidth = this.ActualWidth;
                LayoutRoot.Width = ControlWidth;
                _panel = new Grid { Margin = new Thickness(0), HorizontalAlignment = HorizontalAlignment.Left };
                Enumerable.Range(0, Slides.Count).ToList().ForEach(x => _panel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(ColWidth) }));
                Enumerable.Range(0, Slides.Count).ToList().ForEach(x =>
                {
                    Grid.SetColumn(Slides[x], x);
                    _panel.Children.Add(Slides[x]);
                });
                LayoutRoot.Children.Clear();
                LayoutRoot.Children.Add(_panel);

                NavBar.PointCount = Slides.Count;
                //NavBar.ReadyControl();
                NavBar.CurrentPointChanged += (sender, e) => CurrentSlideNumber = NavBar.CurrentPoint;
                NavBar.CurrentPoint = slidePos;
            }
        }

        public void ScrollToTop()
        {
            scroll1.ScrollToTop();
        }

        //private void SlidePresenter_KeyUp(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.PageDown)
        //    {
        //        CurrentSlideNumber += 1;
        //    }
        //    else if (e.Key == Key.PageUp)
        //    {
        //        CurrentSlideNumber -= 1;
        //    }
        //}
    }

    public class SlidePresenter : UserControl
    {
        public List<FrameworkElement> Slides { get; protected set; }

        protected int _currentSlideNumber = -1;
        public int CurrentSlideNumber
        {
            get
            {
                return _currentSlideNumber;
            }
            set
            {
                _lastSlideNumber = _currentSlideNumber;
                if (value < 0)
                {
                    _currentSlideNumber = 0;
                }
                else if (value >= Slides.Count)
                {
                    _currentSlideNumber = Slides.Count - 1;
                }
                else
                {
                    _currentSlideNumber = value;
                }

                OnSlideChanged(new EventArgs());
            }
        }

        protected int _lastSlideNumber = -1;
        public int LastSlideNumber { get { return _lastSlideNumber; } }

        public FrameworkElement CurrentSlide
        {
            get
            {
                if (_currentSlideNumber >= 0 && _currentSlideNumber < Slides.Count)
                {
                    return Slides[_currentSlideNumber];
                }
                else
                {
                    return null;
                }
            }
        }

        public FrameworkElement LastSlide
        {
            get
            {
                if (_lastSlideNumber >= 0 && _lastSlideNumber < Slides.Count)
                {
                    return Slides[_lastSlideNumber];
                }
                else
                {
                    return null;
                }
            }
        }

        public event EventHandler SlideChanged;

        //public SlidePresenter()  // 为什么这个构造函数会导致出错？
        //{
        //    Slides = new List<FrameworkElement>();
        //    CurrentSlideNumber = -1;
        //}

        public virtual void OnSlideChanged(EventArgs e)
        {
            if (LastSlide != null)
            {
                FadeOut();
            }
            if (CurrentSlide != null)
            {
                FadeIn(this.CurrentSlide);
            }
            if (SlideChanged != null)
            {
                SlideChanged(this, e);
            }
        }

        protected virtual void FadeOut()
        {
        }

        protected virtual void FadeIn(FrameworkElement slide)
        {
        }

        public virtual void ReadyControl()
        {
        }
    }
}
