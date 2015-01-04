using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;

using TongJi.Geometry;

namespace TongJi.City
{
    public partial class CanvasWindow : DockContent
    {
        private static double[] _zoomLevels = new double[] { 10, 8, 6, 4, 2, 1, 0.8, 0.6, 0.4, 0.2, 0.1, 0.05, 0.02, 0.01 };
        private int _zoomLevelIndex;
        private double _initialScale;
        public bool IsDragging { get; set; }

        private static CanvasWindow _current;
        public static CanvasWindow Current { get { return _current; } }
        public PictureBox Canvas { get { return this.pictureBox1; } }

        public double ScaleMultiple { get { return 1 / _zoomLevels[_zoomLevelIndex]; } }

        public CanvasWindow()
        {
            InitializeComponent();

            _current = this;
            IsDragging = false;
            //this.DoubleBuffered = true;
        }

        public void ZoomExtent()
        {
            var _display = DisplayManager.Current;

            double cityXRange = _display.CityModel.Extents.max.x - _display.CityModel.Extents.min.x;
            double cityYRange = _display.CityModel.Extents.max.y - _display.CityModel.Extents.min.y;
            _display.Scale = Math.Max(cityXRange / pictureBox1.Width, cityYRange / pictureBox1.Height);
            _display.Center = (0.5 * ((_display.CityModel.Extents.min - new Point2D(0, 0)) + (_display.CityModel.Extents.max - new Point2D(0, 0)))).ToPoint();
            //Utility.CreateLog(_display.Center);
            //Utility.CreateLog(_display.CanvasCoordinate(_display.Center));
            //Utility.CreateLog(_display.CityCoordinate(pictureBox1.Width / 2, pictureBox1.Height / 2));

            _zoomLevelIndex = 5;
            _initialScale = _display.Scale;
        }

        #region Canvas event handlers

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.ActiveControl = pictureBox1;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            DisplayManager.Current.g = e.Graphics;
            DisplayManager.Current.Width = pictureBox1.Width;
            DisplayManager.Current.Height = pictureBox1.Height;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.Clear(OptionsManager.Singleton.ViewportBackground);
            if (TaskPanel.Current.cbShowResult.Checked)
            {
                DisplayManager.Current.PaintValue();
            }
            if (TaskPanel.Current.cbShowParcels.Checked)
            {
                DisplayManager.Current.PaintParcel();
            }
            if (TaskPanel.Current.cbShowRoads.Checked)
            {
                DisplayManager.Current.PaintRoad();
            }
            if (TaskPanel.Current.cbShowSpot.Checked)
            {
                DisplayManager.Current.PaintSpot();
            }
            Viewer.Current.Tool.Paint();
            DisplayManager.Current.PaintSSet(); // newly 20110808
            DisplayManager.Current.PaintAdditional(); // newly 20110901
        }

        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            DisplayManager.Current.Width = pictureBox1.Width;
            DisplayManager.Current.Height = pictureBox1.Height;
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            _zoomLevelIndex += e.Delta / 120;
            if (_zoomLevelIndex < 0)
            {
                _zoomLevelIndex = 0;
            }
            else if (_zoomLevelIndex > _zoomLevels.Length - 1)
            {
                _zoomLevelIndex = _zoomLevels.Length - 1;
            }
            DisplayManager.Current.Scale = _initialScale * _zoomLevels[_zoomLevelIndex];
            pictureBox1.Invalidate();
            Viewer.Current.ShowStatusInfo(e.X, e.Y);
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            //this.ActiveControl = pictureBox1;
            ValueBuffer.UpdateValues();
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            //this.ActiveControl = menuStrip1;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            pictureBox1.Cursor = Cursors.Default;
            IsDragging = false;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            Viewer.Current.Tool.MouseHoverHandler(sender, e);
            if (e.Button == MouseButtons.None)
            {
                // Show value at mouse pointer
                Viewer.Current.ShowStatusInfo(e.X, e.Y);
            }
            if (e.Button == MouseButtons.Left) // Take care: this happens before MouseClick
            {
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Viewer.Current.Tool.MouseClickHandler(sender, e);
            }
            else if (e.Button == MouseButtons.Right) // exit tools
            {
                Viewer.Current.ResetTool();
                TaskPanel.Current.cbbAddEnt.SelectedIndex = 0;
            }
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                CanvasWindow.Current.ZoomExtent();
                pictureBox1.Invalidate();
            }
        }

        #endregion
    }
}
