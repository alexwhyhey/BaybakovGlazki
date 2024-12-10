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
            CBoxType.ItemsSource = currentAgentsType;
        }

        private void UpdateAgent()
        {
            var currentAgents = BaybakovGlazkiSaveEntities.GetContext().Agent.ToList();
            var currentAgentsType = BaybakovGlazkiSaveEntities.GetContext().AgentType.ToList();

            currentAgents = currentAgents.Where(p => (Convert.ToInt32(p.AgentTypeID) == CBoxType.SelectedIndex+1)).ToList();

            switch(CBoxSorting.SelectedIndex)
            {
                case 0: break;
            }

            AgentListView.ItemsSource = currentAgents;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage());
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void CBoxSorting_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CBoxType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgent();
        }
    }
}
