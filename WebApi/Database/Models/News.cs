using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Database.Models
{
    /// <summary>
    /// Модель новостей
    /// </summary>
    public class News : BaseEntity
    {
        /// <summary>
        /// Заголовок
        /// </summary>
        public string Header { get; set; }
        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Дата новости
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Картинка
        /// </summary>
        public byte[] NewsImage { get; set; }
    }
}
