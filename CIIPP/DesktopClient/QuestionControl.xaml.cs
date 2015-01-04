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
    /// QuestionControl.xaml 的交互逻辑
    /// </summary>
    public partial class QuestionControl : UserControl
    {
        public QuestionControl()
        {
            InitializeComponent();
        }

        public QuestionControl(string questionName, Dictionary<string, int> answers, bool useChinese = false)
        {
            InitializeComponent();

            this.DataContext = QuestionnaireManager.Questions[questionName];
            lstOptions.SetBinding(ListBox.SelectedIndexProperty, new Binding(string.Format("[{0}]", questionName)) { Source = answers });
            if (useChinese)
            {
                UseChinese();
            }
        }

        public void UseChinese()
        {
            tbQuestion.SetBinding(TextBlock.TextProperty, new Binding("Chinese"));
            lstOptions.DisplayMemberPath = "Chinese";
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
