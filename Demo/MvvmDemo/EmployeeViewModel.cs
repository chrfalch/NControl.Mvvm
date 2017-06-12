using System;
using NControl.Mvvm;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

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

		public ObservableCollectionWithAddRange<Employee> Employees => GetValue(()=>
					new ObservableCollectionWithAddRange<Employee> ());

		public ICommand SelectEmployeeCommand => GetCommand(()=> new NavigatePopupCommand<EmployeeDetailsViewModel>(
			(b) => System.Diagnostics.Debug.WriteLine("EmployeeDetailsViewModel Closed.")));

		public ICommand ShowAboutCommand => GetCommand(()=> new NavigateModalCommand<AboutViewModel>());
		public ICommand ShowModalCommand => ShowAboutCommand;
	}
}

