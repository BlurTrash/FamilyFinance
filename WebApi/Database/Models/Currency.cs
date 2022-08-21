using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Database.Models
{
    //Таблица валют
    public class Currency : BaseEntity
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public List<Check> Checks { get; set; } = new List<Check>();
    }
}
