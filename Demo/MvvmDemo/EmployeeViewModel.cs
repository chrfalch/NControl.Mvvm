using System;
using NControl.MVVM;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace MvvmDemo
{
	public class EmployeeViewModel: BaseViewModel<Company>
	{
		public EmployeeViewModel ()
		{			
		}

		public override async Task InitializeAsync (Company parameter)
		{
			await base.InitializeAsync (parameter);

			Title = parameter.Name;
			Employees.AddRange (Employee.EmployeeRepository.Where(e => e.CompanyId == parameter.Id));
		}

		public ObservableCollectionWithAddRange<Employee> Employees {
			get{
				return GetValue<ObservableCollectionWithAddRange<Employee>>(()=>
					new ObservableCollectionWithAddRange<Employee> ());
			}
		}

		public Command<Employee> SelectEmployeeCommand
		{
			get {
				return GetOrCreateCommand<Employee> (async(employee) => {
					await MvvmApp.Current.Presenter.ShowViewModelAsPopupAsync<EmployeeDetailsViewModel>(
						employee);
				});
			}
		}

		public Command ShowModalCommand
		{
			get {
				return GetOrCreateCommand (async() => {
					await MvvmApp.Current.Presenter.ShowViewModelModalAsync<AboutViewModel>();
				});
			}
		}
	}
}

