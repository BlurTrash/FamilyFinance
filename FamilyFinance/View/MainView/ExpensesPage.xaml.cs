using FamilyFinance.ViewModel.MainVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FamilyFinance.View.MainView
{
    /// <summary>
    /// Логика взаимодействия для Expenses.xaml
    /// </summary>
    public partial class ExpensesPage : UserControl
    {
        public ExpensesPage()
        {
            InitializeComponent();
            DataContext = new ExpensesVM();
        }
    }
}
