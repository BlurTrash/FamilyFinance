using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WebApi.Database.Models
{
    public class User
    {
        /// <summary>
        /// Id Cущности
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Деактивирована ли сущность
        /// </summary>
        public bool IsInvalid { get; set; }
        public string Login { get; set; }
        [JsonIgnore]
        public string? Password { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string? Email { get; set; }
        public int RoleId { get; set; }
        [JsonIgnore]
        public Role? Role { get; set; }

        //public List<Category> Categories { get; set; } = new List<Category>();
        //public List<Check> Checks { get; set; } = new List<Check>();
    }
}
