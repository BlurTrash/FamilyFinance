using FamilyFinance.WebApi.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FamilyFinance.Model
{
    public enum ManipulatonType
    {
        Load,
        Update
    }

    /// <summary>
    /// Статус манипуляции данных
    /// </summary>
    public enum ManipulatonStatus
    {
        Start,
        Complete,
        Error
    }

    public class ManipulatonEventArgs
    {
        public ManipulatonStatus Status { get; set; }
        public ManipulatonType Type { get; set; }
    }

    /// <summary>
    /// Класс для управления web-api клиентом
    /// </summary>
    public static class ClientManager
    {
        private static string _baseUrl;

        public static string BaseUrl
        {
            get
            {
                return _baseUrl;
            }
            set
            {
                _baseUrl = value;
                _client = new Client(_baseUrl, _httpClient);
                // Обязательно включаем!
                _client.ReadResponseAsString = true;
            }
        }

        private static HttpClient _httpClient;

        private static Client _client;

        public delegate void NotifyHandler(ManipulatonEventArgs e);

        /// <summary>
        /// Событие возникающее при манипуляции с данными
        /// </summary>
        public static event NotifyHandler? NotifyManipulatonData;

        static ClientManager()
        {
            _httpClient = new HttpClient();
            BaseUrl = "https://localhost:5001/";
        }

        /// <summary>
        /// Устанавливает ключ авторизации
        /// </summary>
        /// <param name="token">Ключ авторизации</param>
        public static void SetAuthorizationBearer(string token)
        {
            // При успешном результате сохраняем token в _httpClient для последующих запросов
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
        }

        private static List<Task> _tasks = new List<Task>();

        public static async Task<HttpStatusCode> ManipulatonData(
            Func<Client, Task> manipulatonDataMethod,
            ManipulatonType manipulatonType = ManipulatonType.Load
        )
        {
            HttpStatusCode resultStatus = HttpStatusCode.OK; // Список ошибок https://docs.microsoft.com/ru-ru/dotnet/api/system.net.httpstatuscode?view=net-6.0

            Task _currentTask = null;
            try
            {
                Debug.WriteLine($"Начало ManipulatonData - незавершенных задач: {_tasks.Count}");
                // Эммитим события если нет запущеных задач
                if (_tasks.Count == 0)
                {
                    NotifyManipulatonData?
                       .Invoke(
                           new ManipulatonEventArgs()
                           {
                               Status = ManipulatonStatus.Start,
                               Type = manipulatonType
                           }
                       );
                }
                
                _currentTask = manipulatonDataMethod(_client);
                _tasks.Add(_currentTask);

                await _currentTask;
            }
            catch (ApiException ex)
            {
                resultStatus = ProcessingApiExeption(ex);

                NotifyManipulatonData?
                    .Invoke(
                        new ManipulatonEventArgs()
                        {
                            Status = ManipulatonStatus.Error
                        }
                    );
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Внимание", MessageBoxButton.OK, MessageBoxImage.Error);
                NotifyManipulatonData?
                    .Invoke(
                        new ManipulatonEventArgs()
                        {
                            Status = ManipulatonStatus.Error,
                            Type = manipulatonType
                        }
                    );
                resultStatus = HttpStatusCode.InternalServerError;
            }
            finally
            {
                // Удаляем текущую завершенную задачу
                _tasks.Remove(_currentTask);
                Debug.WriteLine($"Конец ManipulatonData - незавершенных задач: {_tasks.Count}");

                if (_tasks.Count == 0)
                {
                    NotifyManipulatonData?
                   .Invoke(
                       new ManipulatonEventArgs()
                       {
                           Status = ManipulatonStatus.Complete,
                           Type = manipulatonType
                       }
                   );
                }
            }
            return resultStatus;
        }

        private static HttpStatusCode ProcessingApiExeption(ApiException ex)
        {
            HttpStatusCode resultStatus = HttpStatusCode.InternalServerError;

            if (ex.StatusCode == 200)
            {
                resultStatus = HttpStatusCode.OK;
            }
            else if (ex.StatusCode == 204)
            {
                resultStatus = HttpStatusCode.NoContent;
            }
            else if (ex.StatusCode == 401) // Пользователь не авторизован
            {
                MessageBox.Show("Вы не авторизованы, введите имя пользователя и пароль.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                resultStatus = HttpStatusCode.Unauthorized;
            }
            else if (ex.StatusCode == 403) // Недостаточно прав
            {
                var message = (string.IsNullOrEmpty(ex.Response)) ? ex.Message : ex.Response;
                MessageBox.Show(message, "Внимание (403)", MessageBoxButton.OK, MessageBoxImage.Error);

                resultStatus = HttpStatusCode.Forbidden;
            }
            else if (ex.StatusCode == 409) // Конфликт обновления
            {
                var message = (string.IsNullOrEmpty(ex.Response)) ? "Возможно другой пользователь обновил запись." : ex.Response;
                MessageBox.Show(message, "Внимание (409)", MessageBoxButton.OK, MessageBoxImage.Warning);

                resultStatus = HttpStatusCode.Conflict;
            }
            else if (ex.StatusCode == 400) // Плохой запрос
            {
                var message = (string.IsNullOrEmpty(ex.Response)) ? ex.Message : ex.Response;
                MessageBox.Show(message, "Внимание (400)", MessageBoxButton.OK, MessageBoxImage.Error);

                resultStatus = HttpStatusCode.BadRequest;
            }
            else if (ex.StatusCode == 404) // Не найден
            {
                var message = (string.IsNullOrEmpty(ex.Response)) ? ex.Message : ex.Response;
                MessageBox.Show(message, "Внимание (404)", MessageBoxButton.OK, MessageBoxImage.Warning);

                resultStatus = HttpStatusCode.NotFound;
            }
            else
            {
                MessageBox.Show(ex.Message, "Внимание", MessageBoxButton.OK, MessageBoxImage.Error);
                resultStatus = HttpStatusCode.InternalServerError;
            }
            return resultStatus;
        }


        /// <summary>
        /// Добавляет задачу к списку выполняемых задач и эммитит события подписчиткам о начале и окончании операции
        /// </summary>
        /// <param name="task"></param>
        public static async void AddTaskAsync(Task task)
        {
            Debug.WriteLine($"Начало AddTaskAsync - незавершенных задач: {_tasks.Count}");
            if (_tasks.Count == 0)
            {
                // Эммитим события если нет запущеных задач
                NotifyManipulatonData?.Invoke(
                    new ManipulatonEventArgs()
                    {
                        Status = ManipulatonStatus.Start,
                        Type = ManipulatonType.Load,
                    }
                );
            }
            _tasks.Add(task);

            await task;

            _tasks.Remove(task);

            Debug.WriteLine($"Конец AddTaskAsync - незавершенных задач: {_tasks.Count}");
            if (_tasks.Count == 0)
            {
                NotifyManipulatonData?.Invoke(
                    new ManipulatonEventArgs()
                    {
                        Status = ManipulatonStatus.Complete,
                        Type = ManipulatonType.Load,
                    }
                );
            }
        }
    }
}
