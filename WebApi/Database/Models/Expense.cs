using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WebApi.Database.Models
{
    /// <summary>
    /// Модель расходов
    /// </summary>
    public class Expense : BaseEntity
    {
        /// <summary>
        /// Пользователь которому принадлежит расход
        /// </summary>
        public int UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; }
        /// <summary>
        /// Катерия расхода
        /// </summary>
        public int? CategoryId { get; set; }
        [JsonIgnore]
        public Category? Category { get; set; }
        /// <summary>
        /// Подкатегория расхода
        /// </summary>
        public int? SubCategoryId { get; set; }
        [JsonIgnore]
        public SubCategory? SubCategory { get; set; }
        /// <summary>
        /// Счет списания расхода
        /// </summary>
        public int? CheckId { get; set; }
        public Check? Check { get; set; }
        /// <summary>
        /// Дата расхода
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Потраченная сумма расхода
        /// </summary>
        [Column(TypeName = "money")]
        public decimal SpentMoney { get; set; }
        /// <summary>
        /// Описание расхода
        /// </summary>
        public string? Description { get; set; }
    }
}
