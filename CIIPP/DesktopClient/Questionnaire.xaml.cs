using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using CIIPP;

namespace DesktopClient
{
    /// <summary>
    /// Questionnaire.xaml 的交互逻辑
    /// </summary>
    public partial class Questionnaire : Page
    {
        private TextBlock tbTotal = new TextBlock();
        private Dictionary<string, ComboBox> lists = new Dictionary<string, ComboBox>();

        public Questionnaire()
        {
            InitializeComponent();
            tbTotal.FontSize = 18;
            tbTotal.Foreground = new SolidColorBrush(Colors.Red);
            tbTotal.Text = "0";
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Button btnToQ1 = new Button { Content = "演示模式", Width = 200, HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(0, 0, 0, 20) };
            btnToQ1.Click += (s, arg) => NavigationManager.Navigate("QuestionPage.xaml", "type=city");
            layoutRoot.Children.Add(btnToQ1);

            string[] questions = { "CQ01", "CQ02", "CQ03", "CQ04", "CQ05", "CQ06", "CQ07" };
            foreach (var q in questions)
            {
                StackPanel panel = new StackPanel { Margin = new Thickness(0, 0, 0, 20) };
                layoutRoot.Children.Add(panel);

                TextBlock tb1 = new TextBlock();
                tb1.Foreground = new SolidColorBrush(Colors.White);
                tb1.Text = string.Format("{0} . {1}", QuestionnaireManager.Questions[q].DisplayName, QuestionnaireManager.Questions[q].GroupName);
                panel.Children.Add(tb1);

                TextBlock tb2 = new TextBlock { Width = 400, HorizontalAlignment = HorizontalAlignment.Left };
                tb2.Foreground = new SolidColorBrush(Colors.Black);
                tb2.TextWrapping = TextWrapping.Wrap;
                tb2.Text = string.Format("{0}: {1}", q, QuestionnaireManager.Questions[q].English);
                panel.Children.Add(tb2);

                ComboBox box = new ComboBox { HorizontalAlignment = HorizontalAlignment.Left };
                box.Foreground = new SolidColorBrush(Colors.Black);
                box.Width = 400;
                QuestionnaireManager.Questions[q].Options.ForEach(x => box.Items.Add(x.English));
                box.SelectedIndex = DocumentManager.CurrentDocument.City.Questionnaire[q];
                panel.Children.Add(box);

                string qq = q; // 循环变量要复制后才能用于匿名函数。
                box.SelectionChanged += (s, arg) =>
                {
                    DocumentManager.CurrentDocument.City.Questionnaire[qq] = (s as ComboBox).SelectedIndex;
                    UpdatePoints();
                };

                lists.Add(q, box);
            }

            StackPanel layout2 = new StackPanel();
            layout2.Orientation = Orientation.Horizontal;
            layout2.Margin = new Thickness(0, 20, 0, 0);
            TextBlock tbPoint = new TextBlock();
            tbPoint.FontSize = 18;
            tbPoint.Text = "统计得分：";
            layout2.Children.Add(tbPoint);
            layout2.Children.Add(tbTotal);
            layoutRoot.Children.Add(layout2);

            UpdatePoints();
        }

        private void UpdatePoints()
        {
            tbTotal.Text = lists.Sum(x => QuestionnaireManager.Questions[x.Key].Options[x.Value.SelectedIndex].Point).ToString();
        }
    }
}
