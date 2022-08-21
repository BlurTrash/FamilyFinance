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
using System.Windows.Media;

namespace FamilyFinance.Shared.Controls
{
    internal class KeyboardUtilities
    {
        internal static bool IsKeyModifyingPopupState(KeyEventArgs e)
        {
            return (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt && (e.SystemKey == Key.Down || e.SystemKey == Key.Up)
                  || e.Key == Key.F4;
        }
    }

    [TemplatePart(Name = "PART_TimeSpanUpDown", Type = typeof(TimeSpanUpDown))]
    [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_ToggleButton", Type = typeof(ToggleButton))]
    public class TimePicker : Control
    {
        #region Const
        private const string PART_TextBox = "PART_TextBox";
        private const string PART_Popup = "PART_Popup";
        private const string PART_TimeSpanUpDown = "PART_TimeSpanUpDown";
        private const string PART_ToggleButton = "PART_ToggleButton";

        internal const int MAX_DAYS = 1500;
        internal const int MAX_HOURS = 24;
        internal const int MAX_MINUTES = 60;
        internal const int MAX_SECONDS = 60;
        internal const int MAX_MINUTES_IN_DAY = 1440;
        #endregion

        #region Events
        public static readonly RoutedEvent ValueChangedEvent =
         EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Direct,
                       typeof(ValueChangedEventHandler), typeof(TimePicker));

        public event ValueChangedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        public delegate void ValueChangedEventHandler(object sender, TimeSpanUpDownValueChangedEventArgs e);

        public static readonly RoutedEvent OpenedEvent = EventManager.RegisterRoutedEvent("Opened", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TimePicker));
        public event RoutedEventHandler Opened
        {
            add { AddHandler(OpenedEvent, value); }
            remove { RemoveHandler(OpenedEvent, value); }
        }

        public static readonly RoutedEvent ClosedEvent = EventManager.RegisterRoutedEvent("Closed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TimePicker));
        public event RoutedEventHandler Closed
        {
            add { AddHandler(ClosedEvent, value); }
            remove { RemoveHandler(ClosedEvent, value); }
        }

        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TimePicker));
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }
        #endregion

        #region Properties

        #region Value
        //основное свойство Value для timepicker
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value",
           typeof(TimeSpan), typeof(TimePicker), new FrameworkPropertyMetadata(TimeSpan.Zero, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ValueChangedCallback, CoerceValue));

        public TimeSpan Value
        {
            get { return (TimeSpan)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }


        private static void ValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((TimeSpan)e.NewValue != (TimeSpan)e.OldValue)
            {

                TimePicker control = (TimePicker)d;
                var newValue = (TimeSpan)e.NewValue;
                control.isValueUpdate = true;

                if (control._timeSpanUpDown != null)
                {
                    control._timeSpanUpDown.Value = newValue;
                }

                control.Days = newValue.Days;
                control.Hours = newValue.Hours;
                control.Minutes = newValue.Minutes;
                control.Seconds = newValue.Seconds;
                control.TotalDays = newValue.TotalDays;
                control.TotalHours = newValue.TotalHours;
                control.TotalMinutes = newValue.TotalMinutes;
                control.TotalSeconds = newValue.TotalSeconds;

                control.OnValueChanged(
                    new TimeSpanUpDownValueChangedEventArgs(TimeSpanUpDown.ValueChangedEvent,
                        newValue));

                control.isValueUpdate = false;
            }
        }

        protected virtual void OnValueChanged(TimeSpanUpDownValueChangedEventArgs e)
        {
            RaiseEvent(e);
        }

        //проверка значения и принудительная установка Value
        private static object CoerceValue(DependencyObject element, object baseValue)
        {
            var control = (TimePicker)element;
            var value = (TimeSpan)baseValue;

            if (control._textBox != null)
            {
                control._textBox.Text = value.ToString();
            }

            return value;
        }
        #endregion

        #region IsOpen

        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(TimePicker), new UIPropertyMetadata(false, OnIsOpenChanged));
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (TimePicker)d;
            if (control != null)
            {
                if ((bool)e.NewValue)
                    control.RaiseRoutedEvent(OpenedEvent);
                else
                    control.RaiseRoutedEvent(ClosedEvent);
            }
        }
        private void RaiseRoutedEvent(RoutedEvent routedEvent)
        {
            RoutedEventArgs args = new RoutedEventArgs(routedEvent, this);
            RaiseEvent(args);
        }

        #endregion //IsOpen

        //обычные свойства с макс значениями для времени поидее должны быть (привязанные к константам)
        public static readonly DependencyProperty SecondsProperty = DependencyProperty.Register("Seconds",
          typeof(int), typeof(TimePicker), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTimeChanged, CoerceSeconds));

        public static readonly DependencyProperty MinutesProperty = DependencyProperty.Register("Minutes",
            typeof(int), typeof(TimePicker), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTimeChanged, CoerceMinutes));

        public static readonly DependencyProperty HoursProperty = DependencyProperty.Register("Hours",
           typeof(int), typeof(TimePicker), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTimeChanged, CoerceHours));

        public static readonly DependencyProperty DaysProperty = DependencyProperty.Register("Days",
           typeof(int), typeof(TimePicker), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTimeChanged, CoerceDays));

        //тотал свойства
        public static readonly DependencyProperty TotalSecondsProperty = DependencyProperty.Register("TotalSeconds",
         typeof(double), typeof(TimePicker), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTotalTimeChanged, CoerceTotalTime));

        public static readonly DependencyProperty TotalMinutesProperty = DependencyProperty.Register("TotalMinutes",
           typeof(double), typeof(TimePicker), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTotalTimeChanged, CoerceTotalTime));

        public static readonly DependencyProperty TotalHoursProperty = DependencyProperty.Register("TotalHours",
           typeof(double), typeof(TimePicker), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTotalTimeChanged, CoerceTotalTime));

        public static readonly DependencyProperty TotalDaysProperty = DependencyProperty.Register("TotalDays",
           typeof(double), typeof(TimePicker), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTotalTimeChanged, CoerceTotalTime));




        public int Seconds
        {
            get { return (int)GetValue(SecondsProperty); }
            set { SetValue(SecondsProperty, value); }
        }

        public int Minutes
        {
            get { return (int)GetValue(MinutesProperty); }
            set { SetValue(MinutesProperty, value); }
        }

        public int Hours
        {
            get { return (int)GetValue(HoursProperty); }
            set { SetValue(HoursProperty, value); }
        }

        public int Days
        {
            get { return (int)GetValue(DaysProperty); }
            set { SetValue(DaysProperty, value); }
        }

        public double TotalSeconds
        {
            get { return (double)GetValue(TotalSecondsProperty); }
            set { SetValue(TotalSecondsProperty, value); }
        }

        public double TotalMinutes
        {
            get { return (double)GetValue(TotalMinutesProperty); }
            set { SetValue(TotalMinutesProperty, value); }
        }

        public double TotalHours
        {
            get { return (double)GetValue(TotalHoursProperty); }
            set { SetValue(TotalHoursProperty, value); }
        }

        public double TotalDays
        {
            get { return (double)GetValue(TotalDaysProperty); }
            set { SetValue(TotalDaysProperty, value); }
        }

        //изменение времени
        private static void OnTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            var timePicker = (TimePicker)d;
            TimeSpan timeSpan = timePicker.Value;

            if (!timePicker.isValueUpdate)
            {
                if (e.Property == MinutesProperty)
                {
                    var newMinutes = (int)e.NewValue;
                    timeSpan = new TimeSpan(timeSpan.Days, timeSpan.Hours, newMinutes, timeSpan.Seconds);
                }
                else if (e.Property == HoursProperty)
                {
                    var newHours = (int)e.NewValue;
                    timeSpan = new TimeSpan(timeSpan.Days, newHours, timeSpan.Minutes, timeSpan.Seconds);
                }
                else if (e.Property == DaysProperty)
                {
                    var newDays = (int)e.NewValue;
                    timeSpan = new TimeSpan(newDays, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
                }
                else if (e.Property == SecondsProperty)
                {
                    var newSeconds = (int)e.NewValue;
                    timeSpan = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, newSeconds);
                }

                timePicker.TotalDays = timeSpan.TotalDays;
                timePicker.TotalHours = timeSpan.TotalHours;
                timePicker.TotalMinutes = timeSpan.TotalMinutes;
                timePicker.TotalSeconds = timeSpan.TotalSeconds;

                timePicker.Value = timeSpan;
            }
        }

        //изменение тотал времени
        private static void OnTotalTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var timePicker = (TimePicker)d;
            TimeSpan timeSpan = timePicker.Value;

            if (!timePicker.isValueUpdate)
            {
                if (e.Property == TotalMinutesProperty)
                {
                    timeSpan = TimeSpan.FromMinutes((double)e.NewValue);
                }
                else if (e.Property == TotalHoursProperty)
                {
                    timeSpan = TimeSpan.FromHours((double)e.NewValue);
                }
                else if (e.Property == TotalDaysProperty)
                {
                    timeSpan = TimeSpan.FromDays((double)e.NewValue);
                }
                else if (e.Property == TotalSecondsProperty)
                {
                    timeSpan = TimeSpan.FromSeconds((double)e.NewValue);
                }

                timePicker.Value = timeSpan;
            }
        }

        //проверка значения и принудительная установка тотал времени
        private static object CoerceTotalTime(DependencyObject element, object baseValue)
        {
            var control = (TimePicker)element;
            var value = (double)baseValue;

            control.CoerceValueToBounds(ref value);

            return value;
        }

        //проверка значения и принудительная установка секунд 
        private static object CoerceSeconds(DependencyObject element, object baseValue)
        {
            var control = (TimePicker)element;
            var value = (int)baseValue;

            if (value < 0)
            {
                value = 0;
            }
            else if (value >= MAX_SECONDS)
            {
                control.Minutes++;
                value = 0;
            }

            return value;
        }

        //проверка значения и принудительная установка минут 
        private static object CoerceMinutes(DependencyObject element, object baseValue)
        {
            var control = (TimePicker)element;
            var value = (int)baseValue;

            if (value < 0)
            {
                value = 0;
            }
            else if (value >= MAX_MINUTES)
            {
                control.Hours++;
                value = 0;
            }

            return value;
        }

        //проверка значения и принудительная установка часов
        private static object CoerceHours(DependencyObject element, object baseValue)
        {
            var control = (TimePicker)element;
            var value = (int)baseValue;

            if (value < 0)
            {
                value = 0;
            }
            else if (value >= MAX_HOURS)
            {
                control.Days++;
                value = 0;
            }

            return value;
        }

        //проверка значения и принудительная установка дней
        private static object CoerceDays(DependencyObject element, object baseValue)
        {
            var control = (TimePicker)element;
            var value = (int)baseValue;

            if (value < 0)
            {
                value = 0;
            }
            else if (value >= MAX_DAYS)
            {
                value = MAX_DAYS;
            }

            return value;
        }

        private void CoerceValueToBounds(ref double value)
        {
            if (value < 0d)
            {
                value = 0d;
            }
        }
        #endregion

        #region Fields
        private TextBox _textBox;
        private Popup _popup;
        private TimeSpanUpDown _timeSpanUpDown;
        private ButtonBase _button;
        private bool isValueUpdate = false;
        #endregion

        #region Constructors
        static TimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimePicker),
                                                     new FrameworkPropertyMetadata(
                                                         typeof(TimePicker)));

            EventManager.RegisterClassHandler(typeof(TimePicker), AccessKeyManager.AccessKeyPressedEvent, new AccessKeyPressedEventHandler(OnAccessKeyPressed));
        }

        public TimePicker()
        {
            Loaded += OnLoaded;
            Mouse.AddPreviewMouseDownOutsideCapturedElementHandler(this, OnMouseDownOutsideCapturedElement);
            Keyboard.AddKeyDownHandler(this, OnKeyDown);
        }
        #endregion

        #region Event handlers
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            AttachToVisualTree();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            InvalidateProperty(ValueProperty);
        }

        //private void TextBoxOnGotFocus(object sender, RoutedEventArgs e)
        //{

        //}

        private void TextBoxOnLostFocus(object sender, RoutedEventArgs e)
        {


            if (TryParseStringToTimeSpan(_textBox.Text))
            {
                Value = TimeSpan.Parse(_textBox.Text);
            }
            else
            {
                _textBox.Text = Value.ToString();
            }
        }

        //private void TextBoxTextChanged(object sender, TextChangedEventArgs e)
        //{

        //}

        private void TimeSpanUpDownValueChanged(object sender, TimeSpanUpDownValueChangedEventArgs e)
        {
            Value = e.Value;
        }

        private void OnMouseDownOutsideCapturedElement(object sender, MouseButtonEventArgs e)
        {
            if (_popup != null && !_popup.IsMouseDirectlyOver)
            {
                CloseDropDown(true);
            }
        }

        private void Popup_Opened(object sender, EventArgs e)
        {
            // Set the focus on the content of the ContentPresenter.
            if (_timeSpanUpDown != null)
            {
                _timeSpanUpDown.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            }
        }

        private void DropDownButton_Click(object sender, RoutedEventArgs e)
        {
            OnClick();
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            if (_button != null)
            {
                _button.Focus();
            }
        }

        protected override void OnAccessKey(AccessKeyEventArgs e)
        {
            if (e.IsMultiple)
            {
                base.OnAccessKey(e);
            }
            else
            {
                OnClick();
            }
        }

        private static void OnAccessKeyPressed(object sender, AccessKeyPressedEventArgs e)
        {
            if (!e.Handled && e.Scope == null && e.Target == null)
            {
                e.Target = sender as TimePicker;
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!IsOpen)
            {
                if (KeyboardUtilities.IsKeyModifyingPopupState(e))
                {
                    IsOpen = true;
                    e.Handled = true;
                }
            }
            else
            {
                if (KeyboardUtilities.IsKeyModifyingPopupState(e))
                {
                    CloseDropDown(true);
                    e.Handled = true;
                }
                else if (e.Key == Key.Escape)
                {
                    CloseDropDown(true);
                    e.Handled = true;
                }
            }
        }
        #endregion

        #region Utility Methods
        #region Attachment
        private void AttachToVisualTree()
        {
            AttachTextBox();
            AttachButton();
            AttachPopup();
            AttachTimeSpanUpDown();

        }

        private void AttachTextBox()
        {
            var textBox = GetTemplateChild(PART_TextBox) as TextBox;
            if (textBox != null)
            {
                _textBox = textBox;
                _textBox.LostFocus += TextBoxOnLostFocus;
                //this._textBox.GotFocus += TextBoxOnGotFocus;
                //this._textBox.TextChanged += TextBoxTextChanged;
                _textBox.UndoLimit = 1;
                _textBox.IsUndoEnabled = true;
            }
        }

        private void AttachButton()
        {
            if (_button != null)
                _button.Click -= DropDownButton_Click;

            var button = GetTemplateChild(PART_ToggleButton) as ToggleButton;

            if (button != null)
            {
                _button = button;
                _button.Click += DropDownButton_Click;
            }
        }

        private void AttachPopup()
        {

            if (_popup != null)
                _popup.Opened -= Popup_Opened;

            var popup = GetTemplateChild(PART_Popup) as Popup;

            if (popup != null)
            {
                _popup = popup;
                _popup.ApplyTemplate();
                _popup.Opened += Popup_Opened;
                _popup.Focusable = false;

                if (IsOpen)
                {
                    _popup.IsOpen = true;
                }
            }
        }
        private void AttachTimeSpanUpDown()
        {
            var timeSpanUpDown = GetTemplateChild(PART_TimeSpanUpDown) as TimeSpanUpDown;
            if (timeSpanUpDown != null)
            {
                timeSpanUpDown.ApplyTemplate();
                _timeSpanUpDown = timeSpanUpDown;
                _timeSpanUpDown.ValueChanged += TimeSpanUpDownValueChanged;
            }
        }


        #endregion

        private bool TryParseStringToTimeSpan(string text)
        {
            TimeSpan value;
            bool result = TimeSpan.TryParse(text, out value);
            return result;
        }

        private void CloseDropDown(bool isFocusOnTextBox)
        {
            if (IsOpen)
            {
                IsOpen = false;
            }
            ReleaseMouseCapture();

            if (isFocusOnTextBox && _textBox != null)
            {
                _textBox.Focus();
            }
        }

        protected virtual void OnClick()
        {
            RaiseRoutedEvent(ClickEvent);
        }
        #endregion
    }
}
