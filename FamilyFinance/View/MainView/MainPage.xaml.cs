using FamilyFinance.Model;
using FamilyFinance.WebApi.Service;
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

namespace FamilyFinance.View.MainView
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public User currentUser { get; set; } = DataManager.CurrentUser;
        public MainPage()
        {
            InitializeComponent();
        }

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
    }
}
