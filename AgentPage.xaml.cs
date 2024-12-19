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
        int CountRecords;
        int CountPage;
        int CurrentPage = 0;

        List<Agent> CurrentPageList = new List<Agent>();
        List<Agent> TableList;
        public AgentPage()
        {
            InitializeComponent();

            var currentAgents = BaybakovGlazkiSaveEntities.GetContext().Agent.ToList();
            var currentAgentsType = BaybakovGlazkiSaveEntities.GetContext().AgentType.ToList();

            AgentListView.ItemsSource = currentAgents;
            CBoxType.SelectedIndex = 0;
            CBoxSorting.SelectedIndex = 0;
        }

        private void ChangePage(int direction, int? selectedPage)
        {
            CurrentPageList.Clear();
            CountRecords = TableList.Count;

            if (CountRecords % 10 > 0)
            {
                CountPage = CountRecords / 10 + 1;
            }
            else
            {
                CountPage = CountRecords / 10;
            }

            Boolean ifUpdate = true;

            int min;

            if (selectedPage.HasValue)
            {
                if (selectedPage >= 0 && selectedPage <= CountPage)
                {
                    CurrentPage = (int)selectedPage;
                    min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                    for (int i = CurrentPage * 10; i < min; i++)
                    {
                        CurrentPageList.Add(TableList[i]);
                    }
                }
            }
            else
            {
                switch (direction)
                {
                    case 1:
                        if (CurrentPage > 0)
                        {
                            CurrentPage--;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            ifUpdate = false;
                        }
                        break;
                    case 2:
                        if (CurrentPage < CountPage - 1)
                        {
                            CurrentPage++;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            ifUpdate = false;
                        }
                        break;
                }
            }

            if (ifUpdate)
            {
                PageListBox.Items.Clear();

                for (int i = 1; i <= CountPage; i++)
                {
                    PageListBox.Items.Add(i);
                }
                PageListBox.SelectedIndex = CurrentPage;

                AgentListView.ItemsSource = CurrentPageList;
                AgentListView.Items.Refresh();

                min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                TBCount.Text = min.ToString();
                TBAllRecords.Text = " из " + CountRecords.ToString();
            }
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
            TableList = currentAgents;

            ChangePage(0, 0);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Manager.MainFrame.Navigate(new AddEditPage());
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

        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1, null);
        }

        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(2, null);
        }

        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangePage(0, Convert.ToInt32(PageListBox.SelectedItem.ToString()) - 1);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage((sender as Button).DataContext as Agent));
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null));
        }
    }
}
