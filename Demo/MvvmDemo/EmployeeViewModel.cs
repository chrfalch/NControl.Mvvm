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

		public ObservableCollectionWithAddRange<Employee> Employees {
			get{
				return GetValue(()=>
					new ObservableCollectionWithAddRange<Employee> ());
			}
		}

		public ICommand SelectEmployeeCommand
		{
			get {
				return GetOrCreateCommandAsync<Employee> (async (employee) => {
					await MvvmApp.Current.Presenter.ShowViewModelAsync<EmployeeDetailsViewModel>(
						employee, PresentationMode.Popup, dismissedCallback: (b) =>
					{
						System.Diagnostics.Debug.WriteLine("Yup", "EmployeeDetailsViewModel Closed.");
					});
				});
			}
		}

		public ICommand ShowAboutCommand
		{
			get
			{
				return GetOrCreateCommandAsync(async _ =>
				{

					await MvvmApp.Current.Presenter.ShowViewModelAsync<AboutViewModel>(PresentationMode.Modal);

				});
			}
		}

		public ICommand ShowModalCommand
		{
			get {
				return GetOrCreateCommandAsync (async _=> {
					await MvvmApp.Current.Presenter.ShowViewModelAsync<AboutViewModel>(PresentationMode.Modal);
				});
			}
		}
	}
}

