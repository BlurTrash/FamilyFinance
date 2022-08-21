using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Database.Models
{
    //Таблица категорий
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public List<User> Users { get; set; } = new List<User>();
        public List<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
        public List<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
