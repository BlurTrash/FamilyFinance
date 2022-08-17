using FamilyFinance.WebApi.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyFinance.Model
{
    public sealed class DataManager
    {
        private static readonly DataManager _instance = new DataManager();
        public static DataManager Instance { get { return _instance; } }

        static DataManager() { }

        private DataManager() { }

        public static string CurrentDataBase { get; set; }

        public static User CurrentUser { get; set; } = new User();
    }
}
