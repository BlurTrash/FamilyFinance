using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Database.Models
{
    public class Role : BaseEntity
    {
        public string Name { get; set; }
        public List<User> Users { get; set; } = new List<User>();
    }
}
