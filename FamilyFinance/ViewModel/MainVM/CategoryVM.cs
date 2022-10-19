using FamilyFinance.Core.Commands;
using FamilyFinance.Model;
using FamilyFinance.View.MainView.CategoryDialogWindows;
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

namespace FamilyFinance.ViewModel.MainVM
{
    public class CategoryVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public CategoryVM()
        {
            IncomeCategories = DataManager.IncomeСategories;
            ExpensesCategories = DataManager.ExpensesСategories;
        }

        public Role UserRole { get { return DataManager.UserRole; } }

        //категории доходов
        public ObservableCollection<Category> IncomeCategories { get; set; } = new ObservableCollection<Category>();

        private RelayCommand _addCategoryCommand;
        public RelayCommand AddCategoryCommand =>
            _addCategoryCommand ??= new RelayCommand(AddCategory);

        private RelayCommand<Category> _editCategoryCommand;
        public RelayCommand<Category> EditCategoryCommand =>
            _editCategoryCommand ??= new RelayCommand<Category>(EditCategory);

        private RelayCommand<Category> _deleteCategoryCommand;
        public RelayCommand<Category> DeleteCategoryCommand =>
            _deleteCategoryCommand ??= new RelayCommand<Category>(DeleteCategory);

        private async void AddCategory()
        {
            Category category = new Category();

            if (ShowCategoryEdit(category, true) == true)
            {
                //обновляем на клиенте категории по пользователю
                await DataManager.UpdateCategories();
            }
        }

        private async void EditCategory(Category obj)
        {
            if (ShowCategoryEdit(obj, false) == true)
            {
                //обновляем на клиенте категории по пользователю
                await DataManager.UpdateCategories();
            }
        }
        private async void DeleteCategory(Category obj)
        {
            var result = MessageBox.Show("Вместе с категорией будут удалены все доходы и подкатегории, удалить?", "FamilyFinance", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Func<Client, Task> manipulationDataMethod = async (client) =>
                {
                    var category = await client.ApiCategoryDeleteAsync(obj.Id);
                };

                var status = await ClientManager.ManipulatonData(manipulationDataMethod);

                if (status == HttpStatusCode.OK)
                {
                    await DataManager.UpdateCategories();
                }
            }
        }

        private bool? ShowCategoryEdit(Category category, bool isNew)
        {
            CategoryIncomeWindow window = new CategoryIncomeWindow(category, isNew);

            return window.ShowDialog();
        }


        //подкатегории доходов
        public ObservableCollection<SubCategory> IncomeSubCategories { get; set; } = new ObservableCollection<SubCategory>();
        public Visibility SubCategoryInfoVisibility { get; set; } = Visibility.Collapsed;
        public string SelectionCategoryName { get; set; }
        public string SelectionCategoryDescription { get; set; }
        public string SubCategoryName { get; set; }

        private Category SelectedCategory;

        private RelayCommand<Category> _selectCategoryCommand;
        public RelayCommand<Category> SelectCategoryCommand =>
            _selectCategoryCommand ??= new RelayCommand<Category>(SelectCategory);

        private RelayCommand _addSubCategoryCommand;
        public RelayCommand AddSubCategoryCommand =>
            _addSubCategoryCommand ??= new RelayCommand(AddSubCategory);

        private RelayCommand<SubCategory> _deleteSubCategoryCommand;
        public RelayCommand<SubCategory> DeleteSubCategoryCommand =>
            _deleteSubCategoryCommand ??= new RelayCommand<SubCategory>(DeleteSubCategory);

        private async void AddSubCategory()
        {
            if (SelectedCategory != null && !string.IsNullOrWhiteSpace(SubCategoryName))
            {
                SubCategory subCategory = new SubCategory { CategoryId = SelectedCategory.Id, Name = SubCategoryName };

                Func<Client, Task> manipulationDataMethod = async (client) =>
                {
                    var newSubCategory = await client.ApiSubCategoryPostAsync(subCategory);
                };

                var status = await ClientManager.ManipulatonData(manipulationDataMethod);

                if (status == HttpStatusCode.OK)
                {
                    await UpdateSubCategories(SelectedCategory.Id);
                    SubCategoryName = null;
                }
            }
        }

        private async void SelectCategory(Category obj)
        {
            if (obj != null)
            {
                SelectedCategory = obj;

                SubCategoryInfoVisibility = Visibility.Visible;
                SelectionCategoryName = obj.Name;
                SelectionCategoryDescription = obj.Description;

                await UpdateSubCategories(obj.Id);
            }
            else
            {
                SelectedCategory = null;
                SubCategoryInfoVisibility = Visibility.Collapsed;
                SelectionCategoryName = null;
                SelectionCategoryDescription = null;
            }
        }

        private async void DeleteSubCategory(SubCategory obj)
        {
            var result = MessageBox.Show("Вместе с подкатегорией будут удалены все доходы по этой подкатегории, удалить?", "FamilyFinance", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Func<Client, Task> manipulationDataMethod = async (client) =>
                {
                    var subCategory = await client.ApiSubCategoryDeleteAsync(obj.Id);
                };

                var status = await ClientManager.ManipulatonData(manipulationDataMethod);

                if (status == HttpStatusCode.OK)
                {
                    await UpdateSubCategories(SelectedCategory.Id);
                }
            }
        }

        private async Task UpdateSubCategories(int id)
        {
            Func<Client, Task> manipulationDataMethod = async (client) =>
            {
                var collectionSubCategories = await client.ApiSubCategoryGetAllByCategoryIdAsync(id);
                IncomeSubCategories = new ObservableCollection<SubCategory>(collectionSubCategories.ToList());
            };

            var status = await ClientManager.ManipulatonData(manipulationDataMethod);
        }


        //категории расходов
        public ObservableCollection<CategoryExpense> ExpensesCategories { get; set; } = new ObservableCollection<CategoryExpense>();

        private RelayCommand _addCategoryExpensesCommand;
        public RelayCommand AddCategoryExpensesCommand =>
            _addCategoryExpensesCommand ??= new RelayCommand(AddCategoryExpenses);

        private RelayCommand<CategoryExpense> _editCategoryExpensesCommand;
        public RelayCommand<CategoryExpense> EditCategoryExpensesCommand =>
            _editCategoryExpensesCommand ??= new RelayCommand<CategoryExpense>(EditCategoryExpenses);

        private RelayCommand<CategoryExpense> _deleteCategoryExpensesCommand;
        public RelayCommand<CategoryExpense> DeleteCategoryExpensesCommand =>
            _deleteCategoryExpensesCommand ??= new RelayCommand<CategoryExpense>(DeleteCategoryExpenses);

        private async void AddCategoryExpenses()
        {
            CategoryExpense category = new CategoryExpense();

            if (ShowCategoryExpenseEdit(category, true) == true)
            {
                //обновляем на клиенте категории по пользователю
                await DataManager.UpdateCategoriesExpenses();
            }
        }

        private async void EditCategoryExpenses(CategoryExpense obj)
        {
            if (ShowCategoryExpenseEdit(obj, false) == true)
            {
                //обновляем на клиенте категории по пользователю
                await DataManager.UpdateCategoriesExpenses();
            }
        }
        private async void DeleteCategoryExpenses(CategoryExpense obj)
        {
            var result = MessageBox.Show("Вместе с категорией будут удалены все расходы и подкатегории, удалить?", "FamilyFinance", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Func<Client, Task> manipulationDataMethod = async (client) =>
                {
                    var category = await client.ApiCategoryExpenseDeleteAsync(obj.Id);
                };

                var status = await ClientManager.ManipulatonData(manipulationDataMethod);

                if (status == HttpStatusCode.OK)
                {
                    await DataManager.UpdateCategoriesExpenses();
                }
            }
        }

        private bool? ShowCategoryExpenseEdit(CategoryExpense category, bool isNew)
        {
            CategoryExpenseWindow window = new CategoryExpenseWindow(category, isNew);

            return window.ShowDialog();
        }


        //подкатегории расходов
        public ObservableCollection<SubCategoryExpense> SubCategoriesExpense { get; set; } = new ObservableCollection<SubCategoryExpense>();
        public Visibility SubCategoryExpenseInfoVisibility { get; set; } = Visibility.Collapsed;
        public string SelectionCategoryExpenseName { get; set; }
        public string SelectionCategoryExpenseDescription { get; set; }
        public string SubCategoryExpenseName { get; set; }

        private CategoryExpense SelectedCategoryExpense;

        private RelayCommand<CategoryExpense> _selectCategoryExpenseCommand;
        public RelayCommand<CategoryExpense> SelectCategoryExpenseCommand =>
            _selectCategoryExpenseCommand ??= new RelayCommand<CategoryExpense>(SelectCategoryExpense);

        private RelayCommand _addSubCategoryExpenseCommand;
        public RelayCommand AddSubCategoryExpenseCommand =>
            _addSubCategoryExpenseCommand ??= new RelayCommand(AddSubCategoryExpense);

        private RelayCommand<SubCategoryExpense> _deleteSubCategoryExpenseCommand;
        public RelayCommand<SubCategoryExpense> DeleteSubCategoryExpenseCommand =>
            _deleteSubCategoryExpenseCommand ??= new RelayCommand<SubCategoryExpense>(DeleteSubCategoryExpense);

        private async void AddSubCategoryExpense()
        {
            if (SelectedCategoryExpense != null && !string.IsNullOrWhiteSpace(SubCategoryExpenseName))
            {
                SubCategoryExpense subCategory = new SubCategoryExpense { CategoryExpenseId = SelectedCategoryExpense.Id, Name = SubCategoryExpenseName };

                Func<Client, Task> manipulationDataMethod = async (client) =>
                {
                    var newSubCategory = await client.ApiSubCategoryExpensePostAsync(subCategory);
                };

                var status = await ClientManager.ManipulatonData(manipulationDataMethod);

                if (status == HttpStatusCode.OK)
                {
                    await UpdateSubCategoriesExpense(SelectedCategoryExpense.Id);
                    SubCategoryExpenseName = null;
                }
            }
        }

        private async void SelectCategoryExpense(CategoryExpense obj)
        {
            if (obj != null)
            {
                SelectedCategoryExpense = obj;

                SubCategoryExpenseInfoVisibility = Visibility.Visible;
                SelectionCategoryExpenseName = obj.Name;
                SelectionCategoryExpenseDescription = obj.Description;

                await UpdateSubCategoriesExpense(obj.Id);
            }
            else
            {
                SelectedCategoryExpense = null;
                SubCategoryExpenseInfoVisibility = Visibility.Collapsed;
                SelectionCategoryExpenseName = null;
                SelectionCategoryExpenseDescription = null;
            }
        }

        private async void DeleteSubCategoryExpense(SubCategoryExpense obj)
        {
            var result = MessageBox.Show("Вместе с подкатегорией будут удалены все расходы по этой подкатегории, удалить?", "FamilyFinance", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Func<Client, Task> manipulationDataMethod = async (client) =>
                {
                    var subCategory = await client.ApiSubCategoryExpenseDeleteAsync(obj.Id);
                };

                var status = await ClientManager.ManipulatonData(manipulationDataMethod);

                if (status == HttpStatusCode.OK)
                {
                    await UpdateSubCategoriesExpense(SelectedCategoryExpense.Id);
                }
            }
        }

        private async Task UpdateSubCategoriesExpense(int id)
        {
            Func<Client, Task> manipulationDataMethod = async (client) =>
            {
                var collectionSubCategories = await client.ApiSubCategoryExpenseGetAllByCategoryExpenseIdAsync(id);
                SubCategoriesExpense = new ObservableCollection<SubCategoryExpense>(collectionSubCategories.ToList());
            };

            var status = await ClientManager.ManipulatonData(manipulationDataMethod);
        }
    }
}
