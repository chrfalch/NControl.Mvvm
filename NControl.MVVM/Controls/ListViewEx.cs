using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace NControl.Mvvm
{
    /// <summary>
    /// Extended listview
    /// </summary>
    public class ListViewEx: ListView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListViewEx"/> class.
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
		public static BindableProperty ItemSelectedCommandProperty = BindableProperty.Create(nameof(ItemSelectedCommand), typeof(ICommand), typeof(ListViewEx), null, BindingMode.Default, 
            propertyChanged: (bindable, oldValue, newValue) => {
            var ctrl = (ListViewEx)bindable;
			ctrl.ItemSelectedCommand = (ICommand)newValue;                       
		});

        /// <summary>
        /// Gets or sets the ItemSelectedCommand of the ListViewEx instance.
        /// </summary>
        /// <value>The color of the buton.</value>
        public ICommand ItemSelectedCommand
        {
            get{ return (ICommand)GetValue(ItemSelectedCommandProperty); }
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

