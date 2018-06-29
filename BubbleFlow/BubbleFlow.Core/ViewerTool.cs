using Dreambuild.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace BubbleFlow
{
    public static class ViewerToolManager
    {
        private static DateTime _time = new DateTime(1, 1, 1, 1, 0, 0, 0, 0);
        private static ViewerTool _exclusiveTool;
        public static ViewerTool ExclusiveTool
        {
            get
            {
                return _exclusiveTool;
            }
            set
            {
                if (_exclusiveTool != null)
                {
                    _exclusiveTool.ExitToolHandler();
                }
                _exclusiveTool = value;
                _exclusiveTool.EnterToolHandler();
            }
        }

        private static List<ViewerTool> _overlayTools = new List<ViewerTool>();
        public static System.Collections.ObjectModel.ReadOnlyCollection<ViewerTool> OverlayTools
        {
            get
            {
                return _overlayTools.AsReadOnly();
            }
        }

        public static IEnumerable<ViewerTool> Tools
        {
            get
            {
                if (ExclusiveTool != null)
                {
                    yield return ExclusiveTool;
                }
                foreach (var tool in OverlayTools)
                {
                    yield return tool;
                }
            }
        }

        public static void AddTool(ViewerTool tool)
        {
            tool.EnterToolHandler();
            _overlayTools.Add(tool);
        }

        public static void RemoveTool(ViewerTool tool)
        {
            if (_overlayTools.Remove(tool))
            {
                tool.ExitToolHandler();
            }
        }

        public static void ClearTools()
        {
            _overlayTools.ForEach(x => x.ExitToolHandler());
            _overlayTools.Clear();
        }

        public static void SetFrameworkElement(FrameworkElement element)
        {
            element.MouseMove += (s, args) => Tools.ForEach(t => t.MouseMoveHandler(s, args));
            element.MouseLeftButtonDown += (s, args) =>
            {
                DateTime nextTime = DateTime.Now;
                TimeSpan span = nextTime - _time;
                if (span.TotalMilliseconds <= 300)
                {
                    Tools.ForEach(t => t.MouseLDoubleClickHandler(s, args));
                }
                else
                {
                    Tools.ForEach(t => t.MouseLDownHandler(s, args));
                }
                _time = nextTime;
            };
            element.MouseLeftButtonUp += (s, args) => Tools.ForEach(t => t.MouseLUpHandler(s, args));
            //element.MouseLeftButtonDown += (s, args) => Tools.ForEach(t => t.MouseLDoubleClickHandler(s, args));
            element.MouseWheel += (s, args) => Tools.ForEach(t => t.MouseWheelHandler(s, args));
            element.KeyDown += (s, args) => Tools.ForEach(t => t.KeyDownHandler(s, args));
        }
    }

    public abstract class ViewerTool
    {
        public virtual IEnumerable<UIElement> TempElements { get { yield break; } }

        public virtual void Render()
        {
        }

        public virtual void MouseMoveHandler(object sender, MouseEventArgs e)
        {
        }

        public virtual void MouseLDownHandler(object sender, MouseButtonEventArgs e)
        {
        }

        public virtual void MouseLUpHandler(object sender, MouseButtonEventArgs e)
        {
        }

        public virtual void MouseLDoubleClickHandler(object sender, MouseEventArgs e)
        {
        }

        public virtual void MouseWheelHandler(object sender, MouseWheelEventArgs e)
        {
        }

        public virtual void KeyDownHandler(object sender, KeyEventArgs e)
        {
        }

        public virtual void EnterToolHandler()
        {
            //TempElements.ForEach(x => MapControl.Current.Children.Add(x));
        }

        public virtual void ExitToolHandler()
        {
            //TempElements.ForEach(x => MapControl.Current.Children.Remove(x));
        }
    }

    public class CombinedViewerTool : ViewerTool
    {
        private ViewerTool[] _tools;

        public CombinedViewerTool(params ViewerTool[] tools)
        {
            _tools = tools;
        }

        public override IEnumerable<UIElement> TempElements
        {
            get
            {
                return _tools.SelectMany(x => x.TempElements);
            }
        }

        public override void Render()
        {
            _tools.ForEach(x => x.Render());
        }

        public override void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            _tools.ForEach(x => x.MouseMoveHandler(sender, e));
        }

        public override void MouseLDownHandler(object sender, MouseButtonEventArgs e)
        {
            _tools.ForEach(x => x.MouseLDownHandler(sender, e));
        }

        public override void MouseLUpHandler(object sender, MouseButtonEventArgs e)
        {
            _tools.ForEach(x => x.MouseLUpHandler(sender, e));
        }

        public override void MouseLDoubleClickHandler(object sender, MouseEventArgs e)
        {
            _tools.ForEach(x => x.MouseLDoubleClickHandler(sender, e));
        }

        public override void MouseWheelHandler(object sender, MouseWheelEventArgs e)
        {
            _tools.ForEach(x => x.MouseWheelHandler(sender, e));
        }

        public override void KeyDownHandler(object sender, KeyEventArgs e)
        {
            _tools.ForEach(x => x.KeyDownHandler(sender, e));
        }

        public override void EnterToolHandler()
        {
            _tools.ForEach(x => x.EnterToolHandler());
        }

        public override void ExitToolHandler()
        {
            _tools.ForEach(x => x.ExitToolHandler());
        }
    }
}
