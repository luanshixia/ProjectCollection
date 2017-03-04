using System;
using System.Collections.Generic;
using System.Linq;


namespace Dreambuild
{
    public static class Maths
    {
        public static double SquareDeviation(this IEnumerable<double> data)
        {
            return data.Average(x => x * x) - data.Average().Pow(2);
        }

        public static double StandardDeviation(this IEnumerable<double> data)
        {
            return Math.Sqrt(data.SquareDeviation());
        }

        public static double Pow(this double a, int b)
        {
            double result;
            if (b > 0)
            {
                result = Enumerable.Repeat(a, b).Reduce((x, y) => x * y);
            }
            else if (b == 0)
            {
                result = 1;
            }
            else // b < 0
            {
                result = 1.0 / Enumerable.Repeat(a, -b).Reduce((x, y) => x * y);
            }
            return result;
        }

        public static Func<double, double> LinearFunction(double x1, double y1, double x2, double y2)
        {
            return x => y1 + (x - x1) / (x2 - x1) * (y2 - y1);
        }

        public static Func<double, double> LinearFunction(double k, double b)
        {
            return x => k * x + b;
        }

        public static Func<double, double> QuadricFunction(double a, double b, double c)
        {
            return x => a * x * x + b * x + c;
        }

        public static int MonthDifference(DateTime date1, DateTime date2)
        {
            return (date1.Year * 12 + date1.Month) - (date2.Year * 12 + date2.Month);
        }

        public static double LinearInterpolate(double x1, double x2, double y1, double y2, double x) // newly 20130808
        {
            return LinearFunction(x1, y1, x2, y2)(x);
        }
    }
}

namespace Dreambuild.Mathematics
{
    public static class DynamicGrading
    {
        public static Tuple<List<List<double>>, List<double>, int> Perform(IEnumerable<double> data, int gradeCount, int maxSteps)
        {
            List<double> values = data.OrderBy(x => x).ToList();
            int n = values.Count;

            var deltas = Enumerable.Range(0, values.Count - 1).Select(i => Tuple.Create(i, values[i + 1] - values[i])).ToList();
            var maxDeltas = deltas.OrderByDescending(x => x.Item2).Take(gradeCount - 1).ToList();
            var borderIndices = maxDeltas.Select(x => x.Item1).OrderBy(x => x).ToList();
            var borderValues = borderIndices.Select(i => (values.ElementAt(i) + values.ElementAt(i + 1)) / 2).ToList(); //values.ElementsAt(borderIndices).ToList();
            borderValues.Insert(0, values.Min());
            borderValues.Add(values.Max() + 0.001 * (values.Max() - values.Min()));
            var grades = borderValues.PairwiseSelect((x, y) => values.Where(z => x <= z && z < y).ToList()).ToList();
            var pattern = string.Join(",", grades.Select(x => x.Count.ToString()).ToArray());
            int finalSteps = 0;
            for (int step = 0; step < maxSteps; step++)
            {
                var centroids = grades.Select(x => x.Average());
                var dists = centroids.Select(x => values.Select(y => Math.Abs(y - x)).ToList()).ToList();
                var belongings = Enumerable.Range(0, n).Select(i =>
                {
                    double min = dists.Min(x => x[i]);
                    return dists.FindIndex(y => y[i] == min);
                }).ToList();
                grades = Enumerable.Range(0, gradeCount).Select(g => values.Where((x, i) => belongings[i] == g).ToList()).ToList();

                finalSteps = step + 1;
                var pattern1 = string.Join(",", grades.Select(x => x.Count.ToString()).ToArray());
                if (pattern1 == pattern)
                {
                    break;
                }
                else
                {
                    pattern = pattern1;
                }
            }
            borderValues = grades.Select(x => x.Min()).ToList();
            borderValues.Add(values.Max());
            return Tuple.Create(grades, borderValues, finalSteps);
        }
    }
}
