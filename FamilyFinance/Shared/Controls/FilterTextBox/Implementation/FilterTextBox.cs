using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FamilyFinance.Shared.Controls
{
    public enum FilterTypes
    {
        [Display(Name = "Начинается")]
        StartsWith,
        [Display(Name = "Содержит")]
        Contains,
        [Display(Name = "Заканчивается")]
        EndsWith,
        [Display(Name = "Равно")]
        Equals,
        [Display(Name = "Не равно")]
        NotEquals
    }

    [TemplatePart(Name = PART_ListBox, Type = typeof(ListBox))]
    [TemplatePart(Name = PART_TextBox, Type = typeof(WatermarkTextBox))]
    [TemplatePart(Name = PART_DropDownButtonFilter, Type = typeof(Button))]
    [TemplatePart(Name = PART_ButtonResetFilter, Type = typeof(Button))]
    public class FilterTextBox : Control
    {
        #region Properties
        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register("IsDropDownOpen",
              typeof(bool), typeof(FilterTextBox), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        public static readonly DependencyProperty FilterTypeProperty = DependencyProperty.Register("FilterType",
             typeof(FilterTypes), typeof(FilterTextBox), new FrameworkPropertyMetadata(FilterTypes.StartsWith, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnFilterTypeChanged));

        private static void OnFilterTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
           
        }

        public FilterTypes FilterType
        {
            get { return (FilterTypes)GetValue(FilterTypeProperty); }
            set { SetValue(FilterTypeProperty, value); }
        }

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source",
             typeof(ICollectionView), typeof(FilterTextBox), new FrameworkPropertyMetadata(null, OnSourceChanged));
        
        public ICollectionView Source
        {
            get { return (ICollectionView)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FilterTextBox filterTextBox = (FilterTextBox)d;

            filterTextBox._collectionView = (CollectionView)CollectionViewSource.GetDefaultView(e.NewValue);
        }

        //public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register("PropertyName",
        //    typeof(string), typeof(FilterTextBox), new FrameworkPropertyMetadata(null, OnPropertyNameChanged));

        //private static void OnPropertyNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
            
        //}

        //public string PropertyName
        //{
        //    get { return (string)GetValue(PropertyNameProperty); }
        //    set { SetValue(PropertyNameProperty, value); }
        //}

        public static readonly DependencyProperty PropertyPathProperty = DependencyProperty.Register("PropertyPath",
           typeof(string), typeof(FilterTextBox), new FrameworkPropertyMetadata(null));

        public string PropertyPath
        {
            get { return (string)GetValue(PropertyPathProperty); }
            set { SetValue(PropertyPathProperty, value); }
        }

        private ICollectionView _collectionView;
        public ICollectionView CollectionView { get { return _collectionView; } }

        #endregion

        #region Methods
        private object FollowPropertyPath(object value, string path)
        {
            Type currentType = value.GetType();

            foreach (string propertyName in path.Split('.'))
            {
                PropertyInfo property = currentType.GetProperty(propertyName);
                if (property != null)
                {
                    value = property.GetValue(value, null);
                    currentType = property.PropertyType;
                }
                else
                {
                    return null;
                }    
            }
            return value;
        }

        private void Filter()
        {
            if (Source != null)
            {
                var propertyPath = PropertyPath;

                CollectionView.Filter = (item) =>
                {
                    //Type type = item.GetType();

                    //var property = type.GetProperty(propertyPath);

                    switch (FilterType)
                    {
                        case FilterTypes.StartsWith:

                            if (!string.IsNullOrWhiteSpace(propertyPath))
                            {
                                //var value = property.GetValue(item);
                                var value = FollowPropertyPath(item, propertyPath);
                                if (value != null)
                                {
                                    return value.ToString().StartsWith(_textBox.Text, StringComparison.CurrentCultureIgnoreCase);
                                }
                            }
                            return item.ToString().StartsWith(_textBox.Text, StringComparison.CurrentCultureIgnoreCase);
                           
                        case FilterTypes.Contains:
                            if (!string.IsNullOrWhiteSpace(propertyPath))
                            {
                                //var value = property.GetValue(item);
                                var value = FollowPropertyPath(item, propertyPath);
                                if (value != null)
                                {
                                    return value.ToString().Contains(_textBox.Text, StringComparison.CurrentCultureIgnoreCase);
                                }
                            }
                            return item.ToString().Contains(_textBox.Text, StringComparison.CurrentCultureIgnoreCase);
                            
                        case FilterTypes.EndsWith:
                            if (!string.IsNullOrWhiteSpace(propertyPath))
                            {
                                //var value = property.GetValue(item);
                                var value = FollowPropertyPath(item, propertyPath);
                                if (value != null)
                                {
                                    return value.ToString().EndsWith(_textBox.Text, StringComparison.CurrentCultureIgnoreCase);
                                }
                            }
                            return item.ToString().EndsWith(_textBox.Text, StringComparison.CurrentCultureIgnoreCase);
                            
                        case FilterTypes.Equals:
                            if (!string.IsNullOrWhiteSpace(propertyPath))
                            {
                                //var value = property.GetValue(item);
                                var value = FollowPropertyPath(item, propertyPath);
                                if (value != null)
                                {
                                    return value.ToString().Equals(_textBox.Text, StringComparison.CurrentCultureIgnoreCase);
                                }
                            }
                            return item.ToString().Equals(_textBox.Text, StringComparison.CurrentCultureIgnoreCase);
                            
                        case FilterTypes.NotEquals:
                            if (!string.IsNullOrWhiteSpace(propertyPath))
                            {
                                //var value = property.GetValue(item);
                                var value = FollowPropertyPath(item, propertyPath);
                                if (value != null)
                                {
                                    return !value.ToString().Equals(_textBox.Text, StringComparison.CurrentCultureIgnoreCase);
                                }
                            }
                            return !item.ToString().Equals(_textBox.Text, StringComparison.CurrentCultureIgnoreCase);

                        default:
                            if (!string.IsNullOrWhiteSpace(propertyPath))
                            {
                                //var value = property.GetValue(item);
                                var value = FollowPropertyPath(item, propertyPath);
                                if (value != null)
                                {
                                    return value.ToString().IndexOf(_textBox.Text, StringComparison.CurrentCultureIgnoreCase) >= 0;
                                }
                            }
                            return item.ToString().IndexOf(_textBox.Text, StringComparison.CurrentCultureIgnoreCase) >= 0;
                    }
                };
            }
        }

        private void Reset()
        {
            _textBox.Clear();
            if (Source != null)
            {
                CollectionView.Filter = null;
            }
        }
        #endregion

        #region Const
        private const string PART_ListBox = "PART_ListBox";
        private const string PART_TextBox = "PART_TextBox";
        private const string PART_DropDownButtonFilter = "PART_Button";
        private const string PART_ButtonResetFilter = "PART_ButtonResetFilter";
        #endregion

        #region Ctor
        static FilterTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FilterTextBox),
                                                     new FrameworkPropertyMetadata(
                                                         typeof(FilterTextBox)));
        }
        public FilterTextBox()
        {
        }
        #endregion

        #region Fields
        private ListBox _listBox;
        private TextBox _textBox;
        private Button _dropDownButtonFilter;
        private Button _buttonResetFilter;
        #endregion

        #region OnApplyTemplate
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            AttacListBox();
            AttachWatermarkTextBox();
            AttachDropDownButtonFilter();
            AttachDropDownButtonResetFilter();
        }

        #region AttacListBox
        private void AttacListBox()
        {
            if (_listBox != null)
            {
                _listBox.PreviewMouseDown += _listBox_PreviewMouseDown;
                //_listBox.SelectionChanged += _listBox_SelectionChanged;
            }

            var element = GetTemplateChild(PART_ListBox) as ListBox;

            if (element != null)
            {
                _listBox = (ListBox)element;
            }

            if (_listBox != null)
            {
                _listBox.PreviewMouseDown += _listBox_PreviewMouseDown;
                //_listBox.SelectionChanged += _listBox_SelectionChanged;
            }
        }


        private void _listBox_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox != null)
            {
                DependencyObject mouseItem = e.OriginalSource as DependencyObject;
                if (mouseItem != null)
                {
                    var container = listBox.ContainerFromElement(mouseItem) as ListBoxItem;
                    if (container != null)
                    {
                        container.IsSelected = true;

                        if (!string.IsNullOrWhiteSpace(_textBox.Text))
                        {
                            Filter();
                        }
                    }
                }
            }
        }

        //private void _listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var selectedItem = _listBox.SelectedItem;

        //    if (!string.IsNullOrWhiteSpace(_textBox.Text))
        //    {
        //        Filter();
        //    }
        //}
        #endregion

        #region AttachWatermarkTextBox
        private void AttachWatermarkTextBox()
        {
            if (_textBox != null)
            {
                _textBox.TextChanged -= _textBox_TextChanged;
                _textBox.PreviewKeyDown -= _textBox_PreviewKeyDown;
            }

            var element = GetTemplateChild(PART_TextBox) as TextBox;

            if (element != null)
            {
                _textBox = (TextBox)element;
            }

            if (_textBox != null)
            {
                _textBox.TextChanged += _textBox_TextChanged;
                _textBox.PreviewKeyDown += _textBox_PreviewKeyDown;
            }
        }

        private void _textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_textBox.Text))
            {
                Reset();
            }
        }

        private void _textBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Filter();
            }
        }
        #endregion

        #region AttachDropDownButtonFilter
        private void AttachDropDownButtonFilter()
        {
            if (_dropDownButtonFilter != null)
            {
                _dropDownButtonFilter.Click -= _dropDownButtonFilter_Click;
                _dropDownButtonFilter.LostFocus -= _dropDownButtonFilter_LostFocus;
            }

            var element = GetTemplateChild(PART_DropDownButtonFilter) as Button;

            if (element != null)
            {
                _dropDownButtonFilter = (Button)element;
            }

            if (_dropDownButtonFilter != null)
            {
                _dropDownButtonFilter.Click += _dropDownButtonFilter_Click;
                _dropDownButtonFilter.LostFocus += _dropDownButtonFilter_LostFocus;
            }
        }

        private void _dropDownButtonFilter_LostFocus(object sender, RoutedEventArgs e)
        {
            IsDropDownOpen = false;
        }

        private void _dropDownButtonFilter_Click(object sender, RoutedEventArgs e)
        {
            IsDropDownOpen = !IsDropDownOpen;
        }
        #endregion

        #region AttachDropDownButtonResetFilter
        private void AttachDropDownButtonResetFilter()
        {
            if (_buttonResetFilter != null)
            {
                _buttonResetFilter.Click -= _buttonResetFilter_Click;
            }

            var element = GetTemplateChild(PART_ButtonResetFilter) as Button;

            if (element != null)
            {
                _buttonResetFilter = (Button)element;
            }

            if (_buttonResetFilter != null)
            {
                _buttonResetFilter.Click += _buttonResetFilter_Click;
            }
        }

        private void _buttonResetFilter_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }
        #endregion

        #endregion
    }
}
