using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Database.Models
{
    //Таблица видов счетов
    public class AccountType : BaseEntity
    {
        public string Name { get; set; }
        public List<Check> Checks { get; set; } = new List<Check>();
    }
}
