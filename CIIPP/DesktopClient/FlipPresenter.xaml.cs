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
    /// FlipPresenter.xaml 的交互逻辑
    /// </summary>
    public partial class FlipPresenter : SlidePresenter
    {
        public FlipPresenter()
        {
            InitializeComponent();

            Slides = new List<FrameworkElement>();
            CurrentSlideNumber = -1;
        }

        public override void OnSlideChanged(EventArgs e)
        {
            cbbSlideNumber.SelectedIndex = _currentSlideNumber;
            base.OnSlideChanged(e);
        }

        protected override void FadeOut()
        {
            ScaleTransform st = new ScaleTransform(1, 1, LastSlide.ActualWidth / 2, LastSlide.ActualHeight / 2);
            LastSlide.RenderTransform = st;
            DoubleAnimation da1 = new DoubleAnimation(1, 10, new Duration(TimeSpan.Parse("0:0:0.5")));
            st.BeginAnimation(ScaleTransform.ScaleXProperty, da1);
            st.BeginAnimation(ScaleTransform.ScaleYProperty, da1);

            DoubleAnimation da2 = new DoubleAnimation(1, 0, new Duration(TimeSpan.Parse("0:0:0.5")));
            LastSlide.BeginAnimation(FrameworkElement.OpacityProperty, da2);
        }

        protected override void FadeIn(FrameworkElement slide)
        {
            ScaleTransform st = new ScaleTransform(0.1, 0.1, CurrentSlide.ActualWidth / 2, CurrentSlide.ActualHeight / 2);
            CurrentSlide.RenderTransform = st;
            DoubleAnimation da1 = new DoubleAnimation(0.1, 1, new Duration(TimeSpan.Parse("0:0:0.5")));
            st.BeginAnimation(ScaleTransform.ScaleXProperty, da1);
            st.BeginAnimation(ScaleTransform.ScaleYProperty, da1);

            DoubleAnimation da2 = new DoubleAnimation(0, 1, new Duration(TimeSpan.Parse("0:0:0.5")));
            CurrentSlide.BeginAnimation(FrameworkElement.OpacityProperty, da2);

            Panel.SetZIndex(LastSlide, 0);
            Panel.SetZIndex(CurrentSlide, 100);
        }

        public override void ReadyControl()
        {
            LayoutRoot.Children.Clear();
            Slides.ForEach(x =>
            {
                x.Opacity = 0;
                x.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                x.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                LayoutRoot.Children.Add(x);
            });

            cbbSlideNumber.Items.Clear();
            Enumerable.Range(1, Slides.Count).ToList().ForEach(x => cbbSlideNumber.Items.Add(string.Format("第 {0} 页", x)));
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            CurrentSlideNumber = CurrentSlideNumber - 1;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            CurrentSlideNumber = CurrentSlideNumber + 1;
        }

        private void cbbSlideNumber_DropDownClosed(object sender, EventArgs e)
        {
            CurrentSlideNumber = cbbSlideNumber.SelectedIndex; //Convert.ToInt32(cbbSlideNumber.SelectedItem) - 1;
        }
    }
}
