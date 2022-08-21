using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FamilyFinance.Shared.Controls
{
    public class WatermarkTextBox : TextBox
    {
        #region Properties
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register("Watermark",
           typeof(object), typeof(WatermarkTextBox), new UIPropertyMetadata(null));

        public object Watermark
        {
            get { return (object)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        public static readonly DependencyProperty IsWatermarkOnGotFocusProperty = DependencyProperty.Register("IsWatermarkOnGotFocus",
           typeof(bool), typeof(WatermarkTextBox), new UIPropertyMetadata(false));

        public bool IsWatermarkOnGotFocus
        {
            get { return (bool)GetValue(IsWatermarkOnGotFocusProperty); }
            set { SetValue(IsWatermarkOnGotFocusProperty, value); }
        }

        public static readonly DependencyProperty WatermarkTemplateProperty = DependencyProperty.Register("WatermarkTemplate",
           typeof(DataTemplate), typeof(WatermarkTextBox), new UIPropertyMetadata(null));

        public DataTemplate WatermarkTemplate
        {
            get { return (DataTemplate)GetValue(WatermarkTemplateProperty); }
            set { SetValue(WatermarkTemplateProperty, value); }
        }
        #endregion

        #region Constructor
        public WatermarkTextBox() { }
        
        static WatermarkTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WatermarkTextBox), new FrameworkPropertyMetadata(typeof(WatermarkTextBox)));
        }
        #endregion
    }
}
