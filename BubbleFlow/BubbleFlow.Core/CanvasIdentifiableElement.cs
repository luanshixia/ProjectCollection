using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace BubbleFlow
{
    public class CanvasIdentifiableElement : Canvas
    {
        public string IdentifiableName { get; set; }

        public static CanvasIdentifiableElement GetIdentifiableElement(MouseButtonEventArgs args)
        {
            var element = args.OriginalSource as FrameworkElement;
            while (element != null && !(element is CanvasIdentifiableElement))
            {
                element = element.Parent as FrameworkElement;
            }

            return element as CanvasIdentifiableElement;
        }
    }
}
