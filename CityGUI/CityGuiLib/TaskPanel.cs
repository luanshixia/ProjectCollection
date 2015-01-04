using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;

namespace TongJi.City
{
    public partial class TaskPanel : DockContent
    {
        private static TaskPanel _current;
        public static TaskPanel Current { get { return _current; } }

        public TaskPanel()
        {
            InitializeComponent();

            _current = this;
        }

        #region Not interesting events

        private void cbShowResult_CheckedChanged(object sender, EventArgs e)
        {
            Viewer.Current.Canvas.Invalidate();
        }

        private void cbShowParcels_CheckedChanged(object sender, EventArgs e)
        {
            Viewer.Current.Canvas.Invalidate();
        }

        private void cbShowRoads_CheckedChanged(object sender, EventArgs e)
        {
            Viewer.Current.Canvas.Invalidate();
        }

        private void cbShowSpot_CheckedChanged(object sender, EventArgs e)
        {
            Viewer.Current.Canvas.Invalidate();
        }

        private void cbbValueMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbValueMode.SelectedIndex == ValueMode.ParcelPrice)
            {
                ValueBuffer.BuildParcels();
            }
            else if (cbbValueMode.SelectedIndex == ValueMode.GridPrice)
            {
                ValueBuffer.BuildGrid();
            }
            else if (cbbValueMode.SelectedIndex == ValueMode.Voronoi)
            {
                ValueBuffer.BuildCoverRangeGrid();
            }
            Viewer.Current.Canvas.Invalidate();
        }

        private void numGridSize_ValueChanged(object sender, EventArgs e)
        {
            if (cbbValueMode.SelectedIndex == ValueMode.GridPrice)
            {
                ValueBuffer.BuildGrid();
                Viewer.Current.Canvas.Invalidate();
            }
            else if (cbbValueMode.SelectedIndex == ValueMode.Voronoi)
            {
                ValueBuffer.BuildCoverRangeGrid();
                Viewer.Current.Canvas.Invalidate();
            }
        }

        private void cbShowGridLine_CheckedChanged(object sender, EventArgs e)
        {
            Viewer.Current.Canvas.Invalidate();
        }

        private void cbGradient_CheckedChanged(object sender, EventArgs e)
        {
            Viewer.Current.Canvas.Invalidate();
        }

        private void nudLevels_ValueChanged(object sender, EventArgs e)
        {
            Viewer.Current.Canvas.Invalidate();
        }

        #endregion

        private BiColorGradientMapper CheckColorMapper()
        {
            if (!(DisplayManager.Current.ColorMapper is BiColorGradientMapper))
            {
                DisplayManager.Current.ColorMapper = new BiColorGradientMapper();
            }
            return DisplayManager.Current.ColorMapper as BiColorGradientMapper;
        }

        private void btnMinClr_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                (sender as Button).BackColor = cd.Color;
                if (sender == btnMinClr)
                {
                    CheckColorMapper().ColorA = cd.Color;
                }
                else
                {
                    CheckColorMapper().ColorB = cd.Color;
                }
                Viewer.Current.Canvas.Invalidate();
            }
        }

        private void btnDftClr_Click(object sender, EventArgs e)
        {
            btnMinClr.BackColor = Color.FromArgb(192, 192, 0);
            btnMaxClr.BackColor = Color.FromArgb(192, 0, 0);
            CheckColorMapper().ColorA = Color.FromArgb(192, 192, 0);
            CheckColorMapper().ColorB = Color.FromArgb(192, 0, 0);
            Viewer.Current.Canvas.Invalidate();
        }

        private void cbbAddEnt_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbAddEnt.SelectedIndex > 0)
            {
                AddEntityTool aet = new AddEntityTool();
                Viewer.Current.SetTool(aet);
            }
            else
            {
                Viewer.Current.ResetTool();
            }
        }

        private void TaskPanel_Load(object sender, EventArgs e)
        {
            cbbAddEnt.SelectedIndex = 0;
            cbbValueMode.SelectedIndex = ValueMode.ParcelPrice;
            cbbAggreType.SelectedIndex = (int)AggregateType.Sum;

            // Set from options
            txtMin.Text = OptionsManager.Singleton.MinValue.ToString();
            txtMax.Text = OptionsManager.Singleton.MaxValue.ToString();
            btnMinClr.BackColor = OptionsManager.Singleton.MinValueColor;
            btnMaxClr.BackColor = OptionsManager.Singleton.MaxValueColor;
        }

        private void txtMax_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\b')
            {
                return;
            }
            TextBox tb = sender as TextBox;
            double temp;
            if (double.TryParse(tb.Text.Insert(tb.SelectionStart, e.KeyChar.ToString()), out temp) == false)
            {
                e.Handled = true;
            }
        }

        private void TaskPanel_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 阻止窗口关闭，代之以隐藏。否则，主窗口将失效。
            e.Cancel = true;
            this.Hide();
        }

        private void txtMax_Validating(object sender, CancelEventArgs e)
        {
            CheckColorMapper().MaxValue = Convert.ToDouble(txtMax.Text);
        }

        private void txtMin_Validating(object sender, CancelEventArgs e)
        {
            CheckColorMapper().MinValue = Convert.ToDouble(txtMin.Text);
        }

        private void cbbAggreType_SelectedIndexChanged(object sender, EventArgs e)
        {
            OptionsManager.Singleton.AggregateType = (AggregateType)cbbAggreType.SelectedIndex;
            DisplayManager.Current.CityModel.AggregateType = OptionsManager.Singleton.AggregateType;
            ValueBuffer.UpdateValues();
            Viewer.Current.Canvas.Invalidate();
        }

        private void cbbCountRoads_CheckedChanged(object sender, EventArgs e)
        {
            if (cbbCountRoads.Checked)
            {
                DisplayManager.Current.CityModel.CountRoads(true);
            }
            else
            {
                DisplayManager.Current.CityModel.CountRoads(false);
            }
            ValueBuffer.UpdateValues();
            Viewer.Current.Canvas.Invalidate();
        }

        private void cbbCountParcels_CheckedChanged(object sender, EventArgs e)
        {
            if (cbbCountParcels.Checked)
            {
                DisplayManager.Current.CityModel.CountParcels(true);
            }
            else
            {
                DisplayManager.Current.CityModel.CountParcels(false);
            }
            ValueBuffer.UpdateValues();
            Viewer.Current.Canvas.Invalidate();
        }
    }
}
