using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DesktopClient
{
    public class ImageButton : Button
    {
        public bool IsToggleButton { get; set; }
        private bool _isToggleOn = false;
        public bool IsToggleOn
        {
            get
            {
                return _isToggleOn;
            }
            set
            {
                _isToggleOn = value;
            }
        }

        public void ToggleOn()
        {
            IsToggleOn = true;
            SetImage(_downImage);
        }

        public void ToggleOff()
        {
            IsToggleOn = false;
            SetImage(_normalImage);
        }

        public void Toggle()
        {
            IsToggleOn = !IsToggleOn;
            if (IsToggleOn)
            {
                SetImage(_downImage);
            }
            else
            {
                SetImage(_normalImage);
            }
        }

        public ImageButton()
        {
            this.Style = FindResource("image_button") as Style;
        }

        public ImageButton(string normal, string hover, string down)
            : this()
        {
            NormalImage = new BitmapImage(new Uri(normal));
            HoverImage = new BitmapImage(new Uri(hover));
            DownImage = new BitmapImage(new Uri(down));

            IsToggleButton = false;
            //IsToggleOn = false;
        }

        private ImageSource _normalImage;
        public ImageSource NormalImage
        {
            get
            {
                return _normalImage;
            }
            set
            {
                _normalImage = value;
                SetImage(_normalImage);
            }
        }

        private ImageSource _hoverImage;
        public ImageSource HoverImage
        {
            get
            {
                return _hoverImage;
            }
            set
            {
                _hoverImage = value;
            }
        }

        private ImageSource _downImage;
        public ImageSource DownImage
        {
            get
            {
                return _downImage;
            }
            set
            {
                _downImage = value;
            }
        }

        public string NavTarget { get; set; }

        protected override void OnClick()
        {
            base.OnClick();
            if (!string.IsNullOrEmpty(NavTarget))
            {
                NavigationManager.Navigate(NavTarget);
            }
        }

        protected void SetImage(ImageSource source)
        {
            this.Content = new Image { Source = source ?? _normalImage };
        }

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if (!(IsToggleButton && IsToggleOn))
            {
                SetImage(_hoverImage);
            }
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (!(IsToggleButton && IsToggleOn))
            {
                SetImage(_normalImage);
            }
        }

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (IsToggleButton)
            {
                //Toggle();
            }
            else
            {
                SetImage(_downImage);
            }
        }

        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (!IsToggleButton)
            {
                SetImage(_hoverImage);
            }
        }
    }

    public class ImageButtonWithGray : ImageButton
    {
        public void Gray()
        {
            SetImage(DownImage);
        }

        public void Restore()
        {
            SetImage(NormalImage);
        }
    }

    public class RichImageButton : Button
    {
        public FrameworkElement Overlay { get; set; }

        public RichImageButton()
        {
            this.Style = FindResource("rich_image_button") as Style;
        }

        public RichImageButton(string normal, string hover, string down)
            : this()
        {
            NormalImage = new BitmapImage(new Uri(normal));
            HoverImage = new BitmapImage(new Uri(hover));
            DownImage = new BitmapImage(new Uri(down));
        }

        private ImageSource _normalImage;
        public ImageSource NormalImage
        {
            get
            {
                return _normalImage;
            }
            set
            {
                _normalImage = value;
                SetImage(_normalImage);
            }
        }

        public ImageSource HoverImage { get; set; }

        public ImageSource DownImage { get; set; }

        public string NavTarget { get; set; }

        protected override void OnClick()
        {
            base.OnClick();
            if (!string.IsNullOrEmpty(NavTarget))
            {
                NavigationManager.Navigate(NavTarget);
            }
        }

        protected void SetImage(ImageSource source)
        {
            this.Background = new ImageBrush(source ?? _normalImage);
        }

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            SetImage(HoverImage);
            this.Content = Overlay;
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            SetImage(_normalImage);
            this.Content = null;
        }

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            SetImage(DownImage);
        }

        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            SetImage(HoverImage);
        }
    }
}
