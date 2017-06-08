using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NControl.Mvvm
{
	/// <summary>
	/// Base ViewModel for list based view/viewmodels
	/// </summary>
	public abstract class BaseItemListViewModel<TItemType> : BaseViewModel
		where TItemType : class
	{
		#region Initialization

		/// <summary>
		/// Initializes the viewmodel and loads the contents
		/// </summary>
		/// <returns>The async.</returns>
		public override async Task InitializeAsync()
		{
			await base.InitializeAsync();
			await ReloadItemsAsync();
		}

		#endregion

		#region Abstract Members

		/// <summary>
		/// Override to implement loading of elements
		/// </summary>
		public abstract Task<IEnumerable<TItemType>> LoadItemsAsync();

		/// <summary>
		/// Override to implement handling selection of elements
		/// </summary>
		public virtual Task OnItemSelectedAsync(TItemType item)
		{
			return Task.FromResult(true);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Returns the observable items collection
		/// </summary>
		public ObservableCollectionWithAddRange<TItemType> Items =>
			GetValue(() => new ObservableCollectionWithAddRange<TItemType>());

		/// <summary>
		/// Get/sets the state of the collection
		/// </summary>
		/// <value>The state of the collection.</value>
		public CollectionState CollectionState
		{
			get { return GetValue<CollectionState>(); }
			set { SetValue(value); }
		}

		#endregion

		#region Commands

		/// <summary>
		/// Returns the select items command
		/// </summary>
		public virtual ICommand SelectItemCommand => GetOrCreateCommandAsync<TItemType>(
			async (item) => await OnItemSelectedAsync(item));

		/// <summary>
		/// Returns the refresh command
		/// </summary>
		/// <value>The refresh command.</value>
		[DependsOn(nameof(CollectionState))]
		public ICommand RefreshCommand => GetOrCreateCommandAsync(async _ =>
			await ReloadItemsAsync(), _ => CollectionState != CollectionState.Loading);

		#endregion

		#region Private Members

		/// <summary>
		/// Reload function
		/// </summary>
		async Task ReloadItemsAsync()
		{
			CollectionState = CollectionState.Loading;

			try
			{
				Items.Clear();

				Items.AddRange(await LoadItemsAsync());
			}
			finally
			{
				CollectionState = CollectionState.Loaded;
			}
		}
		#endregion
	}
}
