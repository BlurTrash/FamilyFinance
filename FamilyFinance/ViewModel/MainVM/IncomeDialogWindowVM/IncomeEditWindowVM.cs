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

namespace FamilyFinance.ViewModel.MainVM.IncomeDialogWindowVM
{
    public class IncomeEditWindowVM : INotifyPropertyChanged
    {
        //Коллекции
        public ObservableCollection<Category> Categories { get; set; } = new ObservableCollection<Category>();
        public ObservableCollection<SubCategory> SubCategory { get; set; } = new ObservableCollection<SubCategory>();
        public ObservableCollection<Check> Checks { get; set; } = new ObservableCollection<Check>();

        //Поля
        private bool _isNew;
        private bool _isModified;

        //DialogResult
        public Boolean? UpdateResult { get; set; }

        //Свойства
        public DateTime? SelectedDay { get; set; }
        public TimeSpan? SelectedTime { get; set; }
        public Check SelectedCheck { get; set; }
        public decimal MaxTransferSumm { get; set; } = decimal.MaxValue;
        public Income TempIncome { get; set; }
        public Income Income { get; set; }
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

        private RelayCommand _saveIncomeCommand;
        public RelayCommand SaveIncomeCommand =>
            _saveIncomeCommand ??= new RelayCommand(SaveIncome, () =>
            {
                return !HasErrorsCategories && !HasErrorsDate && !HasErrorsCheck; //проверка валидации
            });

        private RelayCommand _cancelIncomeCommand;
        public RelayCommand CancelIncomeCommand =>
            _cancelIncomeCommand ??= new RelayCommand(CancelIncome);

        public event PropertyChangedEventHandler PropertyChanged;
        public IncomeEditWindowVM(Income income, bool isNew)
        {
            Categories = DataManager.IncomeСategories;
            Checks = DataManager.UserChecks;
            _isNew = isNew;

            TempIncome = new Income();
            TempIncome.CategoryId = income.CategoryId;
            TempIncome.SubCategoryId = income.SubCategoryId;
            TempIncome.ReplenishmentMoney = income.ReplenishmentMoney;
            TempIncome.Description = income.Description;

            Income = income;

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
                SelectedDay = income.Date;
                SelectedTime = income.Date.TimeOfDay;
                Check baseCheck = Checks.FirstOrDefault(c => c.Id == income.Check.Id);
                SelectedCheck = baseCheck;
                //MaxTransferSumm = income.TransactionInvoiceAmount;

                IsCreateMode = false;
            }

            _isModified = false;
        }

        private async void ChangeCategory()
        {
            if (TempIncome.CategoryId != null)
            {
                ICollection<SubCategory> subCategory = null;
                Func<Client, Task> manipulationDataMethod = async (client) =>
                {
                    subCategory = await client.ApiSubCategoryGetAllByCategoryIdAsync((int)TempIncome.CategoryId);
                };

                var status = await ClientManager.ManipulatonData(manipulationDataMethod);

                if (status == HttpStatusCode.OK)
                {
                    SubCategory = new ObservableCollection<SubCategory>(subCategory);
                }
            }
        }

        private void ChangeCheck()
        {
            if (SelectedCheck != null && _isNew)
            {
                //MaxTransferSumm = SelectedCheck.Amount;
            }
        }

        private void ChangeData()
        {
            //modified
        }

        private async void SaveIncome()
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

        private void CancelIncome()
        {
            UpdateResult = false;
        }

        private async Task<bool> SaveChanges()
        {
            if (!HasErrorsCategories && !HasErrorsDate && !HasErrorsCheck)
            {
                Income.CategoryId = TempIncome.CategoryId;
                Income.SubCategoryId = TempIncome.SubCategoryId;
                Income.Description = TempIncome.Description;
                if (_isNew)
                {
                    Income.CheckId = SelectedCheck.Id;
                    Income.TransactionInvoiceAmount = SelectedCheck.Amount;
                    DateTime dateTime = new DateTime(SelectedDay.Value.Year, SelectedDay.Value.Month, SelectedDay.Value.Day, SelectedTime.Value.Hours, SelectedTime.Value.Minutes, SelectedTime.Value.Seconds);
                    Income.Date = dateTime;
                    Income.ReplenishmentMoney = TempIncome.ReplenishmentMoney;

                    Income newIncome = null;

                    Func<Client, Task> manipulationDataMethod = async (client) =>
                    {
                        newIncome = await client.ApiIncomePostAsync(Income);
                    };

                    var status = await ClientManager.ManipulatonData(manipulationDataMethod);

                    if (status == HttpStatusCode.OK && newIncome != null)
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
                    Income.ReplenishmentMoney = TempIncome.ReplenishmentMoney;

                    Income newIncome = null;

                    Func<Client, Task> manipulationDataMethod = async (client) =>
                    {
                        newIncome = await client.ApiIncomePutAsync(Income);
                    };

                    var status = await ClientManager.ManipulatonData(manipulationDataMethod);

                    if (status == HttpStatusCode.OK && newIncome != null)
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
