using FamilyFinance.Core.Commands;
using FamilyFinance.Model;
using FamilyFinance.Shared.Controls;
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
    public class ChecksTransferWindowVM : INotifyPropertyChanged
    {
        //DialogResult
        public Boolean? UpdateResult { get; set; }

        //Свойства
        public ObservableCollection<Check> Checks { get; set; } = new ObservableCollection<Check>();
        public Check SelectedOutgoingCheck { get; set; }
        public Check SelectedIncomingCheck { get; set; }
        public decimal TransferSumm { get; set; }
        public decimal MaxTransferSumm { get; set; } = 0;
        public decimal? NewIncomingAmount { get; set; }

        //Валидация
        public bool HasErrorsOutgoingCheck { get; set; }
        public bool HasErrorsIncomingCheck { get; set; }

        //Комманды
        private RelayCommand _transferCommand;
        public RelayCommand TransferCommand =>
             _transferCommand ??= new RelayCommand(Transfer, () =>
             {
                 return !HasErrorsOutgoingCheck && !HasErrorsIncomingCheck && SelectedOutgoingCheck != SelectedIncomingCheck; //проверка валидации
             });

        private RelayCommand _changeOutgoingCheckDataCommand;
        public RelayCommand ChangeOutgoingCheckDataCommand =>
            _changeOutgoingCheckDataCommand ??= new RelayCommand(ChangeOutgoingCheckData);

        private RelayCommand _cancelCheckTrasferCommand;
        public RelayCommand CancelCheckTrasferCommand =>
            _cancelCheckTrasferCommand ??= new RelayCommand(CancelCheckTrasfer);

        private RelayCommand _updateNewIncomingAmountCommand;
        public RelayCommand UpdateNewIncomingAmountCommand =>
            _updateNewIncomingAmountCommand ??= new RelayCommand(UpdateNewIncomingAmount);

        public event PropertyChangedEventHandler PropertyChanged;

        public ChecksTransferWindowVM()
        {
            Checks = DataManager.UserChecks;
        }

        private async void Transfer()
        {
            Func<Client, Task> manipulationDataMethod = async (client) =>
            {
                await client.ApiCheckTransferToCheckAsync(SelectedOutgoingCheck.Id, SelectedIncomingCheck.Id, TransferSumm);
            };

            var status = await ClientManager.ManipulatonData(manipulationDataMethod);
            if (status == HttpStatusCode.OK)
            {
                UpdateResult = true;
            }
        }

        private void CancelCheckTrasfer()
        {
            UpdateResult = false;
        }

        private void ChangeOutgoingCheckData()
        {
            if (SelectedOutgoingCheck != null)
            {
                MaxTransferSumm = SelectedOutgoingCheck.Amount;
            }
        }

        private void UpdateNewIncomingAmount()
        {
            if (SelectedIncomingCheck != null && SelectedOutgoingCheck != null)
            {
                decimal outgoingSummBaseCurrency = SelectedOutgoingCheck.CurrencyRate.ExchangeRate * TransferSumm;
                decimal incomingSummBaseCurrency = outgoingSummBaseCurrency / SelectedIncomingCheck.CurrencyRate.ExchangeRate;

                NewIncomingAmount = SelectedIncomingCheck.Amount + incomingSummBaseCurrency;
            }
            else
            {
                NewIncomingAmount = null;
            }
        }
    }
}
