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

namespace TongJi.Gis.Display
{
    /// <summary>
    /// ScaleRuler.xaml 的交互逻辑
    /// </summary>
    public partial class ScaleRuler : UserControl
    {
        private double _Scale = 0;
        private String PreScale = string.Empty;

        public ScaleRuler()
        {
            InitializeComponent();
        }

        public void SetScaleRulerValue(double scale)
        {
            _Scale = scale;

            setScaleRuler();
        }

        private void setScaleRuler()
        {
            double tempScaleRuler = _Scale * 100;
            string scaleRuler = getCurrentScaleRuler(tempScaleRuler);

            this.txtLabel.Text = scaleRuler;

            setScaleLine(scaleRuler);

            if (PreScale != string.Empty)
            {
                setScaleLine(scaleRuler);
            }

            PreScale = scaleRuler;
        }

        private void setScaleLine(string currentScale)
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

            if (_Scale > 0)
            {
                double width = scaleLen / _Scale;
                this.LineBase.X2 = this.LineBase.X1 + width;
                this.LineRight.X1 = this.LineBase.X2;
                this.LineRight.X2 = this.LineBase.X2;
            }
        }

        private string getCurrentScaleRuler(double tempScaleRuler)
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
            string s = string.Empty;
            s = pro;

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
