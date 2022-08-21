using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FamilyFinance.Shared.Converters
{
    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var format = "dd-MM-yyyy";

            if (value is DateTime dateTime)
            {
                return dateTime.ToString(format);
            }
            else 
            {
                if (value != null)
                {
                    var type = value.GetType();
                    var underlyingType = Nullable.GetUnderlyingType(type);

                    if (underlyingType != null)
                    {
                        var dt = (DateTime)value;

                        return dt.ToString(format);
                    }
                }
                else
                {
                    return null;
                }
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(DateTime.TryParse((string)value, out DateTime dateTime))
            {
                return dateTime;
            }

            return Binding.DoNothing;
        }
    }
}
