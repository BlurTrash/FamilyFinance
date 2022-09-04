using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WebApi.Database.Models
{
    //Таблица категорий расходов
    public class CategoryExpense : BaseEntity
    {
        public int UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        //public List<User> Users { get; set; } = new List<User>();
        //public List<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
        //public List<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
