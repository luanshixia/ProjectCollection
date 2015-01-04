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
    /// QuestionPage.xaml 的交互逻辑
    /// </summary>
    public partial class QuestionPage : Page
    {
        public QuestionPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //flip1.Slides.Add(new TextBlock { Text = "hello" });
            //flip1.Slides.Add(new TextBlock { Text = "nihao" });
            //flip1.ReadyControl();
            //flip1.CurrentSlideNumber = 0;

            string type = NavigationManager.GetQueryString("type");
            if (type == "city")
            {
                var answers = DocumentManager.CurrentDocument.City.Questionnaire;
                QuestionnaireManager.Questions.Where(x => x.Name.StartsWith("C")).ToList().ForEach(x => flip1.Slides.Add(new QuestionControl(x.Name, answers)));
                flip1.ReadyControl();
                flip1.CurrentSlideNumber = 0;
            }
            else if (type == "project")
            {
                int id = Convert.ToInt32(NavigationManager.GetQueryString("id"));
                var answers = DocumentManager.CurrentDocument.Projects[id].Questionnaire;
                QuestionnaireManager.Questions.Where(x => x.Name.StartsWith("P")).ToList().ForEach(x => flip1.Slides.Add(new QuestionControl(x.Name, answers, true)));
                flip1.ReadyControl();
                flip1.CurrentSlideNumber = 0;
            }
        }

        public FrameworkElement GetContent(string type, int id)
        {
            FlipPresenter flip = new FlipPresenter();
            if (type == "city")
            {
                var answers = DocumentManager.CurrentDocument.City.Questionnaire;
                QuestionnaireManager.Questions.Where(x => x.Name.StartsWith("C")).ToList().ForEach(x => flip.Slides.Add(new QuestionControl(x.Name, answers)));
                var questionSummary = new QuestionSummary();
                flip.Slides.Add(questionSummary);
                flip.ReadyControl();
                flip.CurrentSlideNumber = 0;
                flip.SlideChanged += (s, arg) =>
                {
                    if (flip.CurrentSlide == questionSummary)
                    {
                        var points = answers.Sum(x => QuestionnaireManager.Questions[x.Key].Options[x.Value].Point) / 16.0 * 10;
                        questionSummary.Update(10, points);
                    }
                };
            }
            else if (type == "project")
            {
                var proj = DocumentManager.CurrentDocument.Projects[id];
                var answers = proj.Questionnaire;
                QuestionnaireManager.Questions.Where(x => x.Name.StartsWith("P")).ToList().ForEach(x => flip.Slides.Add(new QuestionControl(x.Name, answers, true)));
                var questionSummary = new QuestionSummary();                

                PropertyTable tablePS = new PropertyTable { SectionNumber = "2.S", Title = "场景得分 Senario Scores", Brush_ResultCell = Brushes.DarkRed, Brush_TitleCell = FindResource("gradient_red") as Brush, Margin = new Thickness(0, 20, 0, 0) };
                tablePS.Rows.Add("EnvironmentScenario", Tuple.Create<object, object>(proj, 0.0));
                tablePS.Rows.Add("EconomicScenario", Tuple.Create<object, object>(proj, 0.0));
                tablePS.Rows.Add("RevenueScenario", Tuple.Create<object, object>(proj, 0.0));
                tablePS.Rows.Add("Social", Tuple.Create<object, object>(proj, 0.0));
                tablePS.Rows.Add("PovertyScenario", Tuple.Create<object, object>(proj, 0.0));
                tablePS.LockRows("EnvironmentScenario", "EconomicScenario", "RevenueScenario", "Social", "PovertyScenario");
                tablePS.Render();

                flip.Slides.Add(questionSummary);
                flip.Slides.Add(tablePS);
                flip.ReadyControl();
                flip.CurrentSlideNumber = 0;

                flip.SlideChanged += (s, arg) =>
                {
                    if (flip.CurrentSlide == questionSummary)
                    {
                        var points = answers.Sum(x => QuestionnaireManager.Questions[x.Key].Options[x.Value].Point) / 113.0 * 10;
                        questionSummary.Update(10, points);
                    }
                    else if (flip.CurrentSlide == tablePS)
                    {
                        tablePS.UpdateData();
                    }
                };
            }
            return flip;
        }

        private void ImageButton_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow.Current.ToggleFullscreen();
        }

        private void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Navigate("TileMainPage.xaml");
        }
    }
}
