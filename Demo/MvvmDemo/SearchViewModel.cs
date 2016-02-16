using System;
using NControl.MVVM;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;

namespace MvvmDemo
{
	public class SearchViewModel: BaseViewModel
	{
		public SearchViewModel ()
		{
		}

		public override async Task InitializeAsync ()
		{
			await base.InitializeAsync ();

			Employees.AddRange(Employee.EmployeeRepository);
		}

		public ObservableCollectionWithAddRange<Employee> Employees {
			get { return GetValue<ObservableCollectionWithAddRange<Employee>> (()=> 
				new ObservableCollectionWithAddRange<Employee>()); }		
		}

		public string Query {
			get { return GetValue<string> (); }
			set { SetValue<string> (value); }
		}

		/// <summary>
		/// Returns the Search command
		/// </summary>
		/// <value>The view Search command.</value>
		[ExecuteOnChange(nameof(Query))]
		public Command SearchCommand {
			get {
				return GetOrCreateCommand (() => {

					Employees.Clear();
					if(string.IsNullOrEmpty(Query))
						Employees.AddRange(Employee.EmployeeRepository);
					else
						Employees.AddRange(Employee.EmployeeRepository.Where(mn => mn.Name.Contains(Query)));					
				});
			}
		}

		/// <summary>
		/// Returns the EmployeSelected command
		/// </summary>
		/// <value>The view EmployeSelected command.</value>
		[DependsOn(nameof(Query))]
		public Command<Employee> EmployeSelectedCommand {
			get {
				return GetOrCreateCommand<Employee> (async(emp) => {
		            
					await MvvmApp.Current.Presenter.ShowViewModelAsPopupAsync<EmployeeDetailsViewModel>(emp);

				}, (emp) => emp != null);
			}
		}
	}
}

