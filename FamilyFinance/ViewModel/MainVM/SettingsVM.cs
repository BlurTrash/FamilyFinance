using FamilyFinance.Core.Commands;
using FamilyFinance.Model;
using FamilyFinance.View.MainView.SettingsDialogWindow;
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
    public class SettingsVM : INotifyPropertyChanged
    {
        public Role UserRole { get { return DataManager.UserRole; } }

        //Коллекции
        public ObservableCollection<News> News { get; set; } = new ObservableCollection<News>();

        //Комманды
        private RelayCommand _addNewsCommand;
        public RelayCommand AddNewsCommand =>
            _addNewsCommand ??= new RelayCommand(AddNews);

        private RelayCommand<News> _editNewsCommand;
        public RelayCommand<News> EditNewsCommand =>
            _editNewsCommand ??= new RelayCommand<News>(EditNews);

        private RelayCommand<News> _deleteNewsCommand;
        public RelayCommand<News> DeleteNewsCommand =>
            _deleteNewsCommand ??= new RelayCommand<News>(DeleteCategoryNews);

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsVM()
        {
            News = DataManager.NewsFamilyFinance;
        }

        private async void AddNews()
        {
            News news = new News();

            if (ShowNewsEdit(news, true) == true)
            {
                //обновляем на клиенте новости
                await DataManager.UpdateNews();
            }
        }

        private async void EditNews(News obj)
        {
            if (ShowNewsEdit(obj, false) == true)
            {
                //обновляем на клиенте новости
                await DataManager.UpdateNews();
            }
        }

        private async void DeleteCategoryNews(News obj)
        {
            var result = MessageBox.Show("Удалить новость?", "FamilyFinance", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Func<Client, Task> manipulationDataMethod = async (client) =>
                {
                    var news = await client.ApiNewsDeleteAsync(obj.Id);
                };

                var status = await ClientManager.ManipulatonData(manipulationDataMethod);

                if (status == HttpStatusCode.OK)
                {
                    await DataManager.UpdateNews();
                }
            }
        }

        private bool? ShowNewsEdit(News news, bool isNew)
        {
            SettingsNewsEditWindow window = new SettingsNewsEditWindow(news, isNew);

            return window.ShowDialog();
        }
    }
}
