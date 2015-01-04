// TongJi.Geometry.Spline.cs
// author: ZHANG Jiajian

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TongJi.Geometry
{
    /// <summary>
    /// x-t spline interpolation
    /// </summary>
    public class CubicSpline
    {
        int i, j;
        double h0, h1;
        double[] dy;
        double[] s;
        int n;
        double[] x;
        double[] y;

        public CubicSpline(double[] ts, double[] xs)
        {
            double alpha, beta;
            // 初值
            n = ts.Length;
            x = ts;
            y = xs;
            dy = new double[n];
            s = new double[n];
            s[0] = 0.0;
            dy[0] = 0.0;
            dy[n - 1] = 0.0;
            h0 = ts[1] - ts[0];

            for (j = 1; j <= n - 2; j++)
            {
                h1 = ts[j + 1] - ts[j];
                alpha = h0 / (h0 + h1);
                beta = (1.0 - alpha) * (xs[j] - xs[j - 1]) / h0;
                beta = 3.0 * (beta + alpha * (xs[j + 1] - xs[j]) / h1);
                dy[j] = -alpha / (2.0 + (1.0 - alpha) * dy[j - 1]);
                s[j] = (beta - (1.0 - alpha) * s[j - 1]);
                s[j] = s[j] / (2.0 + (1.0 - alpha) * dy[j - 1]);
                h0 = h1;
            }

            for (j = n - 2; j >= 0; j--)
            {
                dy[j] = dy[j] * dy[j + 1] + s[j];
            }

            for (j = 0; j <= n - 2; j++)
            {
                s[j] = ts[j + 1] - ts[j];
            }
        }

        public double SolveAt(double t)
        {
            if (t >= x[n - 1])
            {
                i = n - 2;
            }
            else
            {
                i = 0;
                while (t > x[i + 1])
                {
                    i = i + 1;
                }
            }

            h1 = (x[i + 1] - t) / s[i];
            h0 = h1 * h1;
            double z = (3.0 * h0 - 2.0 * h0 * h1) * y[i];
            z = z + s[i] * (h0 - h0 * h1) * dy[i];
            h1 = (t - x[i]) / s[i];
            h0 = h1 * h1;
            z = z + (3.0 * h0 - 2.0 * h0 * h1) * y[i + 1];
            z = z - s[i] * (h0 - h0 * h1) * dy[i + 1];

            return (z); ;
        }
    }

    /// <summary>
    /// 2D spline curve, based on CubicSpline class
    /// </summary>
    public class CubicSpline2D
    {
        private List<Point2D> points;
        private CubicSpline x;
        private CubicSpline y;

        public CubicSpline2D()
        {
            points = new List<Point2D>();
            Update();
        }

        public CubicSpline2D(List<Point2D> pts)
        {
            points = pts;
            Update();
        }

        public void AddPoint(Point2D p)
        {
            points.Add(p);
            Update();
        }

        private void Update()
        {
            x = new CubicSpline(Enumerable.Range(0, points.Count).Select(n => (double)n).ToArray(), points.Select(pt => pt.x).ToArray());
            y = new CubicSpline(Enumerable.Range(0, points.Count).Select(n => (double)n).ToArray(), points.Select(pt => pt.y).ToArray());
        }

        public Point2D SolveAt(double t)
        {
            double xx = x.SolveAt(t);
            double yy = y.SolveAt(t);
            return new Point2D(xx, yy);
        }
    }

    public class BSpline
    {
        int i;
        int n;
        double[] ts;
        double[,] a;

        public BSpline(double[] ts, double[] xs)
        {
            this.ts = ts;
            n = xs.Length;
            a = new double[n, 4];
            if (n > 2)
            {
                if (xs[n - 1] != xs[0])
                {
                    double tt = 2 * xs[0] - xs[1];
                    a[0, 0] = (-tt + 3 * xs[0] - 3 * xs[1] + xs[2]) / 6.0;
                    a[0, 1] = (3 * tt - 6 * xs[0] + 3 * xs[1]) / 6.0;
                    a[0, 2] = (-3 * tt + 3 * xs[1]) / 6.0;
                    a[0, 3] = (tt + 4 * xs[0] + xs[1]) / 6.0;
                    for (i = 1; i < n - 2; i++)
                    {
                        a[i, 0] = (-xs[i - 1] + 3 * xs[i] - 3 * xs[i + 1] + xs[i + 2]) / 6.0;
                        a[i, 1] = (3 * xs[i - 1] - 6 * xs[i] + 3 * xs[i + 1]) / 6.0;
                        a[i, 2] = (-3 * xs[i - 1] + 3 * xs[i + 1]) / 6.0;
                        a[i, 3] = (xs[i - 1] + 4 * xs[i] + xs[i + 1]) / 6.0;
                    }
                    tt = 2 * xs[n - 1] - xs[n - 2];
                    a[n - 2, 0] = (-xs[n - 3] + 3 * xs[n - 2] - 3 * xs[n - 1] + tt) / 6.0;
                    a[n - 2, 1] = (3 * xs[n - 3] - 6 * xs[n - 2] + 3 * xs[n - 1]) / 6.0;
                    a[n - 2, 2] = (-3 * xs[n - 3] + 3 * xs[n - 1]) / 6.0;
                    a[n - 2, 3] = (xs[n - 3] + 4 * xs[n - 2] + xs[n - 1]) / 6.0;
                }
                else
                {
                    a[0, 0] = (-xs[n - 2] + 3 * xs[0] - 3 * xs[1] + xs[2]) / 6.0;
                    a[0, 1] = (3 * xs[n - 2] - 6 * xs[0] + 3 * xs[1]) / 6.0;
                    a[0, 2] = (-3 * xs[n - 2] + 3 * xs[1]) / 6.0;
                    a[0, 3] = (xs[n - 2] + 4 * xs[0] + xs[1]) / 6.0;
                    for (i = 1; i < n - 2; i++)
                    {
                        a[i, 0] = (-xs[i - 1] + 3 * xs[i] - 3 * xs[i + 1] + xs[i + 2]) / 6.0;
                        a[i, 1] = (3 * xs[i - 1] - 6 * xs[i] + 3 * xs[i + 1]) / 6.0;
                        a[i, 2] = (-3 * xs[i - 1] + 3 * xs[i + 1]) / 6.0;
                        a[i, 3] = (xs[i - 1] + 4 * xs[i] + xs[i + 1]) / 6.0;
                    }
                    a[n - 2, 0] = (-xs[n - 3] + 3 * xs[n - 2] - 3 * xs[0] + xs[1]) / 6.0;
                    a[n - 2, 1] = (3 * xs[n - 3] - 6 * xs[n - 2] + 3 * xs[0]) / 6.0;
                    a[n - 2, 2] = (-3 * xs[n - 3] + 3 * xs[0]) / 6.0;
                    a[n - 2, 3] = (xs[n - 3] + 4 * xs[n - 2] + xs[0]) / 6.0;
                }
            }
        }

        public double SolveAt(double t)
        {
            double z;
            if (t >= ts[n - 1])
            {
                i = n - 2;
            }
            else
            {
                i = 0;
                while (t > ts[i + 1])
                {
                    i = i + 1;
                }
            }
            double b = (double)(t - ts[i]) / (double)(ts[i + 1] - ts[i]);
            z = (a[i, 2] + b * (a[i, 1] + b * a[i, 0])) * b + a[i, 3];
            return z;
        }
    }

    public class BSpline2D
    {
        private List<Point2D> points;
        private BSpline x;
        private BSpline y;

        public BSpline2D()
        {
            points = new List<Point2D>();
            Update();
        }

        public BSpline2D(List<Point2D> pts)
        {
            points = pts;
            Update();
        }

        public void AddPoint(Point2D p)
        {
            points.Add(p);
            Update();
        }

        private void Update()
        {
            x = new BSpline(Enumerable.Range(0, points.Count).Select(n => (double)n).ToArray(), points.Select(pt => pt.x).ToArray());
            y = new BSpline(Enumerable.Range(0, points.Count).Select(n => (double)n).ToArray(), points.Select(pt => pt.y).ToArray());
        }

        public Point2D SolveAt(double t)
        {
            double xx = x.SolveAt(t);
            double yy = y.SolveAt(t);
            return new Point2D(xx, yy);
        }
    }

    public class Interpolation_BSpline
    {
        /// <summary>
        /// B样条曲线(逼近曲线，由四个控制点确定)
        /// </summary>
        /// <param name="p1">控制点1</param>
        /// <param name="p2">控制点2</param>
        /// <param name="p3">控制点3</param>
        /// <param name="p4">控制点4</param>
        /// <param name="divisions">分割为短直线段的数量</param>
        /// <returns></returns>
        public static Point2D[] Execute(Point2D p1, Point2D p2, Point2D p3, Point2D p4, int divisions)
        {
            // 计算4x4矩阵系数
            double[] a = new double[4];
            double[] b = new double[4];
            a[0] = (-p1.x + 3 * p2.x - 3 * p3.x + p4.x) / 6.0;
            a[1] = (3 * p1.x - 6 * p2.x + 3 * p3.x) / 6.0;
            a[2] = (-3 * p1.x + 3 * p3.x) / 6.0;
            a[3] = (p1.x + 4 * p2.x + p3.x) / 6.0;
            b[0] = (-p1.y + 3 * p2.y - 3 * p3.y + p4.y) / 6.0;
            b[1] = (3 * p1.y - 6 * p2.y + 3 * p3.y) / 6.0;
            b[2] = (-3 * p1.y + 3 * p3.y) / 6.0;
            b[3] = (p1.y + 4 * p2.y + p3.y) / 6.0;

            Point2D[] splinexy = new Point2D[divisions + 1];
            splinexy[0].x = a[3];
            splinexy[0].y = b[3];

            // 计算参数步长
            double alpha = 0.0;
            double delta = 1.0 / divisions;
            for (int i = 0; i <= divisions; i++)
            {
                splinexy[i].x = (a[2] + alpha * (a[1] + alpha * a[0])) * alpha + a[3];
                splinexy[i].y = (b[2] + alpha * (b[1] + alpha * b[0])) * alpha + b[3];
                alpha += delta;
            }
            return splinexy;
        }
    }
}
