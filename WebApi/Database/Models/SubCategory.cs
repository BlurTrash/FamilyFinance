using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WebApi.Database.Models
{
    //Таблица подкатегорий доходов
    public class SubCategory : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        [JsonIgnore]
        public Category Category { get; set; }
        //public List<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
