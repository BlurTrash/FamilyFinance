using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FamilyFinance.Shared.Controls
{
    public class FFButton : Button
    {
        public static readonly DependencyProperty IconSizeProperty = DependencyProperty.Register("IconSize", typeof(double), typeof(FFButton), new PropertyMetadata(11d));
        public double IconSize
        {
            get
            {
                return (double)GetValue(IconSizeProperty);
            }
            set
            {
                SetValue(IconSizeProperty, value);
            }
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(IconType), typeof(FFButton), new PropertyMetadata(IconType.Plus));
        public IconType Icon
        {
            get
            {
                return (IconType)GetValue(IconProperty);
            }
            set
            {
                SetValue(IconProperty, value);
            }
        }

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(FFButton));
        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(IconProperty);
            }
            set
            {
                SetValue(IconProperty, value);
            }
        }
    }
}
