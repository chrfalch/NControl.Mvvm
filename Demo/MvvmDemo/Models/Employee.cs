using System;
using NControl.Mvvm;
using System.Collections.Generic;

namespace MvvmDemo
{
	public class Employee: BaseModel
	{
		public static IEnumerable<Employee> EmployeeRepository = new Employee[]{
			new Employee{Id ="1", CompanyId ="1", Name="Peter J."},
			new Employee{Id ="2", CompanyId ="1", Name="John P."},
			new Employee{Id ="3", CompanyId ="2", Name="Mark S."},
			new Employee{Id ="4", CompanyId ="3", Name="Alan K."},
			new Employee{Id ="5", CompanyId ="3", Name="Keith P."},
			new Employee{Id ="6", CompanyId ="3", Name="Rick S."},
			new Employee{Id ="7", CompanyId ="3", Name="Don M."},
			new Employee{Id ="8", CompanyId ="2", Name="Ronny R."},
		};

		public Employee ()
		{
		}
		public string Id {
			get { return GetValue<string> (); }
			set { SetValue<string> (value); }
		}

		public string CompanyId {
			get { return GetValue<string> (); }
			set { SetValue<string> (value); }
		}

		public string Name {
			get { return GetValue<string> (); }
			set { SetValue<string> (value); }
		}
	}
}

