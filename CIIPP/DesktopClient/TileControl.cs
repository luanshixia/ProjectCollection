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
    public abstract class TileControl : UserControl
    {
        protected IEnumerable<FrameworkElement> _tiles;
        public IEnumerable<FrameworkElement> Tiles
        {
            get
            {
                return _tiles;
            }
            set
            {
                _tiles = value;
                OnTilesChanged(new EventArgs());
            }
        }

        public event EventHandler TilesChanged;
        public virtual void OnTilesChanged(EventArgs e)
        {
            foreach (var tile in _tiles)
            {
                var t = tile;
                tile.MouseEnter += (s, arg) => DoHoverEffect(t);
                tile.MouseLeave += (s, arg) => DoNormalEffect(t);
                tile.MouseLeftButtonDown += (s, arg) => DoClickEffect(t);
                tile.MouseLeftButtonUp += (s, arg) => DoHoverEffect(t);
            }

            ReadyTemplate();
            ItemAnimation();

            if (TilesChanged != null)
            {
                TilesChanged(this, e);
            }

            UpdateLayout();
        }

        protected virtual void DoHoverEffect(FrameworkElement tile)
        {
        }

        protected virtual void DoClickEffect(FrameworkElement tile)
        {
        }

        protected virtual void DoNormalEffect(FrameworkElement tile)
        {
        }

        protected virtual void ReadyTemplate()
        {
            this.ClipToBounds = false;
        }

        protected void ItemAnimation()
        {
            int i = 1;
            foreach (var tile in _tiles)
            {
                tile.Opacity = 0;
                DoubleAnimation da = new DoubleAnimation(0, 1, new Duration(TimeSpan.Parse(string.Format("0:0:{0}", 0.05 * i))));
                da.BeginTime = TimeSpan.Parse(string.Format("0:0:{0}", 0.1 * i));
                tile.BeginAnimation(FrameworkElement.OpacityProperty, da);
                i++;
            }
        }
    }

    public class TileStackControl : TileControl
    {
        protected override void ReadyTemplate()
        {
            base.ReadyTemplate();

            StackPanel sp = new StackPanel();
            Tiles.ToList().ForEach(x => sp.Children.Add(x));
            this.Content = sp;
        }
    }

    public class TileGridControl : TileControl
    {
        public int ColumnCount { get; set; }
        public double TileWidth { get; set; }
        public double TileHeight { get; set; }
        public double SpaceWidth { get; set; }
        public double SpaceHeight { get; set; }

        protected override void ReadyTemplate()
        {
            base.ReadyTemplate();

            WrapPanel wp = new WrapPanel();
            wp.ClipToBounds = false;
            Tiles.ToList().ForEach(x => wp.Children.Add(x));
            this.Content = wp;
        }

        //protected override void DoHoverEffect(FrameworkElement tile)
        //{
        //    base.DoHoverEffect(tile);

        //    ScaleTransform st = tile.RenderTransform as ScaleTransform;
        //    if (st == null)
        //    {
        //        st = new ScaleTransform(1, 1, tile.ActualWidth / 2, tile.ActualHeight / 2);
        //        tile.RenderTransform = st;
        //    }
        //    DoubleAnimation da1 = new DoubleAnimation(1, new Duration(TimeSpan.Parse("0:0:0.05")));
        //    st.BeginAnimation(ScaleTransform.ScaleXProperty, da1);
        //    st.BeginAnimation(ScaleTransform.ScaleYProperty, da1);

        //    Tiles.ToList().ForEach(x => Panel.SetZIndex(x, 0));
        //    Panel.SetZIndex(tile, 100);
        //}

        //protected override void DoClickEffect(FrameworkElement tile)
        //{
        //    base.DoClickEffect(tile);

        //    ScaleTransform st = tile.RenderTransform as ScaleTransform;
        //    if (st == null)
        //    {
        //        st = new ScaleTransform(1, 1, tile.ActualWidth / 2, tile.ActualHeight / 2);
        //        tile.RenderTransform = st;
        //    }
        //    DoubleAnimation da1 = new DoubleAnimation(1, new Duration(TimeSpan.Parse("0:0:0.05")));
        //    st.BeginAnimation(ScaleTransform.ScaleXProperty, da1);
        //    st.BeginAnimation(ScaleTransform.ScaleYProperty, da1);
        //}

        //protected override void DoNormalEffect(FrameworkElement tile)
        //{
        //    base.DoNormalEffect(tile);

        //    ScaleTransform st = tile.RenderTransform as ScaleTransform;
        //    if (st != null)
        //    {
        //        DoubleAnimation da1 = new DoubleAnimation(1.0, new Duration(TimeSpan.Parse("0:0:0.05")));
        //        st.BeginAnimation(ScaleTransform.ScaleXProperty, da1);
        //        st.BeginAnimation(ScaleTransform.ScaleYProperty, da1);
        //    }
        //}
    }

    public class AddRemoveGridControl : TileGridControl
    {

    }
}
