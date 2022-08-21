using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Database.Models
{
    public abstract class BaseEntity
    {
        /// <summary>
        /// Id Cущности
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Деактивирована ли сущность
        /// </summary>
        public bool IsInvalid { get; set; }
    }
}
