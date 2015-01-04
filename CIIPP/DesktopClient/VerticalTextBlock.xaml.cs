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
    /// VerticalTextBlock.xaml 的交互逻辑
    /// </summary>
    public partial class VerticalTextBlock : UserControl
    {
        //public static DependencyProperty TextProperty;

        //static VerticalTextBlock()
        //{
        //    DependencyProperty.Register("Text", typeof(string), typeof(VerticalTextBlock), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender));
        //}

        //public string Text
        //{
        //    get
        //    {
        //        return GetValue(TextProperty) as string;
        //    }
        //    set
        //    {
        //        SetValue(TextProperty, value);
        //        //BuildText();
        //    }
        //}

        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                BuildText();
            }
        }

        private void BuildText()
        {
            LayoutRoot.Children.Clear();
            var chars = Text.ToList();
            foreach (var cha in chars)
            {
                TextBlock tb = new TextBlock();
                if (cha.ToString() == "（" || cha.ToString() == "）")
                {
                    tb = new TextBlock { Text = cha.ToString(), LayoutTransform = new RotateTransform(90) };
                }
                else 
                {
                    tb = new TextBlock { Text = cha.ToString() };
                }
                ;
                LayoutRoot.Children.Add(tb);
            }
        }

        public VerticalTextBlock()
        {
            InitializeComponent();
        }
    }
}
