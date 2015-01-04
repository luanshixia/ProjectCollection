using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CIIPP;
using System.Windows;

namespace DesktopClient
{
    public static class Nav
    {
        public static void City(int slide)
        {
            NavigationManager.Navigate("RichCityPage.xaml", "slide=" + slide);
            if (RichCityPage.Current != null)
            {
                RichCityPage.Current.SetSlidePos(slide);
            }            
        }

        public static void Financial(int slide)
        {
            NavigationManager.Navigate("RichFinancialPage.xaml", "slide=" + slide);
            if (RichFinancialPage.Current != null)
            {
                RichFinancialPage.Current.SetSlidePos(slide);
            }
        }

        public static void Project(int slide)
        {
            if (CheckProjectExist())
            {
                NavigationManager.Navigate("RichProjectPage.xaml", "slide=" + slide);
                if (RichProjectPage.Current != null)
                {
                    RichProjectPage.Current.SetSlidePos(slide);
                }
            }
        }

        public static void Pip(int slide)
        {
            NavigationManager.Navigate("PipPage.xaml", "slide=" + slide);
            if (PipPage.Current != null)
            {
                PipPage.Current.SetSlidePos(slide);
            }
        }

        public static bool CheckProjectExist()
        {
            if (DocumentManager.CurrentDocument.Projects.Count > 0)
            {
                return true;
            }
            else
            {
                MessageBox.Show("还没有创建项目，请先创建项目。", DesktopClient.Properties.Resources.AppName, MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
        }
    }

    public static class TileNav
    {
        public static void City(int slide)
        {
            NavigationManager.Navigate("TileContentPage.xaml", "part=city&slide=" + slide);
        }

        public static void Financial(int slide)
        {
            NavigationManager.Navigate("TileContentPage.xaml", "part=financial&slide=" + slide);
        }

        public static void Project(int slide)
        {
            if (Nav.CheckProjectExist())
            {
                NavigationManager.Navigate("TileContentPage.xaml", "part=project&slide=" + slide);
            }
        }

        public static void ProjectById(int projId)
        {
            if (Nav.CheckProjectExist())
            {
                NavigationManager.Navigate("TileContentPage.xaml", "part=project&id=" + projId);
            }
        }

        public static void Pip(int slide)
        {
            NavigationManager.Navigate("TileContentPage.xaml", "part=pip&slide=" + slide);
        }

        //public static void Question(string type, int id = 0)
        //{
        //    NavigationManager.Navigate("TileContentPage.xaml", string.Format("part={0}_q&id={1}", type, id));
        //}

        public static void Summary()
        {
            NavigationManager.Navigate("TileContentPage.xaml","part=summary");
        }
    }

    public static class Caption
    {
        private static string[] _cityTitles = { "当地政府关键数据", "当地政府收入", "当地政府支出", "当地政府资产", "当地政府债务", "城市信用等级评估" };
        private static string[] _financialTitles = { "收入与支出", "总投资能力", "贷款条件假设", "其他条件假设", "其他条件假设", "图表：预算预测", "继续浏览" };
        private static string[] _projectTitles = { "项目基本信息", "成本和资金来源", "资金来源情况", "项目问卷", "项目附加信息", "项目对城市经济的影响", "项目对城市经济的影响", "图表：项目对投资总量的影响", "资金来源", "图表：所需投资vs支出上限", "图表：所需投资vs偿还能力", "继续浏览" };
        private static string[] _pipTitles = { "图表", "总体情况", "项目详情" };

        public static string City(int slide)
        {
            return _cityTitles[slide];
        }

        public static string Financial(int slide)
        {
            return _financialTitles[slide];
        }

        public static string Project(int slide)
        {
            return _projectTitles[slide];
        }

        public static string Pip(int slide)
        {
            return _pipTitles[slide];
        }
    }
}
