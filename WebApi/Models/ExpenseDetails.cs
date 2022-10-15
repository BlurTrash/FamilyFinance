using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class ExpenseDetails
    {
        public string ExpenseCategoryName { get; set; }
        public decimal Summ { get; set; }
        public double Persent { get; set; }
    }
}
