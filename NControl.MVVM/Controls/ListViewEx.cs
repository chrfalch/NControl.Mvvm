using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
    /// <summary>
    /// Extended listview
    /// </summary>
    public class ListViewEx: ListView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sin4U.FormsApp.Controls.ListViewEx"/> class.
        /// </summary>
        public ListViewEx()
        {
            BackgroundColor = Color.Transparent;
            ItemSelected += ListViewEx_ItemSelected;
        }

        #region Commands

        /// <summary>
        /// The ItemSelectedCommand property.
        /// </summary>
        public static BindableProperty ItemSelectedCommandProperty = 
            BindableProperty.Create<ListViewEx, Command>(p => p.ItemSelectedCommand, null,
                defaultBindingMode: BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) => {
                    var ctrl = (ListViewEx)bindable;
                    ctrl.ItemSelectedCommand = newValue;
                        });

        /// <summary>
        /// Gets or sets the ItemSelectedCommand of the ListViewEx instance.
        /// </summary>
        /// <value>The color of the buton.</value>
        public Command ItemSelectedCommand
        {
            get{ return (Command)GetValue(ItemSelectedCommandProperty); }
            set
            {
                SetValue(ItemSelectedCommandProperty, value);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Lists the view ex item selected.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void ListViewEx_ItemSelected (object sender, SelectedItemChangedEventArgs e)
        {            
            if (SelectedItem == null)
                return;
            
            if (ItemSelectedCommand != null && ItemSelectedCommand.CanExecute(SelectedItem))
                ItemSelectedCommand.Execute(SelectedItem);
            
            SelectedItem = null;
        }
        #endregion
    }
}

