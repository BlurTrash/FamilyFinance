using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FamilyFinance.Shared.Controls
{
    public class TimeSpanUpDownValueChangedEventArgs : RoutedEventArgs
    {
        private TimeSpan _value;

        public TimeSpanUpDownValueChangedEventArgs(RoutedEvent id, TimeSpan num)
        {
            _value = num;
            RoutedEvent = id;
        }

        public TimeSpan Value
        {
            get { return _value; }
        }
    }

    [TemplatePart(Name = "PART_DaysUpDown", Type = typeof(IntegerUpDown))]
    [TemplatePart(Name = "PART_HoursUpDown", Type = typeof(IntegerUpDown))]
    [TemplatePart(Name = "PART_MinutesUpDown", Type = typeof(IntegerUpDown))]
    [TemplatePart(Name = "PART_SecondsUpDown", Type = typeof(IntegerUpDown))]
    public class TimeSpanUpDown : Control
    {
        #region Const
        private const string PART_DaysUpDown = "PART_DaysUpDown";
        private const string PART_HoursUpDown = "PART_HoursUpDown";
        private const string PART_MinutesUpDown = "PART_MinutesUpDown";
        private const string PART_SecondsUpDown = "PART_SecondsUpDown";

        internal const int MAX_DAYS = 1500;
        internal const int MAX_HOURS = 24;
        internal const int MAX_MINUTES = 60;
        internal const int MAX_SECONDS = 60;
        internal const int MAX_MINUTES_IN_DAY = 1440;
        #endregion

        #region Events
        public static readonly RoutedEvent ValueChangedEvent =
         EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Direct,
                       typeof(ValueChangedEventHandler), typeof(TimeSpanUpDown));

        public event ValueChangedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        public delegate void ValueChangedEventHandler(object sender, TimeSpanUpDownValueChangedEventArgs e);
        #endregion

        #region Properties
        //основное свойство Value для timespan
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value",
           typeof(TimeSpan), typeof(TimeSpanUpDown), new FrameworkPropertyMetadata(TimeSpan.Zero, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ValueChangedCallback, CoerceValue));

        private static void ValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((TimeSpan)e.NewValue != (TimeSpan)e.OldValue)
            {
                TimeSpanUpDown control = (TimeSpanUpDown)d;
                var newValue = (TimeSpan)e.NewValue;

                control.DaysUpDown.Value = newValue.Days;
                control.HoursUpDown.Value = newValue.Hours;
                control.MinutesUpDown.Value = newValue.Minutes;
                control.SecondsUpDown.Value = newValue.Seconds;

                control.OnValueChanged(
                    new TimeSpanUpDownValueChangedEventArgs(ValueChangedEvent,
                        newValue));
            }
        }

        protected virtual void OnValueChanged(TimeSpanUpDownValueChangedEventArgs e)
        {
            RaiseEvent(e);
        }

        //проверка значения и принудительная установка Value
        private static object CoerceValue(DependencyObject element, object baseValue)
        {
            var control = (TimeSpanUpDown)element;
            var value = (TimeSpan)baseValue;

            //основные действия и извенения при обновлении значения value

            return value;
        }

        //свойства видимости секунд и дней
        public static readonly DependencyProperty SecondsVisibilityProperty = DependencyProperty.Register("SecondsVisibility",
            typeof(Visibility), typeof(TimeSpanUpDown), new FrameworkPropertyMetadata(Visibility.Hidden, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public Visibility SecondsVisibility
        {
            get { return (Visibility)GetValue(SecondsVisibilityProperty); }
            set { SetValue(SecondsVisibilityProperty, value); }
        }

        public static readonly DependencyProperty DaysVisibilityProperty = DependencyProperty.Register("DaysVisibility",
            typeof(Visibility), typeof(TimeSpanUpDown), new FrameworkPropertyMetadata(Visibility.Visible, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public Visibility DaysVisibility
        {
            get { return (Visibility)GetValue(DaysVisibilityProperty); }
            set { SetValue(DaysVisibilityProperty, value); }
        }


        //обычные свойства с макс значениями для времени поидее должны быть (привязанные к константам)
        public static readonly DependencyProperty SecondsProperty = DependencyProperty.Register("Seconds",
          typeof(int), typeof(TimeSpanUpDown), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTimeChanged, CoerceSeconds));

        public static readonly DependencyProperty MinutesProperty = DependencyProperty.Register("Minutes",
            typeof(int), typeof(TimeSpanUpDown), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTimeChanged, CoerceMinutes));

        public static readonly DependencyProperty HoursProperty = DependencyProperty.Register("Hours",
           typeof(int), typeof(TimeSpanUpDown), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTimeChanged, CoerceHours));

        public static readonly DependencyProperty DaysProperty = DependencyProperty.Register("Days",
           typeof(int), typeof(TimeSpanUpDown), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTimeChanged, CoerceDays));

        //тотал свойства
        public static readonly DependencyProperty TotalSecondsProperty = DependencyProperty.Register("TotalSeconds",
         typeof(double), typeof(TimeSpanUpDown), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTotalTimeChanged, CoerceTotalTime));

        public static readonly DependencyProperty TotalMinutesProperty = DependencyProperty.Register("TotalMinutes",
           typeof(double), typeof(TimeSpanUpDown), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTotalTimeChanged, CoerceTotalTime));

        public static readonly DependencyProperty TotalHoursProperty = DependencyProperty.Register("TotalHours",
           typeof(double), typeof(TimeSpanUpDown), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTotalTimeChanged, CoerceTotalTime));

        public static readonly DependencyProperty TotalDaysProperty = DependencyProperty.Register("TotalDays",
           typeof(double), typeof(TimeSpanUpDown), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTotalTimeChanged, CoerceTotalTime));


        public TimeSpan Value
        {
            get { return (TimeSpan)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

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
            var asoTimeSpan = (TimeSpanUpDown)d;
            TimeSpan timeSpan = asoTimeSpan.Value;

            asoTimeSpan._isControlUpdateTime = true;

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

            asoTimeSpan.TotalDays = timeSpan.TotalDays;
            asoTimeSpan.TotalHours = timeSpan.TotalHours;
            asoTimeSpan.TotalMinutes = timeSpan.TotalMinutes;
            asoTimeSpan.TotalSeconds = timeSpan.TotalSeconds;

            asoTimeSpan.Value = timeSpan;

            asoTimeSpan._isControlUpdateTime = false;
        }

        //изменение тотал времени
        private static void OnTotalTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            var asoTimeSpan = (TimeSpanUpDown)d;
            TimeSpan timeSpan = asoTimeSpan.Value;

            if (!asoTimeSpan._isControlUpdateTime)
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

                asoTimeSpan.Value = timeSpan;
            }
        }

        //проверка значения и принудительная установка тотал времени
        private static object CoerceTotalTime(DependencyObject element, object baseValue)
        {
            var control = (TimeSpanUpDown)element;
            var value = (double)baseValue;

            control.CoerceValueToBounds(ref value);

            return value;
        }

        //проверка значения и принудительная установка секунд 
        private static object CoerceSeconds(DependencyObject element, object baseValue)
        {
            var control = (TimeSpanUpDown)element;
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
            var control = (TimeSpanUpDown)element;
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
            var control = (TimeSpanUpDown)element;
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
            var control = (TimeSpanUpDown)element;
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
        protected IntegerUpDown DaysUpDown;
        protected IntegerUpDown HoursUpDown;
        protected IntegerUpDown MinutesUpDown;
        protected IntegerUpDown SecondsUpDown;
        private bool _isControlUpdateTime = false;
        private bool _isUpdateNumericValue = false;
        #endregion

        #region Constructors
        static TimeSpanUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimeSpanUpDown),
                                                     new FrameworkPropertyMetadata(
                                                         typeof(TimeSpanUpDown)));
        }

        public TimeSpanUpDown()
        {
            //InitializeComponent();

            Loaded += OnLoaded;
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
        #endregion

        #region Utility Methods

        #region Attachment
        private void AttachToVisualTree()
        {
            AttachDaysUpDown();
            AttachHoursUpDown();
            AttachMinutesUpDown();
            AttachSecondsUpDown();
        }

        private void AttachDaysUpDown()
        {
            var daysUpDown = GetTemplateChild(PART_DaysUpDown) as IntegerUpDown;
            if (daysUpDown != null)
            {
                DaysUpDown = daysUpDown;
                DaysUpDown.MinValue = 0;
                DaysUpDown.MaxValue = MAX_DAYS;
                DaysUpDown.ValueChanged += DaysUpDown_ValueChanged;
            }
        }

        private void DaysUpDown_ValueChanged(object sender, ValueChangedEventArgs<int> e)
        {
            Days = e.Value;
        }

        private void AttachHoursUpDown()
        {
            var hoursUpDown = GetTemplateChild(PART_HoursUpDown) as IntegerUpDown;
            if (hoursUpDown != null)
            {
                HoursUpDown = hoursUpDown;
                HoursUpDown.MinValue = 0;
                HoursUpDown.MaxValue = MAX_HOURS;
                HoursUpDown.ValueChanged += HoursUpDown_ValueChanged;
            }
        }

        private void HoursUpDown_ValueChanged(object sender, ValueChangedEventArgs<int> e)
        {
            Hours = e.Value;
        }

        private void AttachMinutesUpDown()
        {
            var minutesUpDown = GetTemplateChild(PART_MinutesUpDown) as IntegerUpDown;
            if (minutesUpDown != null)
            {
                MinutesUpDown = minutesUpDown;
                MinutesUpDown.MinValue = 0;
                MinutesUpDown.MaxValue = MAX_MINUTES;
                MinutesUpDown.ValueChanged += MinutesUpDown_ValueChanged;
            }
        }

        private void MinutesUpDown_ValueChanged(object sender, ValueChangedEventArgs<int> e)
        {
            Minutes = e.Value;
        }

        private void AttachSecondsUpDown()
        {
            var secondsUpDown = GetTemplateChild(PART_SecondsUpDown) as IntegerUpDown;
            if (secondsUpDown != null)
            {
                SecondsUpDown = secondsUpDown;
                SecondsUpDown.MinValue = 0;
                SecondsUpDown.MaxValue = MAX_SECONDS;
                SecondsUpDown.ValueChanged += SecondsUpDown_ValueChanged;
            }
        }

        private void SecondsUpDown_ValueChanged(object sender, ValueChangedEventArgs<int> e)
        {
            Seconds = e.Value;
        }
        #endregion

        #endregion
    }
}
