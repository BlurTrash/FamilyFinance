using FamilyFinance.Core.Commands;
using FamilyFinance.Model;
using FamilyFinance.View.MainView.ExpensesDialogWindow;
using FamilyFinance.WebApi.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyFinance.ViewModel.MainVM
{
    public class ExpensesVM : INotifyPropertyChanged
    {
        public Role UserRole { get { return DataManager.UserRole; } }

        //Коллекции
        public ObservableCollection<string> Months { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<int> Years { get; set; } = new ObservableCollection<int>();
        public ObservableCollection<Expense> Expenses { get; set; } = new ObservableCollection<Expense>();

        //Свойства
        public string SelectedMonth { get; set; }
        public int? SelectedYear { get; set; }

        //Комманды
        private RelayCommand _addExpenseCommand;
        public RelayCommand AddExpenseCommand =>
            _addExpenseCommand ??= new RelayCommand(AddExpense);

        private RelayCommand<Expense> _editExpenseCommand;
        public RelayCommand<Expense> EditExpenseCommand =>
            _editExpenseCommand ??= new RelayCommand<Expense>(EditExpense);

        private RelayCommand<Expense> _deleteExpenseCommand;
        public RelayCommand<Expense> DeleteExpenseCommand =>
            _deleteExpenseCommand ??= new RelayCommand<Expense>(DeleteExpense);

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

            //DateTime date = DateTime.Now;
            //var month = date.Month;
            //string mymonth = DateTime.Now.ToString("MMMM", CultureInfo.GetCultureInfo("ru-RU"));
        }

        private async void AddExpense()
        {
            Expense expense = new Expense();

            if (ShowExpenseEdit(expense, true) == true)
            {
                //обновляем расходы с учетом текущих параметров
                //await DataManager.UpdateUserChecks();
            }
        }

        private void EditExpense(Expense obj)
        {
            
        }

        private void DeleteExpense(Expense obj)
        {
            
        }

        private bool? ShowExpenseEdit(Expense expense, bool isNew)
        {
            ExpenseEditWindow window = new ExpenseEditWindow(expense, isNew);

            return window.ShowDialog();
        }
    }
}
