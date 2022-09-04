using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyFinance.Core.Extensions
{
    public static class Extensions
    {
        public static void AddRange<T>(this ObservableCollection<T> collection, ICollection<T> range)
        {
            foreach (T item in range)
            {
                collection.Add(item);
            }
        }
    }
}
