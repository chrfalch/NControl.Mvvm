using System;
using NControl.Mvvm;
using System.Threading.Tasks;
using System.Linq;
using System.Windows.Input;

namespace MvvmDemo
{
	public class SearchViewModel: BaseViewModel
	{
		public override async Task InitializeAsync ()
		{
			await base.InitializeAsync ();

			ReloadItems();
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

				ReloadItems();
			}
		}

		public ICommand EmployeSelectedCommand => GetCommand(
			()=> new NavigatePopupCommand<EmployeeDetailsViewModel>(canExecuteFunc:(emp)=> emp != null));

		void ReloadItems()
		{
			Employees.Clear();
			if(string.IsNullOrEmpty(Query))
				Employees.AddRange(Employee.EmployeeRepository);
			else
				Employees.AddRange(Employee.EmployeeRepository
					.Where(mn => mn.Name.ToLowerInvariant()
						.Contains(Query.ToLowerInvariant())));	
		}
	}
}

