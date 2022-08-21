using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FamilyFinance.Shared.Controls
{
    public enum SelectionMode
    {
        Single,
        Multiple
    }

    public abstract class WatermarkComboBox : ComboBox
    {
        #region Watermark
        public static readonly DependencyProperty WatermarkTextProperty = DependencyProperty.Register("WatermarkText",
           typeof(string), typeof(WatermarkComboBox), new PropertyMetadata(null));

        public string WatermarkText
        {
            get { return (string)GetValue(WatermarkTextProperty); }
            set { SetValue(WatermarkTextProperty, value); }
        }
        #endregion

        #region ViewBase
        public static readonly DependencyProperty ViewProperty = DependencyProperty.Register("View",
            typeof(ViewBase), typeof(WatermarkComboBox));

        public ViewBase View
        {
            get { return (ViewBase)GetValue(ViewProperty); }
            set { SetValue(ViewProperty, value); }
        }
        #endregion

        #region SelectionMode
        public static readonly DependencyProperty CanSelectMultipleProperty = DependencyProperty.Register("CanSelectMultiple",
            typeof(bool), typeof(WatermarkComboBox), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool CanSelectMultiple
        {
            get { return (bool)GetValue(CanSelectMultipleProperty); }
            set { SetValue(CanSelectMultipleProperty, value); }
        }

        public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register("SelectionMode",
                       typeof(SelectionMode), typeof(WatermarkComboBox), new FrameworkPropertyMetadata(SelectionMode.Single, new PropertyChangedCallback(OnSelectionModeChanged)), new ValidateValueCallback(IsValidSelectionMode));

        public SelectionMode SelectionMode
        {
            get { return (SelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        private static void OnSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WatermarkComboBox comboBox = (WatermarkComboBox)d;
            comboBox.CanSelectMultiple = true;
        }

        private static bool IsValidSelectionMode(object o)
        {
            SelectionMode value = (SelectionMode)o;
            return value == SelectionMode.Single || value == SelectionMode.Multiple;
        }
        #endregion

        #region SelectedItems

        //private ObservableCollection<object> selectedItems = new ObservableCollection<object>();

        //public IList SelectedItems
        //{
        //    get { return selectedItems; }
        //}

        //public static readonly DependencyProperty SelectedItemsProperty =
        //    DependencyProperty.Register("SelectedItems", typeof(ICollection<object>), typeof(WatermarkComboBox),
        //        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SelectedItemsPropertyChangedCallback));
        //public ICollection<object> SelectedItems
        //{
        //    get => (ICollection<object>)GetValue(SelectedItemsProperty);
        //    set => SetValue(SelectedItemsProperty, value);
        //}

        //private static void SelectedItemsPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        //{
        //    if (dependencyObject is WatermarkComboBox control)
        //    {
        //        //основные действия когда задается коллекция
        //    }
        //}
        #endregion
    }
}
