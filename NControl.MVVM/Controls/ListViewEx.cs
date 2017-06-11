using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	/// <summary>
	/// Extended listview
	/// </summary>
	public class ListViewEx : ListView
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ListViewEx"/> class.
		/// </summary>
		public ListViewEx()
		{
			BackgroundColor = Color.Transparent;
			ItemSelected += ListViewEx_ItemSelected;
		}

		#region Overridden Members

		/// <summary>
		/// Override to handle cells
		/// </summary>
		protected override void SetupContent(Cell content, int index)
		{
			base.SetupContent(content, index);
			content.Tapped += CellTapped;
		}

		/// <summary>
		/// Unhooks the content.
		/// </summary>
		protected override void UnhookContent(Cell content)
		{
			base.UnhookContent(content);
			content.Tapped -= CellTapped;
		}

		#endregion

		#region Properties

		/// <summary>
		/// The SelectedCell property.
		/// </summary>
		public static BindableProperty SelectedCellProperty = BindableProperty.Create(
			nameof(SelectedCell), typeof(Cell), typeof(ListViewEx), null,
			BindingMode.OneWay, null);

		/// <summary>
		/// Gets or sets the SelectedCell of the ListViewEx instance.
		/// </summary>
		public Cell SelectedCell
		{
			get { return (Cell)GetValue(SelectedCellProperty); }
			set { SetValue(SelectedCellProperty, value); }
		}
		#endregion

		#region Commands

		/// <summary>
		/// The ItemSelectedCommand property.
		/// </summary>
		public static BindableProperty ItemSelectedCommandProperty =
			BindableProperty.Create(nameof(ItemSelectedCommand), typeof(ICommand),
			typeof(ListViewEx), null, BindingMode.Default,
			propertyChanged: (bindable, oldValue, newValue) =>
			{
				var ctrl = (ListViewEx)bindable;
				ctrl.ItemSelectedCommand = (ICommand)newValue;
			});

		/// <summary>
		/// Gets or sets the ItemSelectedCommand of the ListViewEx instance.
		/// </summary>
		/// <value>The color of the buton.</value>
		public ICommand ItemSelectedCommand
		{
			get { return (ICommand)GetValue(ItemSelectedCommandProperty); }
			set
			{
				SetValue(ItemSelectedCommandProperty, value);
			}
		}

		#endregion

		#region Events

		void ListViewEx_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			// SelectedCell = null;
		}

		void CellTapped(object sender, EventArgs e)
		{
			SelectedCell = sender as Cell;

			if (SelectedItem == null)
				return;

			if (ItemSelectedCommand != null && ItemSelectedCommand.CanExecute(SelectedItem))
				ItemSelectedCommand.Execute(SelectedItem);

			SelectedItem = null;
		}

		#endregion
	}
}

