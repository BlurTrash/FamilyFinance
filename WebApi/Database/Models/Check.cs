using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Database.Models
{
    //Таблица счетов
    public class Check : BaseEntity
    {
        /// <summary>
        /// Владелец счета
        /// </summary>
        public int UserId { get; set; }
        public User User { get; set; }
        public int AccountTypeId { get; set; }
        public AccountType AccountType { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        [Column(TypeName = "money")]
        public decimal Amount { get; set; }
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
        public bool IsMasterCheck { get; set; }
    }
}
