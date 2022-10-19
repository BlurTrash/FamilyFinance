using FamilyFinance.Core.Commands;
using FamilyFinance.Model;
using FamilyFinance.WebApi.Service;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace FamilyFinance.ViewModel.MainVM.SettingsDialogWindowVM
{
    public class SettingsNewsEditWindowVM : INotifyPropertyChanged
    {
        //DialogResult
        public Boolean? UpdateResult { get; set; }

        //Поля
        private bool _isNew;
        private bool _isModified;
        private bool _isImageLoaded;

        //Изображение
        public string ImagePath { get; set; }

        //Свойства
        public News News { get; set; }
        public News TempNews { get; set; }
        public DateTime? SelectedDay { get; set; }
        public TimeSpan? SelectedTime { get; set; }
        public byte[] UserImage { get; set; }

        //Валидация
        public bool HasErrorsHeader { get; set; }
        public bool HasErrorsDescription { get; set; }
        public bool HasErrorsDate { get; set; }

        //Комманды
        private RelayCommand _saveNewsCommand;
        public RelayCommand SaveNewsCommand =>
            _saveNewsCommand ??= new RelayCommand(SaveNews, () =>
            {
                return !HasErrorsHeader && !HasErrorsDescription && !HasErrorsDate; //проверка валидации
            });

        private RelayCommand _сancelNewsCommand;
        public RelayCommand CancelNewsCommand =>
            _сancelNewsCommand ??= new RelayCommand(CancelNews);

        private RelayCommand _changeDataCommand;
        public RelayCommand ChangeDataCommand =>
            _changeDataCommand ??= new RelayCommand(ChangeData);

        private RelayCommand _loadImageCommand;
        public RelayCommand LoadImageCommand =>
            _loadImageCommand ??= new RelayCommand(LoadImage, () => { return !_isImageLoaded; });

        private RelayCommand _clearImageCommand;
        public RelayCommand ClearImageCommand =>
            _clearImageCommand ??= new RelayCommand(ClearImage);

        private RelayCommand<object> _closingNewsEditWindowCommand;
        public RelayCommand<object> ClosingNewsEditWindowCommand =>
            _closingNewsEditWindowCommand ??= new RelayCommand<object>(ClosingNewsEditWindow);

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsNewsEditWindowVM(News news, bool isNew)
        {
            _isNew = isNew;
            TempNews = new News();
            TempNews.Header = news.Header;
            TempNews.Description = news.Description;

            if (isNew)
            {
                SelectedDay = DateTime.Now;
                SelectedTime = DateTime.Now.TimeOfDay;
                string noneImagePath = Path.GetFullPath("./Media/Image/NoneImage.jpg");
                UserImage = File.ReadAllBytes(noneImagePath);
                _isImageLoaded = false;
            }
            else
            {
                SelectedDay = news.Date;
                SelectedTime = news.Date.TimeOfDay;
                if (news.NewsImage.Length != 0)
                {
                    UserImage = news.NewsImage;
                    _isImageLoaded = true;
                }
            }

            News = news;

            _isModified = false;
        }

        private async void SaveNews()
        {
            if (await SaveChanges())
            {
                _isModified = false;
                UpdateResult = true;
            }
            else
            {
                UpdateResult = false;
            }
        }

        private void CancelNews()
        {
            UpdateResult = false;
        }

        private void ChangeData()
        {
            _isModified = true;
        }

        private void LoadImage()
        {
            if (OpenFileDialog())
            {
                var path = ImagePath;

                //BitmapImage bitmapImage = new BitmapImage(new Uri(ImagePath));
                //UserImage = BufferFromImage(bitmapImage);

                UserImage = File.ReadAllBytes(ImagePath);
                _isImageLoaded = true;
            }
        }

        //1 варинт чтения
        private byte[] BufferFromImage(BitmapImage imageSource)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                if (true)
                {
                    PngBitmapEncoder pngBitmapEncoder = new PngBitmapEncoder();
                    pngBitmapEncoder.Frames.Add(BitmapFrame.Create(imageSource));

                    pngBitmapEncoder.Save(stream);

                    return stream.ToArray();
                }
            }
        }

        //2 варинт чтения
        private byte[] ReadFile(string path)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                byte[] buffer = new byte[fileStream.Length];

                fileStream.Read(buffer, 0, (int)fileStream.Length);

                return buffer;
            }
        }

        private void ClearImage()
        {
             string noneImagePath = Path.GetFullPath("./Media/Image/NoneImage.jpg");
             UserImage = File.ReadAllBytes(noneImagePath);
            _isImageLoaded = false;
        }

        private async Task<bool> SaveChanges()
        {
            if (!HasErrorsHeader && !HasErrorsDescription && !HasErrorsDate)
            {
                News.Header = TempNews.Header;
                News.Description = TempNews.Description;
                News.NewsImage = UserImage;
                DateTime dateTime = new DateTime(SelectedDay.Value.Year, SelectedDay.Value.Month, SelectedDay.Value.Day, SelectedTime.Value.Hours, SelectedTime.Value.Minutes, SelectedTime.Value.Seconds);
                News.Date = dateTime;

                News newNews = null;

                Func<Client, Task> manipulationDataMethod = async (client) =>
                {
                    if (_isNew)
                    {
                        newNews = await client.ApiNewsPostAsync(News);
                    }
                    else
                    {
                        newNews = await client.ApiNewsPutAsync(News);
                    }
                };

                var status = await ClientManager.ManipulatonData(manipulationDataMethod);

                if (status == HttpStatusCode.OK && newNews != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        private bool OpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            //var codecs = ImageCodecInfo.GetImageEncoders();
            //var codecFilter = "Image Files|";

            var codecFilter = "Image Files|*.png;*.jpeg;*.jpg;";

            //foreach (var codec in codecs)
            //{
            //    codecFilter += codec.FilenameExtension + ";";
            //}

            openFileDialog.Filter = codecFilter;
            openFileDialog.Title = "Пожалуйста, выберите изображение.";

            if (openFileDialog.ShowDialog() == true)
            {
                ImagePath = openFileDialog.FileName;
                return true;
            }
            return false;
        }

        private void ClosingNewsEditWindow(object parametrs)
        {
            if (!_isNew)
            {
                if (_isModified && !HasErrorsHeader && !HasErrorsDescription && !HasErrorsDate)
                {
                    var dialogResult = MessageBox.Show("Вы хотите сохранить изменения?", "Family Finance", MessageBoxButton.YesNoCancel, MessageBoxImage.Information);
                    if (dialogResult == MessageBoxResult.Yes)
                    {
                        ((CancelEventArgs)parametrs).Cancel = true;
                        SaveNews();
                    }
                    else if (dialogResult == MessageBoxResult.No)
                    {
                        UpdateResult = false;
                    }
                    else
                    {
                        ((CancelEventArgs)parametrs).Cancel = true;
                    }
                }
            }

            if (UpdateResult == null)
            {
                UpdateResult = false;
            }
        }
    }
}
