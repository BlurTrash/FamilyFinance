using FamilyFinance.Shared.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;


namespace FamilyFinance.Shared.Controls
{
    public class ValueChangedEventArgs<T> : RoutedEventArgs
    {
        private T _value;

        public ValueChangedEventArgs(RoutedEvent id, T num)
        {
            _value = num;
            RoutedEvent = id;
        }

        public T Value
        {
            get { return _value; }
        }
    }


    [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_IncreaseButton", Type = typeof(RepeatButton))]
    [TemplatePart(Name = "PART_DecreaseButton", Type = typeof(RepeatButton))]
    public abstract class NumericUpDownBase<T> : InputBase
    {
        #region Events
        public static readonly RoutedEvent ValueChangedEvent =
          EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Direct,
                        typeof(ValueChangedEventHandler), typeof(NumericUpDownBase<T>));

        public event ValueChangedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        public delegate void ValueChangedEventHandler(object sender, ValueChangedEventArgs<T> e);
        #endregion

        #region Value

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(T), typeof(NumericUpDownBase<T>),
                                        new PropertyMetadata(default(T), ValueChangedCallback, CoerceValue));

        public T Value
        {
            get { return (T)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private static void ValueChangedCallback(DependencyObject obj,
                                           DependencyPropertyChangedEventArgs args)
        {
            NumericUpDownBase<T> ctl = (NumericUpDownBase<T>)obj;
            T newValue = (T)args.NewValue;


            // Call OnValueChanged to raise the ValueChanged event.
            ctl.OnValueChanged(
                new ValueChangedEventArgs<T>(ValueChangedEvent,
                    newValue));
        }
        protected virtual void OnValueChanged(ValueChangedEventArgs<T> e)
        {
            // Raise the ValueChanged event so applications can be alerted
            // when Value changes.
            RaiseEvent(e);
        }



        private static object CoerceValue(DependencyObject element, object baseValue)
        {
            var control = (NumericUpDownBase<T>)element;

            return control.CoerceValue(baseValue);
        }
        protected abstract object CoerceValue(object baseValue);
        #endregion

        #region Format
        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register("Format", typeof(string), typeof(NumericUpDownBase<T>),
                                        new PropertyMetadata(null, null,
                                                             CoerceFormatValue));

        public string Format
        {
            get { return (string)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }


        private static object CoerceFormatValue(DependencyObject element, object baseValue)
        {
            var format = (string)baseValue;

            if (format == "%" || format == "мин." || format == "руб." || format == "$")
            {
                format = " " + format;
                return format;
            }
            format = null;

            return format;
        }
        #endregion

        #region Fields

        protected readonly CultureInfo Culture;
        protected RepeatButton DecreaseButton;
        protected RepeatButton IncreaseButton;
        protected TextBox TextBox;

        #endregion

        #region MaxValue

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(T), typeof(NumericUpDownBase<T>),
                                        new UIPropertyMetadata(default(T), OnMaxValueChanged,
                                                             CoerceMaxValue));

        public T MaxValue
        {
            get { return (T)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        private static void OnMaxValueChanged(DependencyObject element,
                                              DependencyPropertyChangedEventArgs e)
        {
            var control = (NumericUpDownBase<T>)element;

            control.OnMaxValueChanged((T)e.NewValue);
        }

        protected virtual void OnMaxValueChanged(T newValue)
        {
            SetValidMaxValue(newValue);
        }

        private static object CoerceMaxValue(DependencyObject element, object baseValue)
        {
            var maxValue = (T)baseValue;

            return maxValue;
        }

        protected abstract void SetValidMaxValue(T newMaxValue);
        #endregion

        #region MinValue

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(T), typeof(NumericUpDownBase<T>),
                                        new UIPropertyMetadata(default(T), OnMinValueChanged,
                                                             CoerceMinValue));

        public T MinValue
        {
            get { return (T)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        private static void OnMinValueChanged(DependencyObject element,
                                              DependencyPropertyChangedEventArgs e)
        {
            var control = (NumericUpDownBase<T>)element;

            control.OnMinValueChanged((T)e.NewValue);
        }

        protected virtual void OnMinValueChanged(T newValue)
        {
            SetValidMinValue(newValue);
        }

        private static object CoerceMinValue(DependencyObject element, object baseValue)
        {
            var minValue = (T)baseValue;

            return minValue;
        }

        protected abstract void SetValidMinValue(T newMinValue);
        #endregion

        #region MinorDelta

        public static readonly DependencyProperty MinorDeltaProperty =
            DependencyProperty.Register("MinorDelta", typeof(T), typeof(NumericUpDownBase<T>),
                                        new UIPropertyMetadata(default(T), OnMinorDeltaChanged,
                                                             CoerceMinorDelta));

        public T MinorDelta
        {
            get { return (T)GetValue(MinorDeltaProperty); }
            set { SetValue(MinorDeltaProperty, value); }
        }

        private static void OnMinorDeltaChanged(DependencyObject element,
                                                DependencyPropertyChangedEventArgs e)
        {
            var control = (NumericUpDownBase<T>)element;

            control.OnMinorDeltaChanged((T)e.NewValue);
        }

        protected virtual void OnMinorDeltaChanged(T newMinorDelta)
        {
            SetValidMinorDelta(newMinorDelta);
        }

        private static object CoerceMinorDelta(DependencyObject element, object baseValue)
        {
            var minorDelta = (T)baseValue;

            return minorDelta;
        }

        protected abstract void SetValidMinorDelta(T newMinorDelta);
        #endregion

        #region MajorDelta

        public static readonly DependencyProperty MajorDeltaProperty =
            DependencyProperty.Register("MajorDelta", typeof(T), typeof(NumericUpDownBase<T>),
                                        new UIPropertyMetadata(default(T), OnMajorDeltaChanged,
                                                             CoerceMajorDelta));

        public T MajorDelta
        {
            get { return (T)GetValue(MajorDeltaProperty); }
            set { SetValue(MajorDeltaProperty, value); }
        }

        private static void OnMajorDeltaChanged(DependencyObject element,
                                                DependencyPropertyChangedEventArgs e)
        {
            var control = (NumericUpDownBase<T>)element;

            control.OnMajorDeltaChanged((T)e.NewValue);
        }

        protected virtual void OnMajorDeltaChanged(T newMinorDelta)
        {
            SetValidMajorDelta(newMinorDelta);
        }

        private static object CoerceMajorDelta(DependencyObject element, object baseValue)
        {
            var majorDelta = (T)baseValue;

            return majorDelta;
        }

        protected abstract void SetValidMajorDelta(T newMinorDelta);
        #endregion

        #region IsThousandSeparatorVisible

        public static readonly DependencyProperty IsThousandSeparatorVisibleProperty =
            DependencyProperty.Register("IsThousandSeparatorVisible", typeof(bool), typeof(NumericUpDownBase<T>),
                                        new PropertyMetadata(false, OnIsThousandSeparatorVisibleChanged));

        public bool IsThousandSeparatorVisible
        {
            get { return (bool)GetValue(IsThousandSeparatorVisibleProperty); }
            set { SetValue(IsThousandSeparatorVisibleProperty, value); }
        }

        private static void OnIsThousandSeparatorVisibleChanged(DependencyObject element,
                                                                DependencyPropertyChangedEventArgs e)
        {
            var control = (NumericUpDownBase<T>)element;

            control.InvalidateProperty(ValueProperty);
        }

        #endregion

        #region IsAutoSelectionActive

        public static readonly DependencyProperty IsAutoSelectionActiveProperty =
            DependencyProperty.Register("IsAutoSelectionActive", typeof(bool), typeof(NumericUpDownBase<T>),
                                        new PropertyMetadata(false));

        public bool IsAutoSelectionActive
        {
            get { return (bool)GetValue(IsAutoSelectionActiveProperty); }
            set { SetValue(IsAutoSelectionActiveProperty, value); }
        }

        #endregion

        #region IsValueWrapAllowed

        public static readonly DependencyProperty IsValueWrapAllowedProperty =
            DependencyProperty.Register("IsValueWrapAllowed", typeof(bool), typeof(NumericUpDownBase<T>),
                                        new PropertyMetadata(false));

        public bool IsValueWrapAllowed
        {
            get { return (bool)GetValue(IsValueWrapAllowedProperty); }
            set { SetValue(IsValueWrapAllowedProperty, value); }
        }

        #endregion


        #region Commands

        private readonly RoutedUICommand _minorDecreaseValueCommand =
            new RoutedUICommand("MinorDecreaseValue", "MinorDecreaseValue", typeof(NumericUpDownBase<T>));

        private readonly RoutedUICommand _minorIncreaseValueCommand =
            new RoutedUICommand("MinorIncreaseValue", "MinorIncreaseValue", typeof(NumericUpDownBase<T>));

        private readonly RoutedUICommand _majorDecreaseValueCommand =
            new RoutedUICommand("MajorDecreaseValue", "MajorDecreaseValue", typeof(NumericUpDownBase<T>));

        private readonly RoutedUICommand _majorIncreaseValueCommand =
            new RoutedUICommand("MajorIncreaseValue", "MajorIncreaseValue", typeof(NumericUpDownBase<T>));

        private readonly RoutedUICommand _updateValueStringCommand =
            new RoutedUICommand("UpdateValueString", "UpdateValueString", typeof(NumericUpDownBase<T>));

        private readonly RoutedUICommand _cancelChangesCommand =
            new RoutedUICommand("CancelChanges", "CancelChanges", typeof(NumericUpDownBase<T>));

        #endregion

        #region Constructors

        static NumericUpDownBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDownBase<T>),
                                                     new FrameworkPropertyMetadata(
                                                         typeof(NumericUpDownBase<T>)));
        }

        public NumericUpDownBase()
        {
            Culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();

            Loaded += OnLoaded;
        }

        #endregion

        #region Event handlers

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            AttachToVisualTree();
            AttachCommands(); //--------остановился
        }

        private void TextBoxOnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            UpdateValue();
        }

        private void TextBoxOnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (IsAutoSelectionActive)
            {
                TextBox.SelectAll();
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            InvalidateProperty(ValueProperty);
        }


        #endregion

        #region Utility Methods

        #region Attachment

        private void AttachToVisualTree()
        {
            AttachTextBox();
            AttachIncreaseButton();
            AttachDecreaseButton();
        }

        private void AttachTextBox()
        {
            var textBox = GetTemplateChild("PART_TextBox") as TextBox;

            // A null check is advised
            if (textBox != null)
            {
                TextBox = textBox;
                TextBox.LostFocus += TextBoxOnLostFocus;
                TextBox.PreviewMouseLeftButtonUp += TextBoxOnPreviewMouseLeftButtonUp;

                TextBox.UndoLimit = 1;
                TextBox.IsUndoEnabled = true;
            }
        }

        private void AttachIncreaseButton()
        {
            var increaseButton = GetTemplateChild("PART_IncreaseButton") as RepeatButton;
            if (increaseButton != null)
            {
                IncreaseButton = increaseButton;
                IncreaseButton.Focusable = false;
                IncreaseButton.Command = _minorIncreaseValueCommand;
                IncreaseButton.PreviewMouseLeftButtonDown += (sender, args) => RemoveFocus();
            }
        }

        private void AttachDecreaseButton()
        {
            var decreaseButton = GetTemplateChild("PART_DecreaseButton") as RepeatButton;
            if (decreaseButton != null)
            {
                DecreaseButton = decreaseButton;
                DecreaseButton.Focusable = false;
                DecreaseButton.Command = _minorDecreaseValueCommand;
                DecreaseButton.PreviewMouseLeftButtonDown += (sender, args) => RemoveFocus();
            }
        }

        private void AttachCommands()
        {
            CommandBindings.Add(new CommandBinding(_minorIncreaseValueCommand, (a, b) => IncreaseValue(true)));
            CommandBindings.Add(new CommandBinding(_minorDecreaseValueCommand, (a, b) => DecreaseValue(true)));
            CommandBindings.Add(new CommandBinding(_majorIncreaseValueCommand, (a, b) => IncreaseValue(false)));
            CommandBindings.Add(new CommandBinding(_majorDecreaseValueCommand, (a, b) => DecreaseValue(false)));
            CommandBindings.Add(new CommandBinding(_updateValueStringCommand, (a, b) => UpdateValue()));
            CommandBindings.Add(new CommandBinding(_cancelChangesCommand, (a, b) => CancelChanges()));

            CommandManager.RegisterClassInputBinding(typeof(TextBox),
                                                     new KeyBinding(_minorIncreaseValueCommand, new KeyGesture(Key.Up)));
            CommandManager.RegisterClassInputBinding(typeof(TextBox),
                                                     new KeyBinding(_minorDecreaseValueCommand, new KeyGesture(Key.Down)));
            CommandManager.RegisterClassInputBinding(typeof(TextBox),
                                                     new KeyBinding(_majorIncreaseValueCommand,
                                                                    new KeyGesture(Key.PageUp)));
            CommandManager.RegisterClassInputBinding(typeof(TextBox),
                                                     new KeyBinding(_majorDecreaseValueCommand,
                                                                    new KeyGesture(Key.PageDown)));
            CommandManager.RegisterClassInputBinding(typeof(TextBox),
                                                     new KeyBinding(_updateValueStringCommand, new KeyGesture(Key.Enter)));
            CommandManager.RegisterClassInputBinding(typeof(TextBox),
                                                     new KeyBinding(_cancelChangesCommand, new KeyGesture(Key.Escape)));
        }
        #endregion

        #region Data retrieval and deposit
        protected abstract T ParseStringToValue(string source);
        #endregion

        #region SubCoercion
        protected abstract void CoerceValueToBounds(ref T value);
        #endregion
        #endregion

        #region Methods
        private void UpdateValue()
        {
            Value = ParseStringToValue(TextBox.Text);
        }

        private void CancelChanges()
        {
            TextBox.Undo();
        }

        private void RemoveFocus()
        {
            // Passes focus here and then just deletes it
            Focusable = true;
            Focus();
            Focusable = false;
        }

        protected abstract void IncreaseValue(bool minor);

        protected abstract void DecreaseValue(bool minor);
        #endregion
    }
}

