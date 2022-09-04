using FamilyFinance.Core.Commands;
using FamilyFinance.Model;
using FamilyFinance.WebApi.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
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

namespace FamilyFinance.View.MainView.CategoryDialogWindows
{
    /// <summary>
    /// Логика взаимодействия для CategoryIncomeWindow.xaml
    /// </summary>
    public partial class CategoryIncomeWindow : Window, INotifyPropertyChanged
    {
        private bool isModified;
        private bool isNew;

        public event PropertyChangedEventHandler PropertyChanged;
        public WebApi.Service.Category Category { get; set; }
        public WebApi.Service.Category TempCategory { get; set; }

        private RelayCommand _saveCategoryCommand;
        public RelayCommand SaveCategoryCommand =>
        _saveCategoryCommand ??= new RelayCommand(SaveCategory, () =>
        {
            return IsValid(txBoxName);
        });

        private RelayCommand _cancelCategoryCommand;
        public RelayCommand CancelCategoryCommand =>
        _cancelCategoryCommand ??= new RelayCommand(CancelCategory);

        public CategoryIncomeWindow(WebApi.Service.Category category, bool isNew)
        {
            this.isNew = isNew;

            TempCategory = new WebApi.Service.Category();
            TempCategory.Name = category.Name;
            TempCategory.Description = category.Description;

            InitializeComponent();

            Category = category;

            isModified = false;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetModified();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!isNew)
            {
                if (isModified && IsValid(txBoxName))
                {
                    var dialogResult = MessageBox.Show("Вы хотите сохранить изменения?", "Family Finance", MessageBoxButton.YesNoCancel, MessageBoxImage.Information);
                    if (dialogResult == MessageBoxResult.Yes)
                    {
                        e.Cancel = true;
                        SaveCategory();
                    }
                    else if (dialogResult == MessageBoxResult.No)
                    {
                        DialogResult = false;
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
            }

            if (DialogResult == null)
            {
                DialogResult = false;
            }
        }
      
        private async void SaveCategory()
        {
            if (await SaveChanges())
            {
                isModified = false;
                DialogResult = true;
            }
            else
            {
                DialogResult = false;
            }
        }

        private void CancelCategory()
        {
            DialogResult = false;
        }

        private async Task<bool> SaveChanges()
        {
            if (IsValid(txBoxName))
            {
                Category.Name = TempCategory.Name;
                Category.Description = TempCategory.Description;

                WebApi.Service.Category newCategory = null;

                Func<Client, Task> manipulationDataMethod = async (client) =>
                {
                    if (isNew)
                    {
                        newCategory = await client.ApiCategoryPostAsync(Category);
                    }
                    else
                    {
                        newCategory = await client.ApiCategoryPutAsync(Category);
                    }
                };

                var status = await ClientManager.ManipulatonData(manipulationDataMethod);

                if (status == HttpStatusCode.OK && newCategory != null)
                {
                    return true;
                }
            }
            return false;
        }

        private void SetModified()
        {
            isModified = true;
        }

        private bool IsValid(DependencyObject obj)
        {
            if (((Control)obj).IsEnabled)
            {
                return !Validation.GetHasError(obj) &&
                LogicalTreeHelper.GetChildren(obj)
                .OfType<DependencyObject>()
                .All(IsValid);
            }
            else
            {
                return true;
            }
        }
    }
}
