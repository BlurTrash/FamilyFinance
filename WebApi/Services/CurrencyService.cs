using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebApi.Database.Models;

namespace WebApi.Services
{
    /// <summary>
    /// Сервис для получения курсов валют
    /// </summary>
    public class CurrencyService : BackgroundService
    {
        private readonly IMemoryCache memoryCache;

        public CurrencyService(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    //так как эта задача выполняется в другом потоке, то велика вероятность, что
                    //культура по умолчанию может отличаться от той, которая установлена в нашем приложении,
                    //поэтому явно укажем нужную нам, чтобы не было проблем с разделителями, названиями и т.д.
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU"); // <== нужная вам культура

                    //кодировка файла xml с сайта ЦБ == windows-1251
                    //по умолчанию она недоступна в .NET Core, поэтому регистрируем нужный провайдер 
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                    //т.к. мы знаем что данные к нам приходят именно в файле, именно в формате XML,
                    //поэтому нет необходимости использовать WebRequest,
                    //используем в работе класс XDocument и сразу забираем файл с удаленного сервера
                    //XDocument xml = XDocument.Load("http://www.cbr.ru/scripts/XML_daily.asp");

                    //memoryCache.Set("key_currencyXML", xml, TimeSpan.FromMinutes(1440));

                    var currencyRates = CurrencyRates.GetExchangeRates();
                    List<CurrencyRate> currencyList = new List<CurrencyRate>();

                    foreach (var item in currencyRates)
                    {
                        currencyList.Add(new CurrencyRate()
                        {
                            CurrencyName = item.ValuteName,
                            CurrencyStringCode = item.ValuteStringCode,
                            ExchangeRate = Convert.ToDouble(item.ExchangeRate)
                        });
                    }

                    memoryCache.Set("key_currencyList", currencyList, TimeSpan.FromMinutes(1440));
                }
                catch (Exception e)
                {
                    //logger.LogError(e.InnerException.Message);
                }

                //если указаний о завершении данной задачи не поступало,
                //то запрашиваем обновление данных каждый час
                await Task.Delay(3600000, stoppingToken); //3600000
            }
        }
    }
}
