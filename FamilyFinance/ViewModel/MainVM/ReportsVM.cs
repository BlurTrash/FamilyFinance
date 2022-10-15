using FamilyFinance.Core.Commands;
using FamilyFinance.Model;
using FamilyFinance.WebApi.Service;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyFinance.ViewModel.MainVM
{
    public enum ReportsTypes
    {
        [Display(Name = "Остаток на счетах")]
        ChecksBalance,
        [Display(Name = "Расходы за месяц")]
        MonthlyExpenses,
        [Display(Name = "Доходы за месяц")]
        MonthlyIncome,
        [Display(Name = "Расходы за интервал времени")]
        ExpensesPerTimeInterval,
        [Display(Name = "Доходы за интервал времени")]
        IncomePerTimeInterval
    }

    public class ReportsVM : INotifyPropertyChanged
    {
        public Role UserRole { get { return DataManager.UserRole; } }

        //Charts
        public Func<ChartPoint, string> PointLabel { get; set; }
        public SeriesCollection SeriesViews { get; set; }

        //Коллекции
        public ObservableCollection<string> Months { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<int> Years { get; set; } = new ObservableCollection<int>();
        public ObservableCollection<ReportDetails> ReportDetails { get; set; } = new ObservableCollection<ReportDetails>();
        
        //Валидация
        public bool HasErrorStartDate { get; set; }
        public bool HasErrorEndDate { get; set; }

        //Свойства
        public string ReportTitle { get; set; }
        public ReportsTypes SelectedReport { get; set; } = ReportsTypes.ChecksBalance;
        public bool IsIntervalVisibility { get; set; } = false;
        public bool IsMonthsVisibility { get; set; } = false;
        public string SelectedMonth { get; set; }
        public int? SelectedYear { get; set; }
        public DateTime SelectedStartDay { get; set; } = (DateTime.Now).AddDays(-1);
        public DateTime SelectedEndDay { get; set; } = DateTime.Now;
        public decimal AmountSpent { get; set; }

        //Поля
        private DateTime _date;

        //Комманды
        private RelayCommand _filterChangeCommand;
        public RelayCommand FilterChangeCommand =>
            _filterChangeCommand ??= new RelayCommand(FilterChange);

        private RelayCommand _updateDateCommand;
        public RelayCommand UpdateDateCommand =>
            _updateDateCommand ??= new RelayCommand(UpdateDate);

        private RelayCommand _updateReportDataCommand;
        public RelayCommand UpdateReportDataCommand =>
            _updateReportDataCommand ??= new RelayCommand(UpdateReportData, () =>
            {
                return !HasErrorStartDate && !HasErrorEndDate; //проверка валидации
            });

        private RelayCommand _startDateChangeCommand;
        public RelayCommand StartDateChangeCommand =>
            _startDateChangeCommand ??= new RelayCommand(StartDateChange);

        private RelayCommand _endDateChangeCommand;
        public RelayCommand EndDateChangeCommand =>
            _endDateChangeCommand ??= new RelayCommand(EndDateChange);
     
        public ReportsVM()
        {
            PointLabel = chartPoint =>
                string.Format("{0}, {1:P}", chartPoint.SeriesView.Title, chartPoint.Participation);

            SeriesViews = new SeriesCollection
            {
                 new PieSeries
                 {
                    Title = "Opera",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(10) },
                    DataLabels = true,
                    LabelPoint = PointLabel
                 },
                 new PieSeries
                 {
                    Title = "Chrome",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(6) },
                    DataLabels = true,
                    LabelPoint = PointLabel
                 },
                 new PieSeries
                 {
                    Title = "Mozila",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(3) },
                    DataLabels = true,
                    LabelPoint = PointLabel
                 }
            };

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

        public event PropertyChangedEventHandler PropertyChanged;


        private void FilterChange()
        {
            switch (SelectedReport)
            {
                case ReportsTypes.ChecksBalance:
                    IsIntervalVisibility = false;
                    IsMonthsVisibility = false;
                    break;
                case ReportsTypes.MonthlyExpenses:
                    IsIntervalVisibility = false;
                    IsMonthsVisibility = true;
                    break;
                case ReportsTypes.MonthlyIncome:
                    IsIntervalVisibility = false;
                    IsMonthsVisibility = true;
                    break;
                case ReportsTypes.ExpensesPerTimeInterval:
                    IsIntervalVisibility = true;
                    IsMonthsVisibility = false;
                    break;
                case ReportsTypes.IncomePerTimeInterval:
                    IsIntervalVisibility = true;
                    IsMonthsVisibility = false;
                    break;
                default:
                    break;
            }
        }

        private async void UpdateReportData()
        {
            ICollection<ReportDetails> reportDetailsCollection = null;

            Func<Client, Task> manipulationDataMethod = async (client) =>
            {
                switch (SelectedReport)
                {
                    case ReportsTypes.ChecksBalance:
                        reportDetailsCollection = await client.ApiReportsGetAllCheckBalancesAsync();
                        break;
                    case ReportsTypes.MonthlyExpenses:
                        reportDetailsCollection = await client.ApiReportsGetAllExpenseByMonthAsync(_date);
                        break;
                    case ReportsTypes.MonthlyIncome:
                        reportDetailsCollection = await client.ApiReportsGetAllIncomeByMonthAsync(_date);
                        break;
                    case ReportsTypes.ExpensesPerTimeInterval:
                        reportDetailsCollection = await client.ApiReportsGetAllExpenseByIntervalAsync(SelectedStartDay, SelectedEndDay);
                        break;
                    case ReportsTypes.IncomePerTimeInterval:
                        reportDetailsCollection = await client.ApiReportsGetAllIncomeByIntervalAsync(SelectedStartDay, SelectedEndDay);
                        break;
                    default:
                        break;
                }
               
            };

            var status = await ClientManager.ManipulatonData(manipulationDataMethod);

            if (status == System.Net.HttpStatusCode.OK)
            {
                ReportDetails = new ObservableCollection<ReportDetails>(reportDetailsCollection);

                var seriesViews = ReportDetails.Select(r => new PieSeries
                {
                    Title = r.Name,
                    Values = new ChartValues<ObservableValue> { new ObservableValue(r.Persent) },
                    DataLabels = true,
                    LabelPoint = PointLabel
                });

                SeriesViews = new SeriesCollection();
                foreach (var item in seriesViews)
                {
                    SeriesViews.Add(item);
                }

                if (SelectedReport == ReportsTypes.ChecksBalance)
                {
                    AmountSpent = ReportDetails.Sum(r => r.Summ * r.ExchageRate);
                }
                else
                {
                    AmountSpent = ReportDetails.Sum(r => r.Summ);
                }

                switch (SelectedReport)
                {
                    case ReportsTypes.ChecksBalance:
                        ReportTitle = "Счета";
                        break;
                    case ReportsTypes.MonthlyExpenses:
                        ReportTitle = "Расходы за " + SelectedMonth;
                        break;
                    case ReportsTypes.MonthlyIncome:
                        ReportTitle = "Доходы за " + SelectedMonth;
                        break;
                    case ReportsTypes.ExpensesPerTimeInterval:
                        ReportTitle = "Расходы " + SelectedStartDay.ToShortDateString() + "-" + SelectedEndDay.ToShortDateString();
                        break;
                    case ReportsTypes.IncomePerTimeInterval:
                        ReportTitle = "Расходы " + SelectedStartDay.ToShortDateString() + "-" + SelectedEndDay.ToShortDateString();
                        break;
                    default:
                        break;
                }
            }
        }

        private void UpdateDate()
        {
            _date = DateTime.ParseExact(SelectedMonth + "-" + SelectedYear.ToString(), "MMMM-yyyy", CultureInfo.GetCultureInfo("ru-RU"));
        }

        private void StartDateChange()
        {
            if (SelectedStartDay > SelectedEndDay)
            {
                SelectedEndDay = SelectedStartDay.AddDays(1);
            }
        }

        private void EndDateChange()
        {
            if (SelectedEndDay < SelectedStartDay)
            {
                SelectedStartDay = SelectedEndDay.AddDays(-1);
            }
        }
    }
}
