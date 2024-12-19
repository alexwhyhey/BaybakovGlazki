using Microsoft.Win32;
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
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        private Agent currentAgent = new Agent();

        public AddEditPage(Agent SelectedService)
        {
            InitializeComponent();

            if (SelectedService != null)
            {
                currentAgent = SelectedService;
                DelBtn.Visibility = Visibility.Visible;
                ComboType.SelectedIndex = currentAgent.AgentTypeID - 1;
            } else
            {
                DelBtn.Visibility = Visibility.Hidden;
            }

            DataContext = currentAgent;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(currentAgent.Title))
                errors.AppendLine("Укажите наименование услуги");
            if (string.IsNullOrWhiteSpace(currentAgent.Address))
                errors.AppendLine("Укажите адрес агента");
            if (string.IsNullOrWhiteSpace(currentAgent.DirectorName))
                errors.AppendLine("Укажите ФИО директора");
            if (ComboType.SelectedItem == null)
                errors.AppendLine("Укажите тип агента");
            if (string.IsNullOrWhiteSpace(currentAgent.Priority.ToString()))
                errors.AppendLine("Укажите приоритет агента");
            if (currentAgent.Priority <= 0)
                errors.AppendLine("Укажите положительный приоритет агента");
            if (string.IsNullOrWhiteSpace(currentAgent.INN))
                errors.AppendLine("Укажите ИНН агента");
            if (string.IsNullOrWhiteSpace(currentAgent.KPP))
                errors.AppendLine("Укажите КПП агента");
            if (string.IsNullOrWhiteSpace(currentAgent.Phone))
                errors.AppendLine("Укажите телефон агента");
            else
            {
                string ph = currentAgent.Phone.Replace("(", "").Replace("-", "").Replace("+", "");

                if (((ph[1] == '9' || ph[1] == '4' || ph[1] == '8') && ph.Length != 11) ||
                     (ph[1] == '3' && ph.Length != 12))
                    errors.AppendLine("Укажите правильно телефон агента");
            }

            if (string.IsNullOrWhiteSpace(currentAgent.Email))
                errors.AppendLine("Укажите почту агента");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            currentAgent.AgentTypeID = ComboType.SelectedIndex - 1;
            if (currentAgent.ID == 0)
                BaybakovGlazkiSaveEntities.GetContext().Agent.Add(currentAgent);

            try
            {
                BaybakovGlazkiSaveEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");

                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }

        private void DelBtn_Click(object sender, RoutedEventArgs e)
        {
            var currentProductSale = BaybakovGlazkiSaveEntities.GetContext().ProductSale.ToList();
            var currentPriorityHistory = BaybakovGlazkiSaveEntities.GetContext().AgentPriorityHistory.ToList();

            currentProductSale = currentProductSale.Where(p => p.AgentID == currentAgent.ID).ToList();
            currentPriorityHistory = currentPriorityHistory.Where(p => p.AgentID == currentAgent.ID).ToList();

            if (currentProductSale.Count != 0)
            {
                MessageBox.Show("Невозможно удалить, существуют продажи агента");
            }
            else
            {
                if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        BaybakovGlazkiSaveEntities.GetContext().Agent.Remove(currentAgent);

                        foreach (var item in currentPriorityHistory)
                        {

                            BaybakovGlazkiSaveEntities.GetContext().AgentPriorityHistory.Remove(item);
                        }

                        BaybakovGlazkiSaveEntities.GetContext().SaveChanges();

                        //UpdateAgent();

                        Manager.MainFrame.GoBack();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                    AgentListView.ItemsSource = BaybakovGlazkiSaveEntities.GetContext().Agent.ToList();

                }
                AgentListView.ItemsSource = BaybakovGlazkiSaveEntities.GetContext().Agent.ToList();
            }
            AgentListView.ItemsSource = BaybakovGlazkiSaveEntities.GetContext().Agent.ToList();
        }

        private void ChangePictureBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == true)
            {
                currentAgent.Logo = ofd.FileName;

                LogoImage.Source = new BitmapImage(new Uri(ofd.FileName));
            }
        }
    }
}
