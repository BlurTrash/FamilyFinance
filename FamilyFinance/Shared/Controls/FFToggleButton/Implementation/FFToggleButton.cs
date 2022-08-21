using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FamilyFinance.Shared.Controls
{
    public class FFToggleButton : FFButton
    {
        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool),
            typeof(FFButton), new FrameworkPropertyMetadata(false));

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        protected override void OnClick()
        {
            OnToggle();
            base.OnClick();
        }

        protected internal virtual void OnToggle()
        {
            IsChecked = !IsChecked;
        }
    }
}
