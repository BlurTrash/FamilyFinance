using FamilyFinance.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyFinance.ViewModel.MainVM
{
    public class ExpensesVM
    {
        public Role UserRole { get { return DataManager.UserRole; } }
        public ExpensesVM()
        {

        }
    }
}
