using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FamilyFinance.Shared.Controls.Extensions
{
    //Свойство и вычесление для TextBlock обрезания
    public class TextBlockService
    {
        static TextBlockService()
        {
            // Register for the SizeChanged event on all TextBlocks, even if the event was handled.
            EventManager.RegisterClassHandler(typeof(TextBlock),
            FrameworkElement.SizeChangedEvent,
            new SizeChangedEventHandler(OnTextBlockSizeChanged), true);
        }

        public static readonly DependencyPropertyKey IsTextTrimmedKey =
            DependencyProperty.RegisterAttachedReadOnly(
                "IsTextTrimmed",
                typeof(bool),
                typeof(TextBlockService),
                new PropertyMetadata(false)
            );

        public static readonly DependencyProperty IsTextTrimmedProperty =
            IsTextTrimmedKey.DependencyProperty;

        [AttachedPropertyBrowsableForType(typeof(TextBlock))]
        public static Boolean GetIsTextTrimmed(TextBlock target)
        {
            return (Boolean)target.GetValue(IsTextTrimmedProperty);
        }

        public static void OnTextBlockSizeChanged(object sender, SizeChangedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            if (null == textBlock)
            {
                return;
            }
            textBlock.SetValue(IsTextTrimmedKey, calculateIsTextTrimmed(textBlock));
        }

        private static bool calculateIsTextTrimmed(TextBlock textBlock)
        {
            double width = textBlock.ActualWidth;
            if (textBlock.TextTrimming == TextTrimming.None)
            {
                return false;
            }
            if (textBlock.TextWrapping != TextWrapping.NoWrap)
            {
                return false;
            }
            textBlock.Measure(new Size(double.MaxValue, double.MaxValue));
            double totalWidth = textBlock.DesiredSize.Width;
            return width < totalWidth;
        }
    }
}
