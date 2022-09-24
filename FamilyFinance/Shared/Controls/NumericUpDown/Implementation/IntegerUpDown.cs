using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FamilyFinance.Shared.Controls
{
    public class IntegerUpDown : NumericUpDownBase<int>
    {
        #region Constructors
        static IntegerUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IntegerUpDown), new FrameworkPropertyMetadata(typeof(IntegerUpDown)));
            UpdateMetadata(typeof(IntegerUpDown), int.MinValue, int.MaxValue, 1, 10);
        }

        public IntegerUpDown() { }
        #endregion

        #region UpdateMetadata
        protected static void UpdateMetadata(Type type, int minValue, int maxValue, int minorValur, int majorValue)
        {
            UpdateMetadataCommon(type, minValue, maxValue, minorValur, majorValue);
        }

        private static void UpdateMetadataCommon(Type type, int minValue, int maxValue, int minorValur, int majorValue)
        {
            MaxValueProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(maxValue));
            MinValueProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(minValue));
            MinorDeltaProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(minorValur));
            MajorDeltaProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(majorValue));
        }
        #endregion

        #region OverrideMethods
        protected override object CoerceValue(object baseValue)
        {
            var control = this;
            var value = (int)baseValue;

            control.CoerceValueToBounds(ref value);

            // получить текстовое представление значения
            var valueString = value.ToString(control.Culture);

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
                        control.TextBox.Text = value.ToString("G", control.Culture) + control.Format;
                    }
                    else
                    {
                        control.TextBox.Text = value.ToString("G", control.Culture);
                    }
                }
            }

            return value;
        }

        protected override void DecreaseValue(bool minor)
        {
            // Get the value that's currently in the _textBox.Text
            int value = ParseStringToValue(TextBox.Text);
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

        protected override void IncreaseValue(bool minor)
        {
            // Get the value that's currently in the _textBox.Text
            int value = ParseStringToValue(TextBox.Text);

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

        protected override void SetValidMajorDelta(int newMinorDelta)
        {
            if (newMinorDelta < MinorDelta)
            {
                MinorDelta = newMinorDelta;
            }
        }

        protected override void SetValidMaxValue(int newMaxValue)
        {
            if (newMaxValue < MinValue)
            {
                MinValue = newMaxValue;
            }

            if (newMaxValue <= Value)
            {
                Value = newMaxValue;
            }
        }

        protected override void SetValidMinorDelta(int newMinorDelta)
        {
            if (newMinorDelta > MajorDelta)
            {
                MajorDelta = newMinorDelta;
            }
        }

        protected override void SetValidMinValue(int newMinValue)
        {
            if (newMinValue > MaxValue)
            {
                MaxValue = newMinValue;
            }

            if (newMinValue >= Value)
            {
                Value = newMinValue;
            }
        }
        #endregion

        #region SubCoercion
        protected override void CoerceValueToBounds(ref int value)
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

        #region Data retrieval and deposit
        protected override int ParseStringToValue(string source)
        {
            int value;
            if (!string.IsNullOrEmpty(Format))
            {
                string result = source.Replace(Format, "");

                int.TryParse(result, out value);
            }
            else
            {
                int.TryParse(source, out value);
            }

            CoerceValueToBounds(ref value);

            return value;
        }
        #endregion
    }
}
