using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Database.Models
{
    //Таблица валют
    public class CurrencyRate : BaseEntity
    {
        /// <summary>
        /// Закодированное строковое обозначение валюты
        /// Например: USD, EUR, AUD и т.д.
        /// </summary>
        public string CurrencyStringCode { get; set; }

        /// <summary>
        /// Наименование валюты
        /// Например: Доллар, Евро и т.д.
        /// </summary>
        public string CurrencyName { get; set; }

        /// <summary>
        /// Обменный курс
        /// </summary>
        public double ExchangeRate { get; set; }
    }
}
