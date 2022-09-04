using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WebApi.Database.Models
{
    //Таблица подкатегорий расходов
    public class SubCategoryExpense : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int CategoryExpenseId { get; set; }
        [JsonIgnore]
        public CategoryExpense CategoryExpense { get; set; }
        //public List<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
