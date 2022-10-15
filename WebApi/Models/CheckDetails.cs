using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class CheckDetails
    {
        public string CheckName { get; set; }
        public string CurrencyStringCode { get; set; }
        public decimal Summ { get; set; }
        public double Persent { get; set; }
    }
}
