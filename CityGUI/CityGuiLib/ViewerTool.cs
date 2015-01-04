using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using TongJi.Geometry;

namespace TongJi.City
{
    public class ViewerTool
    {
        public Action PaintAction = () => { };

        public virtual void Paint()
        {
            PaintAction();
        }

        public virtual void MouseHoverHandler(object sender, MouseEventArgs e)
        {
        }

        public virtual void MouseClickHandler(object sender, MouseEventArgs e)
        {
        }
    }

    public class BestPathTool : ViewerTool
    {
        private List<Point2D> toolFootprint = new List<Point2D>();

        public override void Paint()
        {
            if (DisplayManager.Current.CityModel.Roads.Count == 0)
            {
                return;
            }
            if (toolFootprint.Count == 1)
            {
                PointF start = DisplayManager.Current.CanvasCoordinate(toolFootprint[0]);
                RectangleF rect = new RectangleF(new PointF(start.X - 5, start.Y - 5), new Size(10, 10));
                DisplayManager.Current.g.FillEllipse(Brushes.Red, rect);
            }
            else if (toolFootprint.Count >= 2)
            {
                int i = toolFootprint.Count - 2;
                Polyline path = DisplayManager.Current.PathModel.GetPathPoly(toolFootprint[i], toolFootprint[i + 1]);
                Point[] pts = path.Points.Select(x => DisplayManager.Current.CanvasCoordinate(x)).ToArray();

                List<Polyline> paths = DisplayManager.Current.PathModel.GetRealPath(toolFootprint[i], toolFootprint[i + 1]);
                foreach (var poly in paths)
                {
                    Point[] pt = poly.Points.Select(x => DisplayManager.Current.CanvasCoordinate(x)).ToArray();
                    DisplayManager.Current.g.DrawLines(new Pen(Color.Red, 5), pt);
                }

                PointF start = pts.First();
                PointF end = pts.Last();
                RectangleF rect = new RectangleF(new PointF(start.X - 5, start.Y - 5), new Size(10, 10));
                DisplayManager.Current.g.FillEllipse(Brushes.Red, rect);
                rect = new RectangleF(new PointF(end.X - 5, end.Y - 5), new Size(10, 10));
                DisplayManager.Current.g.FillEllipse(Brushes.Red, rect);

                double dist = DisplayManager.Current.PathModel.GetDist(toolFootprint[i], toolFootprint[i + 1]);
                if (dist > double.MaxValue / 2)
                {
                    DisplayManager.Current.g.DrawString("No path", new Font("Tahoma", 10), new SolidBrush(OptionsManager.Singleton.ViewportTextColor), new PointF(end.X + 10, end.Y + 10));
                }
                else
                {
                    DisplayManager.Current.g.DrawString(string.Format("Dist: {0:0.00} m", dist), new Font("Tahoma", 10), new SolidBrush(OptionsManager.Singleton.ViewportTextColor), new PointF(end.X + 10, end.Y + 10));
                }
            }
        }

        public override void MouseClickHandler(object sender, MouseEventArgs e)
        {
            toolFootprint.Add(DisplayManager.Current.CityCoordinate(e.X, e.Y));
        }
    }

    public class MeasureTool : ViewerTool
    {
        private List<Point2D> toolFootprint = new List<Point2D>();

        public override void Paint()
        {
            if (toolFootprint.Count > 0)
            {
                Point[] pts = toolFootprint.Select(x => DisplayManager.Current.CanvasCoordinate(x)).ToArray();
                DisplayManager.Current.g.DrawLines(new Pen(Color.Green, 5), pts);
                List<double> dists = getDists();
                for (int i = 0; i < pts.Length; i++)
                {
                    DisplayManager.Current.g.DrawString(string.Format("{0:0.00} m", dists[i]), new Font("Tahoma", 10), new SolidBrush(OptionsManager.Singleton.ViewportTextColor), new PointF(pts[i].X + 10, pts[i].Y + 10));
                }
            }
        }

        private List<double> getDists()
        {
            List<double> dists = new List<double>();
            Point2D last = toolFootprint.First();
            double lastDist = 0;
            foreach (var point in toolFootprint)
            {
                double thisDist = Math.Sqrt((point.x - last.x) * (point.x - last.x) + (point.y - last.y) * (point.y - last.y));
                last = point;
                lastDist += thisDist;
                dists.Add(lastDist);
            }
            return dists;
        }

        public override void MouseHoverHandler(object sender, MouseEventArgs e)
        {
            if (toolFootprint.Count > 0)
            {
                toolFootprint[toolFootprint.Count - 1] = DisplayManager.Current.CityCoordinate(e.X, e.Y);
                Viewer.Current.Canvas.Invalidate();
            }
        }

        public override void MouseClickHandler(object sender, MouseEventArgs e)
        {
            if (toolFootprint.Count > 0)
            {
                toolFootprint.Add(DisplayManager.Current.CityCoordinate(e.X, e.Y));
            }
            else
            {
                toolFootprint.Add(DisplayManager.Current.CityCoordinate(e.X, e.Y));
                toolFootprint.Add(DisplayManager.Current.CityCoordinate(e.X, e.Y));
            }
        }
    }

    public class AddEntityTool : ViewerTool
    {
        public AddEntityTool()
        {
        }

        public override void MouseHoverHandler(object sender, MouseEventArgs e)
        {
        }

        public override void MouseClickHandler(object sender, MouseEventArgs e)
        {
            // mod 20110801
            string[] types = { "0", "Retail", "Transit", "CrossCityTransportation", "Medical", "Education", "Pollution", "ZeroOne" };
            CityEntityType type = types[TaskPanel.Current.cbbAddEnt.SelectedIndex].ParseToEnum<CityEntityType>();
            DisplayManager.Current.AddSpot(type, DisplayManager.Current.CityCoordinate(e.X, e.Y));
            ValueBuffer.UpdateValues();
        }
    }

    public class SpotMoveTool : ViewerTool
    {
        private SpotEntity _hover = null;

        public override void MouseHoverHandler(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // Take care: this happens before MouseClick
            {
                // Move spot
                if (_hover != null)
                {
                    Viewer.Current.Canvas.Cursor = Cursors.Hand;
                    _hover.Position = DisplayManager.Current.CityCoordinate(e.X, e.Y);
                    ValueBuffer.UpdateValues();
                    Viewer.Current.Canvas.Invalidate();
                }
            }
            else if (e.Button == MouseButtons.None)
            {
                // Detect spot
                if (TaskPanel.Current.cbShowSpot.Checked)
                {
                    SpotEntity temp = _hover;
                    _hover = DisplayManager.Current.DetectSpotHover(e.X, e.Y);
                    if (_hover != temp)
                    {
                        Viewer.Current.Canvas.Invalidate();
                    }
                    if (_hover != null)
                    {
                        Viewer.Current.Canvas.Cursor = Cursors.Hand;
                    }
                    else if (CanvasWindow.Current.IsDragging)
                    {
                        Viewer.Current.Canvas.Cursor = Cursors.SizeAll;
                    }
                    else
                    {
                        Viewer.Current.Canvas.Cursor = Cursors.Default;
                    }
                }
            }
        }
    }

    public class PanCanvasTool : ViewerTool
    {
        private Point _mouseDownTemp;

        public override void MouseHoverHandler(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // Take care: this happens before MouseClick
            {
                // Pan canvas
                if (CanvasWindow.Current.IsDragging == false)
                {
                    CanvasWindow.Current.IsDragging = true;
                    _mouseDownTemp = Cursor.Position;
                }
                Viewer.Current.Canvas.Cursor = Cursors.SizeAll;
                DisplayManager.Current.Center = DisplayManager.Current.Center.Move(new Vector2D((_mouseDownTemp.X - Cursor.Position.X) * DisplayManager.Current.Scale, (Cursor.Position.Y - _mouseDownTemp.Y) * DisplayManager.Current.Scale));
                _mouseDownTemp = Cursor.Position;

                Viewer.Current.Canvas.Invalidate();
            }
        }

        public override void MouseClickHandler(object sender, MouseEventArgs e)
        {
        }
    }

    public class SelectionTool : ViewerTool
    {
        protected bool _isDragging = false;
        protected Point _cursor;
        protected Point _draggingStartPoint;
        protected Point _draggingEndPoint;
        protected bool _isCrossing = false;
        protected CityEntity _hover;

        public override void Paint()
        {
            if (_isDragging)
            {
                Point p1 = _draggingStartPoint;
                Point p2 = _cursor;
                Point p = new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
                Size s = new Size(Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y));
                Brush bFill = new SolidBrush(Color.FromArgb(128, 0, 128, 192));
                Pen bStroke = new Pen(Color.FromArgb(0, 128, 192));
                if (_isCrossing)
                {
                    bFill = new SolidBrush(Color.FromArgb(96, 0, 192, 0));
                    bStroke = new Pen(Color.FromArgb(0, 128, 0)) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash, DashPattern = new float[] { 5, 3 } };
                }
                Rectangle rect = new Rectangle(new Point(p.X - s.Width / 2, p.Y - s.Height / 2), s);
                DisplayManager.Current.g.FillRectangle(bFill, rect);
                DisplayManager.Current.g.DrawRectangle(bStroke, rect);
            }
        }

        public override void MouseClickHandler(object sender, MouseEventArgs e)
        {
            if (Viewer.Current.ShiftDown)
            {
                if (_hover == null)
                {
                }
                else
                {
                    SSet.AddSelection(_hover);
                }
            }
            else
            {
                if (_hover == null)
                {
                    SSet.ClearSelection();
                }
                else
                {
                    SSet.Select(_hover);
                }
            }
            Viewer.Current.Canvas.Invalidate();
        }

        public override void MouseHoverHandler(object sender, MouseEventArgs e)
        {
            _cursor = e.Location;
            if (e.Button == MouseButtons.Left)
            {
                if (_isDragging == false)
                {
                    _draggingStartPoint = e.Location;
                }
                _isDragging = true;
                if (e.X >= _draggingStartPoint.X)
                {
                    _isCrossing = false;
                }
                else
                {
                    _isCrossing = true;
                }
            }
            else if (e.Button == MouseButtons.None)
            {
                if (_isDragging == true)
                {
                    _draggingEndPoint = e.Location;
                    var ready = HandleWindowOrCross();  // *************
                    if (Viewer.Current.ShiftDown)
                    {
                        SSet.AddSelection(ready);
                    }
                    else
                    {
                        SSet.Select(ready);
                    }
                }
                _isDragging = false;
            }
            if (_isDragging)
            {
                Viewer.Current.Canvas.Invalidate();
            }
            else
            {
                Viewer.Current.Canvas.Invalidate();
            }
        }

        protected virtual CityEntity[] HandleWindowOrCross()
        {
            return new CityEntity[0];
        }
    }

    public class SpotEntitySelectionTool : SelectionTool
    {
        public override void Paint()
        {
            base.Paint();
            if (_hover == null) return;
            if (!_isDragging)
            {
                SolidBrush sb = new SolidBrush(Color.Red);
                Pen pn = new Pen(Color.Red, 2);
                PointF center = DisplayManager.Current.CanvasCoordinate((_hover as SpotEntity).Position);
                RectangleF rect = new RectangleF(new PointF(center.X - DisplayManager.SpotSize / 2, center.Y - DisplayManager.SpotSize / 2), new Size(DisplayManager.SpotSize, DisplayManager.SpotSize));
                DisplayManager.Current.g.FillEllipse(sb, rect);
                DisplayManager.Current.g.DrawEllipse(pn, rect);
            }
        }

        public override void MouseClickHandler(object sender, MouseEventArgs e)
        {
            _hover = DisplayManager.Current.DetectSpotHover(e.X, e.Y);
            base.MouseClickHandler(sender, e);
        }

        public override void MouseHoverHandler(object sender, MouseEventArgs e)
        {
            base.MouseHoverHandler(sender, e);
            _hover = DisplayManager.Current.DetectSpotHover(e.X, e.Y);
        }

        protected override CityEntity[] HandleWindowOrCross()
        {
            Point p1 = _draggingStartPoint;
            Point p2 = _draggingEndPoint;
            Extent2D extent = new Extent2D(DisplayManager.Current.CityCoordinate(p1.X, p1.Y), DisplayManager.Current.CityCoordinate(p2.X, p2.Y));
            return DisplayManager.Current.CityModel.CitySpots.Where(x => extent.IsPointIn(x.Position)).ToArray();
        }
    }

    public class RoadSelectionTool : SelectionTool
    {
        public override void Paint()
        {
            base.Paint();
            if (_hover == null) return;
            if (!_isDragging)
            {
                var poly = (_hover as LinearEntity).Alignment;
                Point[] pts = poly.Points.Select(x => DisplayManager.Current.CanvasCoordinate(x)).ToArray();
                DisplayManager.Current.g.DrawLines(new Pen(Color.Red, 5), pts);
            }
        }

        public override void MouseClickHandler(object sender, MouseEventArgs e)
        {
            _hover = DisplayManager.Current.DetectRoad(e.X, e.Y);
            base.MouseClickHandler(sender, e);
        }

        public override void MouseHoverHandler(object sender, MouseEventArgs e)
        {
            base.MouseHoverHandler(sender, e);
            _hover = DisplayManager.Current.DetectRoad(e.X, e.Y);
        }

        protected override CityEntity[] HandleWindowOrCross()
        {
            Point p1 = _draggingStartPoint;
            Point p2 = _draggingEndPoint;
            Extent2D extent = new Extent2D(DisplayManager.Current.CityCoordinate(p1.X, p1.Y), DisplayManager.Current.CityCoordinate(p2.X, p2.Y));
            CityRoad[] ready;
            if (_isCrossing)
            {
                ready = DisplayManager.Current.CityModel.Roads.Where(x => extent.IsPointSetCross(x.Alignment)).ToArray();
            }
            else
            {
                ready = DisplayManager.Current.CityModel.Roads.Where(x => extent.IsPointSetIn(x.Alignment)).ToArray();
            }
            return ready;
        }
    }

    public class ParcelSelectionTool : SelectionTool
    {
        public override void Paint()
        {
            base.Paint();
            if (_hover == null) return;
            if (!_isDragging)
            {
                var poly = (_hover as RegionEntity).Domain;
                Point[] pts = poly.Points.Select(x => DisplayManager.Current.CanvasCoordinate(x)).ToArray();
                DisplayManager.Current.g.DrawPolygon(new Pen(Color.Red, 5), pts);
            }
        }

        public override void MouseClickHandler(object sender, MouseEventArgs e)
        {
            _hover = DisplayManager.Current.DetectParcelHover(e.X, e.Y);
            base.MouseClickHandler(sender, e);
        }

        public override void MouseHoverHandler(object sender, MouseEventArgs e)
        {
            base.MouseHoverHandler(sender, e);
            _hover = DisplayManager.Current.DetectParcelHover(e.X, e.Y);
        }

        protected override CityEntity[] HandleWindowOrCross()
        {
            Point p1 = _draggingStartPoint;
            Point p2 = _draggingEndPoint;
            Extent2D extent = new Extent2D(DisplayManager.Current.CityCoordinate(p1.X, p1.Y), DisplayManager.Current.CityCoordinate(p2.X, p2.Y));
            CityParcel[] ready;
            if (_isCrossing)
            {
                ready = DisplayManager.Current.CityModel.Parcels.Where(x => extent.IsPointSetCross(x.Domain)).ToArray();
            }
            else
            {
                ready = DisplayManager.Current.CityModel.Parcels.Where(x => extent.IsPointSetIn(x.Domain)).ToArray();
            }
            return ready;
        }
    }
}
