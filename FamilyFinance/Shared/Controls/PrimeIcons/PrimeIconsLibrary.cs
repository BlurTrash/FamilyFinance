using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyFinance.Shared.Controls
{
    public enum PrimeIconType
    {
        None,
        ArrowLeft,
        Calendar,
        Check,
        ChevronDown,
        Filter,
        FilterSlash,
        Pencil,
        Plus,
        Search,
        Times,
        User
    }

    public static class PrimeIconLibrary
    {
        public readonly static Dictionary<PrimeIconType, string> Icons = new()
        {
            [PrimeIconType.None] = "",
            [PrimeIconType.ArrowLeft] = "\ue91a",
            [PrimeIconType.Calendar] = "\ue927",
            [PrimeIconType.Check] = "\ue909",
            [PrimeIconType.ChevronDown] = "\ue902",
            [PrimeIconType.Filter] = "\ue94c",
            [PrimeIconType.FilterSlash] = "\ue9b7",
            [PrimeIconType.Pencil] = "\ue942",
            [PrimeIconType.Plus] = "\ue90d",
            [PrimeIconType.Search] = "\ue908",
            [PrimeIconType.Times] = "\ue90b",
            [PrimeIconType.User] = "\ue939"
        };
    }
}
