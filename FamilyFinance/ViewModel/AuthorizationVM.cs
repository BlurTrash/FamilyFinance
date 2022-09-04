using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using FamilyFinance.WebApi.Service;
using FamilyFinance.Model;
using FamilyFinance.View.MainView;

namespace FamilyFinance.ViewModel
{
    class AuthorizationVM : INotifyPropertyChanged
    {
        Page _navPage;
        public AuthorizationVM(Page navPage) { _navPage = navPage; }
        //поля ввода
        string _familyLogin;
        string _userLogin;
        string _password;
        //команды
        ActionCommand _authCommand;
        public ICommand GotFocusCommand => new ActionCommand(obj => ClearDataUIElement());      
        //свойства цвета беграуда
        SolidColorBrush _familyLoginBrush;
        SolidColorBrush _userLoginBrush;
        SolidColorBrush _passwordBrush;
        //свойства tooltip подсказки
        string _familyLoginToolTip;
        string _userLoginToolTip;
        string _passwordToolTip;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        //поля ввода
        public string FamilyLogin
        {
            get { return _familyLogin; }
            set
            {
                _familyLogin = value;
                OnPropertyChanged("FamilyLogin");
            }
        }
        public string UserLogin
        {
            get { return _userLogin; }
            set
            {
                _userLogin = value;
                OnPropertyChanged("UserLogin");
            }
        }

        //Команда для кнопки
        public ActionCommand AuthCommand 
        { 
            get { return _authCommand ?? (_authCommand = new ActionCommand(OnAuthorization)); }
        }

        //цвета беграуда для полей
        public SolidColorBrush FamilyLoginBrush
        {
            get { return _familyLoginBrush; }
            set 
            {
                _familyLoginBrush = value;
                OnPropertyChanged("FamilyLoginBrush");
            }
        }
        public SolidColorBrush UserLoginBrush
        {
            get { return _userLoginBrush; }
            set
            {
                _userLoginBrush = value;
                OnPropertyChanged("UserLoginBrush");
            }
        }
        public SolidColorBrush PasswordBrush
        {
            get { return _passwordBrush; }
            set
            {
                _passwordBrush = value;
                OnPropertyChanged("PasswordBrush");
            }
        }

        //подсказски для полей
        public string FamilyLoginToolTip
        {
            get { return _familyLoginToolTip; }
            set
            {
                _familyLoginToolTip = value;
                OnPropertyChanged("FamilyLoginToolTip");
            }
        }
        public string UserLoginToolTip
        {
            get { return _userLoginToolTip; }
            set
            {
                _userLoginToolTip = value;
                OnPropertyChanged("UserLoginToolTip");
            }
        }
        public string PasswordToolTip
        {
            get { return _passwordToolTip; }
            set
            {
                _passwordToolTip = value;
                OnPropertyChanged("PasswordToolTip");
            }
        }

        //проверка авторизации данных, если все совпадает то переходим на новую страницу, если нет уведомляем пользователя
        private async void OnAuthorization(object parametr)
        {
            var passwordBox = parametr as PasswordBox;
            _password = passwordBox.Password;

            try
            {
                Func<Client, Task> manipulatonDataMethod = async (client) =>
                {
                    var token = await client.ApiAccountGetTokenAsync(UserLogin, _password);

                    if (!string.IsNullOrEmpty(token.Value))
                    {
                        ClientManager.SetAuthorizationBearer(token.Value);
                    }

                };
                var resultCode = await ClientManager.ManipulatonData(manipulatonDataMethod);

                if (resultCode == System.Net.HttpStatusCode.OK)
                {
                    Func<Client, Task> manipulatonDataMethod1 = async (client) =>
                    {
                        // Если token получен, получаем информацию о откущем пользователе и загружаем данные в программу
                        var currentUser = await client.ApiAccountGetCurrentUserAsync();
                        DataManager.CurrentUser = currentUser;
                        DataManager.UserRole = (Role)currentUser.RoleId;
                        // Обновляем данные в программе
                        await DataManager.UpdateFromServer();
                    };

                    await ClientManager.ManipulatonData(manipulatonDataMethod1);

                    MessageBox.Show($"Вход выполнен!");

                    //_navPage.NavigationService.Navigate(new MainPage()); //test

                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();

                    var currentWindow = Application.Current.MainWindow;
                    currentWindow.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Внимание", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            //if (_familyLogin == "Pavel" && _userLogin == "Pavel" && _password == "Pavel")
            //{
            //    MessageBox.Show($"Все ок! СемЛогин - {_familyLogin}, ЮзерЛогин - {_userLogin}, Пароль - {_password}");
            //    //...переходим на страницу программы
            //}
            //else
            //{
            //    //..если нет тогда пошла проверка валидации каждого парметра чтобы подсветить красным ошибочные поля
            //    if (_familyLogin != "Pavel")
            //    {
            //        FamilyLoginToolTip = "Неверный логин семьи!";
            //        FamilyLoginBrush = Brushes.DarkRed;
            //    }
            //    if (_userLogin != "Pavel")
            //    {
            //        UserLoginToolTip = "Неверный логин пользователя!";
            //        UserLoginBrush = Brushes.DarkRed;
            //    }
            //    if (_password != "Pavel")
            //    {
            //        PasswordToolTip = "Неверный пароль!";
            //        PasswordBrush = Brushes.DarkRed;
            //    }
            //}
        }

        //Очистка при повторной попытке входа
        private void ClearDataUIElement()
        {
            FamilyLoginToolTip = null;
            FamilyLoginBrush = Brushes.Transparent;
            UserLoginToolTip = null;
            UserLoginBrush = Brushes.Transparent;
            PasswordToolTip = null;
            PasswordBrush = Brushes.Transparent;
        }
        
    }
}
