using System;
using NControl.MVVM;
using System.Threading.Tasks;

namespace MvvmDemo
{
	public class EmployeeDetailsViewModel: BaseViewModel<Employee>
	{
		public EmployeeDetailsViewModel ()
		{
		}

		public override async Task InitializeAsync (Employee parameter)
		{
			await base.InitializeAsync (parameter);
			Employee = parameter;
		}

		public Employee Employee
		{
			get{ return GetValue<Employee> (); }
			set { SetValue<Employee> (value); }
		}
	}
}

