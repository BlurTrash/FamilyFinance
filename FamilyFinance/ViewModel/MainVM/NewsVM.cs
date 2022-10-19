using FamilyFinance.Core.Commands;
using FamilyFinance.Model;
using FamilyFinance.WebApi.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyFinance.ViewModel.MainVM
{
    public class NewsVM : INotifyPropertyChanged
    {
        public Role UserRole { get { return DataManager.UserRole; } }

        //Коллекции
        public ObservableCollection<News> News { get; set; } = new ObservableCollection<News>();

        //Команды
        private RelayCommand _updateNewsCommand;
        public RelayCommand UpdateNewsCommand =>
            _updateNewsCommand ??= new RelayCommand(UpdateNews);

        public event PropertyChangedEventHandler PropertyChanged;

        public NewsVM()
        {
            News = DataManager.NewsFamilyFinance;
        }

        private async void UpdateNews()
        {
            await DataManager.UpdateNews();
        }
    }
}
