using System;
using System.Windows.Controls;

namespace Dreambuild.Gis.Display
{
    /// <summary>
    /// ScaleRuler.xaml 的交互逻辑
    /// </summary>
    public partial class ScaleRuler : UserControl // reviewed by wy on 2014/6/20
    {
        private double _scale = 0;
        private String _preScale = string.Empty;

        public ScaleRuler()
        {
            InitializeComponent();
        }

        public void SetScaleRulerValue(double scale)
        {
            _scale = scale;

            SetScaleRuler();
        }

        private void SetScaleRuler()
        {
            double tempScaleRuler = _scale * 100;
            string scaleRuler = GetCurrentScaleRuler(tempScaleRuler);

            this.txtLabel.Text = scaleRuler;

            SetScaleLine(scaleRuler);

            if (_preScale != string.Empty)
            {
                SetScaleLine(scaleRuler);
            }

            _preScale = scaleRuler;
        }

        private void SetScaleLine(string currentScale)
        {
            double scaleLen = 0;
            foreach (var c in currentScale)
            {
                if (char.IsDigit(c))
                {
                    if (scaleLen == 0)
                    {
                        scaleLen = char.GetNumericValue(c);
                    }
                    else
                    {
                        scaleLen = scaleLen * 10 + char.GetNumericValue(c);
                    }
                }
            }

            if (_scale > 0)
            {
                double width = scaleLen / _scale;
                this.LineBase.X2 = this.LineBase.X1 + width;
                this.LineRight.X1 = this.LineBase.X2;
                this.LineRight.X2 = this.LineBase.X2;
            }
        }

        private string GetCurrentScaleRuler(double tempScaleRuler)
        {
            string scale = string.Empty;

            string tempScale = tempScaleRuler.ToString("0.0");
            if (tempScale.Length == 3)
            {
                if (tempScaleRuler > 0 && tempScaleRuler <= 1.5)
                {
                    scale = "1m";
                }
                else if (tempScaleRuler > 1.5 && tempScaleRuler <= 3.5)
                {
                    scale = "2m";
                }
                else if (tempScaleRuler > 3.5 && tempScaleRuler <= 7.5)
                {
                    scale = "5m";
                }
                else
                {
                    scale = "10m";
                }
            }
            else
            {
                double first = char.GetNumericValue(tempScale[0]);
                double second = char.GetNumericValue(tempScale[1]);
                double temp = first * 10 + second;

                if (temp > 1 && temp <= 15)
                {
                    scale = FormatScaleRuler("1", tempScale);
                }
                else if (temp > 15 && temp <= 35)
                {
                    scale = FormatScaleRuler("2", tempScale);
                }
                else if (temp > 35 && temp <= 75)
                {
                    scale = FormatScaleRuler("5", tempScale);
                }
                else
                {
                    scale = FormatScaleRuler("10", tempScale);
                }
            }

            return scale;
        }

        private String FormatScaleRuler(string pro, string temp)
        {
            string s = pro;

            foreach (var c in temp.ToCharArray())
            {
                if (char.IsDigit(c))
                {
                    s += "0";
                }
                else
                {
                    int end = s.Length;
                    s = s.Remove(end - 1);
                    s += "m";
                    break;
                }
            }

            return s;
        }
    }
}
