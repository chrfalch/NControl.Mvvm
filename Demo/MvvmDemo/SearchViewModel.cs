using System;
using NControl.Mvvm;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;
using System.Reactive.Linq;
using NControl.MVVM;
using System.Windows.Input;

namespace MvvmDemo
{
	public class SearchViewModel: BaseViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MvvmDemo.SearchViewModel"/> class.
		/// </summary>
		public SearchViewModel ()
		{
			this.OnPropertyChanges (d => d.Query)
				.DistinctUntilChanged ()
				.Throttle (TimeSpan.FromSeconds (0.25))
				.Select(t => t.ToLowerInvariant())
				.Subscribe (_ => {
					Employees.Clear();
					if(string.IsNullOrEmpty(Query))
						Employees.AddRange(Employee.EmployeeRepository);
					else
						Employees.AddRange(Employee.EmployeeRepository
							.Where(mn => mn.Name.ToLowerInvariant().Contains(Query)));	
				});
		}

		/// <summary>
		/// Initializes the viewmodel
		/// </summary>
		/// <returns>The async.</returns>
		public override async Task InitializeAsync ()
		{
			await base.InitializeAsync ();

			Employees.AddRange(Employee.EmployeeRepository);
		}

		/// <summary>
		/// Gets the employees.
		/// </summary>
		/// <value>The employees.</value>
		public ObservableCollectionWithAddRange<Employee> Employees {
			get { return GetValue<ObservableCollectionWithAddRange<Employee>> (()=> 
				new ObservableCollectionWithAddRange<Employee>()); }		
		}

		/// <summary>
		/// Gets or sets the query.
		/// </summary>
		/// <value>The query.</value>
		public string Query {
			get { return GetValue<string> (); }
			set { SetValue<string> (value); }
		}

		/// <summary>
		/// Returns the EmployeSelected command
		/// </summary>
		/// <value>The view EmployeSelected command.</value>
		public Command<Employee> EmployeSelectedCommand {
			get {
				return GetOrCreateCommand<Employee> (async(emp) => {
		            
					await MvvmApp.Current.Presenter.ShowViewModelAsPopupAsync<EmployeeDetailsViewModel>(emp);

				}, (emp) => emp != null);
			}
		}
	}
}

