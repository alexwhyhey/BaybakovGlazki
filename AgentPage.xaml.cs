using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Baybakov_Glazki
{
    /// <summary>
    /// Логика взаимодействия для AgentPage.xaml
    /// </summary>
    public partial class AgentPage : Page
    {
        public AgentPage()
        {
            InitializeComponent();

            var currentAgents = BaybakovGlazkiSaveEntities.GetContext().Agent.ToList();
            var currentAgentsType = BaybakovGlazkiSaveEntities.GetContext().AgentType.ToList();

            AgentListView.ItemsSource = currentAgents;
            CBoxType.SelectedIndex = 0;
            CBoxSorting.SelectedIndex = 0;
        }

        private void UpdateAgent()
        {
            var currentAgents = BaybakovGlazkiSaveEntities.GetContext().Agent.ToList();

            if (CBoxType.SelectedIndex != 0)
            {
                currentAgents = currentAgents.Where(p => (Convert.ToInt32(p.AgentTypeID) == CBoxType.SelectedIndex)).ToList(); 
            }

            switch (CBoxSorting.SelectedIndex)
            {
                case 0: 
                    break;
                case 1:
                    currentAgents = currentAgents.OrderBy(p => p.Title).ToList();
                    break;
                case 2:
                    currentAgents = currentAgents.OrderByDescending(p => p.Title).ToList();
                    break;
                case 3:
                    //currentAgents = currentAgents.OrderBy(p => p.Discount).ToList();
                    break;
                case 4:
                    //currentAgents = currentAgents.OrderByDescending(p => p.Discount).ToList();
                    break;
                case 5:
                    currentAgents = currentAgents.OrderBy(p => p.Priority).ToList();
                    break;
                case 6:
                    currentAgents = currentAgents.OrderByDescending(p => p.Priority).ToList();
                    break;
            }

            if (!string.IsNullOrWhiteSpace(TBoxSearch.Text)) {
                currentAgents = currentAgents.Where(p => p.Title.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList().
                    Union(currentAgents.Where(p => p.Email.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList()).ToList().
                    Union(currentAgents.Where(p => p.Phone.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList()).ToList();
            }

            AgentListView.ItemsSource = currentAgents;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage());
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateAgent();
        }

        private void CBoxSorting_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgent();
        }

        private void CBoxType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgent();
        }
    }
}
