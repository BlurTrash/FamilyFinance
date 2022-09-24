using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FamilyFinance.Shared.Controls
{
    public class DecimalUpDown : NumericUpDownBase<decimal>
    {
        #region DecimalPlaces

        public static readonly DependencyProperty DecimalPlacesProperty =
            DependencyProperty.Register("DecimalPlaces", typeof(int), typeof(DecimalUpDown),
                                        new PropertyMetadata(0, OnDecimalPlacesChanged,
                                                             CoerceDecimalPlaces));

        public int DecimalPlaces
        {
            get { return (int)GetValue(DecimalPlacesProperty); }
            set { SetValue(DecimalPlacesProperty, value); }
        }

        private static void OnDecimalPlacesChanged(DependencyObject element,
                                                   DependencyPropertyChangedEventArgs e)
        {
            var control = (DecimalUpDown)element;
            var decimalPlaces = (int)e.NewValue;

            control.Culture.NumberFormat.NumberDecimalDigits = decimalPlaces;

            if (control.IsDecimalPointDynamic)
            {
                control.IsDecimalPointDynamic = false;
                control.InvalidateProperty(ValueProperty);
                control.IsDecimalPointDynamic = true;
            }
            else
            {
                control.InvalidateProperty(ValueProperty);
            }
        }

        private static object CoerceDecimalPlaces(DependencyObject element, object baseValue)
        {
            var decimalPlaces = (int)baseValue;
            var control = (DecimalUpDown)element;

            if (decimalPlaces < control.MinDecimalPlaces)
            {
                decimalPlaces = control.MinDecimalPlaces;
            }
            else if (decimalPlaces > control.MaxDecimalPlaces)
            {
                decimalPlaces = control.MaxDecimalPlaces;
            }

            return decimalPlaces;
        }

        #endregion

        #region MaxDecimalPlaces

        public static readonly DependencyProperty MaxDecimalPlacesProperty =
            DependencyProperty.Register("MaxDecimalPlaces", typeof(int), typeof(DecimalUpDown),
                                        new PropertyMetadata(28, OnMaxDecimalPlacesChanged,
                                                             CoerceMaxDecimalPlaces));

        public int MaxDecimalPlaces
        {
            get { return (int)GetValue(MaxDecimalPlacesProperty); }
            set { SetValue(MaxDecimalPlacesProperty, value); }
        }

        private static void OnMaxDecimalPlacesChanged(DependencyObject element,
                                                      DependencyPropertyChangedEventArgs e)
        {
            var control = (DecimalUpDown)element;

            control.InvalidateProperty(DecimalPlacesProperty);
        }

        private static object CoerceMaxDecimalPlaces(DependencyObject element, object baseValue)
        {
            var maxDecimalPlaces = (int)baseValue;
            var control = (DecimalUpDown)element;

            if (maxDecimalPlaces > 28)
            {
                maxDecimalPlaces = 28;
            }
            else if (maxDecimalPlaces < 0)
            {
                maxDecimalPlaces = 0;
            }
            else if (maxDecimalPlaces < control.MinDecimalPlaces)
            {
                control.MinDecimalPlaces = maxDecimalPlaces;
            }

            return maxDecimalPlaces;
        }

        #endregion

        #region MinDecimalPlaces

        public static readonly DependencyProperty MinDecimalPlacesProperty =
            DependencyProperty.Register("MinDecimalPlaces", typeof(int), typeof(DecimalUpDown),
                                        new PropertyMetadata(0, OnMinDecimalPlacesChanged,
                                                             CoerceMinDecimalPlaces));

        public int MinDecimalPlaces
        {
            get { return (int)GetValue(MinDecimalPlacesProperty); }
            set { SetValue(MinDecimalPlacesProperty, value); }
        }

        private static void OnMinDecimalPlacesChanged(DependencyObject element,
                                                      DependencyPropertyChangedEventArgs e)
        {
            var control = (DecimalUpDown)element;

            control.InvalidateProperty(DecimalPlacesProperty);
        }

        private static object CoerceMinDecimalPlaces(DependencyObject element, object baseValue)
        {
            var minDecimalPlaces = (int)baseValue;
            var control = (DecimalUpDown)element;

            if (minDecimalPlaces < 0)
            {
                minDecimalPlaces = 0;
            }
            else if (minDecimalPlaces > 28)
            {
                minDecimalPlaces = 28;
            }
            else if (minDecimalPlaces > control.MaxDecimalPlaces)
            {
                control.MaxDecimalPlaces = minDecimalPlaces;
            }

            return minDecimalPlaces;
        }

        #endregion

        #region IsDecimalPointDynamic

        public static readonly DependencyProperty IsDecimalPointDynamicProperty =
            DependencyProperty.Register("IsDecimalPointDynamic", typeof(bool), typeof(DecimalUpDown),
                                        new PropertyMetadata(false));

        public bool IsDecimalPointDynamic
        {
            get { return (bool)GetValue(IsDecimalPointDynamicProperty); }
            set { SetValue(IsDecimalPointDynamicProperty, value); }
        }

        #endregion

        #region Constructors
        static DecimalUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DecimalUpDown), new FrameworkPropertyMetadata(typeof(DecimalUpDown)));
            UpdateMetadata(typeof(DecimalUpDown), decimal.MinValue, decimal.MaxValue, 1m, 10m);
        }

        public DecimalUpDown()
        {
            Culture.NumberFormat.NumberDecimalDigits = DecimalPlaces;
        }
        #endregion

        #region UpdateMetadata
        protected static void UpdateMetadata(Type type, decimal minValue, decimal maxValue, decimal minorValur, decimal majorValue)
        {

            UpdateMetadataCommon(type, minValue, maxValue, minorValur, majorValue);
        }

        private static void UpdateMetadataCommon(Type type, decimal minValue, decimal maxValue, decimal minorValur, decimal majorValue)
        {
            MaxValueProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(maxValue));
            MinValueProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(minValue));
            MinorDeltaProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(minorValur));
            MajorDeltaProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(majorValue));
        }
        #endregion

        #region OverrideMethods
        protected override void SetValidMaxValue(decimal maxValue)
        {
            if (maxValue < MinValue)
            {
                MinValue = maxValue;
            }

            if (maxValue <= Value)
            {
                Value = maxValue;
            }
        }

        protected override void SetValidMinValue(decimal minValue)
        {
            if (minValue > MaxValue)
            {
                MaxValue = minValue;
            }

            if (minValue >= Value)
            {
                Value = minValue;
            }
        }

        protected override void SetValidMinorDelta(decimal minorDelta)
        {
            if (minorDelta > MajorDelta)
            {
                MajorDelta = minorDelta;
            }
        }

        protected override void SetValidMajorDelta(decimal majorDelta)
        {
            if (majorDelta < MinorDelta)
            {
                MinorDelta = majorDelta;
            }
        }

        protected override void IncreaseValue(bool minor)
        {
            // Get the value that's currently in the _textBox.Text
            decimal value = ParseStringToValue(TextBox.Text);

            // Coerce the value to min/max
            CoerceValueToBounds(ref value);

            // Only change the value if it has any meaning
            if (value >= MinValue)
            {
                if (minor)
                {
                    if (IsValueWrapAllowed && value + MinorDelta > MaxValue)
                    {
                        value = MinValue;
                    }
                    else
                    {
                        value += MinorDelta;
                        CoerceValueToBounds(ref value);
                    }
                }
                else
                {
                    if (IsValueWrapAllowed && value + MajorDelta > MaxValue)
                    {
                        value = MinValue;
                    }
                    else
                    {
                        value += MajorDelta;
                        CoerceValueToBounds(ref value);
                    }
                }
            }

            Value = value;
        }

        protected override void DecreaseValue(bool minor)
        {
            // Get the value that's currently in the _textBox.Text
            decimal value = ParseStringToValue(TextBox.Text);
            //decimal value = ParseStringToDecimal(textBoxText);

            // Coerce the value to min/max
            CoerceValueToBounds(ref value);

            // Only change the value if it has any meaning
            if (value <= MaxValue)
            {
                if (minor)
                {
                    if (IsValueWrapAllowed && value - MinorDelta < MinValue)
                    {
                        value = MaxValue;
                    }
                    else
                    {
                        value -= MinorDelta;
                        CoerceValueToBounds(ref value);
                    }
                }
                else
                {
                    if (IsValueWrapAllowed && value - MajorDelta < MinValue)
                    {
                        value = MaxValue;
                    }
                    else
                    {
                        value -= MajorDelta;
                        CoerceValueToBounds(ref value);
                    }
                }
            }

            Value = value;
        }

        protected override object CoerceValue(object baseValue)
        {
            var control = this;
            var value = (decimal)baseValue;

            control.CoerceValueToBounds(ref value);

            // получить текстовое представление значения
            var valueString = value.ToString(control.Culture);

            // кол-во символов дробной части если установлены
            var decimalPlaces = control.GetDecimalPlacesCount(valueString);

            if (decimalPlaces > control.DecimalPlaces)
            {
                if (control.IsDecimalPointDynamic)
                {
                    // Assigning DecimalPlaces will coerce the number
                    control.DecimalPlaces = decimalPlaces;

                    // If the specified number of decimal places is still too much
                    if (decimalPlaces > control.DecimalPlaces)
                    {
                        value = control.TruncateValue(valueString, control.DecimalPlaces);
                    }
                }
                else
                {
                    // Remove all overflowing decimal places
                    value = control.TruncateValue(valueString, decimalPlaces);
                }
            }
            else if (control.IsDecimalPointDynamic)
            {
                control.DecimalPlaces = decimalPlaces;
            }

            if (control.IsThousandSeparatorVisible)
            {
                if (control.TextBox != null)
                {
                    //control.TextBox.Text = value.ToString("N", control.Culture);
                    if (!string.IsNullOrEmpty(control.Format))
                    {
                        control.TextBox.Text = value.ToString("N", control.Culture) + control.Format;

                    }
                    else
                    {
                        control.TextBox.Text = value.ToString("N", control.Culture);
                    }
                }
            }
            else
            {
                if (control.TextBox != null)
                {
                    //control.TextBox.Text = value.ToString("F", control.Culture);

                    if (!string.IsNullOrEmpty(control.Format))
                    {
                        control.TextBox.Text = value.ToString("F", control.Culture) + control.Format;
                    }
                    else
                    {
                        control.TextBox.Text = value.ToString("F", control.Culture);
                    }
                }
            }

            return value;
        }
        #endregion

        #region Data retrieval and deposit
        protected override decimal ParseStringToValue(string source)
        {
            decimal value;
            if (!string.IsNullOrEmpty(Format))
            {
                string result = source.Replace(Format, "");

                decimal.TryParse(result, out value);
            }
            else
            {
                decimal.TryParse(source, out value);
            }

            CoerceValueToBounds(ref value);

            return value;
        }

        public int GetDecimalPlacesCount(string valueString)
        {
            return valueString.SkipWhile(c => c.ToString(Culture)
                                              != Culture.NumberFormat.NumberDecimalSeparator).Skip(1).Count();
        }

        private decimal TruncateValue(string valueString, int decimalPlaces)
        {
            var endPoint = valueString.Length - (decimalPlaces - DecimalPlaces);
            endPoint++;

            var tempValueString = valueString.Substring(0, endPoint);

            return decimal.Parse(tempValueString, Culture);
        }
        #endregion

        #region SubCoercion
        protected override void CoerceValueToBounds(ref decimal value)
        {
            if (value < MinValue)
            {
                value = MinValue;
            }
            else if (value > MaxValue)
            {
                value = MaxValue;
            }
        }
        #endregion
    }
}
