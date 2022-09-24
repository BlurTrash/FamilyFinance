using FamilyFinance.ViewModel.MainVM.ExpensesDialogWindowVM;
using FamilyFinance.WebApi.Service;
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
using System.Windows.Shapes;

namespace FamilyFinance.View.MainView.ExpensesDialogWindow
{
    /// <summary>
    /// Логика взаимодействия для ExpenseEditWindow.xaml
    /// </summary>
    public partial class ExpenseEditWindow : Window
    {
        public static readonly DependencyProperty InteractionResultProperty =
           DependencyProperty.Register(nameof(InteractionResult), typeof(Boolean?), typeof(ExpenseEditWindow),
               new FrameworkPropertyMetadata(default(Boolean?), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnInteractionResultChanged));

        public Boolean? InteractionResult
        {
            get => (Boolean?)GetValue(InteractionResultProperty);
            set => SetValue(InteractionResultProperty, value);
        }

        private static void OnInteractionResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ExpenseEditWindow)d).DialogResult = e.NewValue as Boolean?;
        }
        public ExpenseEditWindow(Expense expense, bool isNew)
        {
            DataContext = new ExpenseEditWindowVM(expense, isNew);
            InitializeComponent();
        }
    }
}
