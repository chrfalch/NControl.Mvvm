using System;
using NControl.MVVM;
using System.Collections.Generic;

namespace MvvmDemo
{
	public class Company: BaseModel
	{
		public static IEnumerable<Company> CompanyRepository = new Company[]{ 
			new Company { Id = "1", Name = "ACME" },
			new Company { Id = "2", Name = "MECA" },
			new Company { Id = "3", Name = "ECMA" },
		};

		public Company ()
		{
		}

		public string Id {
			get { return GetValue<string> (); }
			set { SetValue<string> (value); }
		}

		public string Name {
			get { return GetValue<string> (); }
			set { SetValue<string> (value); }
		}

		public override string ToString ()
		{
			return Name;
		}
	}
}

