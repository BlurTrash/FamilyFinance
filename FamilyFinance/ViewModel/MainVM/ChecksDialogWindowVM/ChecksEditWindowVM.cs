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
using System.Windows;

namespace FamilyFinance.ViewModel.MainVM.ChecksDialogWindowVM
{
    public class ChecksEditWindowVM : INotifyPropertyChanged
    {
        //Поля
        private bool _isNew;
        private bool _isModified;

        //Свойства
        public Check TempCheck { get; set; }
        public Check Check { get; set; }
        public ObservableCollection<CurrencyRate> CurrencyRates { get; set; } = new();
        public CurrencyRate SelectedCurrencyRate { get; set; }

        public bool IsDefaultMasterCheck { get; set; }

        //DialogResult
        public Boolean? UpdateResult { get; set; }

        //Валидация
        public bool HasErrorsName { get; set; }
        public bool HasErrorsCurrencyRate { get; set; }

        //Команды
        private RelayCommand _loadedCheckEditWindowCommand;
        public RelayCommand LoadedCheckEditWindowCommand =>
            _loadedCheckEditWindowCommand ??= new RelayCommand(LoadedCheckEditWindow);
        
        private RelayCommand<object> _closingCheckEditWindowCommand;
        public RelayCommand<object> ClosingCheckEditWindowCommand =>
            _closingCheckEditWindowCommand ??= new RelayCommand<object>(ClosingCheckEditWindow);

        private RelayCommand _saveCheckCommand;
        public RelayCommand SaveCheckCommand =>
            _saveCheckCommand ??= new RelayCommand(SaveCheck, () =>
            {
                return !HasErrorsName && !HasErrorsCurrencyRate; //проверка валидации
            });

        private RelayCommand _сancelCheckCommand;
        public RelayCommand CancelCheckCommand =>
            _сancelCheckCommand ??= new RelayCommand(CancelCheck);

        private RelayCommand _changeDataCommand;
        public RelayCommand ChangeDataCommand =>
            _changeDataCommand ??= new RelayCommand(ChangeData);


        public event PropertyChangedEventHandler PropertyChanged;

        public ChecksEditWindowVM(Check check, bool isNew)
        {
            _isNew = isNew;

            TempCheck = new Check();
            TempCheck.Name = check.Name;
            TempCheck.CurrencyRate = check.CurrencyRate;
            TempCheck.Amount = check.Amount;
            TempCheck.Description = check.Description;
            TempCheck.IsMasterCheck = check.IsMasterCheck;

            Check = check;
            IsDefaultMasterCheck = !TempCheck.IsMasterCheck;

            _isModified = false;
        }

        //Методы
        protected async void LoadedCheckEditWindow()
        {
            Func<Client, Task> manipulationDataMethod = async (client) =>
            {
                var currencyRates = await client.ApiCurrencyGetAllAsync();
                CurrencyRates = new ObservableCollection<CurrencyRate>(currencyRates.ToList());
            };

            var status = await ClientManager.ManipulatonData(manipulationDataMethod);
            if (status == System.Net.HttpStatusCode.OK)
            {
                if (!_isNew)
                {
                    var currentCurrency = CurrencyRates.FirstOrDefault(c => c.Id == TempCheck.CurrencyRate.Id);
                    SelectedCurrencyRate = currentCurrency;
                    _isModified = false;
                }
            }
        }

        private void ClosingCheckEditWindow(object parametrs)
        {
            if (!_isNew)
            {
                if (_isModified && !HasErrorsName && !HasErrorsCurrencyRate)
                {
                    var dialogResult = MessageBox.Show("Вы хотите сохранить изменения?", "Family Finance", MessageBoxButton.YesNoCancel, MessageBoxImage.Information);
                    if (dialogResult == MessageBoxResult.Yes)
                    {
                        ((CancelEventArgs)parametrs).Cancel = true;
                        SaveCheck();
                    }
                    else if (dialogResult == MessageBoxResult.No)
                    {
                        UpdateResult = false;
                    }
                    else
                    {
                        ((CancelEventArgs)parametrs).Cancel = true;
                    }
                }
            }

            if (UpdateResult == null)
            {
                UpdateResult = false;
            }
        }

        private async void SaveCheck()
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

        private void CancelCheck()
        {
            UpdateResult = false;
        }

        private async Task<bool> SaveChanges()
        {
            if (!HasErrorsName && !HasErrorsCurrencyRate)
            {
                Check.Name = TempCheck.Name;
                Check.CurrencyRateId = SelectedCurrencyRate.Id;
                Check.Amount = TempCheck.Amount;
                Check.Description = TempCheck.Description;
                Check.IsMasterCheck = TempCheck.IsMasterCheck;


                Check newCheck = null;

                Func<Client, Task> manipulationDataMethod = async (client) =>
                {
                    if (_isNew)
                    {
                        newCheck = await client.ApiCheckPostAsync(Check);
                    }
                    else
                    {
                        newCheck = await client.ApiCheckPutAsync(Check);
                    }
                };

                var status = await ClientManager.ManipulatonData(manipulationDataMethod);

                if (status == HttpStatusCode.OK && newCheck != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        private void ChangeData()
        {
            SetModified();
        }

        private void SetModified()
        {
            _isModified = true;
        }
    }
}
