using FamilyFinance.ViewModel.MainVM.ChecksDialogWindowVM;
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

namespace FamilyFinance.View.MainView.ChecksDialogWindow
{
    /// <summary>
    /// Логика взаимодействия для ChecksEditWindow.xaml
    /// </summary>
    public partial class ChecksEditWindow : Window
    {
        public static readonly DependencyProperty InteractionResultProperty =
           DependencyProperty.Register(nameof(InteractionResult), typeof(Boolean?), typeof(ChecksEditWindow),
               new FrameworkPropertyMetadata(default(Boolean?), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnInteractionResultChanged));

        public Boolean? InteractionResult
        {
            get => (Boolean?)GetValue(InteractionResultProperty);
            set => SetValue(InteractionResultProperty, value);
        }

        private static void OnInteractionResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ChecksEditWindow)d).DialogResult = e.NewValue as Boolean?;
        }

        public ChecksEditWindow(Check check, bool isNew)
        {
            DataContext = new ChecksEditWindowVM(check, isNew);
            InitializeComponent();
        }
    }
}
