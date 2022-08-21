using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FamilyFinance.Shared.ValidationRules
{
    public class ComboBoxItemNotNullValidationRule : ValidationRule
    {
        public bool IsEnabled { get; set; } = true;
        public string ErrorMessage { get; set; }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (IsEnabled)
            {
                if (value is not null)
                {
                    return new ValidationResult(true, null);
                }
                return new ValidationResult(false, ErrorMessage);
            }
            return new ValidationResult(true, null);
        }
    }
}
