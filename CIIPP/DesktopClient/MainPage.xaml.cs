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
    /// Page1.xaml 的交互逻辑
    /// </summary>
    public partial class MainPage : Page
    {
        public static MainPage Current { get; private set; }

        public MainPage()
        {
            InitializeComponent();            

            CityStatistics.CurrencyEnum.ToList().ForEach(x => this.comboBox1.Items.Add(x));
            CityStatistics.MultipleEnum.ToList().ForEach(x => this.comboBox2.Items.Add(x));

            FileBasedUpdate();

            Current = this;
        }

        private void CityAssess_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Navigate("CityPage.xaml");
        }

        private void FinancialAssess_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Navigate("CityFinancialPage.xaml");
        }

        private void BtnGroupTable_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Navigate("GroupSummaryPage.xaml");
        }

        private void SummaarizeTable_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Navigate("SummaryPage.xaml");
        }

        private void AddProject_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Navigate("PipPage.xaml");
        }

        private void btnNewProj_Click(object sender, RoutedEventArgs e)
        {
            NewProj np = new NewProj { Owner = MainWindow.Current };
            if (np.ShowDialog() == true)
            {
                DocumentManager.CurrentDocument.Projects.Add(new ProjectStatistics(np.txtProjName.Text, np.txtProjLocation.Text));
                UpdateProjectList();
            }
        }

        private void btnViewProj_Click(object sender, RoutedEventArgs e)
        {
            if (listProjects.SelectedIndex >= 0)
            {
                NavigationManager.Navigate("ProjectPage.xaml", "id=" + listProjects.SelectedIndex);
            }
        }

        private void btnDelProj_Click(object sender, RoutedEventArgs e)
        {
            if (listProjects.SelectedIndex >= 0)
            {
                DocumentManager.CurrentDocument.Projects.RemoveAt(listProjects.SelectedIndex);
                UpdateProjectList();
            }
        }

        private void UpdateProjectList()
        {
            listProjects.Items.Clear();
            DocumentManager.CurrentDocument.Projects.ForEach(x => listProjects.Items.Add(x.P1A));
        }

        public void FileBasedUpdate()
        {
            this.DataContext = DocumentManager.CurrentDocument.City;
            UpdateProjectList();

            txtCountry.Text = DocumentManager.CurrentDocument.City.Country;
            txtCity.Text = DocumentManager.CurrentDocument.City.C01;
            txtIntro.Text = DocumentManager.CurrentDocument.City.Intro;
        }

        private void txtCountry_LostFocus(object sender, RoutedEventArgs e)
        {
            DocumentManager.CurrentDocument.City.Country = txtCountry.Text;
        }

        private void txtCity_LostFocus(object sender, RoutedEventArgs e)
        {
            DocumentManager.CurrentDocument.City.C01 = txtCity.Text;
        }

        private void txtIntro_LostFocus(object sender, RoutedEventArgs e)
        {
            DocumentManager.CurrentDocument.City.Intro = txtIntro.Text;
        }

        private void listProjects_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listProjects.SelectedIndex >= 0)
            {
                NavigationManager.Navigate("ProjectPage.xaml", "id=" + listProjects.SelectedIndex);
            }
        }
    }
}
