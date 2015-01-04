using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIIPP
{
    public class ProjectGroup
    {
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

        /// <summary>
        /// 新增贷款
        /// </summary>
        public YearwiseExpression P22A { get; set; }

        /// <summary>
        /// 支出上限
        /// </summary>
        public YearwiseExpression C8A { get; set; }

        /// <summary>
        /// 现有支出
        /// </summary>
        public YearwiseExpression C8B { get; set; }

        /// <summary>
        /// 估算财力
        /// </summary>
        public YearwiseExpression C8C_1 { get; set; }

        /// <summary>
        /// 投资预算
        /// </summary>
        public YearwiseExpression C8D { get; set; }

        /// <summary>
        /// 现有年还债能力
        /// </summary>
        public YearwiseExpression C7C { get; set; }

        /// <summary>
        /// 估算最大还债能力
        /// </summary>
        public YearwiseExpression C8E { get; set; }

        public ProjectCollection Projects { get; private set; }

        public ProjectGroup(IEnumerable<ProjectStatistics> projects)
        {
            Projects = new ProjectCollection();
            projects.ToList().ForEach(x => Projects.Add(x));
            int c02 = DocumentManager.CurrentDocument.City.C02;

            P20A = new YearwiseExpression(c02 + 6, 5, false);
            P20B = new YearwiseExpression(c02 + 6, 5, false);
            P20C = new YearwiseExpression(c02 + 6, 5, false);
            P20D = new YearwiseExpression(c02 + 6, 5, false);
            P20E = new YearwiseExpression(c02 + 6, 5, false);
            P20F = new YearwiseExpression(c02 + 6, 5, false);
            P22A = new YearwiseExpression(c02 + 6, 5, false);

            C8A = new YearwiseExpression(c02 + 6, 5, false);
            C8B = new YearwiseExpression(c02 + 6, 5, false);
            C8C_1 = new YearwiseExpression(c02 + 6, 5, false);
            C8D = new YearwiseExpression(c02 + 6, 5, false);
            C7C = new YearwiseExpression(c02 + 6, 5, false);
            C8E = new YearwiseExpression(c02 + 6, 5, false);

            this.SetExpressions();
        }

        public void SetExpressions()
        {
            for (int i = 1; i <= 6; i++)
            {
                int y = i;
                P20A.SetExpr(y, () => Projects.Sum(x => x.P20A[y]));
                P20B.SetExpr(y, () => Projects.Sum(x => x.P20B[y]));
                P20C.SetExpr(y, () => Projects.Sum(x => x.P20C[y]));
                P20D.SetExpr(y, () => Projects.Sum(x => x.P20D[y]));
                P20E.SetExpr(y, () => Projects.Sum(x => x.P20E[y]));
                P20F.SetExpr(y, () => Projects.Sum(x => x.P20F[y]));
                P22A.SetExpr(y, () => Projects.Sum(x => x.P22A[y]));

                var city = DocumentManager.CurrentDocument.City;
                C8A.SetExpr(y, () => city.C8A[y]);
                C8B.SetExpr(y, () => city.C8B[y]);
                C8C_1.SetExpr(y, () => city.C8C[y] * city.C14A);
                C8D.SetExpr(y, () => city.C8D[y]);
                C7C.SetExpr(y, () => city.C7C[y]);
                C8E.SetExpr(y, () => city.C8E[y]);
            }
            P20G = new YearwiseSum(P20A, P20B, P20C, P20D, P20E, P20F);
        }
    }
}
