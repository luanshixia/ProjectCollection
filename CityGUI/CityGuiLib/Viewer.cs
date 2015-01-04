using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using IronPython.Hosting;

using TongJi.Geometry;

using WindowsFormsAero.TaskDialog;

using BinaryFormatter = System.Runtime.Serialization.Formatters.Binary.BinaryFormatter;

namespace TongJi.City
{
    public partial class Viewer : Form
    {
        // Application constructs
        private DisplayManager _display;
        private static ViewerTool _defaultTool = new PanCanvasTool();
        private ViewerTool _tool = _defaultTool;
        public ViewerTool Tool { get { return _tool; } }
        public bool ShiftDown { get; set; }

        // Sub-windows
        private CanvasWindow canvasWindow = new CanvasWindow();
        private TaskPanel taskPanel = new TaskPanel();
        private PyConsole pythonConsole = new PyConsole();
        private PropertyWindow propertyWindow = new PropertyWindow();

        // Accessors
        private static Viewer _current;
        public static Viewer Current { get { return _current; ;} }
        public PictureBox Canvas { get { return canvasWindow.Canvas; } }

        public Viewer()
        {
            InitializeComponent();
            _current = this;

            NewDocument(false, false);
        }

        public void SetTool(ViewerTool tool)
        {
            _tool = tool;
            lblToolRunning.Text = string.Format("{0}", tool.GetType().Name);

            this.SetToolbarRadioButton(null, null);
        }

        public void ResetTool()
        {
            _tool = _defaultTool;
            SSet.ClearSelection();
            lblToolRunning.Text = string.Format("{0}", _defaultTool.GetType().Name);

            this.SetToolbarRadioButton(tsbtnPan, null);
        }

        private void ResetVariables()
        {
            imageInitializerToolStripMenuItem.Checked = false;
            img = null;
            this.ResetTool();
            taskPanel.cbbAddEnt.SelectedIndex = 0;
            taskPanel.cbbAggreType.SelectedIndex = (int)AggregateType.Sum;
            taskPanel.cbbValueMode.SelectedIndex = ValueMode.ParcelPrice;
            taskPanel.cbbCountRoads.Checked = false;
            taskPanel.cbbCountParcels.Checked = false;
            taskPanel.btnMinClr.BackColor = Color.FromArgb(192, 192, 0);
            taskPanel.btnMaxClr.BackColor = Color.FromArgb(192, 0, 0);
        }

        private void Viewer_Load(object sender, EventArgs e)
        {
            dockPanel1.DocumentStyle = WeifenLuo.WinFormsUI.Docking.DocumentStyle.DockingWindow;

            canvasWindow.Show(dockPanel1);
            pythonConsole.Show(dockPanel1);
            propertyWindow.Show(dockPanel1);
            taskPanel.Show(dockPanel1);

            SetToolbarHandlers();
        }

        public void ShowStatusInfo(int col, int row)
        {
            Point2D cityXY = _display.CityCoordinate(col, row);
            double value = _display.CityModel.GetValue(cityXY);
            lblStatus.Text = string.Format("Scale: {2:0.00}x ({3:0.00}mpp) | City point: {0} | Price: {1:0.00}", cityXY, value, canvasWindow.ScaleMultiple, _display.Scale);
        }

        private void SetToolbarHandlers()
        {
            tsbtnOpen.Click += this.openCityXMLToolStripMenuItem_Click;
            tsbtnSave.Click += this.saveCityXMLToolStripMenuItem_Click;

            tsbtnPan.Click += this.panCanvasToolToolStripMenuItem_Click;
            tsbtnMove.Click += this.spotMoveToolToolStripMenuItem_Click;
            tsbtnSelection.Click += this.spotSelectionToolToolStripMenuItem_Click;

            ToolStripButton[] tsbs = new ToolStripButton[] { tsbtnPan, tsbtnSelection, tsbtnMove };
            Array.ForEach(tsbs, x => x.Click += SetToolbarRadioButton);
        }

        private void SetToolbarRadioButton(object sender, EventArgs e)
        {
            ToolStripButton[] tsbs = new ToolStripButton[] { tsbtnPan, tsbtnSelection, tsbtnMove };
            Array.ForEach(tsbs, x => x.Checked = false);
            ToolStripButton senderButton = sender as ToolStripButton;
            if (senderButton != null)
            {
                senderButton.Checked = true;
            }
        }

        #region Menu commands

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveCityXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DocumentManager.ActiveDocument.HasFile)
            {
                DocumentManager.ActiveDocument.Save();
            }
            else
            {
                saveAsToolStripMenuItem_Click(null, null);
            }
        }

        private void openCityXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Open CityXML";
            ofd.Filter = "CityXML File (*.cml)|*.cml";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var doc = CityDocument.Open(ofd.FileName);
                if (DocumentManager.Switch(doc))
                {
                    NewDocument();
                }
            }
        }

        private void NewDocument(bool withoutFactors = false, bool showMessage = true)
        {
            DisplayManager dm;

            if (showMessage)
            {
                Form waiting = new Form { Text = "CityGUI", StartPosition = FormStartPosition.CenterScreen, FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow, ShowInTaskbar = false, Size = new Size(200, 80) };
                waiting.Controls.Add(new Label { Text = "Wait a minute...", Location = new Point(40, 20), Size = new Size(120, 40), Font = new Font("Tahoma", 7.5f) });
                waiting.Show();

                Func<DisplayManager> mayBeSlow = () =>
                {
                    return new DisplayManager(DocumentManager.ActiveDocument.Content, withoutFactors);
                };
                var result = mayBeSlow.BeginInvoke(null, null);
                dm = mayBeSlow.EndInvoke(result);
                waiting.Close();
            }
            else
            {
                dm = new DisplayManager(DocumentManager.ActiveDocument.Content, withoutFactors);
            }

            _display = dm;
            _display.Width = Canvas.Width;
            _display.Height = Canvas.Height;

            ResetVariables();

            canvasWindow.ZoomExtent();
            canvasWindow.Text = string.Format("City canvas: {0}", DocumentManager.ActiveDocument.ShortName);
            ValueBuffer.BuildParcels(); // newly 20110805
            Canvas.Invalidate();

            timer1.Start();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 ab = new AboutBox1();
            ab.ShowDialog();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            CanvasWindow.Current.ZoomExtent();
            Canvas.Invalidate();
        }

        private void parcelInfoToolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ParcelSelectionTool pst = new ParcelSelectionTool();
            this.SetTool(pst);
        }

        public ImageInitiate img;

        private void imageInitializerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (imageInitializerToolStripMenuItem.Checked)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "Choose image";
                ofd.Filter = "Image File (*.png, *.jpg)|*.png;*.jpg";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    double highPrice = Convert.ToDouble(Microsoft.VisualBasic.Interaction.InputBox("Caution: This initializer won't save in CityXML.\nValue represented by color black:", "High price", "2550"));
                    img = new ImageInitiate(_display.CityModel.Extents, ofd.FileName, highPrice);
                    _display.CityModel.Factors.Add(img);

                    Canvas.Invalidate();
                }
            }
            else
            {
                _display.CityModel.Factors.Remove(img);
                img = null;
                Canvas.Invalidate();
            }
        }

        private void removeEntityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PyCmd.sset.ForEach(x =>
            {
                if (x is SpotEntity)
                {
                    _display.RemoveSpot(x as SpotEntity);
                }
            });
            SSet.ClearSelection();
            ValueBuffer.UpdateValues();
            Canvas.Invalidate();
        }

        private void bestPathToolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BestPathTool bpt = new BestPathTool();
            this.SetTool(bpt);
        }

        private void roadInfoToolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RoadSelectionTool rst = new RoadSelectionTool();
            this.SetTool(rst);
        }

        private void exportGridPriceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save Points";
            sfd.Filter = "Text File (*.txt)|*.txt|All File (*.*)|*.*";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                double interval = Convert.ToDouble(TaskPanel.Current.numGridSize.Value);
                List<Point2D> pts = _display.CityModel.Extents.GetGrid(interval);
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(sfd.FileName))
                {
                    foreach (Point2D pt in pts)
                    {
                        double value = _display.CityModel.GetValue(pt);
                        double area = interval * interval;
                        double price = value * area;
                        sw.WriteLine(string.Format("{0} - {1:0.00}", pt, price));
                    }
                }
            }
        }

        private void mesuramentToolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MeasureTool mst = new MeasureTool();
            this.SetTool(mst);
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pythonConsole.RunString("script()");
            ValueBuffer.UpdateValues();
            Canvas.Invalidate();
        }

        private void ironPythonCommandWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pythonConsole.Show(dockPanel1);
        }

        private void spotMoveToolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpotMoveTool smt = new SpotMoveTool();
            this.SetTool(smt);
        }

        private void contentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Utility.DispatchWin7Xp(
                () => TaskDialog.Show("Please note", "CityGUI", "Currently no help contents available.", TaskDialogButton.OK, TaskDialogIcon.Information),
                () => MessageBox.Show("Please note: \nCurrently no help contents available.", "CityGUI", MessageBoxButtons.OK, MessageBoxIcon.Information)
            );
        }

        #endregion

        private void taskPanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            taskPanel.Show(dockPanel1);
        }

        private void panCanvasToolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SetTool(_defaultTool);
        }

        private void zoomExtentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            canvasWindow.ZoomExtent();
        }

        private void spotSelectionToolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpotEntitySelectionTool sst = new SpotEntitySelectionTool();
            this.SetTool(sst);
        }

        private void bakeResultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save Baked Values";
            sfd.Filter = "Baked Value File (*.baked)|*.baked";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                var data = DisplayManager.Current.CityModel.Extents.GetGrid2d(Convert.ToDouble(taskPanel.numGridSize.Value)).MapArray2d(p => DisplayManager.Current.CityModel.GetValue(p));
                BakedResult br = new BakedResult(DisplayManager.Current.CityModel.Extents, data);
                BinaryFormatter bf = new BinaryFormatter();
                using (System.IO.FileStream fs = new System.IO.FileStream(sfd.FileName, System.IO.FileMode.Create,
                    System.IO.FileAccess.Write, System.IO.FileShare.None))
                {
                    bf.Serialize(fs, br);
                }
            }
        }

        private void importBakedResultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Choose Baked Result";
            ofd.Filter = "Baked Value File (*.baked)|*.baked";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (System.IO.Stream fs = System.IO.File.OpenRead(ofd.FileName))
                {
                    BakedResult br = bf.Deserialize(fs) as BakedResult;
                    _display.CityModel.Factors.Add(br);
                    _display.CityModel.Extents = br.Extents;
                }

                taskPanel.cbbValueMode.SelectedIndex = ValueMode.GridPrice;
                canvasWindow.ZoomExtent();
                Canvas.Invalidate();
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var doc = CityDocument.New();
            if (DocumentManager.Switch(doc))
            {
                NewDocument();
            }
        }

        private void openWithoutFactorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Open CityXML";
            ofd.Filter = "CityXML File (*.cml)|*.cml";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var doc = CityDocument.Open(ofd.FileName);
                if (DocumentManager.Switch(doc))
                {
                    NewDocument();
                }
            }
        }

        private void propertyWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            propertyWindow.Show(dockPanel1);
        }

        private void Viewer_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyData & Keys.ShiftKey) == Keys.ShiftKey)
            {
                ShiftDown = true;
            }
        }

        private void Viewer_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyData & Keys.ShiftKey) == Keys.ShiftKey)
            {
                ShiftDown = false;
            }
        }

        private void Viewer_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files[0].EndsWith("cml"))
                {
                    var doc = CityDocument.Open(files[0]);
                    if (DocumentManager.Switch(doc))
                    {
                        NewDocument();
                    }
                }                
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save CityXML";
            sfd.Filter = "CityXML File (*.cml)|*.cml";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                DocumentManager.ActiveDocument.SaveAs(sfd.FileName);
            }
        }

        private void Viewer_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files[0].EndsWith("cml"))
                {
                    e.Effect = DragDropEffects.All;
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            }
        }

        private void tsbtnInfo_Click(object sender, EventArgs e)
        {
            PropertyGridHelper.Show(PyCmd.sset.ToArray());
        }

        private void testToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            PyCmd.voronoi();
        }

    }
}
