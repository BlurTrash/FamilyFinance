using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WebApi.Database.Models
{
    /// <summary>
    /// Модель доходов
    /// </summary>
    public class Income : BaseEntity
    {
        /// <summary>
        /// Пользователь которому принадлежит доход
        /// </summary>
        public int? UserId { get; set; }
        [JsonIgnore]
        public User? User { get; set; }
        /// <summary>
        /// Катерия дохода
        /// </summary>
        public int? CategoryId { get; set; }

        public Category? Category { get; set; }
        /// <summary>
        /// Подкатегория дохода
        /// </summary>
        public int? SubCategoryId { get; set; }

        public SubCategory? SubCategory { get; set; }
        /// <summary>
        /// Счет поплонения дохода
        /// </summary>
        public int CheckId { get; set; }
        public Check Check { get; set; }
        /// <summary>
        /// Дата дохода
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Пополненная сумма дохода
        /// </summary>
        [Column(TypeName = "money")]
        public decimal ReplenishmentMoney { get; set; }
        /// <summary>
        /// Сумма счета на момент операции
        /// </summary>
        public decimal TransactionInvoiceAmount { get; set; }
        /// <summary>
        /// Описание дохода
        /// </summary>
        public string? Description { get; set; }
    }
}
