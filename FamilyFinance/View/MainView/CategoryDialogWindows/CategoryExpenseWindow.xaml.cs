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
    /// Логика взаимодействия для CategoryExpenseWindow.xaml
    /// </summary>
    public partial class CategoryExpenseWindow : Window, INotifyPropertyChanged
    {
        private bool isModified;
        private bool isNew;

        public event PropertyChangedEventHandler PropertyChanged;
        public WebApi.Service.CategoryExpense CategoryExpense { get; set; }
        public WebApi.Service.CategoryExpense TempCategoryExpense { get; set; }

        private RelayCommand _saveCategoryCommand;
        public RelayCommand SaveCategoryCommand =>
        _saveCategoryCommand ??= new RelayCommand(SaveCategory, () =>
        {
            return IsValid(txBoxName);
        });

        private RelayCommand _cancelCategoryCommand;
        public RelayCommand CancelCategoryCommand =>
        _cancelCategoryCommand ??= new RelayCommand(CancelCategory);
        public CategoryExpenseWindow(WebApi.Service.CategoryExpense categoryExpense, bool isNew)
        {
            this.isNew = isNew;

            TempCategoryExpense = new WebApi.Service.CategoryExpense();
            TempCategoryExpense.Name = categoryExpense.Name;
            TempCategoryExpense.Description = categoryExpense.Description;

            InitializeComponent();

            CategoryExpense = categoryExpense;

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
                CategoryExpense.Name = TempCategoryExpense.Name;
                CategoryExpense.Description = TempCategoryExpense.Description;

                WebApi.Service.CategoryExpense newCategory = null;

                Func<Client, Task> manipulationDataMethod = async (client) =>
                {
                    if (isNew)
                    {
                        newCategory = await client.ApiCategoryExpensePostAsync(CategoryExpense);
                    }
                    else
                    {
                        newCategory = await client.ApiCategoryExpensePutAsync(CategoryExpense);
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
