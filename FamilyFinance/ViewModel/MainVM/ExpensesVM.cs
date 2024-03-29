﻿using FamilyFinance.Core.Commands;
using FamilyFinance.Model;
using FamilyFinance.View.MainView.ExpensesDialogWindow;
using FamilyFinance.WebApi.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FamilyFinance.ViewModel.MainVM
{
    public enum FilterTypes
    {
        [Display(Name = "По месяцам")]
        ByMonths,
        [Display(Name = "По дням")]
        ByDays
    }

    public class ExpensesVM : INotifyPropertyChanged
    {
        public Role UserRole { get { return DataManager.UserRole; } }

        //Коллекции
        public ObservableCollection<string> Months { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<int> Years { get; set; } = new ObservableCollection<int>();
        public ObservableCollection<Expense> Expenses { get; set; } = new ObservableCollection<Expense>();
        public ObservableCollection<ExpenseDetails> TopFiveCategories { get; set; } = new ObservableCollection<ExpenseDetails>();
        public ObservableCollection<CheckDetails> TopFiveChecks { get; set; } = new ObservableCollection<CheckDetails>();

        //Свойства
        public string SelectedMonth { get; set; }
        public int? SelectedYear { get; set; }
        public FilterTypes SelectedFilter { get; set; } = FilterTypes.ByMonths;
        public DateTime SelectedDay { get; set; } = DateTime.Now;
        public bool IsMonthsVisibility { get; set; } = true;
        public bool IsDaysVisibility { get; set; } = false;
        public decimal ExpensesAmountSumm { get; set; }

        //Поля
        private DateTime _date;

        //Комманды расхода
        private RelayCommand _addExpenseCommand;
        public RelayCommand AddExpenseCommand =>
            _addExpenseCommand ??= new RelayCommand(AddExpense);

        private RelayCommand<Expense> _editExpenseCommand;
        public RelayCommand<Expense> EditExpenseCommand =>
            _editExpenseCommand ??= new RelayCommand<Expense>(EditExpense);

        private RelayCommand<Expense> _deleteExpenseCommand;
        public RelayCommand<Expense> DeleteExpenseCommand =>
            _deleteExpenseCommand ??= new RelayCommand<Expense>(DeleteExpense);

        //Комманды
        private RelayCommand _filterChangeCommand;
        public RelayCommand FilterChangeCommand =>
            _filterChangeCommand ??= new RelayCommand(FilterChange);

        private RelayCommand _updateDataCommand;
        public RelayCommand UpdateDataCommand =>
            _updateDataCommand ??= new RelayCommand(UpdateData);

        private RelayCommand _updateDateCommand;
        public RelayCommand UpdateDateCommand =>
            _updateDateCommand ??= new RelayCommand(UpdateDate);

        private RelayCommand _updateDayCommand;
        public RelayCommand UpdateDayCommand =>
            _updateDayCommand ??= new RelayCommand(UpdateDay);

        public event PropertyChangedEventHandler PropertyChanged;

        public ExpensesVM()
        {
            //Месяца
            var russuanCulture = new CultureInfo("ru-RU");
            var months = russuanCulture.DateTimeFormat.MonthNames;
            for (int i = 0; i < months.Length-1; i++)
            {
                Months.Add(months[i]);
            }
            SelectedMonth = Months.FirstOrDefault(m => m == DateTime.Now.ToString("MMMM", CultureInfo.GetCultureInfo("ru-RU")));

            //Года
            var years = Enumerable.Range(2000, DateTime.UtcNow.Year-1999).ToList();
            Years = new ObservableCollection<int>(years);
            SelectedYear = Years.FirstOrDefault(y => y == DateTime.Now.Year);

            _date = DateTime.ParseExact(SelectedMonth + "-" + SelectedYear.ToString(), "MMMM-yyyy", CultureInfo.GetCultureInfo("ru-RU"));
        }

        private async void AddExpense()
        {
            Expense expense = new Expense();

            if (ShowExpenseEdit(expense, true) == true)
            {
                await DataManager.UpdateUserChecks();
                UpdateData();
            }
        }

        private async void EditExpense(Expense expense)
        {
            if (ShowExpenseEdit(expense, false) == true)
            {
                await DataManager.UpdateUserChecks();
                UpdateData();
            }
        }

        private async void DeleteExpense(Expense obj)
        {
            var result = MessageBox.Show("Удалить?", "FamilyFinance", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Func<Client, Task> manipulationDataMethod = async (client) =>
                {
                    var expense = await client.ApiExpenseDeleteAsync(obj.Id);
                };

                var status = await ClientManager.ManipulatonData(manipulationDataMethod);

                if (status == HttpStatusCode.OK)
                {
                    await DataManager.UpdateUserChecks();
                    UpdateData();
                }
            }
        }

        private bool? ShowExpenseEdit(Expense expense, bool isNew)
        {
            ExpenseEditWindow window = new ExpenseEditWindow(expense, isNew);

            return window.ShowDialog();
        }

        private void FilterChange()
        {
            switch (SelectedFilter)
            {
                case FilterTypes.ByMonths:
                    IsMonthsVisibility = true;
                    IsDaysVisibility = false;
                    break;
                case FilterTypes.ByDays:
                    IsMonthsVisibility = false;
                    IsDaysVisibility = true;
                    break;
                default:
                    break;
            }
        }

        private async void UpdateData()
        {
            switch (SelectedFilter)
            {
                case FilterTypes.ByMonths:

                    ICollection<Expense> expenseCollection = null;
                    ICollection<ExpenseDetails> expenseDetailsCollection = null;

                    Func<Client, Task> manipulationDataMethod = async (client) =>
                    {
                        expenseCollection = await client.ApiExpenseGetAllByMonthAsync(_date);
                        expenseDetailsCollection = await client.ApiExpenseGetTopFiveExpenseByMonthAsync(_date);
                    };

                    var status = await ClientManager.ManipulatonData(manipulationDataMethod);

                    if (status == System.Net.HttpStatusCode.OK)
                    {
                        Expenses = new ObservableCollection<Expense>(expenseCollection);
                        TopFiveCategories = new ObservableCollection<ExpenseDetails>(expenseDetailsCollection);
                    }
                    break;
                case FilterTypes.ByDays:

                    ICollection<Expense> expenseCollectionByDays = null;
                    ICollection<ExpenseDetails> expenseDetailsCollectionByDays = null;

                    Func<Client, Task> manipulationDataMethodByDays = async (client) =>
                    {
                        expenseCollectionByDays = await client.ApiExpenseGetAllByDayAsync(_date);
                        expenseDetailsCollectionByDays = await client.ApiExpenseGetTopFiveExpenseByDayAsync(_date);
                    };

                    var statusByDays = await ClientManager.ManipulatonData(manipulationDataMethodByDays);

                    if (statusByDays == System.Net.HttpStatusCode.OK)
                    {
                        Expenses = new ObservableCollection<Expense>(expenseCollectionByDays);
                        TopFiveCategories = new ObservableCollection<ExpenseDetails>(expenseDetailsCollectionByDays);
                    }
                    break;
                default:
                    break;
            }

            ExpensesAmountSumm = Expenses.Sum(e => e.SpentMoney * e.Check.CurrencyRate.ExchangeRate);

            ICollection<CheckDetails> checkDetailsCollection = null;
            Func<Client, Task> manipulationDataMethodCheckDetails = async (client) =>
            {
                checkDetailsCollection = await client.ApiCheckGetTopFiveCheckBalancesAsync();
            };
            var statusCheckDetails = await ClientManager.ManipulatonData(manipulationDataMethodCheckDetails);
            if (statusCheckDetails == System.Net.HttpStatusCode.OK)
            {
                TopFiveChecks = new ObservableCollection<CheckDetails>(checkDetailsCollection);
            }
        }

        private void UpdateDate()
        {
            _date = DateTime.ParseExact(SelectedMonth + "-" + SelectedYear.ToString(), "MMMM-yyyy", CultureInfo.GetCultureInfo("ru-RU"));
        }

        private void UpdateDay()
        {
            _date = new DateTime(SelectedDay.Year, SelectedDay.Month, SelectedDay.Day);
        }
    }
}
