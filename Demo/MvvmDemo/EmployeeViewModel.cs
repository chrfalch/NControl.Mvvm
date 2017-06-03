﻿using System;
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

		public ICommand SelectEmployeeCommand => GetOrCreateCommandAsync<Employee> (async (employee) => {
					await MvvmApp.Current.Presenter.ShowViewModelAsync<EmployeeDetailsViewModel>(
						PresentationMode.Popup, dismissedCallback: (b) =>
					{
						System.Diagnostics.Debug.WriteLine("EmployeeDetailsViewModel Closed.");
					}, parameter: employee);
				});			

		public ICommand ShowAboutCommand => GetCommand(()=> new PresentModalCommand<AboutViewModel>());

		public ICommand ShowModalCommand => ShowAboutCommand;
	}
}

