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
					await MvvmApp.Current.Presenter.ShowViewModelAsPopupAsync<EmployeeDetailsViewModel>(
						employee);
				});
			}
		}

		public ICommand ShowAboutCommand
		{
			get
			{
				return GetOrCreateCommandAsync(async _ =>
				{

					//await MvvmApp.Current.Presenter.ShowMessageAsync("Title", "Message");
					//await MvvmApp.Current.Presenter.ShowActionSheet("Title", "Cancel", "Delete", "item 1", "item 2");
					await MvvmApp.Current.Presenter.ShowViewModelModalAsync<AboutViewModel>();

				});
			}
		}

		public ICommand ShowModalCommand
		{
			get {
				return GetOrCreateCommandAsync (async _=> {
					await MvvmApp.Current.Presenter.ShowViewModelModalAsync<AboutViewModel>();
				});
			}
		}
	}
}

