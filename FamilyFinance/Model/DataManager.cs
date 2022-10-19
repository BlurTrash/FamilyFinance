using FamilyFinance.Core.Extensions;
using FamilyFinance.WebApi.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FamilyFinance.Model
{
    public sealed class DataManager
    {
        private static readonly DataManager _instance = new DataManager();
        public static DataManager Instance { get { return _instance; } }

        static DataManager() { }

        private DataManager() { }

        public static string CurrentDataBase { get; set; }

        public static Role UserRole { get; set; } = Role.Admin;

        public static User CurrentUser { get; set; } = new User();

        #region DataProperties
        //Категории доходов
        private static ObservableCollection<Category> _incomeСategories = new();
        public static ObservableCollection<Category> IncomeСategories
        {
            get
            {
                return _incomeСategories;
            }
            set
            {
                _incomeСategories.Clear();
                _incomeСategories.AddRange(value);
            }
        }

        //Категории расходов
        private static ObservableCollection<CategoryExpense> _expensesСategories = new();
        public static ObservableCollection<CategoryExpense> ExpensesСategories
        {
            get
            {
                return _expensesСategories;
            }
            set
            {
                _expensesСategories.Clear();
                _expensesСategories.AddRange(value);
            }
        }

        //Счета пользователя
        private static ObservableCollection<Check> _userChecks = new();
        public static ObservableCollection<Check> UserChecks
        {
            get
            {
                return _userChecks;
            }
            set
            {
                _userChecks.Clear();
                _userChecks.AddRange(value);
            }
        }

        //Новости приложения
        private static ObservableCollection<News> _newsFamilyFinance = new();
        public static ObservableCollection<News> NewsFamilyFinance
        {
            get
            {
                return _newsFamilyFinance;
            }
            set
            {
                _newsFamilyFinance.Clear();
                _newsFamilyFinance.AddRange(value);
            }
        }
        #endregion

        /// <summary>
        /// Заполняет свойства DataManager данными из сервера
        /// </summary>
        public static event EventHandler BeforeUpdate;
        public static event EventHandler AfterUpdate;

        public static async Task<bool> UpdateFromServer()
        {
            BeforeUpdate?.Invoke(typeof(DataManager), EventArgs.Empty);

            Func<Client, Task> manipulationDataMethod = async (client) =>
            {
                // Загрузка данных
                var incomeСategories = client.ApiCategoryGetAllByUserIdAsync(CurrentUser.Id); //загрузка категорий доходов
                var expensesCategories = client.ApiCategoryExpenseGetAllByUserIdAsync(CurrentUser.Id); //загрузка категорий расходов
                var userChecks = client.ApiCheckGetAllByUserIdAsync(CurrentUser.Id); //загрузка всех счетов пользователя
                var ffNews = client.ApiNewsGetAllAsync(); //загрузка всех новостей

                // Загрузка данных
                await Task.WhenAll(
                    incomeСategories,
                    expensesCategories,
                    userChecks,
                    ffNews);

                // Устанавливаем значения свойствам
                IncomeСategories = new(incomeСategories.Result.ToList());
                ExpensesСategories = new(expensesCategories.Result.ToList());
                UserChecks = new(userChecks.Result.ToList());
                NewsFamilyFinance = new(ffNews.Result.ToList());
            };

            var resultStatus = await ClientManager.ManipulatonData(manipulationDataMethod);

            if (resultStatus == HttpStatusCode.OK)
            {
                AfterUpdate?.Invoke(typeof(DataManager), EventArgs.Empty);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static async Task UpdateCategories()
        {
            Func<Client, Task> manipulationDataMethod = async (client) =>
            {
                var incomeСategories = await client.ApiCategoryGetAllByUserIdAsync(CurrentUser.Id); //загрузка категорий доходов
                // Устанавливаем значения свойствам
                IncomeСategories = new(incomeСategories.ToList());
            };
            await ClientManager.ManipulatonData(manipulationDataMethod);
        }

        public static async Task UpdateCategoriesExpenses()
        {
            Func<Client, Task> manipulationDataMethod = async (client) =>
            {
                var expensesСategories = await client.ApiCategoryExpenseGetAllByUserIdAsync(CurrentUser.Id); //загрузка категорий расходов
                // Устанавливаем значения свойствам
                ExpensesСategories = new(expensesСategories.ToList());
            };
            await ClientManager.ManipulatonData(manipulationDataMethod);
        }

        public static async Task UpdateUserChecks()
        {
            Func<Client, Task> manipulationDataMethod = async (client) =>
            {
                var usersChecks = await client.ApiCheckGetAllByUserIdAsync(CurrentUser.Id); //загрузка счетов пользователя
                // Устанавливаем значения свойствам
                UserChecks = new(usersChecks.ToList());
            };
            await ClientManager.ManipulatonData(manipulationDataMethod);
        }

        public static async Task UpdateNews()
        {
            Func<Client, Task> manipulationDataMethod = async (client) =>
            {
                var ffNews = await client.ApiNewsGetAllAsync(); //загрузка новостей
                // Устанавливаем значения свойствам
                NewsFamilyFinance = new(ffNews.ToList());
            };
            await ClientManager.ManipulatonData(manipulationDataMethod);
        }
    }
}
