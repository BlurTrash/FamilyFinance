using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace FamilyFinance.Shared.Controls
{
    public class AutoCompleteComboBox : ComboBox
    {
        private bool isBlock;
        private bool isSelected;
        private bool isBackspace;

        public AutoCompleteComboBox()
        {
            var myResourceDictionary = new ResourceDictionary();
            myResourceDictionary.Source = new Uri("/FamilyFinance;component/FFResourceDictionary.xaml", UriKind.RelativeOrAbsolute);

            var style = myResourceDictionary["FFComboBox"] as Style;

            if (style != null)
            {
                this.Style = style;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (IsEditable)
            {
                var element = GetTemplateChild("PART_EditableTextBox");
                if (element != null)
                {
                    var textBox = (TextBox)element;
                    textBox.TextChanged += TextBox_TextChanged;
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!isSelected && IsEditable)
            {
                var comboBox = this;
                var textBox = (TextBox)e.OriginalSource;

                if (comboBox.SelectedItem != null && string.IsNullOrEmpty(textBox.Text))
                {
                    comboBox.SelectedItem = null; // Если сбросили текст полностью, сбрасываем выбранный элемент
                }

                if (textBox.SelectionStart != 0 && comboBox.SelectedItem != null && !isBackspace)
                {
                    comboBox.SelectedItem = null; // Если набирается текст сбросить выбраный элемент
                }

                if (textBox.SelectionStart == 0)
                {
                    comboBox.IsDropDownOpen = false; // Если сбросили текст и элемент не выбран, сбросить фокус выпадающего списка
                }

                if (isBackspace)
                {
                    isBackspace = false;
                }

                comboBox.IsEditable = false;
                comboBox.IsDropDownOpen = true;
                comboBox.IsEditable = true;

                // Если элемент не выбран менять фильтр
                if (comboBox.SelectedItem == null)
                {
                    CollectionView cv = (CollectionView)CollectionViewSource.GetDefaultView(comboBox.Items);
                    cv.Filter = s =>
                    {
                        var type = s.GetType();
                        var displayMember = comboBox.DisplayMemberPath;
                        var property = type.GetProperty(displayMember);

                        if (property != null)
                        {
                            var value = property.GetValue(s);
                            if (value != null)
                            {
                                return value.ToString().IndexOf(comboBox.Text, StringComparison.CurrentCultureIgnoreCase) >= 0;
                            }
                        }
                        return s.ToString().IndexOf(comboBox.Text, StringComparison.CurrentCultureIgnoreCase) >= 0;
                    };
                }
            }
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            if (IsEditable)
            {
                this.IsEditable = false;
                if (!isBlock && !isBackspace)
                {
                    base.OnSelectionChanged(e);
                }

                this.IsEditable = true;
            }
            else
            {
                base.OnSelectionChanged(e);
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (IsEditable)
            {
                if (e.Key == Key.Up || e.Key == Key.Down)
                {
                    isBlock = true;
                }

                base.OnPreviewKeyDown(e);

                isBlock = false;

                if (e.Key == Key.Back)
                {
                    isBackspace = true;
                    SelectedItem = null;
                }

                if (e.Key == Key.Enter)
                {
                    isSelected = true;
                    base.OnSelectionChanged(new SelectionChangedEventArgs(Selector.SelectionChangedEvent, new List<object>(), new List<object>() { this.SelectedValue }));
                    var textBox = (TextBox)GetTemplateChild("PART_EditableTextBox");
                    textBox.Select(textBox.Text.Length, 0);
                }

                isSelected = false;
            }
            else
            {
                base.OnPreviewKeyDown(e);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (IsEditable)
            {
                base.OnKeyDown(e);

                if (e.Key == Key.Enter)
                {
                    var textBox = (TextBox)GetTemplateChild("PART_EditableTextBox");
                    textBox.Select(textBox.Text.Length, 0);
                }
            }
            else
            {
                base.OnKeyDown(e);
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
    }
}
