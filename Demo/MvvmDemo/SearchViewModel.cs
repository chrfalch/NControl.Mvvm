using System;
using NControl.Mvvm;
using System.Threading.Tasks;
using System.Linq;
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
			//this.OnPropertyChanges (d => d.Query)
			//	.DistinctUntilChanged ()
			//	.Throttle (TimeSpan.FromSeconds (0.25))
			//	.Select(t => t.ToLowerInvariant())
			//	.Subscribe (_ => {
			//		Employees.Clear();
			//		if(string.IsNullOrEmpty(Query))
			//			Employees.AddRange(Employee.EmployeeRepository);
			//		else
			//			Employees.AddRange(Employee.EmployeeRepository
			//				.Where(mn => mn.Name.ToLowerInvariant()
			//					.Contains(Query.ToLowerInvariant())));	
			//	});
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
		public ObservableCollectionWithAddRange<Employee> Employees => 
			GetValue(()=> new ObservableCollectionWithAddRange<Employee>()); 

		/// <summary>
		/// Gets or sets the query.
		/// </summary>
		/// <value>The query.</value>
		public string Query 
		{
			get { return GetValue(()=> MvvmApp.Current.Load<string>(key:"SearchQuery")); }
			set { 
				SetValue (value);
				MvvmApp.Current.Save(value, key: "SearchQuery");
				       
				Employees.Clear();
				if(string.IsNullOrEmpty(Query))
					Employees.AddRange(Employee.EmployeeRepository);
				else
					Employees.AddRange(Employee.EmployeeRepository
						.Where(mn => mn.Name.ToLowerInvariant()
							.Contains(Query.ToLowerInvariant())));	
			}
		}

		public ICommand EmployeSelectedCommand => GetCommand(
			()=> new PresentPopupCommand<EmployeeDetailsViewModel>(canExecuteFunc:(emp)=> emp != null));
	}
}

