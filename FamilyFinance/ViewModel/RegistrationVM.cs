using FamilyFinance.Model;
using FamilyFinance.WebApi.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace FamilyFinance.ViewModel
{
    class RegistrationVM : INotifyPropertyChanged
    {
        Page _navPage;
        public RegistrationVM(Page navPage) { _navPage = navPage; }
        //поля ввода
        //string _familyLogin;
        string _userLogin;
        string _firstName;
        string _secondName;
        string _password;       
        string _mail;
        //команды
        ActionCommand _registrationCommand;
        public ICommand GotFocusCommand => new ActionCommand(obj => ClearDataUIElement());
        //свойства цвета беграуда
        //SolidColorBrush _familyLoginBrush;
        SolidColorBrush _userLoginBrush;
        SolidColorBrush _firstNameBrush;
        SolidColorBrush _secondNameBrush;
        SolidColorBrush _passwordBrush;
        SolidColorBrush _mailBrush;
        //свойства tooltip подсказки
       // string _familyLoginToolTip;
        string _userLoginToolTip;
        string _firstNameToolTip;
        string _secondNameToolTip;
        string _passwordToolTip;
        string _mailToolTip;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        //поля ввода
        //public string FamilyLogin
        //{
        //    get { return _familyLogin; }
        //    set
        //    {
        //        _familyLogin = value.Trim();
        //        OnPropertyChanged("FamilyLogin");
        //    }
        //}
        public string UserLogin
        {
            get { return _userLogin; }
            set
            {
                _userLogin = value.Trim();
                OnPropertyChanged("UserLogin");
            }
        }
        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value.Trim();
                OnPropertyChanged("FirstName");
            }
        }
        public string SecondName
        {
            get { return _secondName; }
            set
            {
                _secondName = value.Trim();
                OnPropertyChanged("SecondName");
            }
        }
        public string Mail
        {
            get { return _mail; }
            set
            {
                _mail = value.Trim();
                OnPropertyChanged("Mail");
            }
        }

        //Команда для кнопки
        public ActionCommand RegistrationCommand
        {
            get { return _registrationCommand ?? (_registrationCommand = new ActionCommand(OnRegistration)); }
        }

        //цвета беграуда для полей
        //public SolidColorBrush FamilyLoginBrush
        //{
        //    get { return _familyLoginBrush; }
        //    set
        //    {
        //        _familyLoginBrush = value;
        //        OnPropertyChanged("FamilyLoginBrush");
        //    }
        //}
        public SolidColorBrush UserLoginBrush
        {
            get { return _userLoginBrush; }
            set
            {
                _userLoginBrush = value;
                OnPropertyChanged("UserLoginBrush");
            }
        }
        public SolidColorBrush FirstNameBrush
        {
            get { return _firstNameBrush; }
            set
            {
                _firstNameBrush = value;
                OnPropertyChanged("FirstNameBrush");
            }
        }
        public SolidColorBrush SecondNameBrush
        {
            get { return _secondNameBrush; }
            set
            {
                _secondNameBrush = value;
                OnPropertyChanged("SecondNameBrush");
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
        public SolidColorBrush MailBrush
        {
            get { return _mailBrush; }
            set
            {
                _mailBrush = value;
                OnPropertyChanged("MailBrush");
            }
        }

        //подсказски для полей
        //public string FamilyLoginToolTip
        //{
        //    get { return _familyLoginToolTip; }
        //    set
        //    {
        //        _familyLoginToolTip = value;
        //        OnPropertyChanged("FamilyLoginToolTip");
        //    }
        //}
        public string UserLoginToolTip
        {
            get { return _userLoginToolTip; }
            set
            {
                _userLoginToolTip = value;
                OnPropertyChanged("UserLoginToolTip");
            }
        }
        public string FirstNameToolTip
        {
            get { return _firstNameToolTip; }
            set
            {
                _firstNameToolTip = value;
                OnPropertyChanged("FirstNameToolTip");
            }
        }
        public string SecondNameToolTip
        {
            get { return _secondNameToolTip; }
            set
            {
                _secondNameToolTip = value;
                OnPropertyChanged("SecondNameToolTip");
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
        public string MailToolTip
        {
            get { return _mailToolTip; }
            set
            {
                _mailToolTip = value;
                OnPropertyChanged("MailToolTip");
            }
        }

        //Проверка заполненных данных для регистрации (основная логика обработки данных)
        private async void OnRegistration(object parametr)
        {
            var passwordBox = parametr as PasswordBox;
            _password = passwordBox.Password;
            try
            {
                if (IsValidField(_userLogin) && IsValidField(_firstName) && IsValidField(_secondName) && IsValidField(_password) && IsValidMail(_mail))
                {
                    User tempUser = new User() { Login = _userLogin, FirstName = _firstName, SecondName = _secondName, Email = _mail };

                    //var api = new Client("https://localhost:5001/", new System.Net.Http.HttpClient());
                    //var newUser = await api.ApiAccountPostAsync(_password, tempUser);

                    User newUser = null;

                    Func<Client, Task> manipulatonDataMethod = async (client) =>
                    {
                        newUser = await client.ApiAccountPostAsync(_password, tempUser);
                    };
                    var resultCode = await ClientManager.ManipulatonData(manipulatonDataMethod);

                    if (resultCode == System.Net.HttpStatusCode.OK)
                    {
                        MessageBox.Show($"Пользователь зарегистрирован!\n ЮзерЛогин - {newUser.Login}\n Имя - {newUser.FirstName}\n Фамилия - {newUser.SecondName}\n Email - {newUser.Email}");
                        //...переходим на страницу входа
                        _navPage.NavigationService.Navigate(new AuthorizationPage());
                    }
                }
                else
                {
                    //..если нет тогда пошла проверка валидации каждого парметра чтобы подсветить красным ошибочные поля
                    if (!IsValidMail(_mail))
                    {
                        MailToolTip = "Email указан неверно!";
                        MailBrush = Brushes.DarkRed;
                    }
                    //if (!IsValidField(_familyLogin))
                    //{
                    //    FamilyLoginToolTip = "Поле не должно содержать пробелов и должно быть больше 3х символов!";
                    //    FamilyLoginBrush = Brushes.DarkRed;
                    //}
                    if (!IsValidField(_userLogin))
                    {
                        UserLoginToolTip = "Поле не должно содержать пробелов и должно быть больше 3х символов!";
                        UserLoginBrush = Brushes.DarkRed;
                    }
                    if (!IsValidField(_firstName))
                    {
                        FirstNameToolTip = "Поле не должно содержать пробелов и должно быть больше 3х символов!";
                        FirstNameBrush = Brushes.DarkRed;
                    }
                    if (!IsValidField(_secondName))
                    {
                        SecondNameToolTip = "Поле не должно содержать пробелов и должно быть больше 3х символов!";
                        SecondNameBrush = Brushes.DarkRed;
                    }
                    if (!IsValidField(_password))
                    {
                        PasswordToolTip = "Поле не должно содержать пробелов и должно быть больше 3х символов!";
                        PasswordBrush = Brushes.DarkRed;
                    }
                }
            } catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        //Очистка при повторной попытке входа
        private void ClearDataUIElement()
        {
            //FamilyLoginToolTip = null;
            //FamilyLoginBrush = Brushes.Transparent;

            UserLoginToolTip = null;
            UserLoginBrush = Brushes.Transparent;

            FirstNameToolTip = null;
            FirstNameBrush = Brushes.Transparent;

            SecondNameToolTip = null;
            SecondNameBrush = Brushes.Transparent;

            PasswordToolTip = null;
            PasswordBrush = Brushes.Transparent;

            MailToolTip = null;
            MailBrush = Brushes.Transparent;
        }

        //простые проверки валидации полей (потом можно доработать, сейчас для тестирования)
        private bool IsValidField(string field)
        {
            if (field.IndexOf(' ') > -1)
                return false;
            return field.Length > 3 ? true : false;
        }
        private bool IsValidMail(string mail)
        {
            try
            {
                MailAddress Email = new MailAddress(mail);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
