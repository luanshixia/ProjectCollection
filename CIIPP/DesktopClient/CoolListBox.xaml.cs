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
    /// CoolListBox.xaml 的交互逻辑
    /// </summary>
    public partial class CoolListBox : ListBox
    {
        public CoolListBox()
        {
            InitializeComponent();
        }

        public void ReadyAnimation()
        {
            if (this.Items.Cast<object>().All(x => x is FrameworkElement))
            {
                this.Items.Cast<FrameworkElement>().ToList().ForEach(item =>
                {
                    item.MouseEnter += new MouseEventHandler(item_MouseEnter);
                    item.MouseLeave += new MouseEventHandler(item_MouseLeave);
                    item.MouseLeftButtonDown += new MouseButtonEventHandler(item_MouseLeftButtonDown);
                    item.MouseLeftButtonUp += new MouseButtonEventHandler(item_MouseLeftButtonUp);
                });
            }
        }

        void item_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement fe = sender as FrameworkElement;
            ScaleTransform st = new ScaleTransform(1.1, 1.1, fe.ActualWidth / 2, fe.ActualHeight / 2);
            fe.RenderTransform = st;
            DoubleAnimation da1 = new DoubleAnimation(1.1, 1.2, new Duration(TimeSpan.Parse("0:0:0.1")));
            st.BeginAnimation(ScaleTransform.ScaleXProperty, da1);
            st.BeginAnimation(ScaleTransform.ScaleYProperty, da1);
        }

        void item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement fe = sender as FrameworkElement;
            ScaleTransform st = new ScaleTransform(1.2, 1.2, fe.ActualWidth / 2, fe.ActualHeight / 2);
            fe.RenderTransform = st;
            DoubleAnimation da1 = new DoubleAnimation(1.2, 1.1, new Duration(TimeSpan.Parse("0:0:0.1")));
            st.BeginAnimation(ScaleTransform.ScaleXProperty, da1);
            st.BeginAnimation(ScaleTransform.ScaleYProperty, da1);
        }

        void item_MouseLeave(object sender, MouseEventArgs e)
        {
            FrameworkElement fe = sender as FrameworkElement;
            ScaleTransform st = new ScaleTransform(1.2, 1.2, fe.ActualWidth / 2, fe.ActualHeight / 2);
            fe.RenderTransform = st;
            DoubleAnimation da1 = new DoubleAnimation(1.2, 1, new Duration(TimeSpan.Parse("0:0:0.1")));
            st.BeginAnimation(ScaleTransform.ScaleXProperty, da1);
            st.BeginAnimation(ScaleTransform.ScaleYProperty, da1);
        }

        void item_MouseEnter(object sender, MouseEventArgs e)
        {
            FrameworkElement fe = sender as FrameworkElement;
            ScaleTransform st = new ScaleTransform(1, 1, fe.ActualWidth / 2, fe.ActualHeight / 2);
            fe.RenderTransform = st;
            DoubleAnimation da1 = new DoubleAnimation(1, 1.2, new Duration(TimeSpan.Parse("0:0:0.1")));
            st.BeginAnimation(ScaleTransform.ScaleXProperty, da1);
            st.BeginAnimation(ScaleTransform.ScaleYProperty, da1);

            this.Items.Cast<FrameworkElement>().ToList().ForEach(x => Panel.SetZIndex(x, 0));
            Panel.SetZIndex(fe, 100);
        }
    }
}
