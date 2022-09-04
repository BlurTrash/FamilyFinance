﻿using FamilyFinance.Core.Extensions;
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

                // Загрузка данных
                await Task.WhenAll(
                    incomeСategories,
                    expensesCategories);

                // Устанавливаем значения свойствам
                IncomeСategories = new(incomeСategories.Result.ToList());
                ExpensesСategories = new(expensesCategories.Result.ToList());
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
    }
}
