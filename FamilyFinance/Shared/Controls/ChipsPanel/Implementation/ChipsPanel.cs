using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace FamilyFinance.Shared.Controls
{
    [TemplatePart(Name = PART_Button, Type = typeof(Button))]
    public class ChipsItem : ContentControl
    {
        #region Consts
        private const string PART_Button = "PART_Button";
        #endregion

        #region DependencyProperty : DeleteCommandProperty
        public ICommand? DeleteCommand
        {
            get => (ICommand?)GetValue(DeleteCommandProperty);
            set => SetValue(DeleteCommandProperty, value);
        }
        public static readonly DependencyProperty DeleteCommandProperty
            = DependencyProperty.Register(nameof(DeleteCommand), typeof(ICommand), typeof(ChipsItem), new PropertyMetadata(default(ICommand?)));
        #endregion

        #region DependencyProperty : DeleteCommandParameterProperty
        public object? DeleteCommandParameter
        {
            get => GetValue(DeleteCommandParameterProperty);
            set => SetValue(DeleteCommandParameterProperty, value);
        }
        public static readonly DependencyProperty DeleteCommandParameterProperty
            = DependencyProperty.Register(nameof(DeleteCommandParameter), typeof(object), typeof(ChipsItem), new PropertyMetadata(default(object?)));
        #endregion

        #region Events
        public event RoutedEventHandler DeleteClick
        {
            add => AddHandler(DeleteClickEvent, value);
            remove => RemoveHandler(DeleteClickEvent, value);
        }

        public static readonly RoutedEvent DeleteClickEvent
            = EventManager.RegisterRoutedEvent(nameof(DeleteClick), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ChipsItem));
        #endregion

        #region Properties
        public ChipsPanel ParentContainer
        {
            get { return ItemsControl.ItemsControlFromItemContainer(this) as ChipsPanel; }
        }
        #endregion

        #region Fields
        protected Button _button;
        #endregion

        #region Constructors
        static ChipsItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChipsItem),
                                                     new FrameworkPropertyMetadata(
                                                         typeof(ChipsItem)));
        }

        public ChipsItem()
        {
        }

       
        #endregion

        #region OnApplyTemplate
        public override void OnApplyTemplate()
        {
            AttachButton();
            base.OnApplyTemplate();
        }

        private void AttachButton()
        {
            if (_button != null)
            {
                _button.Click -= DeleteButtonOnClick;

            }

            _button = GetTemplateChild("PART_Button") as Button;

            if (_button != null)
            {
                _button.Click += DeleteButtonOnClick;
            }
        }


        //private void ChipsItem_DeleteClick(object sender, RoutedEventArgs e)
        //{
        //    ChipsPanel parent = ParentContainer;
        //    if (parent.ItemsSource == null)
        //    {
        //        parent.Items.Remove(this);
        //    }
        //    else
        //    {
        //        var collection = parent.ItemsSource;
        //        (collection as IList).Remove(this);
        //    }
        //}

        protected virtual void OnDeleteClick()
        {
            RaiseEvent(new RoutedEventArgs(DeleteClickEvent, this));

            if (DeleteCommand?.CanExecute(DeleteCommandParameter) ?? false)
            {
                DeleteCommand.Execute(DeleteCommandParameter);
            }
        }

        private void DeleteButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            ChipsPanel parent = ParentContainer;
            if (parent.ItemsSource == null)
            {
                parent.Items.Remove(this);
            }
            else
            {
                OnDeleteClick();
                routedEventArgs.Handled = true;
            }
        }
        #endregion
    }




    [TemplatePart(Name = PART_TextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_ItemsPresenter, Type = typeof(ItemsPresenter))]
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(ChipsItem))]
    public class ChipsPanel : ItemsControl
    {
        #region Consts
        private const string PART_TextBox = "PART_TextBox";
        private const string PART_ItemsPresenter = "PART_ItemsPresenter";
        #endregion

        

        #region Fields
        protected ItemsPresenter _temsPresenter;
        protected TextBox _textBox;
        #endregion

        #region Constructors
        static ChipsPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChipsPanel),
                                                     new FrameworkPropertyMetadata(
                                                         typeof(ChipsPanel)));
        }

        public ChipsPanel()
        {
            this.GotFocus += ChipsPanel_GotFocus;
            
            //this.ItemContainerGenerator.ItemsChanged += ItemContainerGenerator_ItemsChanged;
            this.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
        }

        private void ChipsPanel_GotFocus(object sender, RoutedEventArgs e)
        {
            FocusManager.SetFocusedElement(this, _textBox);
            var focus = _textBox.Focusable;
        }

        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (this.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                if (ItemsSource != null)
                {
                    foreach (var item in Items.SourceCollection)
                    {
                        ChipsItem containerItem = this.ItemContainerGenerator.ContainerFromItem(item) as ChipsItem;
                        if (containerItem == null)
                        {
                            this.UpdateLayout();
                            containerItem = this.ItemContainerGenerator.ContainerFromItem(item) as ChipsItem;
                        }

                        if (containerItem != null)
                        {
                            containerItem.DeleteCommand = DeleteCommand;
                            containerItem.DeleteCommandParameter = item;
                        }
                    }
                }
            }
        }

        public ICommand? DeleteCommand
        {
            get => (ICommand?)GetValue(DeleteCommandProperty);
            set => SetValue(DeleteCommandProperty, value);
        }
        public static readonly DependencyProperty DeleteCommandProperty
            = DependencyProperty.Register(nameof(DeleteCommand), typeof(ICommand), typeof(ChipsPanel), new PropertyMetadata(default(ICommand?)));

        public object? DeleteCommandParameter
        {
            get => GetValue(DeleteCommandParameterProperty);
            set => SetValue(DeleteCommandParameterProperty, value);
        }
        public static readonly DependencyProperty DeleteCommandParameterProperty
            = DependencyProperty.Register(nameof(DeleteCommandParameter), typeof(object), typeof(ChipsPanel), new PropertyMetadata(default(object?)));


        //private void ItemContainerGenerator_ItemsChanged(object sender, System.Windows.Controls.Primitives.ItemsChangedEventArgs e)
        //{
        //    this.ItemContainerGenerator.ItemsChanged -= ItemContainerGenerator_ItemsChanged;
        //    if (e.Action == NotifyCollectionChangedAction.Add)
        //    {
        //        int index = ItemContainerGenerator.Items.Count - 1;
        //        var item = ItemContainerGenerator.Items[index];

        //        object containerItem = this.ItemContainerGenerator.ContainerFromItem(item);
        //        var containerItemFromIndex = this.ItemContainerGenerator.ContainerFromIndex(index);
        //        if (containerItem == null)
        //        {
        //            this.UpdateLayout();
                    
        //            containerItem = this.ItemContainerGenerator.ContainerFromItem(item);
        //        }


        //        //item.DeleteCommand = DeleteCommand;
        //        //item.DeleteCommandParameter = DeleteCommandParameter;
        //    }
        //    this.ItemContainerGenerator.ItemsChanged += ItemContainerGenerator_ItemsChanged;
        //}
        #endregion

        #region OnApplyTemplate
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            AttachTextBox();
            AttachItemsPresenter();
        }

        private void AttachTextBox()
        {
            var textBox = GetTemplateChild(PART_TextBox) as TextBox;

            if (textBox != null)
            {
                _textBox = textBox;
                _textBox.KeyDown += _textBox_KeyDown;
            }
        }

        private void _textBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            if (e.Key == System.Windows.Input.Key.Enter && !string.IsNullOrWhiteSpace(textBox.Text))
            {
                if (ItemsSource == null)
                {
                    this.Items.Insert(Items.Count - 1, new ChipsItem() { Content = textBox.Text });
                    textBox.Clear();
                }
            }
        }

        private void AttachItemsPresenter()
        {
            var itemsPresenter = GetTemplateChild("PART_ItemsPresenter") as ItemsPresenter;

            if (itemsPresenter != null)
            {
                _temsPresenter = itemsPresenter;
            }
        }
        #endregion

        #region Methods
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is ChipsItem);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ChipsItem();
        }
        #endregion
    }
}
