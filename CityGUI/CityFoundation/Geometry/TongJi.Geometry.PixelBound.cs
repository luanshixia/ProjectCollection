using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace TongJi.Geometry
{
    public class PixelBound
    {
        public List<Point> Pixels { get; private set; }
        public static readonly Point[] EightDirections = { new Point(1, 0), new Point(1, 1), new Point(0, 1), new Point(-1, 1), new Point(-1, 0), new Point(-1, -1), new Point(0, -1), new Point(1, -1) };

        public PixelBound(List<Point> pixels)
        {
            Pixels = pixels;
        }

        public bool IsBoundPixel(Point p)
        {
            return EightDirections.Any(dir => Pixels.IndexOf(new Point(p.X + dir.X, p.Y + dir.Y)) == -1);
        }

        public int GetDegree(Point p)
        {
            return EightDirections.Count(dir => Pixels.IndexOf(new Point(p.X + dir.X, p.Y + dir.Y)) != -1);
        }

        public List<List<Point>> Trace()
        {
            var boundPixels = Pixels.Where(x => IsBoundPixel(x)).ToList();
            var handledPixels = new List<Point>();
            var result = new List<List<Point>>();
            var currentLoop = new List<Point>();

            bool newLoop = true;
            Point currentPixel = new Point();
            while (handledPixels.Count < boundPixels.Count)
            {
                int temp = handledPixels.Count;
                List<Point> candidates = new List<Point>();
                foreach (var pixel in boundPixels)
                {
                    if (!handledPixels.Contains(pixel))
                    {
                        if (newLoop)
                        {
                            currentLoop = new List<Point>();
                            result.Add(currentLoop);
                            newLoop = false;
                            currentPixel = pixel;
                            handledPixels.Add(pixel);
                        }
                        if (Adjacent(pixel, currentPixel))
                        {
                            candidates.Add(pixel);
                            if (Dist(pixel, currentPixel) == 1) break;
                        }
                    }
                }
                if (candidates.Count > 0)
                {
                    double minDist = candidates.Min(x => Dist(x, currentPixel));
                    var candidate = candidates.First(x => Dist(x, currentPixel) == minDist);
                    currentLoop.Add(candidate);
                    handledPixels.Add(candidate);
                    currentPixel = candidate;
                }

                // newly 20110902
                if (currentLoop.Count == 0)
                {
                    newLoop = true;
                    continue;
                }

                if (handledPixels.Count == temp) // 一轮下来没有收获
                {
                    if (Adjacent(currentPixel, currentLoop[0]) || currentPixel == currentLoop[0]) // 回到开头
                    {
                        newLoop = true;
                    }
                    else // 遇到单线，折回
                    {
                        int index = currentLoop.IndexOf(currentPixel);
                        currentPixel = currentLoop[(index + currentLoop.Count - 1) % currentLoop.Count];
                    }
                }
            }

            return result;
        }

        public static List<Point2D> Condense(List<Point2D> origin, int level)
        {
            if (level < 2)
            {
                return origin;
            }
            List<Point2D> result = new List<Point2D>();
            for (int i = 0; i < origin.Count; i += level)
            {

                double x = Enumerable.Range(0, level).Average(ii => origin[(i + ii) % origin.Count].x);
                double y = Enumerable.Range(0, level).Average(ii => origin[(i + ii) % origin.Count].y);
                result.Add(new Point2D(x, y));
            }
            return result;
        }

        public static List<Point2D> Align(List<Point2D> origin, int level)
        {
            var points = origin;
            for (int n = 0; n < level; n++)
            {
                for (int i = 0; i < origin.Count; i++)
                {
                    Point2D p1 = origin[(i + origin.Count - 1) % origin.Count];
                    Point2D p = origin[i];
                    Point2D p2 = origin[(i + 1) % origin.Count];
                    Point2D pNew = p.Move(0.4 * ((p1 - p) + (p2 - p)));
                    points[i] = new Point2D(pNew.x, pNew.y);
                }
            }
            return points;
        }

        // static functions

        public static double Dist(Point p1, Point p2)
        {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }

        public static bool Adjacent(Point p1, Point p2)
        {
            return Dist(p1, p2) < 1.5 && Dist(p1, p2) > 0;
        }
    }

    //public static class LocalExtensions
    //{
    //    public static T GetItemCycled
    //}
}
