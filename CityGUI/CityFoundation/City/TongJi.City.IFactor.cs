using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using TongJi.Geometry;

namespace TongJi.City
{
    public delegate double FactorFormula(Point2D p);

    public interface IFactor
    {
        //FactorFormula Formula { get; }
        double Formula(Point2D p);
    }

    public enum FactorFormulaType
    {
        ZeroOne,
        LinearToZeroInRadius,
        ExponentialAttenution,
        InverseSquare
    }

    public static class Formulas
    {
        public static FactorFormula ZeroOne(SpotEntity spot)
        {
            return p =>
            {
                double d = spot.GetDistTo(p);
                return (d > spot.ServingRadius) ? 0 : spot.Coefficient;
            };
        }

        public static FactorFormula LinearToZeroInRadius(SpotEntity spot)
        {
            return p =>
            {
                double d = spot.GetDistTo(p);
                return (d > spot.ServingRadius) ? 0 : spot.Coefficient * (1 - d / spot.ServingRadius);
            };
        }

        public static FactorFormula ExponentialAttenuation(SpotEntity spot)
        {
            return p =>
            {
                double d = spot.GetDistTo(p);
                return Math.Pow(spot.Coefficient, 1 - d / spot.ServingRadius);
            };
        }

        public static FactorFormula InverseSquare(SpotEntity spot, double maxValue = double.MaxValue)
        {
            return p =>
            {
                double d = spot.GetDistTo(p);
                double v = spot.Coefficient / d / d;
                if (v > 0)
                {
                    return v > maxValue ? maxValue : v;
                }
                else
                {
                    return v < -maxValue ? -maxValue : v;
                }
            };
        }

        public static FactorFormula LinearEntity(LinearEntity linear)
        {
            return p =>
            {
                if (linear.BufferSize == 0) return 0;
                double d = linear.Alignment.DistToPoint(p);
                return (d > linear.BufferSize) ? 0 : linear.Coefficient * (1 - d / linear.BufferSize);
            };
        }

        public static FactorFormula RegionEntity(RegionEntity region)
        {
            return p =>
            {
                double value1 = region.Coefficient;
                double value2 = 0;
                if (value1 == 0 && value2 == 0) return 0;
                if (region.BufferSize == 0)
                {
                    if (region.Domain.IsPointIn(p))
                    {
                        return value1;
                    }
                    else
                    {
                        return value2;
                    }
                }
                else
                {
                    if (region.Domain.IsPointIn(p))
                    {
                        return value1;
                    }
                    else
                    {
                        double d = region.Domain.Points.Min(x => x.DistTo(p));
                        return (d > region.BufferSize) ? value2 : (value1 - d / region.BufferSize * (value1 - value2));
                    }
                }
            };
        }
    }

    [Serializable]
    public abstract class ValueInitiate : IFactor
    {
        public abstract double Formula(Point2D p);
    }

    public abstract class FuncInitiate : ValueInitiate
    {
        public override abstract double Formula(Point2D p);
    }

    [Serializable]
    public abstract class ArrayInitiate : ValueInitiate
    {
        private Extent2D _extents;
        public Extent2D Extents { get { return _extents; } }

        public virtual int Width { get { return 0; } }
        public virtual int Height { get { return 0; } }

        private FactorFormula _formula;

        public ArrayInitiate(Extent2D extents)
        {
            _extents = extents;

            double a = _extents.min.x;
            double b = _extents.max.x;
            double c = _extents.min.y;
            double d = _extents.max.y;
            _formula = p => this[(int)((p.x - a) / (b - a) * Width * 0.999), (int)((p.y - c) / (d - c) * Height * 0.999)];
        }

        public override double Formula(Point2D p)
        {
            return _formula(p);
        }

        protected virtual double this[int col, int row]
        {
            get
            {
                return 0;
            }
        }
    }

    [Serializable]
    public class BakedResult : ArrayInitiate
    {
        private double[,] _data;

        public BakedResult(Extent2D extents, double[,] data)
            : base(extents)
        {
            _data = data;
        }

        public override int Width
        {
            get
            {
                return _data.GetLength(0);
            }
        }

        public override int Height
        {
            get
            {
                return _data.GetLength(1);
            }
        }

        protected override double this[int col, int row]
        {
            get
            {
                try
                {
                    return _data[col, row];
                }
                catch
                {
                    return 0;
                }
            }
        }
    }

    public class ImageInitiate : ArrayInitiate
    {
        private Bitmap _image;
        public double HighPrice { get; set; }

        public ImageInitiate(Extent2D extents, string imageFile, double highPrice)
            : base(extents)
        {
            _image = new Bitmap(imageFile);
            HighPrice = highPrice;
        }

        protected override double this[int col, int row]
        {
            get
            {
                try
                {
                    Color clr = _image.GetPixel(col, Height - 1 - row);
                    return (1 - (clr.R + clr.G + clr.B) / 765.0) * HighPrice; // you will always get 0 if you for wrong use 255 instead of 255.0
                }
                catch
                {
                    return 0;
                }
            }
        }

        public override int Width
        {
            get
            {
                return _image.Width;
            }
        }

        public override int Height
        {
            get
            {
                return _image.Height;
            }
        }
    }

    public class UniformInitiate : FuncInitiate
    {
        private double _priceHorizon;

        public UniformInitiate(double priceHorizon)
        {
            _priceHorizon = priceHorizon;
        }

        public override double Formula(Point2D p)
        {
            return _priceHorizon;
        }
    }
}