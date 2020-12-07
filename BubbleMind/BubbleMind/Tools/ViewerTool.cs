using BubbleMind.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace BubbleMind.Tools
{
    public static class ViewerToolManager
    {
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

        private static readonly List<ViewerTool> _overlayTools = new List<ViewerTool>();
        public static ReadOnlyCollection<ViewerTool> OverlayTools => _overlayTools.AsReadOnly();

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

        public static void SetFrameworkElement(FrameworkElement element) // mod 20180629
        {
            element.MouseMove += (s, args) => Tools.ForEach(t => t.MouseMoveHandler(s, args));
            element.MouseDown += (s, args) =>
            {
                if (args.ClickCount >= 2)
                {
                    Tools.ForEach(t => t.MouseDoubleClickHandler(s, args));
                }
                else
                {
                    Tools.ForEach(t => t.MouseDownHandler(s, args));
                }
            };
            element.MouseUp += (s, args) => Tools.ForEach(t => t.MouseUpHandler(s, args));
            element.MouseWheel += (s, args) => Tools.ForEach(t => t.MouseWheelHandler(s, args));
            element.KeyDown += (s, args) => Tools.ForEach(t => t.KeyDownHandler(s, args));
        }
    }

    public abstract class ViewerTool
    {
        public virtual void MouseMoveHandler(object sender, MouseEventArgs e)
        {
        }

        public virtual void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
        }

        public virtual void MouseUpHandler(object sender, MouseButtonEventArgs e)
        {
        }

        public virtual void MouseDoubleClickHandler(object sender, MouseButtonEventArgs e)
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
        }

        public virtual void ExitToolHandler()
        {
        }
    }

    public class CombinedViewerTool : ViewerTool
    {
        private readonly ViewerTool[] Tools;

        public CombinedViewerTool(params ViewerTool[] tools)
        {
            this.Tools = tools;
        }

        public override void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            this.Tools.ForEach(x => x.MouseMoveHandler(sender, e));
        }

        public override void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            this.Tools.ForEach(x => x.MouseDownHandler(sender, e));
        }

        public override void MouseUpHandler(object sender, MouseButtonEventArgs e)
        {
            this.Tools.ForEach(x => x.MouseUpHandler(sender, e));
        }

        public override void MouseDoubleClickHandler(object sender, MouseButtonEventArgs e)
        {
            this.Tools.ForEach(x => x.MouseDoubleClickHandler(sender, e));
        }

        public override void MouseWheelHandler(object sender, MouseWheelEventArgs e)
        {
            this.Tools.ForEach(x => x.MouseWheelHandler(sender, e));
        }

        public override void KeyDownHandler(object sender, KeyEventArgs e)
        {
            this.Tools.ForEach(x => x.KeyDownHandler(sender, e));
        }

        public override void EnterToolHandler()
        {
            this.Tools.ForEach(x => x.EnterToolHandler());
        }

        public override void ExitToolHandler()
        {
            this.Tools.ForEach(x => x.ExitToolHandler());
        }
    }
}
