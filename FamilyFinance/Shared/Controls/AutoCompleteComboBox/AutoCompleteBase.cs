using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using FamilyFinance.Shared.Controls;

namespace FamilyFinance.Shared.Controls
{
    public abstract class AutoCompleteBase<T1, T2> : WatermarkComboBox
    {
        #region Properties
        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter",
            typeof(T2), typeof(AutoCompleteBase<T1, T2>), new FrameworkPropertyMetadata(default(T2), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)); //OnFilterChanged

        public T2 Filter
        {
            get { return (T2)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        //private static async void OnFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var autoComplete = d as AutoCompleteBase<T1, T2>;

        //    if (autoComplete != null)
        //    {
        //        autoComplete.ItemsSource = await autoComplete.Search(null, null, autoComplete.Filter);
        //    }
        //}

        public static readonly DependencyProperty DelayProperty = DependencyProperty.Register("Delay",
            typeof(int), typeof(AutoCompleteBase<T1, T2>), new FrameworkPropertyMetadata(1000, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public int Delay
        {
            get { return (int)GetValue(DelayProperty); }
            set { SetValue(DelayProperty, value); }
        }

        public static readonly DependencyProperty MinLengthProperty = DependencyProperty.Register("MinLength",
            typeof(int), typeof(AutoCompleteBase<T1, T2>), new FrameworkPropertyMetadata(3, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public int MinLength
        {
            get { return (int)GetValue(MinLengthProperty); }
            set { SetValue(MinLengthProperty, value); }
        }
        #endregion

        #region Fields
        private ListView _listView;
        private CancellationTokenSource tokenSource;
        private bool isSwipe;
        private bool isBackspace;
        #endregion

        #region Constructor
        public AutoCompleteBase()
        {
            IsEditable = true;
            IsTextSearchEnabled = false;

            var myResourceDictionary = new ResourceDictionary();
            myResourceDictionary.Source = new Uri("/FamilyFinance;component/FFResourceDictionary.xaml", UriKind.RelativeOrAbsolute);

            var style = myResourceDictionary["AutoCompleteListViewTemplate"] as Style;

            if (style != null)
            {
                Style = style;
            }

            Loaded += AutoCompleteBase_Loaded;
        }

        private async void AutoCompleteBase_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }
            ItemsSource = await Search("", null, Filter);
        }
        #endregion

        #region Search
        public abstract Task<ICollection<T1>> Search(string text, int? maxRows, T2 filter);
        #endregion

        #region OnApplyTemplate
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (IsEditable)
            {
                var element = GetTemplateChild("PART_EditableTextBox");
                if (element != null)
                {
                    //var textBox = (WatermarkTextBox)element;
                    if (element is TextBox textBox)
                    {
                        textBox.TextChanged += TextBox_TextChanged;
                        //textBox.LostFocus += TextBox_LostFocus;
                    }
                }
            }

            AttachListView();
        }

        private void AttachListView()
        {
            var element = GetTemplateChild("ListView") as ListView;

            if (element != null)
            {
                _listView = element;
                _listView.SelectionChanged += _listView_SelectionChanged;
            }
        }

        private void _listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsDropDownOpen)
            {
                IsDropDownOpen = false;
            }
        }
        #endregion

        #region Event handlers
        private async void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Filter != null && IsEditable)
            {
                var comboBox = this;
                var textBox = sender as TextBox;
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    tokenSource?.Cancel();

                    ItemsSource = await Search("", null, Filter);
                    comboBox.IsDropDownOpen = false;
                    textBox.Select(textBox.SelectionStart + textBox.SelectionLength, 0);
                    //textBox.Focus();
                    return;
                }
                else
                {
                    if (textBox.Text.Length < MinLength)
                    {
                        return;
                    }
                }

                tokenSource?.Cancel();
                tokenSource?.Dispose();

                tokenSource = new CancellationTokenSource();
                var token = tokenSource.Token;

                string text = textBox.Text;

                await Task.Delay(Delay);

                if (!token.IsCancellationRequested)
                {
                    ItemsSource = await Search(text, null, Filter);
                    comboBox.IsDropDownOpen = true;
                    textBox.Select(textBox.SelectionStart + textBox.SelectionLength, 0);
                    //textBox.Focus();
                }
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (IsEditable)
            {
                var textBox = sender as TextBox;

                if (SelectedItem == null && !string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = null;
                }
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (IsEditable)
            {
                if (e.Key == Key.Back)
                {
                    isBackspace = true;
                    SelectedItem = null;
                }

                base.OnPreviewKeyDown(e);

                isBackspace = false;
            }
            else
            {
                base.OnPreviewKeyDown(e);
            }
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            if (IsEditable)
            {
                var element = GetTemplateChild("PART_EditableTextBox");
                if (element != null)
                {
                    var textBox = (TextBox)element;
                    textBox.TextChanged -= TextBox_TextChanged;

                    if (!isSwipe && !isBackspace)
                    {
                        base.OnSelectionChanged(e);
                        textBox.Select(textBox.SelectionStart + textBox.SelectionLength, 0);
                    }

                    textBox.TextChanged += TextBox_TextChanged;
                }
            }
            else
            {
                base.OnSelectionChanged(e);
            }
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (IsEditable)
            {
                base.OnGotFocus(e);

                var textBox = (TextBox)GetTemplateChild("PART_EditableTextBox");
                textBox.Select(textBox.Text.Length, 0);
            }
            else
            {
                base.OnGotFocus(e);
            }
        }
        #endregion
    }
}
