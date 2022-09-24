using FamilyFinance.Core.Commands;
using FamilyFinance.Model;
using FamilyFinance.WebApi.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FamilyFinance.ViewModel.MainVM.ExpensesDialogWindowVM
{
    public class ExpenseEditWindowVM : INotifyPropertyChanged
    {
        //Коллекции
        public ObservableCollection<CategoryExpense> Categories { get; set; } = new ObservableCollection<CategoryExpense>();
        public ObservableCollection<SubCategoryExpense> SubCategory { get; set; } = new ObservableCollection<SubCategoryExpense>();
        public ObservableCollection<Check> Checks { get; set; } = new ObservableCollection<Check>();

        //Поля
        private bool _isNew;
        private bool _isModified;

        //DialogResult
        public Boolean? UpdateResult { get; set; }

        //Свойства
        //public int? SelectedCategoryId { get; set; }
        //public int? SelectedSubCategoryId { get; set; }
        public DateTime? SelectedDay { get; set; }
        public TimeSpan? SelectedTime { get; set; }
        public Check SelectedCheck { get; set; }
        //public decimal ExpenseSumm { get; set; }
        public decimal MaxTransferSumm { get; set; } = decimal.MaxValue;
        public Expense TempExpense { get; set; }
        public Expense Expense { get; set; }
        public bool IsCreateMode { get; set; }

        //Валидация
        public bool HasErrorsCategories { get; set; }
        public bool HasErrorsDate { get; set; }
        public bool HasErrorsCheck { get; set; }

        //Комманды
        private RelayCommand _changeCategoryCommand;
        public RelayCommand ChangeCategoryCommand =>
            _changeCategoryCommand ??= new RelayCommand(ChangeCategory);

        private RelayCommand _changeCheckCommand;
        public RelayCommand ChangeCheckCommand =>
            _changeCheckCommand ??= new RelayCommand(ChangeCheck);

        private RelayCommand _changeDataCommand;
        public RelayCommand ChangeDataCommand =>
            _changeDataCommand ??= new RelayCommand(ChangeData);

        private RelayCommand _saveExpenseCommand;
        public RelayCommand SaveExpenseCommand =>
            _saveExpenseCommand ??= new RelayCommand(SaveExpense, () =>
            {
                return !HasErrorsCategories && !HasErrorsDate && !HasErrorsCheck; //проверка валидации
            });

        private RelayCommand _cancelExpenseCommand;
        public RelayCommand CancelExpenseCommand =>
            _cancelExpenseCommand ??= new RelayCommand(CancelExpense);

        public event PropertyChangedEventHandler PropertyChanged;

        public ExpenseEditWindowVM(Expense expense, bool isNew)
        {
            Categories = DataManager.ExpensesСategories;
            Checks = DataManager.UserChecks;
            _isNew = isNew;

            TempExpense = new Expense();
            TempExpense.CategoryExpenseId = expense.CategoryExpenseId;
            TempExpense.SubCategoryExpenseId = expense.SubCategoryExpenseId;
            TempExpense.SpentMoney = expense.SpentMoney;
            TempExpense.Description = expense.Description;

            Expense = expense;

            if (isNew)
            {
                SelectedDay = DateTime.Now;
                SelectedTime = DateTime.Now.TimeOfDay;
                Check baseCheck = Checks.FirstOrDefault(c => c.IsMasterCheck == true);
                SelectedCheck = baseCheck;

                IsCreateMode = true;
            }
            else
            {
                SelectedDay = expense.Date;
                SelectedTime = expense.Date.TimeOfDay;
                Check baseCheck = Checks.FirstOrDefault(c => c.Id == expense.Check.Id);
                SelectedCheck = baseCheck;
                MaxTransferSumm = expense.TransactionInvoiceAmount;

                IsCreateMode = false;
            }

            _isModified = false;
        }

        private async void ChangeCategory()
        {
            if (TempExpense.CategoryExpenseId != null)
            {
                ICollection<SubCategoryExpense> subCategoryExpenses = null;
                Func<Client, Task> manipulationDataMethod = async (client) =>
                {
                    subCategoryExpenses = await client.ApiSubCategoryExpenseGetAllByCategoryExpenseIdAsync((int)TempExpense.CategoryExpenseId);
                };

                var status = await ClientManager.ManipulatonData(manipulationDataMethod);

                if (status == HttpStatusCode.OK)
                {
                    SubCategory = new ObservableCollection<SubCategoryExpense>(subCategoryExpenses);
                }
            }
        }

        private void ChangeCheck()
        {
            if (SelectedCheck != null && _isNew)
            {
                MaxTransferSumm = SelectedCheck.Amount;
            }
        }

        private void ChangeData()
        {
            //modified
        }

        private async void SaveExpense()
        {
            if (await SaveChanges())
            {
                _isModified = false;
                UpdateResult = true;
            }
            else
            {
                UpdateResult = false;
            }
        }

        private void CancelExpense()
        {
            UpdateResult = false;
        }

        private async Task<bool> SaveChanges()
        {
            if (!HasErrorsCategories && !HasErrorsDate && !HasErrorsCheck)
            {
                Expense.CategoryExpenseId = TempExpense.CategoryExpenseId;
                Expense.SubCategoryExpenseId = TempExpense.SubCategoryExpenseId;
                Expense.Description = TempExpense.Description;
                if (_isNew)
                {
                    Expense.CheckId = SelectedCheck.Id;
                    Expense.TransactionInvoiceAmount = SelectedCheck.Amount;
                    DateTime dateTime = new DateTime(SelectedDay.Value.Year, SelectedDay.Value.Month, SelectedDay.Value.Day, SelectedTime.Value.Hours, SelectedTime.Value.Minutes, SelectedTime.Value.Seconds);
                    Expense.Date = dateTime;
                    Expense.SpentMoney = TempExpense.SpentMoney;

                    Expense newExpense = null;

                    Func<Client, Task> manipulationDataMethod = async (client) =>
                    {
                        newExpense = await client.ApiExpensePostAsync(Expense);
                    };

                    var status = await ClientManager.ManipulatonData(manipulationDataMethod);

                    if (status == HttpStatusCode.OK && newExpense != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    Expense.SpentMoney = TempExpense.SpentMoney;

                    Expense newExpense = null;

                    Func<Client, Task> manipulationDataMethod = async (client) =>
                    {
                        newExpense = await client.ApiExpensePutAsync(Expense);
                    };

                    var status = await ClientManager.ManipulatonData(manipulationDataMethod);

                    if (status == HttpStatusCode.OK && newExpense != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }
    }
}
