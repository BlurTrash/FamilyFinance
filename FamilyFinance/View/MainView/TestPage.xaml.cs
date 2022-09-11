using FamilyFinance.Model;
using FamilyFinance.WebApi.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace FamilyFinance.View.MainView
{
    /// <summary>
    /// Логика взаимодействия для TestPage.xaml
    /// </summary>
    public partial class TestPage : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<CurrencyRate> RatesList { get; set; } = new ObservableCollection<CurrencyRate>();
        public User currentUser { get; set; } = DataManager.CurrentUser;
        public TestPage()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string text = null;

            Func<Client, Task> manipulatonDataMethod = async (client) =>
            {
                var result = await client.ApiAccountGetTextAsync();

                if (!string.IsNullOrEmpty(result.Value))
                {
                    text = result.Value;
                }
            };

            var res = await ClientManager.ManipulatonData(manipulatonDataMethod);

            txBox.Text = text;
        }

        private async void Button_Click_Admin(object sender, RoutedEventArgs e)
        {
            string text = null;

            Func<Client, Task> manipulatonDataMethod = async (client) =>
            {
                var result = await client.ApiAccountGetAdminRoleAsync();

                if (!string.IsNullOrEmpty(result.Value))
                {
                    text = result.Value;
                }
            };

            var res = await ClientManager.ManipulatonData(manipulatonDataMethod);

            txBoxAdminRole.Text = text;
        }

        private async void Button_Click_User(object sender, RoutedEventArgs e)
        {
            string text = null;

            Func<Client, Task> manipulatonDataMethod = async (client) =>
            {
                var result = await client.ApiAccountGetUserRoleAsync();

                if (!string.IsNullOrEmpty(result.Value))
                {
                    text = result.Value;
                }
            };

            var res = await ClientManager.ManipulatonData(manipulatonDataMethod);

            txBoxUserRole.Text = text;
        }

    

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Func<Client, Task> manipulatonDataMethod = async (client) =>
            {
                var result = await client.ApiCurrencyGetAsync();
                RatesList = new ObservableCollection<CurrencyRate>(result);
            };

            var res = await ClientManager.ManipulatonData(manipulatonDataMethod);
        }
    }
}
