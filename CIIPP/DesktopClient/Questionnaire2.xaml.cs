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
    /// Questionnaire2.xaml 的交互逻辑
    /// </summary>
    public partial class Questionnaire2 : Page
    {
        private int id;
        private TextBlock tbTotal = new TextBlock();
        private Dictionary<String, ComboBox> lists = new Dictionary<string, ComboBox>();

        public Questionnaire2()
        {
            InitializeComponent();
            tbTotal.FontSize = 18;
            tbTotal.Foreground = new SolidColorBrush(Colors.Red);
            tbTotal.Text = "0";
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            id = Convert.ToInt32(NavigationManager.GetQueryString("id"));

            Button btnToQ1 = new Button { Content = "演示模式", Width = 200, HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(0, 0, 0, 20) };
            btnToQ1.Click += (s, arg) => NavigationManager.Navigate("QuestionPage.xaml", "type=project&id=" + id.ToString());
            layoutRoot.Children.Insert(0, btnToQ1);

            string[] qs1 = { "PQ01", "PQ02", "PQ03", "PQ04", "PQ05", "PQ06" };
            setQuestionnaire(qs1, layoutRoot1);

            string[] qs2 = { "PQ07", "PQ08", "PQ09", "PQ10", "PQ11", "PQ12", "PQ13", "PQ14" };
            setQuestionnaire(qs2, layoutRoot2);

            string[] qs3 = { "PQ15", "PQ16", "PQ17", "PQ18", "PQ19", "PQ20" };
            setQuestionnaire(qs3, layoutRoot3);

            string[] qs4 = { "PQ21", "PQ22", "PQ23", "PQ24", "PQ25", "PQ26", "PQ27", "PQ28", "PQ29" };
            setQuestionnaire(qs4, layoutRoot4);

            string[] qs5 = { "PQ30", "PQ31", "PQ32", "PQ33", "PQ34", "PQ35", "PQ36", "PQ37", "PQ38", "PQ39" };
            setQuestionnaire(qs5, layoutRoot5);

            StackPanel layout2 = new StackPanel();
            layout2.Orientation = Orientation.Horizontal;
            layout2.Margin = new Thickness(0, 20, 0, 0);
            TextBlock tbPoint = new TextBlock();
            tbPoint.FontSize = 18;
            tbPoint.Text = "统计得分： ";
            layout2.Children.Add(tbPoint);
            layout2.Children.Add(tbTotal);
            layoutRoot.Children.Add(layout2);

            UpdatePoints();
        }

        void setQuestionnaire(string[] qs, StackPanel layoutroot)
        {
            layoutroot.Margin = new Thickness(0, 20, 0, 0);
            foreach (var q in qs)
            {
                TextBlock tb1 = new TextBlock();
                tb1.Foreground = new SolidColorBrush(Colors.White);
                tb1.Text = string.Format("{0} . {1}", QuestionnaireManager.Questions[q].DisplayName, QuestionnaireManager.Questions[q].GroupName);
                layoutroot.Children.Add(tb1);

                TextBlock tb2 = new TextBlock { Width = 400, HorizontalAlignment = HorizontalAlignment.Left, TextWrapping = TextWrapping.Wrap };
                tb2.Foreground = new SolidColorBrush(Colors.Black);
                tb2.Text = string.Format("{0}: {1}", q, QuestionnaireManager.Questions[q].Chinese);
                layoutroot.Children.Add(tb2);

                ComboBox box = new ComboBox { HorizontalAlignment = System.Windows.HorizontalAlignment.Left };
                box.Foreground = new SolidColorBrush(Colors.Black);
                box.Width = 400;
                QuestionnaireManager.Questions[q].Options.ForEach(x => box.Items.Add(x.Chinese));
                box.SelectedIndex = DocumentManager.CurrentDocument.Projects[id].Questionnaire[q];
                layoutroot.Children.Add(box);

                string qq = q;
                box.SelectionChanged += (s, arg) =>
                {
                    DocumentManager.CurrentDocument.Projects[id].Questionnaire[qq] = (s as ComboBox).SelectedIndex;
                    UpdatePoints();
                };

                lists.Add(q, box);
            }
        }

        private void UpdatePoints()
        {
            //tbTotal.Text = lists.Sum(x => QuestionnaireManager.Questions[x.Key].Options.First(y => y.Chinese == x.Value.SelectedItem.ToString()).Point).ToString();
            tbTotal.Text = lists.Sum(x => QuestionnaireManager.Questions[x.Key].Options[x.Value.SelectedIndex].Point).ToString();
        }
    }
}
