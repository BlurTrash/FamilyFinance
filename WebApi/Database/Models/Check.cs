using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
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
        [JsonIgnore]
        public User User { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        [Column(TypeName = "money")]
        public decimal Amount { get; set; }
        public bool IsMasterCheck { get; set; }
        public int CurrencyRateId { get; set; }
        public CurrencyRate CurrencyRate { get; set; }
        public string PresentName
        {
            get
            {
                return
                    $"[ {Name} ]" +
                    " - " +
                    $"{Amount.ToString("F2")} " + $"{(CurrencyRate != null ? CurrencyRate.CurrencyStringCode : string.Empty)}";
            }
        }
    }
}
