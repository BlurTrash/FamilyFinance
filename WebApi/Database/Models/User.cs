using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WebApi.Database.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        [JsonIgnore]
        public string? Password { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string? Email { get; set; }
    }
}
