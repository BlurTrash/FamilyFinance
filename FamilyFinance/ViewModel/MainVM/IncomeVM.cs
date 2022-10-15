using FamilyFinance.Core.Commands;
using FamilyFinance.Model;
using FamilyFinance.View.MainView.IncomeDialogWindow;
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
    public class IncomeVM : INotifyPropertyChanged
    {
        public Role UserRole { get { return DataManager.UserRole; } }

        //Коллекции
        public ObservableCollection<string> Months { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<int> Years { get; set; } = new ObservableCollection<int>();
        public ObservableCollection<Income> Incomes { get; set; } = new ObservableCollection<Income>();
        public ObservableCollection<IncomeDetails> TopFiveCategories { get; set; } = new ObservableCollection<IncomeDetails>();
        public ObservableCollection<CheckDetails> TopFiveChecks { get; set; } = new ObservableCollection<CheckDetails>();

        //Свойства
        public string SelectedMonth { get; set; }
        public int? SelectedYear { get; set; }
        public FilterTypes SelectedFilter { get; set; } = FilterTypes.ByMonths;
        public DateTime SelectedDay { get; set; } = DateTime.Now;
        public bool IsMonthsVisibility { get; set; } = true;
        public bool IsDaysVisibility { get; set; } = false;
        public decimal IncomesAmountSumm { get; set; }

        //Поля
        private DateTime _date;

        //Комманды расхода
        private RelayCommand _addIncomeCommand;
        public RelayCommand AddIncomeCommand =>
            _addIncomeCommand ??= new RelayCommand(AddIncome);

        private RelayCommand<Income> _editIncomeCommand;
        public RelayCommand<Income> EditIncomeCommand =>
            _editIncomeCommand ??= new RelayCommand<Income>(EditIncome);

        private RelayCommand<Income> _deleteIncomeCommand;
        public RelayCommand<Income> DeleteIncomeCommand =>
            _deleteIncomeCommand ??= new RelayCommand<Income>(DeleteIncome);

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

        public IncomeVM()
        {
            //Месяца
            var russuanCulture = new CultureInfo("ru-RU");
            var months = russuanCulture.DateTimeFormat.MonthNames;
            for (int i = 0; i < months.Length - 1; i++)
            {
                Months.Add(months[i]);
            }
            SelectedMonth = Months.FirstOrDefault(m => m == DateTime.Now.ToString("MMMM", CultureInfo.GetCultureInfo("ru-RU")));

            //Года
            var years = Enumerable.Range(2000, DateTime.UtcNow.Year - 1999).ToList();
            Years = new ObservableCollection<int>(years);
            SelectedYear = Years.FirstOrDefault(y => y == DateTime.Now.Year);

            _date = DateTime.ParseExact(SelectedMonth + "-" + SelectedYear.ToString(), "MMMM-yyyy", CultureInfo.GetCultureInfo("ru-RU"));
        }

        private async void AddIncome()
        {
            Income income = new Income();

            if (ShowIncomeEdit(income, true) == true)
            {
                await DataManager.UpdateUserChecks();
                UpdateData();
            }
        }

        private async void EditIncome(Income income)
        {
            if (ShowIncomeEdit(income, false) == true)
            {
                await DataManager.UpdateUserChecks();
                UpdateData();
            }
        }

        private async void DeleteIncome(Income obj)
        {
            var result = MessageBox.Show("Удалить?", "FamilyFinance", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Func<Client, Task> manipulationDataMethod = async (client) =>
                {
                    var income = await client.ApiIncomeDeleteAsync(obj.Id);
                };

                var status = await ClientManager.ManipulatonData(manipulationDataMethod);

                if (status == HttpStatusCode.OK)
                {
                    await DataManager.UpdateUserChecks();
                    UpdateData();
                }
            }
        }

        private bool? ShowIncomeEdit(Income income, bool isNew)
        {
            IncomeEditWindow window = new IncomeEditWindow(income, isNew);

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

                    ICollection<Income> incomeCollection = null;
                    ICollection<IncomeDetails> incomeDetailsCollection = null;

                    Func<Client, Task> manipulationDataMethod = async (client) =>
                    {
                        incomeCollection = await client.ApiIncomeGetAllByMonthAsync(_date);
                        incomeDetailsCollection = await client.ApiIncomeGetTopFiveIncomeByMonthAsync(_date);
                    };

                    var status = await ClientManager.ManipulatonData(manipulationDataMethod);

                    if (status == System.Net.HttpStatusCode.OK)
                    {
                        Incomes = new ObservableCollection<Income>(incomeCollection);
                        TopFiveCategories = new ObservableCollection<IncomeDetails>(incomeDetailsCollection);
                    }
                    break;
                case FilterTypes.ByDays:

                    ICollection<Income> incomeCollectionByDays = null;
                    ICollection<IncomeDetails> incomeDetailsCollectionByDays = null;

                    Func<Client, Task> manipulationDataMethodByDays = async (client) =>
                    {
                        incomeCollectionByDays = await client.ApiIncomeGetAllByDayAsync(_date);
                        incomeDetailsCollectionByDays = await client.ApiIncomeGetTopFiveIncomeByDayAsync(_date);
                    };

                    var statusByDays = await ClientManager.ManipulatonData(manipulationDataMethodByDays);

                    if (statusByDays == System.Net.HttpStatusCode.OK)
                    {
                        Incomes = new ObservableCollection<Income>(incomeCollectionByDays);
                        TopFiveCategories = new ObservableCollection<IncomeDetails>(incomeDetailsCollectionByDays);
                    }
                    break;
                default:
                    break;
            }

            IncomesAmountSumm = Incomes.Sum(i => i.ReplenishmentMoney * i.Check.CurrencyRate.ExchangeRate);

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
