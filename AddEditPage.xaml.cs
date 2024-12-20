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
        private CollectionViewSource _productsView;
        private CollectionViewSource _sales;

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
            DataContext = _productsView;

            _sales = new CollectionViewSource();
            var saless = BaybakovGlazkiSaveEntities.GetContext().ProductSale.Where(p => p.AgentID == currentAgent.ID).ToList();

            _sales.Source = saless;
            Realze.ItemsSource = _sales.View;
            Realze.DisplayMemberPath = "DataName";
            Realze.SelectedValuePath = "AgentID";

            _productsView = new CollectionViewSource();
            var products = BaybakovGlazkiSaveEntities.GetContext().Product.ToList();

            _productsView.Source = products;
            Products.ItemsSource = _productsView.View;
            Products.DisplayMemberPath = "Title";
            Products.SelectedValuePath = "ID";
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
            else if (Convert.ToInt32(currentAgent.INN.Count()) != 9)
            {
                errors.AppendLine("ИНН должен быть 9-значным");
            }
            if (string.IsNullOrWhiteSpace(currentAgent.KPP))
                errors.AppendLine("Укажите КПП агента");
            else if (Convert.ToInt64(currentAgent.KPP.Count()) != 9)
            {
                errors.AppendLine("КПП должен быть 9-значным");
            }
            if (string.IsNullOrWhiteSpace(currentAgent.Phone))
                errors.AppendLine("Укажите телефон агента");
            else
            {
                string ph = currentAgent.Phone.Replace("(", "").Replace("-", "").Replace("+", "").Replace(" ", "").Replace(")", "");

                if (((ph[1] == '9' || ph[1] == '4' || ph[1] == '8') && ph.Length != 11) ||
                     (ph[1] == '3' && ph.Length != 12) ||
                     ph.Length > 12 ||
                     ph[0] != '7')
                     errors.AppendLine("Укажите правильно телефон агента");
            }

            if (string.IsNullOrWhiteSpace(currentAgent.Email))
                errors.AppendLine("Укажите почту агента");
            else if (!currentAgent.Email.Contains("@") || !currentAgent.Email.Contains("."))
            {
                errors.AppendLine("Правильно укажите почту агента");
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            currentAgent.AgentTypeID = ComboType.SelectedIndex + 1;
            if (currentAgent.ID == 0)
                BaybakovGlazkiSaveEntities.GetContext().Agent.Add(currentAgent);

            try
            {
                BaybakovGlazkiSaveEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");

                Manager.MainFrame.Navigate(new AgentPage());

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

                        Manager.MainFrame.Navigate(new AgentPage());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }

                }
            }
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

        private void add_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            
            if (Products.SelectedItem == null)
                errors.AppendLine("Укажите продукт");
            if (string.IsNullOrWhiteSpace(ProductCountTB.Text))
                errors.AppendLine("Укажите количество продуктов");
            
            bool isProductCountDigits = true;

            for (int i = 0; i < ProductCountTB.Text.Length; i++)
            {
                if (ProductCountTB.Text[i] < '0' || ProductCountTB.Text[i] > '9')
                {
                    isProductCountDigits = false;
                }
            }
            if (!isProductCountDigits)
                errors.AppendLine("Укажите численное положительное продуктов");
            if (ProductCountTB.Text == "0")
            {
                errors.AppendLine("Укажите количество продаж");
            }
            if (string.IsNullOrWhiteSpace(saleData.Text))
                errors.AppendLine("Укажите дату продажи");
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString()); return;
            }

            var _currentProductSale = new ProductSale();

            _currentProductSale.AgentID = currentAgent.ID;
            _currentProductSale.ProductID = Products.SelectedIndex + 1; _currentProductSale.ProductCount = Convert.ToInt32(ProductCountTB.Text);
            _currentProductSale.SaleDate = Convert.ToDateTime(saleData.Text);

            if (_currentProductSale.ID == 0)
                BaybakovGlazkiSaveEntities.GetContext().ProductSale.Add(_currentProductSale);

            try
            {
                BaybakovGlazkiSaveEntities.GetContext().SaveChanges();

                MessageBox.Show("информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        private void delete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    if (Realze.SelectedItem != null)
                    {
                        ProductSale selectedHistory = (ProductSale)Realze.SelectedItem;
                        BaybakovGlazkiSaveEntities.GetContext().ProductSale.Remove(selectedHistory);
                        BaybakovGlazkiSaveEntities.GetContext().SaveChanges();

                        MessageBox.Show("Информация удалена!");
                        Manager.MainFrame.GoBack();
                    }
                    else
                    {
                        MessageBox.Show("Пожалуйста, выберите запись для удаления.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }
        private void searchprod_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = searchprod.Text.ToLower();

            _productsView.View.Filter = o =>
            {
                Product p = o as Product;
                return p != null && p.Title.ToLower().Contains(searchText);
            };
        }
    }
}
