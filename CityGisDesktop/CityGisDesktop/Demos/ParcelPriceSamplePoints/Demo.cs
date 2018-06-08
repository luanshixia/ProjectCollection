using Dreambuild.Extensions;
using Dreambuild.Gis.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Dreambuild.Gis.Desktop.Demos.ParcelPriceSamplePoints
{
    public class Demo : DemoBase
    {
        public Demo()
        {
            Name = "地价样本点选取";
            Description = "一个基于专利的地价样本点选取程序。";
        }

        public override void OnLoad()
        {
            base.OnLoad();

            this.GetDemoTools().ForEach(t =>
            {
                MenuItem mi = new MenuItem { Header = t.Item1 };
                mi.Click += t.Item2;
                MainWindow.Current.ToolsMenu.Items.Add(mi);
            });

            PythonConsole.Current.SetStaticVariable("ga", typeof(GridAnalysis));
            PythonConsole.Current.SetStaticVariable("ppsp", typeof(PPSP));
        }

        [DemoTool("打开工具箱")]
        public void OpenToolbox()
        {
            PPSP.tools();
        }
    }

    public static class PPSP
    {
        public static void tools()
        {
            Dictionary<string, string> tools = new Dictionary<string, string>();
            tools.Add("添加因素点", "ga.addspotui()");
            tools.Add("删除因素点", "ga.delspotui()");
            tools.Add("确定样本点数", "ppsp.nsamples()");
            tools.Add("运行分析", "ppsp.run()");
            tools.Add("重置分析", "ga.resetgrid()");
            tools.Add("重置网格", "ga.readygrid()");
            tools.Add("关闭网格", "ga.hidegrid()");
            PyCmd.toolbox("工具箱", tools);

            GridAnalysis.init();
        }

        public static void run()
        {
            GridAnalysis.clearmark();
            GridAnalysis.readygrid();
            PPSP.pickpoints();
        }

        public static void nsamples()
        {
            UserInput.pause("即将确定样本容量。");
            double[] cellSizesProbabilitys = { 0.2, 0.1, 0.05 };
            int cellSizesProbabilitysOption = UserInput.option("调查预期概率精度", "0.2", "0.1", "0.05");
            double totalSampleProbability = cellSizesProbabilitys[cellSizesProbabilitysOption];

            double[] cellSizesConfidences = { 1.645, 1.960, 2.576 };
            int cellSizesConfidencesOption = UserInput.option("调查样本置信度", "0.90", "0.95", "0.99");
            double totalSampleConfidence = cellSizesConfidences[cellSizesConfidencesOption];

            double[] cellSizesGetSampleProbabilitys = { 0.60, 0.80, 0.90 };
            int cellSizesGetSampleProbabilitysOption = UserInput.option("预计可获取有效样本数据比例", "0.60", "0.80", "0.90");
            double totalGetSampleProbability = cellSizesGetSampleProbabilitys[cellSizesGetSampleProbabilitysOption];

            int gridCount = Gui.InputBox("网格总数", "100").TryParseToInt32();
            int totalSampleCount = (int)Math.Ceiling(totalSampleConfidence * totalSampleConfidence * 0.25 / totalSampleProbability / totalSampleProbability);
            totalSampleCount = (int)Math.Ceiling((double)totalSampleCount * ((double)gridCount / ((double)totalSampleCount + (double)gridCount)));
            totalSampleCount = (int)Math.Ceiling((double)totalSampleCount / totalGetSampleProbability);
            //向上取余
            UserInput.pause("总样本点数为" + totalSampleCount);
        }

        public static void pickpoints()
        {
            UserInput.pause("即将进行分等定级。");
            int levelCount = Gui.InputBox("级别数量", "5").TryParseToInt32();
            var values = GridAnalysis.valuecache.Values.OrderBy(x => x).ToList();
            List<List<double>> grades = null;
            List<double> borderValues = null;
            if (UserInput.option("选择分等定级算法。", "值域均分法", "动态聚类法") == 0)
            {
                double max = values.Max();
                double min = values.Min();
                double delta = (max - min) / levelCount;
                borderValues = Enumerable.Range(0, levelCount).Select(i => min + i * delta).ToList();
                borderValues.Add(max + 0.001 * delta);
                grades = borderValues.PairwiseSelect((x, y) => values.Where(z => x <= z && z < y).ToList()).ToList();
                GradeTheme gt = new GradeTheme("value", borderValues);
                MapControl.Current.GridLayer.ApplyColorTheme(gt);
            }
            else
            {
                var steps = Enumerable.Range(0, 5).ToList();
                steps.Add(1000);
                foreach (var step in steps)
                {
                    var result = Dreambuild.Mathematics.DynamicGrading.Perform(values, levelCount, step);
                    grades = result.Item1;
                    borderValues = result.Item2;
                    GradeTheme gt = new GradeTheme("value", borderValues);
                    MapControl.Current.GridLayer.ApplyColorTheme(gt);
                    UserInput.pause(string.Format("已进行第 {0} 步聚类。", result.Item3));
                }
            }

            UserInput.pause("即将确定样本容量。");
            int totalSampleCount = Gui.InputBox("总样本点数", "100").TryParseToInt32();
            var Es = grades.Select(x => x.StandardDeviation()).ToList();
            var Hs = grades.Select(x => x.Sum()).ToList();
            var EHs = Es.Zip(Hs, (e, h) => e * h).ToList();
            double EHSum = EHs.Sum();
            var proportions = EHs.Select(x => x / EHSum).ToList();
            var sampleCounts = proportions.Select(x => (int)Math.Ceiling(x * totalSampleCount)).ToArray();
            var symbolValues = borderValues.PairwiseSelect((x, y) => (x + y) / 2).ToArray();
            UserInput.pause(string.Format("各级别样本点数：{0}\n各级别标志分值：{1}", string.Join(", ", sampleCounts), string.Join(", ", symbolValues.Select(x => x.ToString("0.00")).ToArray())));

            UserInput.pause("即将开始选点。");
            if (UserInput.option("选择选点算法。", "多重因素法", "单一因素法") == 0)
            {
                Random rand = new Random(0);
                Enumerable.Range(0, levelCount).ForEach(i =>
                {
                    var cells = GridAnalysis.valuecache.Where(z => borderValues[i] <= z.Value && z.Value < borderValues[i + 1]).Select(z => z.Key).ToList();
                    var searches = Enumerable.Range(0, 1000).Select(x =>
                    {
                        var pts = Enumerable.Range(0, sampleCounts[i]).Select(j => cells[rand.Next(0, cells.Count - 1)]).ToList();
                        return System.Tuple.Create(pts, GridAnalysis.evaluate(pts, symbolValues[i]));
                    }).ToList();
                    var target = searches.First(x => x.Item2 == searches.Min(y => y.Item2));
                    target.Item1.ForEach(p => GridAnalysis.markcell(p));
                });
            }
            else
            {
                Enumerable.Range(0, levelCount).ForEach(i =>
                {
                    var cells = GridAnalysis.valuecache.Where(z => borderValues[i] <= z.Value && z.Value < borderValues[i + 1]).Select(z => z.Key).ToList();
                    var targets = cells.OrderBy(p => Math.Abs(GridAnalysis.valuecache[p] - symbolValues[i])).Take(sampleCounts[i]).ToList();
                    targets.ForEach(p => GridAnalysis.markcell(p));
                });
            }
        }
    }
}
