using System.Windows;
using System.Windows.Input;

namespace BubbleFlow.Viewer
{
    public class WheelScalingTool : ViewerTool
    {
        private static readonly double[] ZoomLevels = new double[] { 64, 32, 16, 8, 4, 2, 1, 0.5, 0.25, 0.125, 0.0625, 0.03125, 0.015625 };

        public override void MouseWheelHandler(object sender, MouseWheelEventArgs e)
        {
            base.MouseWheelHandler(sender, e);

            var basePoint = e.GetPosition(MainWindow.Current);
            int index = WheelScalingTool.FindScaleIndex(MainWindow.Current.Scale);
            index += e.Delta / 120;
            if (index > ZoomLevels.Length - 1)
            {
                index = ZoomLevels.Length - 1;
            }
            else if (index < 0)
            {
                index = 0;
            }

            double scale = ZoomLevels[index];
            MainWindow.Current.ScaleCanvas(scale, basePoint);
        }

        private static int FindScaleIndex(double scale)
        {
            for (int i = 0; i < ZoomLevels.Length; i++)
            {
                if (scale > ZoomLevels[i] * 0.75)
                {
                    return i;
                }
            }

            return ZoomLevels.Length - 1;
        }
    }

    public class PanCanvasTool : ViewerTool
    {
        private Point PreviousPosition;

        public override void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(MainWindow.Current);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                MainWindow.Current.PanCanvas(position - this.PreviousPosition);
            }

            this.PreviousPosition = position;
        }
    }
}
