using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Linq.Expressions;

namespace CIIPP
{
    /// <summary>
    /// 分年double值接口
    /// </summary>
    public interface IYearwiseValue
    {
        /// <summary>
        /// 获取指定年份的值
        /// </summary>
        /// <param name="year">年份</param>
        /// <returns>值</returns>
        double Year(int year);

        /// <summary>
        /// 获取并设置指定年份的值
        /// </summary>
        /// <param name="year">年份</param>
        /// <returns>值</returns>
        double this[int year] { get; set; }

        /// <summary>
        /// 获取已知年份的平均值
        /// </summary>
        double KnownYearsAverage { get; }

        /// <summary>
        /// 获取已知年份的和
        /// </summary>
        double KnownYearsSum { get; }
    }

    [Serializable]
    /// <summary>
    /// 分年double值的和
    /// </summary>
    public class YearwiseSum : IYearwiseValue
    {
        public int LatestYear { get; private set; }
        public int Years { get; private set; }
        [NonSerialized]
        private IEnumerable<IYearwiseValue> _items;
        public IEnumerable<IYearwiseValue> Items { get { return _items; } }

        public YearwiseSum(int latestYear, int years, params IYearwiseValue[] items)
        {
            LatestYear = latestYear;
            Years = years;
            _items = items;
        }

        public YearwiseSum(params IYearwiseValue[] items)
        {
            LatestYear = 0;
            Years = 0;
            _items = items;
        }

        public double Year(int year)
        {
            return Items.Sum(x => x.Year(year));
        }

        public double this[int year]
        {
            get
            {
                if (year < 1900)
                {
                    year = CityStatistics.Current.C02 + year;
                }
                return Items.Sum(x => x.Year(year));
            }
            set
            {
            }
        }

        public double KnownYearsAverage
        {
            get
            {
                return Items.Sum(x => x.KnownYearsAverage);
            }
        }

        public double KnownYearsSum
        {
            get
            {
                return KnownYearsAverage * Years;
            }
        }
    }

    //[Serializable]
    //public delegate double Expression();

    [Serializable]
    /// <summary>
    /// 分年double表达式
    /// </summary>
    public class YearwiseExpression : IYearwiseValue
    {
        public int LatestYear { get { return CityStatistics.Current.C02 + _delta; } }
        public int Years { get; private set; }
        private Dictionary<int, double> _values = new Dictionary<int, double>();
        [NonSerialized]
        private Dictionary<int, Func<double>> _expressions = new Dictionary<int, Func<double>>();
        private Dictionary<int, Func<double>> Expressions
        {
            get
            {
                if (_expressions == null)
                {
                    _expressions = new Dictionary<int, Func<double>>();
                }
                return _expressions;
            }
        }

        private int _delta;
        private bool _init;

        public YearwiseExpression(int latestYear, int years, bool init = true) // todo: 把latestYear改为相对年份
        {
            //LatestYear = latestYear;
            Years = years;
            _delta = latestYear - CityStatistics.Current.C02;
            _init = init;
            if (init)
            {
                Initialize();
            }
        }

        public YearwiseExpression() // newly 20120208
        {
            //LatestYear = latestYear;
            Years = 0;
            _delta = 0;
            _init = false;
        }

        public void Initialize()
        {
            Enumerable.Range(0, Years).ToList().ForEach(x => this[LatestYear - x] = 0);
        }

        /// <summary>
        /// 获取由指定年份表达式计算而得的值
        /// </summary>
        /// <param name="year">年份</param>
        /// <returns>值</returns>
        public double Year(int year)
        {
            return ExprA(year)();
        }

        public double YearR(int year)
        {
            return Expr(year)();
        }

        /// <summary>
        /// 获取指定年份的表达式
        /// </summary>
        /// <param name="year">年份</param>
        /// <returns>表达式</returns>
        public Func<double> ExprA(int year)
        {
            if (_init && LatestYear - year < Years && LatestYear - year >= 0)
            {
                if (_values.Keys.Contains(year))
                {
                    return () => _values[year];
                }
                else
                {
                    return () => 0;
                }
            }
            else
            {
                int rYear = year - CityStatistics.Current.C02;
                if (Expressions.Keys.Contains(rYear))
                {
                    return Expressions[rYear];
                }
                else
                {
                    return () => double.NaN;
                }
            }
        }

        /// <summary>
        /// 获取指定相对年份的表达式
        /// </summary>
        /// <param name="year">相对年份</param>
        /// <returns>表达式</returns>
        public Func<double> Expr(int rYear)
        {
            int year = CityStatistics.Current.C02 + rYear;
            return ExprA(year);
        }

        /// <summary>
        /// 设置指定年份的表达式
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="expr">表达式</param>
        public void SetExprA(int year, Func<double> expr)
        {
            int rYear = year - CityStatistics.Current.C02;
            SetExpr(rYear, expr);
        }

        /// <summary>
        /// 设置指定相对年份的表达式
        /// </summary>
        /// <param name="year">相对年份</param>
        /// <param name="expr">表达式</param>
        public void SetExpr(int rYear, Func<double> expr)
        {
            Expressions[rYear] = expr;
        }

        /// <summary>
        /// 获取由指定年份表达式计算而得的值，或者设置年份表达式为返回指定值的常量表达式
        /// </summary>
        /// <param name="year">年份，绝对或相对均可</param>
        /// <returns>值</returns>
        public double this[int year]
        {
            get
            {
                if (year > 1900)
                {
                    return ExprA(year)();
                }
                else
                {
                    return Expr(year)();
                }
            }
            set
            {
                if (year > 1900)
                {
                    _values[year] = value;
                    //SetExpr(year, () => value);
                }
                else
                {
                    _values[CityStatistics.Current.C02 + year] = value;
                }
            }
        }

        public double KnownYearsAverage
        {
            get
            {
                //return _values.Where(x => (LatestYear - x.Key) >= 0 && (LatestYear - x.Key) < Years).Select(x => x.Value).Average();
                return Enumerable.Range(0, Years).Select(x => Year(LatestYear - x)).Average();
            }
        }

        public double KnownYearsSum
        {
            get
            {
                return KnownYearsAverage * Years;
            }
        }

        public double[] TestValues
        {
            get
            {
                return Enumerable.Range(0, Years).Select(x => Year(LatestYear - x)).ToArray();
            }
        }
    }

    [Serializable]
    public class DoubleExpression
    {
        [NonSerialized]
        private Func<double> _expression;

        public DoubleExpression(Func<double> expression)
        {
            _expression = expression;
        }

        public double GetValue()
        {
            return _expression();
        }

        public override string ToString()
        {
            return GetValue().ToString();
        }
    }
}
