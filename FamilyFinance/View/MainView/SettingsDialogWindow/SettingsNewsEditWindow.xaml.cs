using FamilyFinance.ViewModel.MainVM.SettingsDialogWindowVM;
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

namespace FamilyFinance.View.MainView.SettingsDialogWindow
{
    /// <summary>
    /// Логика взаимодействия для SettingsNewsEditWindow.xaml
    /// </summary>
    public partial class SettingsNewsEditWindow : Window
    {
        public static readonly DependencyProperty InteractionResultProperty =
          DependencyProperty.Register(nameof(InteractionResult), typeof(Boolean?), typeof(SettingsNewsEditWindow),
              new FrameworkPropertyMetadata(default(Boolean?), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnInteractionResultChanged));

        public Boolean? InteractionResult
        {
            get => (Boolean?)GetValue(InteractionResultProperty);
            set => SetValue(InteractionResultProperty, value);
        }

        private static void OnInteractionResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SettingsNewsEditWindow)d).DialogResult = e.NewValue as Boolean?;
        }

        public SettingsNewsEditWindow(News news, bool isNew)
        {
            DataContext = new SettingsNewsEditWindowVM(news, isNew);
            InitializeComponent();
        }
    }
}
