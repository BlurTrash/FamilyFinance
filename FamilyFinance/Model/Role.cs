using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyFinance.Model
{
    //Роли пользователей
   public enum Role
    {
        Admin = 1,
        Moderator = 2,
        User = 3,
        Guest = 4
    }
}
