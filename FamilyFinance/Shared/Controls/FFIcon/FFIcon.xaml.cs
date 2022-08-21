using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FamilyFinance.Shared.Controls
{
    class IconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is IconType iconType)
            {
                var icon = IconLibrary.Icons[iconType];

                if (parameter is string propName)
                {
                    return icon.GetType().GetProperty(propName).GetValue(icon);
                }

                return icon;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public partial class FFIcon : UserControl
    {
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(IconType), typeof(FFIcon), new PropertyMetadata(IconType.None));
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

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Brush), typeof(FFIcon), new PropertyMetadata(SystemColors.ControlTextBrush));
        public Brush Color
        {
            get
            {
                return (Brush)GetValue(ColorProperty);
            }
            set
            {
                SetValue(ColorProperty, value);
            }
        }

        public FFIcon()
        {
            InitializeComponent();
        }
    }
}
