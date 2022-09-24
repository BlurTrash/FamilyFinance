using FamilyFinance.Core.Commands;
using FamilyFinance.Model;
using FamilyFinance.View.MainView.ChecksDialogWindow;
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

namespace FamilyFinance.ViewModel.MainVM
{
    public class Result
    {
        public string ResultText { get; set; }
        public decimal Summ { get; set; } 
    }

    public class ChecksVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private decimal checksAmountSumm = 0;
        private string resultText = "Итого:";

        public ChecksVM()
        {
            Checks = DataManager.UserChecks;
            Checks.CollectionChanged += Checks_CollectionChanged;

            checksAmountSumm = Checks.Sum(c=> c.Amount * c.CurrencyRate.ExchangeRate);
            Results = new ObservableCollection<Result>
            {
                new Result { ResultText = resultText, Summ = checksAmountSumm }
            };
        }

        public Role UserRole { get { return DataManager.UserRole; } }

        private RelayCommand _addCheckCommand;
        public RelayCommand AddCheckCommand =>
            _addCheckCommand ??= new RelayCommand(AddCheck);

        private RelayCommand<Check> _editCheckCommand;
        public RelayCommand<Check> EditCheckCommand =>
            _editCheckCommand ??= new RelayCommand<Check>(EditCheck);

        private RelayCommand<Check> _deleteCheckCommand;
        public RelayCommand<Check> DeleteCheckCommand =>
            _deleteCheckCommand ??= new RelayCommand<Check>(DeleteCheck);

        private RelayCommand _updateCheckCommand;
        public RelayCommand UpdateCheckCommand =>
            _updateCheckCommand ??= new RelayCommand(UpdateCheck);

        private RelayCommand _transferCheckCommand;
        public RelayCommand TransferCheckCommand =>
            _transferCheckCommand ??= new RelayCommand(TransferCheck);

        public ObservableCollection<Check> Checks { get; set; } = new ObservableCollection<Check>();
        public ObservableCollection<Result> Results { get; private set; } = new ObservableCollection<Result>();

        private void Checks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            checksAmountSumm = Checks.Sum(c => c.Amount * c.CurrencyRate.ExchangeRate);

            Results = new ObservableCollection<Result>
            {
                new Result { ResultText = resultText, Summ = checksAmountSumm }
            };
        }

        private async void AddCheck()
        {
            Check check = new Check();

            if (ShowCheckEdit(check, true) == true)
            {
                //обновляем на клиенте счета по пользователю
                await DataManager.UpdateUserChecks();
            }
        }

        private async void EditCheck(Check obj)
        {
            if (ShowCheckEdit(obj, false) == true)
            {
                //обновляем на клиенте счета по пользователю
                await DataManager.UpdateUserChecks();
            }
        }
        private async void DeleteCheck(Check obj)
        {
            var result = MessageBox.Show("Удалить?", "FamilyFinance", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Func<Client, Task> manipulationDataMethod = async (client) =>
                {
                    var check = await client.ApiCheckDeleteAsync(obj.Id);
                };

                var status = await ClientManager.ManipulatonData(manipulationDataMethod);

                if (status == HttpStatusCode.OK)
                {
                    await DataManager.UpdateUserChecks();
                }
            }
        }

        private async void UpdateCheck()
        {
            await DataManager.UpdateUserChecks();
        }

        private async void TransferCheck()
        {
            ChecksTransferWindow window = new ChecksTransferWindow();

            if (window.ShowDialog() == true)
            {
                await DataManager.UpdateUserChecks();
            }
        }

        private bool? ShowCheckEdit(Check check, bool isNew)
        {
            ChecksEditWindow window = new ChecksEditWindow(check, isNew);
           
            return window.ShowDialog();
        }
    }
}
