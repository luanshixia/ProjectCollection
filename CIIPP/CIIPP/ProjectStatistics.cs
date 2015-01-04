using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIIPP
{
    [Serializable]
    /// <summary>
    /// 项目数据
    /// </summary>
    public class ProjectStatistics
    {
        #region Enums

        public static readonly string[] Sector = { "固体废物", "水和污水", "公路、铁路、桥梁、机场（港口）", "供电、电力", "商业/工业/技术设施", "卫生", "教育", "城市改造", "其他" };
        public static readonly string[] Purpose = { "ENV=环境", "SOC=社会", "ECO=经济" };
        public static readonly string[] StatusOfProject = { "COMM=项目/承诺资金", "PREP=项目筹备", "PROP=项目设想已提出" };

        #endregion

        #region Project Descriptors

        /// <summary>
        /// Latest year
        /// </summary>
        public int P02 { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string P1A { get; set; }

        /// <summary>
        /// 项目位置
        /// </summary>
        public string P1B { get; set; }

        /// <summary>
        /// 项目隶属部门
        /// </summary>
        public List<int> P1C { get; set; }

        /// <summary>
        /// 项目主要方面
        /// </summary>
        public int P1D { get; set; }

        /// <summary>
        /// 项目现状
        /// </summary>
        public int P1E { get; set; }

        /// <summary>
        /// 预计开始年份
        /// </summary>
        public int P1F_1 { get; set; }

        /// <summary>
        /// 预计完成年份
        /// </summary>
        public int P1F_2 { get; set; }

        /// <summary>
        /// 包括在PIP中
        /// </summary>
        public int PIP { get; set; }

        public int ProjIconId { get; set; }

        #endregion

        #region 2.2资本成本

        /// <summary>
        /// 规划、筹备和采购
        /// </summary>
        public YearwiseExpression P2A { get; set; }

        /// <summary>
        /// 土地收购
        /// </summary>
        public YearwiseExpression P2B { get; set; }

        /// <summary>
        /// 施工建设
        /// </summary>
        public YearwiseExpression P2C { get; set; }

        /// <summary>
        /// 设备和设施
        /// </summary>
        public YearwiseExpression P2D { get; set; }

        /// <summary>
        /// 其他成本
        /// </summary>
        public YearwiseExpression P2E { get; set; }

        /// <summary>
        /// Total
        /// </summary>
        public YearwiseSum P2F { get; set; }

        #endregion

        #region 2.3资本投资的预期资金来源

        /// <summary>
        /// 自有资金（当地政府预算）
        /// </summary>
        public YearwiseExpression P3A { get; set; }

        /// <summary>
        /// 国家或地区基金/拨款/转移
        /// </summary>
        public YearwiseExpression P3B { get; set; }

        /// <summary>
        /// 私营部门 /公众参与
        /// </summary>
        public YearwiseExpression P3C { get; set; }

        /// <summary>
        /// 商业贷款
        /// </summary>
        public YearwiseExpression P3D { get; set; }

        /// <summary>
        /// 优惠贷款（捐助机构）
        /// </summary>
        public YearwiseExpression P3E { get; set; }

        /// <summary>
        /// Total
        /// </summary>
        public YearwiseSum P3T { get; set; }

        /// <summary>
        /// 融资缺口（将自动算出）
        /// </summary>
        public YearwiseExpression P3F { get; set; }

        #endregion

        #region 2.4运营与维护成本

        /// <summary>
        /// 年均预计额
        /// </summary>
        public double P4A { get; set; }

        #endregion

        #region 2.5新增商业贷款

        /// <summary>
        /// 贷款额
        /// </summary>
        public DoubleExpression P5A { get; set; }

        /// <summary>
        /// 发放年份
        /// </summary>
        public double P5B { get; set; }

        /// <summary>
        /// 偿还期(年）
        /// </summary>
        public DoubleExpression P5C { get; set; }

        /// <summary>
        /// 利率
        /// </summary>
        public DoubleExpression P5D { get; set; }

        #endregion

        #region 2.6新增优惠贷款

        /// <summary>
        /// 贷款额
        /// </summary>
        public DoubleExpression P6A { get; set; }

        /// <summary>
        /// 发放年份
        /// </summary>
        public double P6B { get; set; }

        /// <summary>
        /// 偿还期(年）
        /// </summary>
        public DoubleExpression P6C { get; set; }

        /// <summary>
        /// 利率
        /// </summary>
        public DoubleExpression P6D { get; set; }

        #endregion

        #region 2.7运营与维护成本的资金来源

        /// <summary>
        /// 自有资金
        /// </summary>
        public double P7A { get; set; }

        /// <summary>
        /// 国家/地区基金/拨款/支付
        /// </summary>
        public double P7B { get; set; }

        /// <summary>
        /// 费用收入
        /// </summary>
        public double P7C { get; set; }

        /// <summary>
        /// 其他
        /// </summary>
        public double P7D { get; set; }

        /// <summary>
        /// 总计
        /// </summary>
        public DoubleExpression P7E { get; set; }

        #endregion

        #region 2.13 Additional Project Data

        /// <summary>
        /// 简短项目说明
        /// </summary>
        public string P13A1 { get; set; }

        /// <summary>
        /// 直接受益者
        /// </summary>
        public string P13A2 { get; set; }

        /// <summary>
        /// 间接受益者
        /// </summary>
        public string P13A3 { get; set; }

        /// <summary>
        /// 为什么说该项目最有效利用了纳税人的钱
        /// </summary>
        public string P13B { get; set; }

        /// <summary>
        /// 谁参与了本项目的启动进程
        /// </summary>
        public string P13C { get; set; }

        /// <summary>
        /// 项目设计
        /// </summary>
        public string P13D1 { get; set; }

        /// <summary>
        /// 项目实施
        /// </summary>
        public string P13D2 { get; set; }

        /// <summary>
        /// 项目运营
        /// </summary>
        public string P13D3 { get; set; }

        #endregion

        #region 2.14新增商业贷款预测

        /// <summary>
        /// 初始贷款额
        /// </summary>
        public YearwiseExpression P14A { get; set; }

        /// <summary>
        /// 贷款净额
        /// </summary>
        public YearwiseExpression P14B { get; set; }

        /// <summary>
        /// 利息付款
        /// </summary>
        public YearwiseExpression P14C { get; set; }

        /// <summary>
        /// 本金付款
        /// </summary>
        public YearwiseExpression P14D { get; set; }

        #endregion

        #region 2.15新增优惠贷款预测

        /// <summary>
        /// 初始贷款额
        /// </summary>
        public YearwiseExpression P15A { get; set; }

        /// <summary>
        /// 贷款净额
        /// </summary>
        public YearwiseExpression P15B { get; set; }

        /// <summary>
        /// 利息付款
        /// </summary>
        public YearwiseExpression P15C { get; set; }

        /// <summary>
        /// 本金付款
        /// </summary>
        public YearwiseExpression P15D { get; set; }

        #endregion

        #region 2.16额外收入预估值

        /// <summary>
        /// 一般收入
        /// </summary>
        public YearwiseExpression P16A { get; set; }

        /// <summary>
        /// 用户费用
        /// </summary>
        public YearwiseExpression P16B { get; set; }

        #endregion

        #region 2.17额外支出预估值

        /// <summary>
        /// 运营和维护
        /// </summary>
        public YearwiseExpression P17A { get; set; }

        /// <summary>
        /// 还本付息额
        /// </summary>
        public YearwiseExpression P17B { get; set; }

        #endregion

        #region 2.18 ORIGINAL BUDGET FORECAST (FROM FORECAST SHEET)

        /// <summary>
        /// 预计收入
        /// </summary>
        public YearwiseExpression P18_1A { get; set; }

        /// <summary>
        /// 预计运营支出
        /// </summary>
        public YearwiseExpression P18_1B { get; set; }

        /// <summary>
        /// 预计运营盈余/亏损
        /// </summary>
        public YearwiseExpression P18_1C { get; set; }

        /// <summary>
        /// 预计投资运算
        /// </summary>
        public YearwiseExpression P18_1D { get; set; }

        /// <summary>
        /// 预计偿债能力
        /// </summary>
        public YearwiseExpression P18_1E { get; set; }

        /// <summary>
        /// Total
        /// </summary>
        public YearwiseSum P18_1F { get; set; }

        /// <summary>
        /// 预计收入
        /// </summary>
        public YearwiseExpression P18_2A { get; set; }

        /// <summary>
        /// 预计运营支出
        /// </summary>
        public YearwiseExpression P18_2B { get; set; }

        /// <summary>
        /// 预计运营盈余/亏损
        /// </summary>
        public YearwiseExpression P18_2C { get; set; }

        /// <summary>
        /// Total
        /// </summary>
        public YearwiseSum P18_2D { get; set; }

        /// <summary>
        /// 预计收入
        /// </summary>
        public YearwiseExpression P18A { get; set; }

        /// <summary>
        /// 预计运营支出
        /// </summary>
        public YearwiseExpression P18B { get; set; }

        /// <summary>
        /// 预计运营盈余/亏损
        /// </summary>
        public YearwiseExpression P18C { get; set; }

        /// <summary>
        /// 预计投资运算
        /// </summary>
        public YearwiseExpression P18D { get; set; }

        /// <summary>
        /// 预计偿债能力
        /// </summary>
        public YearwiseExpression P18E { get; set; }

        #endregion

        #region 2.19 对预算预测的影响

        /// <summary>
        /// 预计收入
        /// </summary>
        public YearwiseExpression P19_1A { get; set; }

        /// <summary>
        /// 预计运营支出
        /// </summary>
        public YearwiseExpression P19_1B { get; set; }

        /// <summary>
        /// 预计运营盈余/亏损
        /// </summary>
        public YearwiseExpression P19_1C { get; set; }

        /// <summary>
        /// 预计投资运算
        /// </summary>
        public YearwiseExpression P19_1D { get; set; }

        /// <summary>
        /// 预计偿债能力
        /// </summary>
        public YearwiseExpression P19_1E { get; set; }

        /// <summary>
        /// Total
        /// </summary>
        public YearwiseSum P19_1F { get; set; }

        /// <summary>
        /// 预计收入
        /// </summary>
        public YearwiseExpression P19_2A { get; set; }

        /// <summary>
        /// 预计运营支出
        /// </summary>
        public YearwiseExpression P19_2B { get; set; }

        /// <summary>
        /// 预计运营盈余/亏损
        /// </summary>
        public YearwiseExpression P19_2C { get; set; }

        /// <summary>
        /// Total
        /// </summary>
        public YearwiseSum P19_2D { get; set; }

        /// <summary>
        /// 预计收入
        /// </summary>
        public YearwiseExpression P19A { get; set; }

        /// <summary>
        /// 预计运营支出
        /// </summary>
        public YearwiseExpression P19B { get; set; }

        /// <summary>
        /// 预计运营盈余/亏损
        /// </summary>
        public YearwiseExpression P19C { get; set; }

        /// <summary>
        /// 预计投资运算
        /// </summary>
        public YearwiseExpression P19D { get; set; }

        #endregion

        #region 2.20资本投资资金来源

        /// <summary>
        /// 自有资金（当地政府预算）
        /// </summary>
        public YearwiseExpression P20A { get; set; }

        /// <summary>
        /// 国家或地区 基金/拨款/转移
        /// </summary>
        public YearwiseExpression P20B { get; set; }

        /// <summary>
        /// 私营部门 /公众参与 
        /// </summary>
        public YearwiseExpression P20C { get; set; }

        /// <summary>
        /// 商业贷款
        /// </summary>
        public YearwiseExpression P20D { get; set; }

        /// <summary>
        /// 优惠贷款（捐助机构）
        /// </summary>
        public YearwiseExpression P20E { get; set; }

        /// <summary>
        /// 融资缺口
        /// </summary>
        public YearwiseExpression P20F { get; set; }

        /// <summary>
        /// Total
        /// </summary>
        public YearwiseSum P20G { get; set; }

        #endregion

        #region 2.21资本支出

        /// <summary>
        /// 资本投资
        /// </summary>
        public YearwiseExpression P21A { get; set; }

        /// <summary>
        /// 现有支出
        /// </summary>
        public YearwiseExpression P21B { get; set; }

        /// <summary>
        /// 支出限额
        /// </summary>
        public YearwiseExpression P21C { get; set; }

        #endregion

        #region 2.22资本支出

        /// <summary>
        /// 新增贷款
        /// </summary>
        public YearwiseExpression P22A { get; set; }

        /// <summary>
        /// 当前还本付息额
        /// </summary>
        public YearwiseExpression P22B { get; set; }

        /// <summary>
        /// 预计最大偿债能力
        /// </summary>
        public YearwiseExpression P22C { get; set; }

        #endregion

        #region 总结表单所需内容

        /// <summary>
        /// 资本成本
        /// </summary>
        public double ProjectCostsForNext5Years { get { return P2F.KnownYearsSum; } }

        /// <summary>
        /// 成本在预算中的占比
        /// </summary>
        public double CostsBudgetRatio { get { return ProjectCostsForNext5Years / DocumentManager.CurrentDocument.City.InvestimentBudgetForFutureYear2To6CNY; } }

        /// <summary>
        /// 自有资源在成本中的占比
        /// </summary>
        public double ZiyouCostsRatio { get { return P3A.KnownYearsSum / ProjectCostsForNext5Years; } }

        /// <summary>
        /// 国家政府投入在成本中的占比
        /// </summary>
        public double ZhengfuCostsRatio { get { return P3B.KnownYearsSum / ProjectCostsForNext5Years; } }

        /// <summary>
        /// 私人部门投入在成本中的占比
        /// </summary>
        public double SirenCostsRatio { get { return P3C.KnownYearsSum / ProjectCostsForNext5Years; } }
        /// <summary>
        /// 贷款在成本中的占比
        /// </summary>
        public double DaikuanCostsRatio { get { return (P3D.KnownYearsSum + P3E.KnownYearsSum) / ProjectCostsForNext5Years; } }

        /// <summary>
        /// 融资缺口在成本中的占比
        /// </summary>
        public double QuekouCostsRatio { get { return Math.Abs(P3F.KnownYearsSum) / ProjectCostsForNext5Years; } }

        public int Rank { get; set; }

        #endregion

        #region 问卷评分

        /// <summary>
        /// 问卷选项
        /// </summary>
        public Dictionary<string, int> Questionnaire { get; private set; }

        private double Q(string key)
        {
            return QuestionnaireManager.Questions[key].Options[Questionnaire[key]].Point;
        }

        //***********问卷统计**************

        public double Necessity { get { return (Q("PQ01") + Q("PQ02") + Q("PQ03") + Q("PQ04") + Q("PQ05") + Q("PQ06")) / 18.0 * 10; } }

        public double PublicResponse { get { return (Q("PQ07") + Q("PQ08") + Q("PQ09") + Q("PQ10") + Q("PQ11") + Q("PQ12") + Q("PQ13") + Q("PQ14")) / 22.0 * 10; } }

        public double Environmental { get { return (Q("PQ15") + Q("PQ16") + Q("PQ17") + Q("PQ18") + Q("PQ19") + Q("PQ20")) / 18.0 * 10; } }

        public double Economic { get { return (Q("PQ21") + Q("PQ22") + Q("PQ23") + Q("PQ24") + Q("PQ25") + Q("PQ26") + Q("PQ27") + Q("PQ28") + Q("PQ29")) / 26.0 * 10; } }

        public double Feasibility { get { return (Q("PQ30") + Q("PQ31") + Q("PQ32") + Q("PQ33") + Q("PQ34") + Q("PQ35") + Q("PQ36") + Q("PQ37") + Q("PQ38") + Q("PQ39")) / 29.0 * 10; } }

        public double Final
        {
            get
            {
                var city = DocumentManager.CurrentDocument.City;
                return Necessity * city.Weight_Necessity + PublicResponse * city.Weight_PublicResponse + Environmental * city.Weight_Environmental + Economic * city.Weight_Economic + Feasibility * city.Weight_Feasibility;
            }
        }

        //***********场景得分**************

        public double EnvironmentScenario
        {
            get
            {
                double temp = (3.5 * Q("PQ15") + 3 * Q("PQ16") + 3 * Q("PQ17") + 2.5 * Q("PQ18") + 2.5 * Q("PQ19") + 3 * Q("PQ20")) / 52.5 * 10;
                return temp > 0 ? temp : 0;
            }
        }

        public double EconomicScenario
        {
            get
            {
                double temp = (4 * Q("PQ21") + 3.5 * Q("PQ22") + 3.5 * Q("PQ23") + 2.5 * Q("PQ06") + 2 * Q("PQ33") + 1.5 * Q("PQ05") + 1 * Q("PQ14")) / 54.0 * 10;
                return temp > 0 ? temp : 0;
            }
        }

        public double RevenueScenario
        {
            get
            {
                double r103 = P7C / P4A;
                if (r103 <= 0)
                {
                    r103 = 0;
                }
                else if (r103 < 0.5)
                {
                    r103 = 1;
                }
                else if (r103 < 1)
                {
                    r103 = 2;
                }
                else
                {
                    r103 = 3;
                }
                double temp = (4 * Q("PQ32") + 4 * Q("PQ33") + 3 * Q("PQ34") + 3 * Q("PQ35") + 2 * Q("PQ21") + 2 * Q("PQ22") + 1.5 * Q("PQ26") + 1.5 * Q("PQ30") + 5 * r103) / 75.0 * 10;
                return temp > 0 ? temp : 0;
            }
        }

        public double Social
        {
            get
            {
                double temp = (3 * Q("PQ17") + 2 * Q("PQ20") + 2 * Q("PQ05") + 1 * Q("PQ02") + 2 * Q("PQ29") + 2 * Q("PQ27")) / 34.0 * 10;
                return temp > 0 ? temp : 0;
            }
        }

        public double PovertyScenario
        {
            get
            {
                double temp = (5 * Q("PQ24") + 5 * Q("PQ25") + 1 * Q("PQ02") + 1 * Q("PQ26") + 1 * Q("PQ28")) / 39.0 * 10;
                return temp > 0 ? temp : 0;
            }
        }

        #endregion

        public ProjectStatistics(string name, string location )
        {
            P02 = DocumentManager.CurrentDocument.City.C02;
            P1A = name;
            P1B = location;
            P1C = new List<int> { -1 };
            P1D = -1;
            P1E = -1;
            P1F_1 = P02 + 1;
            P1F_2 = P02 + 5;
            Questionnaire = new Dictionary<string, int>();

            P2A = new YearwiseExpression(P02 + 10, 10, true);
            P2B = new YearwiseExpression(P02 + 10, 10, true);
            P2C = new YearwiseExpression(P02 + 10, 10, true);
            P2D = new YearwiseExpression(P02 + 10, 10, true);
            P2E = new YearwiseExpression(P02 + 10, 10, true);

            P3A = new YearwiseExpression(P02 + 10, 10, true);
            P3B = new YearwiseExpression(P02 + 10, 10, true);
            P3C = new YearwiseExpression(P02 + 10, 10, true);
            P3D = new YearwiseExpression(P02 + 10, 10, true);
            P3E = new YearwiseExpression(P02 + 10, 10, true);
            P3F = new YearwiseExpression(P02 + 10, 10, false);

            //P30A = (P31A > 0) ? P31A : 0;
            //P30B = (P31B > 0) ? P31B : 0;
            //P31C = (P31C > 0) ? P31C : 0;
            //P31D = (P31D > 0) ? P31D : 0;

            P13A1 = string.Empty;
            P13A2 = string.Empty;
            P13A3 = string.Empty;
            P13B = string.Empty;
            P13C = string.Empty;
            P13D1 = string.Empty;
            P13D2 = string.Empty;
            P13D3 = string.Empty;

            P14A = new YearwiseExpression(P02 + 1, 5, true);
            P14B = new YearwiseExpression(P02 + 1, 5, true);
            P14C = new YearwiseExpression(P02 + 1, 5, true);
            P14D = new YearwiseExpression(P02 + 1, 5, true);

            P15A = new YearwiseExpression(P02 + 1, 5, true);
            P15B = new YearwiseExpression(P02 + 1, 5, true);
            P15C = new YearwiseExpression(P02 + 1, 5, true);
            P15D = new YearwiseExpression(P02 + 1, 5, true);

            //------------------V91数据来源？-------------------------
            P16A = new YearwiseExpression(P02 + 1, 5, true);
            P16B = new YearwiseExpression(P02 + 1, 5, true);

            //--------------------------------------------------------
            P17A = new YearwiseExpression(P02 + 1, 5, true);
            P17B = new YearwiseExpression(P02 + 1, 5, true);

            P18_1A = new YearwiseExpression(P02, 4, false);
            P18_1B = new YearwiseExpression(P02, 4, false);
            P18_1C = new YearwiseExpression(P02, 4, false);
            P18_1D = new YearwiseExpression(P02, 4, false);
            P18_1E = new YearwiseExpression(P02, 4, false);

            P18_2A = new YearwiseExpression(P02, 4, false);
            P18_2B = new YearwiseExpression(P02, 4, false);
            P18_2C = new YearwiseExpression(P02, 4, false);

            P18A = new YearwiseExpression(P02, 4, false);
            P18B = new YearwiseExpression(P02, 4, false);
            P18C = new YearwiseExpression(P02, 4, false);
            P18D = new YearwiseExpression(P02, 4, false);
            P18E = new YearwiseExpression(P02, 4, false);

            P19_1A = new YearwiseExpression(P02, 4, false);
            P19_1B = new YearwiseExpression(P02, 4, false);
            P19_1C = new YearwiseExpression(P02, 4, false);
            P19_1D = new YearwiseExpression(P02, 4, false);
            P19_1E = new YearwiseExpression(P02, 4, false);

            P19_2A = new YearwiseExpression(P02, 4, false);
            P19_2B = new YearwiseExpression(P02, 4, false);
            P19_2C = new YearwiseExpression(P02, 4, false);

            P19A = new YearwiseExpression(P02, 4, false);
            P19B = new YearwiseExpression(P02, 4, false);
            P19C = new YearwiseExpression(P02, 4, false);
            P19D = new YearwiseExpression(P02, 4, false);

            P20A = new YearwiseExpression(P02, 4, false);
            P20B = new YearwiseExpression(P02, 4, false);
            P20C = new YearwiseExpression(P02, 4, false);
            P20D = new YearwiseExpression(P02, 4, false);
            P20E = new YearwiseExpression(P02, 4, false);
            P20F = new YearwiseExpression(P02, 4, false);

            P21A = new YearwiseExpression(P02, 4, false);
            P21B = new YearwiseExpression(P02, 4, false);
            P21C = new YearwiseExpression(P02, 4, false);

            P22A = new YearwiseExpression(P02, 4, false);
            P22B = new YearwiseExpression(P02, 4, false);
            P22C = new YearwiseExpression(P02, 4, false);

            this.SetExpressions();
            this.SetQuestionnaire();
        }

        public void SetExpressions()
        {
            P2F = new YearwiseSum(P02 + 10, 10, P2A, P2B, P2C, P2D, P2E);
            P3T = new YearwiseSum(P02 + 10, 10, P3A, P3B, P3C, P3D, P3E);
            for (int i = 1; i <= 10; i++) // mod 20120206
            {
                int year = i;
                P3F.SetExpr(year, () => P2F[year] - P3T[year]);
            }
            P5A = new DoubleExpression(() => P3D.KnownYearsSum);
            P5C = new DoubleExpression(() => DocumentManager.CurrentDocument.City.C10B);
            P5D = new DoubleExpression(() => DocumentManager.CurrentDocument.City.C10A);

            P6A = new DoubleExpression(() => P3E.KnownYearsSum);
            P6C = new DoubleExpression(() => DocumentManager.CurrentDocument.City.C11B);
            P6D = new DoubleExpression(() => DocumentManager.CurrentDocument.City.C11A);

            P7E = new DoubleExpression(() => P7A + P7B + P7C + P7D);
            for (int i = 2; i <= 10; i++)
            {
                int year = P02 + i;

                P14A.SetExprA(year, () =>
                {
                    if (year >= P5B + P5C.GetValue() || year < P5B)
                    {
                        return 0;
                    }
                    else
                    {
                        if (year == P5B)
                        {
                            return P5A.GetValue();
                        }
                        else
                        {
                            return P14A[year - 1];
                        }
                    }
                });

                P14B.SetExprA(year, () =>
                {
                    if (P14A[year] == 0 || year - P02 <= 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return P14A[year] - Enumerable.Range(P02 + 1, year - P02).Sum(x => P14D[x]);
                    }
                });

                P14C.SetExprA(year, () => (P14B[year - 1] + P14B[year]) / 2 * P5D.GetValue());
                P14D.SetExprA(year, () => P14A[year] / P5C.GetValue());
            }
            for (int i = 2; i <= 10; i++)
            {
                int year = P02 + i;

                P15A.SetExprA(year, () =>
                {
                    if (year >= P6B + P6C.GetValue() || year < P6B)
                    {
                        return 0;
                    }
                    else
                    {
                        if (year == P6B)
                        {
                            return P6A.GetValue();
                        }
                        else
                        {
                            return P15A[year - 1];
                        }
                    }
                });

                P15B.SetExprA(year, () =>
                {
                    if (P15A[year] == 0 || year - P02 <= 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return P15A[year] - Enumerable.Range(P02 + 1, year - P02).Sum(x => P15D[x]);
                    }
                });

                P15C.SetExprA(year, () => (P15B[year - 1] + P15B[year]) / 2 * P6D.GetValue());
                P15D.SetExprA(year, () => P15A[year] / P6C.GetValue());
            }
            for (int i = 2; i <= 10; i++)
            {
                int year = P02 + i;
                P16A.SetExprA(year, () =>
                {
                    //if(year>P1G+DocumentManager.CurrentDocument.City.C12B)
                    //{

                    //}
                    return 0;
                });

                P16B.SetExprA(year, () => year > P1F_2 ? P7C : 0);
            }
            for (int i = 2; i <= 10; i++)
            {
                int year = P02 + i;
                P17A.SetExprA(year, () => year > P1F_2 ? P4A : 0);
                P17B.SetExprA(year, () => P14C[year] + P14D[year] + P15C[year] + P15D[year]);
            }
            P18_1F = new YearwiseSum(P02 + 8, 14, P18_1A, P18_1B, P18_1C, P18_1D, P18_1E);
            for (int i = -3; i <= 10; i++)
            {
                int year = i;
                P18_1A.SetExpr(year, () => DocumentManager.CurrentDocument.City.C6A[year]);
                P18_1B.SetExpr(year, () => DocumentManager.CurrentDocument.City.C6B[year]);
                P18_1C.SetExpr(year, () => DocumentManager.CurrentDocument.City.C6C[year]);
                P18_1D.SetExpr(year, () => DocumentManager.CurrentDocument.City.C6D[year]);
                P18_1E.SetExpr(year, () => DocumentManager.CurrentDocument.City.C6E[year]);
            }
            P18_2D = new YearwiseSum(P02 + 8, 14, P18_2A, P18_2B, P18_2C);
            for (int i = -3; i <= 10; i++)
            {
                int year = i;
                P18_2A.SetExpr(year, () => DocumentManager.CurrentDocument.City.C7A[year]);
                P18_2B.SetExpr(year, () => DocumentManager.CurrentDocument.City.C7B[year]);
                P18_2C.SetExpr(year, () => DocumentManager.CurrentDocument.City.C7C[year]);
            }
            for (int i = -3; i <= 10; i++)
            {
                int year = i;
                P18A.SetExpr(year, () => DocumentManager.CurrentDocument.City.C8A[year]);
                P18B.SetExpr(year, () => DocumentManager.CurrentDocument.City.C8B[year]);
                P18C.SetExpr(year, () => DocumentManager.CurrentDocument.City.C8C[year]);
                P18D.SetExpr(year, () => DocumentManager.CurrentDocument.City.C8D[year]);
                P18E.SetExpr(year, () => DocumentManager.CurrentDocument.City.C8E[year]);
            }
            P19_1F = new YearwiseSum(P02 + 8, 14, P19_1A, P19_1B, P19_1C, P19_1D, P19_1E);
            for (int i = -3; i <= 10; i++)
            {
                int year = i;
                P19_1A.SetExpr(year, () => P18_1A[year]);
                P19_1B.SetExpr(year, () => P18_1B[year]);
                P19_1C.SetExpr(year, () => P18_1C[year]);
                P19_1D.SetExpr(year, () => P18_1D[year]);
                P19_1E.SetExpr(year, () => P18_1E[year]);
            }
            P19_2D = new YearwiseSum(P02 + 8, 14, P19_2A, P19_2B, P19_2C);
            for (int i = -3; i <= 10; i++)
            {
                int year = i;
                P19_2A.SetExpr(year, () => P18_2A[year]);
                P19_2B.SetExpr(year, () => P18_2B[year]);
                P19_2C.SetExpr(year, () => P18_2C[year]);
            }
            for (int i = -3; i <= 10; i++)
            {
                int year = i;
                P19A.SetExpr(year, () => P19_1F[year] - P19_1D[year]);
                P19B.SetExpr(year, () => P19_2B[year] + P19_2C[year]);
                P19C.SetExpr(year, () => P19A[year] - P19B[year]);
                P19D.SetExpr(year, () => P19C[year] * DocumentManager.CurrentDocument.City.C14A);
            }
            P20G = new YearwiseSum(P02 + 9, 9, P20A, P20B, P20C, P20D, P20E, P20F);
            for (int i = 1; i <= 9; i++)
            {
                int year = i;
                P20A.SetExpr(year, () => NaNto0(P3A[year]));
                P20B.SetExpr(year, () => NaNto0(P3B[year]));
                P20C.SetExpr(year, () => NaNto0(P3C[year]));
                P20D.SetExpr(year, () => NaNto0(P3D[year]));
                P20E.SetExpr(year, () => NaNto0(P3E[year]));
                P20F.SetExpr(year, () => NaNto0(P3F[year]));
            }
            for (int i = 1; i <= 9; i++)
            {
                int year = i;
                P21A.SetExpr(year, () => P20A[year]);
                P21B.SetExpr(year, () => P18B[year + 1]);
                P21C.SetExpr(year, () => P18A[year + 1]);
            }
            for (int i = 1; i <= 9; i++)
            {
                int year = i;
                P22A.SetExpr(year, () => P17B[year + 1]);
                P22B.SetExpr(year, () => DocumentManager.CurrentDocument.City.C7C[year + 1]);
                P22C.SetExpr(year, () => P18E[year + 1]);
            }
        }

        private static double NaNto0(double num)
        {
            if (double.IsNaN(num))
            {
                return 0;
            }
            return num;
        }

        public void SetQuestionnaire()
        {
            string[] qs1 = { "PQ01", "PQ02", "PQ03", "PQ04", "PQ05", "PQ06" };
            string[] qs2 = { "PQ07", "PQ08", "PQ09", "PQ10", "PQ11", "PQ12", "PQ13", "PQ14" };
            string[] qs3 = { "PQ15", "PQ16", "PQ17", "PQ18", "PQ19", "PQ20" };
            string[] qs4 = { "PQ21", "PQ22", "PQ23", "PQ24", "PQ25", "PQ26", "PQ27", "PQ28", "PQ29" };
            string[] qs5 = { "PQ30", "PQ31", "PQ32", "PQ33", "PQ34", "PQ35", "PQ36", "PQ37", "PQ38", "PQ39" };

            var qs = qs1.Union(qs2).Union(qs3).Union(qs4).Union(qs5).ToList();
            qs.ForEach(x =>
            {
                if (!Questionnaire.Keys.Contains(x))
                {
                    Questionnaire.Add(x, 0);
                }
            });
        }
    }

    [Serializable]
    public class ProjectCollection : List<ProjectStatistics>
    {
        public ProjectStatistics this[string name]
        {
            get
            {
                return this.First(x => x.P1A == name);
            }
        }
    }
}
