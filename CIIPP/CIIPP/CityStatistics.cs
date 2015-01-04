using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIIPP
{
    [Serializable]
    /// <summary>
    /// 城市数据
    /// </summary>
    public class CityStatistics
    {
        #region Enums

        public static readonly string[] CurrencyEnum = { "AFA (Afghanistan)", "AMD (Armenia)", "AZM (Azerbaijan)", "BDT (Bangladesh)", "BTN (Bhutan)", "KHR (Cambodia)", "人民币 (China)", "NZD Cook (Islands)", "FJD (Fiji)", "GEL (Georgia)", "HKD (Hong Kong)", "INR (India)", "IDR (Indonesia)", "KZT (Kazakstan)", "AUD (Kiribati)", "KRW (Korea (Republic))", "KGS (Kyrgzystan)", "LAK (Lao)", "MYR (Malaysia)", "MVR (Maldives)", "USD (Marshall Islands)", "USD (Micronesia)", "MNT (Mongolia)", "MMK (Myanmar)", "AUD (Nauru)", "NPR (Nepal)", "PKR (Pakistan)", "USD (Palau)", "PGK (Papua New Guinea (PGK))", "PHP (Philippines)", "WSY (Samoa)", "SGD (Singapore)", "SBD (Solomon Islands)", "LRK (Sri Lanka)", "TWD (Taipei, China)", "TJS (Tajikistan)", "USD (Timor)", "THB (Thailand)", "TOP (Tonga)", "TMM (Turkmenistan)", "AUD (Tuvalu)", "UZS (Uzbekistan)", "VUV (Vanuatu)", "VND (Viet Nam)" };
        public static readonly string[] MultipleEnum = { "百 (100)", "千 (1,000)", "万 (10,000)", "十万 (100,000)", "百万 (1,000,000)", "千万 (10,000,000)", "亿 (100,000,000)", "十亿 (1,000,000,000)" };

        #endregion

        #region Local Government Key Data

        /// <summary>
        /// Name of city
        /// </summary>
        public string C01 { get; set; }

        /// <summary>
        /// Latest year
        /// </summary>
        public int C02 { get; set; }

        /// <summary>
        /// Populaton size
        /// </summary>
        public int C03 { get; set; }

        /// <summary>
        /// Annual population growth
        /// </summary>
        public double C04 { get; set; }

        /// <summary>
        /// Date
        /// </summary>
        public DateTime C05 { get; set; }

        /// <summary>
        /// 所在国家
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// 介绍
        /// </summary>
        public string Intro { get; set; }

        /// <summary>
        /// 货币
        /// </summary>
        public int Currency { get; set; }

        /// <summary>
        /// 倍数
        /// </summary>
        public int Multiple { get; set; }

        #endregion

        #region 1.1当地政府收入

        /// <summary>
        /// 经常性收入 - 市政税基
        /// </summary>
        public YearwiseExpression C1A { get; set; }

        /// <summary>
        /// 经常性收入 - 用户费用及罚款
        /// </summary>
        public YearwiseExpression C1B { get; set; }

        /// <summary>
        /// 资本收入 - 共享收入
        /// </summary>
        public YearwiseExpression C1C { get; set; }

        /// <summary>
        /// 资本收入 - 专项拨款
        /// </summary>
        public YearwiseExpression C1D { get; set; }

        /// <summary>
        /// 资本收入 - 土地/市政资产出售所得
        /// </summary>
        public YearwiseExpression C1E { get; set; }

        /// <summary>
        /// Total LG Revenues
        /// </summary>
        public YearwiseSum C1F { get; set; }

        /// <summary>
        /// Tax revenue yoy % change
        /// </summary>
        public YearwiseExpression C1G { get; set; }

        /// <summary>
        /// Non-tax revenue yoy % change
        /// </summary>
        public YearwiseExpression C1H { get; set; }

        /// <summary>
        /// Assigned revenues & grants yoy % change
        /// </summary>
        public YearwiseExpression C1I { get; set; }

        #endregion

        #region 1.2当地政府支出

        /// <summary>
        /// 资本支出 - 投资
        /// </summary>
        public YearwiseExpression C2A { get; set; }

        /// <summary>
        /// 经常性支出 - 运营/维护
        /// </summary>
        public YearwiseExpression C2B { get; set; }

        /// <summary>
        /// 经常性支出 - 还本付息
        /// </summary>
        public YearwiseExpression C2C { get; set; }

        /// <summary>
        /// Total LG Expenditures
        /// </summary>
        public YearwiseSum C2D { get; set; }

        /// <summary>
        /// General expenditure yoy % change
        /// </summary>
        public YearwiseExpression C2E { get; set; }

        /// <summary>
        /// User charges yoy % change
        /// </summary>
        public YearwiseExpression C2F { get; set; }

        #endregion

        #region 1.3当地政府资产

        /// <summary>
        /// 现金
        /// </summary>
        public YearwiseExpression C3A { get; set; }

        /// <summary>
        /// 证券
        /// </summary>
        public YearwiseExpression C3B { get; set; }

        /// <summary>
        /// 长期债券
        /// </summary>
        public YearwiseExpression C3C { get; set; }

        /// <summary>
        /// 其他有形资产
        /// </summary>
        public YearwiseExpression C3D { get; set; }

        /// <summary>
        /// Total LG Assets
        /// </summary>
        public YearwiseSum C3E { get; set; }

        #endregion

        #region 1.4当地政府债务

        /// <summary>
        /// 未偿还贷款项
        /// </summary>
        public YearwiseExpression C4A { get; set; }

        /// <summary>
        /// 年度利息付款
        /// </summary>
        public YearwiseExpression C4B { get; set; }

        /// <summary>
        /// 年度本金付款
        /// </summary>
        public YearwiseExpression C4C { get; set; }

        /// <summary>
        /// Annual Debt Service
        /// </summary>
        public YearwiseSum C4D { get; set; }

        #endregion

        #region 1.6当地政府收入预测

        /// <summary>
        /// 经常性收入：一般收入
        /// </summary>
        public YearwiseExpression C6A { get; set; }

        /// <summary>
        /// 经常性收入：用户费用及罚款
        /// </summary>
        public YearwiseExpression C6B { get; set; }

        /// <summary>
        /// 资本收入：共享收入/税收和一般性拨款
        /// </summary>
        public YearwiseExpression C6C { get; set; }

        /// <summary>
        /// 资本收入：专项拨款
        /// </summary>
        public YearwiseExpression C6D { get; set; }

        /// <summary>
        /// 资本收入：市政资产出售所得
        /// </summary>
        public YearwiseExpression C6E { get; set; }

        /// <summary>
        /// Total LG Revenues
        /// </summary>
        public YearwiseSum C6F { get; set; }

        #endregion

        #region 1.7当地政府支出预测

        /// <summary>
        /// 资本支出 - 投资
        /// </summary>
        public YearwiseExpression C7A { get; set; }

        /// <summary>
        /// 经常性支出 - 运营/维护
        /// </summary>
        public YearwiseExpression C7B { get; set; }

        /// <summary>
        /// 经常性支出 - 还本付息
        /// </summary>
        public YearwiseExpression C7C { get; set; }

        /// <summary>
        /// Total LG Expenditures
        /// </summary>
        public YearwiseSum C7D { get; set; }

        #endregion

        #region 1.8总投资能力

        /// <summary>
        /// 经营性收入（自有+共享）
        /// </summary>
        public YearwiseExpression C8A { get; set; }

        /// <summary>
        /// 经营性支出
        /// </summary>
        public YearwiseExpression C8B { get; set; }

        /// <summary>
        /// 净运营盈余/亏损
        /// </summary>
        public YearwiseExpression C8C { get; set; }

        /// <summary>
        /// 投资预算
        /// </summary>
        public YearwiseExpression C8D { get; set; }

        /// <summary>
        /// Est.debt servicing capacity 
        /// </summary>
        public YearwiseExpression C8E { get; set; }

        #endregion

        #region 1.9宏观经济数据假设

        /// <summary>
        /// 通货膨胀率
        /// </summary>
        public YearwiseExpression C9A { get; set; }

        /// <summary>
        /// 国内生产总值增长率
        /// </summary>
        public YearwiseExpression C9B { get; set; }

        #endregion

        #region 1.10商业贷款贷款条件假设

        /// <summary>
        /// 利率
        /// </summary>
        public double C10A { get; set; }

        /// <summary>
        /// 偿还期（年）
        /// </summary>
        public double C10B { get; set; }

        #endregion

        #region 1.11优惠贷款贷款条件假设

        /// <summary>
        /// 利率
        /// </summary>
        public double C11A { get; set; }

        /// <summary>
        /// 偿还期（年）
        /// </summary>
        public double C11B { get; set; }

        #endregion

        #region 1.12当地税收征管假设

        /// <summary>
        /// 当地征税率
        /// </summary>
        public double C12A { get; set; }

        /// <summary>
        /// 项目延迟对当地一般收入的影响（年）
        /// </summary>
        public double C12B { get; set; }

        /// <summary>
        /// 项目对当地税基的影响（百分比）
        /// </summary>
        public double C12C { get; set; }

        #endregion

        #region 1.13汇率假设（兑美元）

        /// <summary>
        /// CNY
        /// </summary>
        public double C13A { get; set; }

        #endregion

        #region 1.14投资预算和债务清偿假设

        /// <summary>
        /// 运营盈余用于战略投资项目的比例
        /// </summary>
        public double C14A { get; set; }

        /// <summary>
        /// 还本付息额占运运营盈余的比例
        /// </summary>
        public double C14B { get; set; }

        #endregion

        #region 1.15收入预测假设

        /// <summary>
        /// 经常性收入：一般收入（通货膨胀率）
        /// </summary>
        public double C15A_1 { get; set; }

        /// <summary>
        /// 经常性收入：一般收入（GDP）
        /// </summary>
        public double C15A_2 { get; set; }

        /// <summary>
        /// 用户费用及罚款（通货膨胀率）
        /// </summary>
        public double C15B_1 { get; set; }

        /// <summary>
        /// 用户费用及罚款（GDP）
        /// </summary>
        public double C15B_2 { get; set; }

        /// <summary>
        /// 资本收入：共享收入/税收和一般性拨款（通货膨胀率）
        /// </summary>
        public double C15C_1 { get; set; }

        /// <summary>
        /// 资本收入：共享收入/税收和一般性拨款（GDP）
        /// </summary>
        public double C15C_2 { get; set; }

        /// <summary>
        /// 资本收入：专项性拨款（通货膨胀率）
        /// </summary>
        public double C15D_1 { get; set; }

        /// <summary>
        /// 资本收入：专项性拨款（GDP）
        /// </summary>
        public double C15D_2 { get; set; }

        /// <summary>
        /// 资本收入：市政资产出售所得（通货膨胀率）
        /// </summary>
        public double C15E_1 { get; set; }

        /// <summary>
        /// 资本收入：市政资产出售所得（GDP）
        /// </summary>
        public double C15E_2 { get; set; }

        #endregion

        #region 1.16支出预测假设

        /// <summary>
        /// 资本支出：投资用款（通货膨胀率）
        /// </summary>
        public double C16A_1 { get; set; }

        /// <summary>
        /// 资本支出：投资用款（GDP）
        /// </summary>
        public double C16A_2 { get; set; }

        /// <summary>
        /// 经常性支出：运营/维护（通货膨胀率）
        /// </summary>
        public double C16B_1 { get; set; }

        /// <summary>
        /// 经常性支出：运营/维护（GDP）
        /// </summary>
        public double C16B_2 { get; set; }

        /// <summary>
        /// 经常性支出：年度还本付息额（通货膨胀率）
        /// </summary>
        public double C16C_1 { get; set; }

        /// <summary>
        /// 经常性支出：年度还本付息额（GDP）
        /// </summary>
        public double C16C_2 { get; set; }

        #endregion

        /// <summary>
        /// 未来第2-6年的投资预算，人民币
        /// </summary>
        public double InvestimentBudgetForFutureYear2To6CNY
        {
            get
            {
                return Enumerable.Range(C02 + 2, 5).Sum(x => C8C[x]) * C14A;
            }
        }

        /// <summary>
        /// 未来第2-6年的投资预算，美元
        /// </summary>
        public double InvestimentBudgetForFutureYear2To6USD
        {
            get
            {
                return InvestimentBudgetForFutureYear2To6CNY * C13A;
            }
        }

        /// <summary>
        /// 问卷选项
        /// </summary>
        public Dictionary<string, int> Questionnaire { get; private set; }

        //***********项目问卷统计的权重，它们作用于所有项目，故定义在City中**************

        public double Weight_Necessity { get; set; }

        public double Weight_PublicResponse { get; set; }

        public double Weight_Environmental { get; set; }

        public double Weight_Economic { get; set; }

        public double Weight_Feasibility { get; set; }

        internal static CityStatistics Current { get; set; }

        public CityStatistics()
        {
            Current = this;

            C01 = "Shanghai";
            C02 = DateTime.Now.Year;
            C03 = 0;
            C04 = 0;
            C05 = DateTime.Now;
            Country = "China";
            Currency = 6;
            Multiple = 2;
            Questionnaire = new Dictionary<string, int>();

            Weight_Economic = 0.2;
            Weight_Environmental = 0.2;
            Weight_Feasibility = 0.2;
            Weight_Necessity = 0.2;
            Weight_PublicResponse = 0.2;

            C1A = new YearwiseExpression(C02, 4);
            C1B = new YearwiseExpression(C02, 4);
            C1C = new YearwiseExpression(C02, 4);
            C1D = new YearwiseExpression(C02, 4);
            C1E = new YearwiseExpression(C02, 4);
            C1G = new YearwiseExpression(C02, 3, false);
            C1H = new YearwiseExpression(C02, 3, false);
            C1I = new YearwiseExpression(C02, 3, false);

            C2A = new YearwiseExpression(C02, 4);
            C2B = new YearwiseExpression(C02, 4);
            C2C = new YearwiseExpression(C02, 4, false); // mod 20120206
            C2E = new YearwiseExpression(C02, 3, false); //////
            C2F = new YearwiseExpression(C02, 3, false); //////

            C3A = new YearwiseExpression(C02, 4);
            C3B = new YearwiseExpression(C02, 4);
            C3C = new YearwiseExpression(C02, 4);
            C3D = new YearwiseExpression(C02, 4);

            C4A = new YearwiseExpression(C02 + 10, 14, true);
            C4B = new YearwiseExpression(C02 + 10, 14, true);
            C4C = new YearwiseExpression(C02 + 10, 14, true);

            C6A = new YearwiseExpression(C02, 4, false);
            C6B = new YearwiseExpression(C02, 4, false);
            C6C = new YearwiseExpression(C02, 4, false);
            C6D = new YearwiseExpression(C02, 4, false);
            C6E = new YearwiseExpression(C02, 4, false);

            C7A = new YearwiseExpression(C02, 4, false);
            C7B = new YearwiseExpression(C02, 4, false);
            C7C = new YearwiseExpression(C02, 4, false);

            C8A = new YearwiseExpression(C02, 4, false);
            C8B = new YearwiseExpression(C02, 4, false);
            C8C = new YearwiseExpression(C02, 4, false);
            C8D = new YearwiseExpression(C02, 4, false);
            C8E = new YearwiseExpression(C02, 4, false);

            C9A = new YearwiseExpression(C02 + 10, 14);
            C9B = new YearwiseExpression(C02 + 10, 14);

            this.SetExpressions();
            this.SetQuestionnaire();
        }

        public void SetExpressions()
        {
            C1F = new YearwiseSum(C02, 4, C1A, C1B, C1C, C1D, C1E);
            for (int i = 0; i < 3; i++)
            {
                int year = -i; // 循环变量复制后才能用于匿名函数
                C1G.SetExpr(year, () =>
                {
                    return (C1A[year] - C1A[year - 1]) / C1A[year - 1];
                });
                C1H.SetExpr(year, () => (C1B[year] - C1B[year - 1]) / C1B[year - 1]);
                C1I.SetExpr(year, () => (C1C[year] - C1C[year - 1]) / C1C[year - 1]);
            }
            C2D = new YearwiseSum(C02, 4, C2A, C2B, C2C);
            for (int i = 0; i < 3; i++)
            {
                int year = -i;
                C2C.SetExpr(year, () => C4D[year]); // mod 20120206
                C2E.SetExpr(year, () => (C2A[year] - C2A[year - 1]) / C2A[year - 1]);
                C2F.SetExpr(year, () => (C2B[year] - C2B[year - 1]) / C2B[year - 1]);
            }
            C2C.SetExpr(-3, () => C4D[-3]); // mod 20120206
            C3E = new YearwiseSum(C02, 4, C3A, C3B, C3C, C3D);
            C4D = new YearwiseSum(C02 + 10, 4, C4B, C4C);
            C6F = new YearwiseSum(C02, 4, C6A, C6B, C6C, C6D, C6E);
            for (int i = 0; i <= 3; i++)
            {
                int year = -i;
                C6A.SetExpr(year, () => C1A[year]);
                C6B.SetExpr(year, () => C1B[year]);
                C6C.SetExpr(year, () => C1C[year]);
                C6D.SetExpr(year, () => C1D[year]);
                C6E.SetExpr(year, () => C1E[year]);
            }
            for (int i = 1; i <= 10; i++)
            {
                if (i == 1)
                {
                    int y = i;
                    C6A.SetExpr(y, () => C6A[y - 1] + C6A[y - 1] * C1G.KnownYearsAverage);
                    C6B.SetExpr(y, () => C6B[y - 1] + C6B[y - 1] * C1H.KnownYearsAverage);
                    C6C.SetExpr(y, () => C6C[y - 1] + C6C[y - 1] * C1I.KnownYearsAverage);
                    C6D.SetExpr(y, () => C1D.KnownYearsAverage);
                    C6E.SetExpr(y, () => C1E[y - 1]);
                }
                else
                {
                    int y = i;
                    C6A.SetExpr(y, () => C6A[y - 1] * (1 + C9A[y] * C15A_1) * (1 + C9B[y] * C15A_2));
                    C6B.SetExpr(y, () => C6B[y - 1] * (1 + C9A[y] * C15B_1) * (1 + C9B[y] * C15B_2));
                    C6C.SetExpr(y, () => C6C[y - 1] * (1 + C9A[y] * C15C_1) * (1 + C9B[y] * C15C_2));
                    C6D.SetExpr(y, () => C6D[y - 1] * (1 + C9A[y] * C15D_1) * (1 + C9B[y] * C15D_2));
                    C6E.SetExpr(y, () => C6E[y - 1] * (1 + C9A[y] * C15E_1) * (1 + C9B[y] * C15E_2));
                }
            }
            C7D = new YearwiseSum(C02, 4, C7A, C7B, C7C);
            for (int i = 0; i <= 3; i++)
            {
                int year = -i;
                C7A.SetExpr(year, () => C2A[year]);
                C7B.SetExpr(year, () => C2B[year]);
                C7C.SetExpr(year, () => C2C[year]);
            }
            for (int i = 1; i <= 10; i++)
            {
                if (i == 1)
                {
                    int y = i;
                    C7A.SetExpr(y, () => C7A[y - 1] + C7A[y - 1] * C2E.KnownYearsAverage);
                    C7B.SetExpr(y, () => C7B[y - 1] + C7B[y - 1] * C2F.KnownYearsAverage);
                    C7C.SetExpr(y, () => C4D[y]);
                }
                else
                {
                    int y = i;
                    C7A.SetExpr(y, () => C7A[y - 1] * (1 + C9A[y] * C16A_1) * (1 + C9B[y] * C16A_2));
                    C7B.SetExpr(y, () => C7B[y - 1] * (1 + C9A[y] * C16B_1) * (1 + C9B[y] * C16B_2));
                    C7C.SetExpr(y, () => C4D[y]);
                }
            }
            for (int i = -3; i <= 10; i++)
            {
                int year = i;
                C8A.SetExpr(year, () =>
                {
                    if (year > 0)
                    {
                        return C6F[year] - C6D[year];
                    }
                    else
                    {
                        return C6F[year] - C6C[year];
                    }
                });
                C8B.SetExpr(year, () => C7B[year] + C7C[year]);
                C8C.SetExpr(year, () => C8A[year] - C8B[year]);
                C8D.SetExpr(year, () => C8C[year] * C14A);
                C8E.SetExpr(year, () => C8C[year] * C14B);
            }
        }

        public void SetQuestionnaire()
        {
            string[] questions = { "CQ01", "CQ02", "CQ03", "CQ04", "CQ05", "CQ06", "CQ07" };
            questions.ToList().ForEach(x =>
            {
                if (!Questionnaire.Keys.Contains(x))
                {
                    Questionnaire.Add(x, 0);
                }
            });
        }
    }
}
