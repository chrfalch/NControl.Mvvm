using System;
using System.Windows.Input;
using NControl.Mvvm;

namespace MvvmDemo
{
	public class CityViewModel: BaseViewModel<FeedItem>
	{
		public override System.Threading.Tasks.Task InitializeAsync(FeedItem parameter)
		{
			CityModel = parameter;
			Title = parameter.City;
			return base.InitializeAsync(parameter);
		}

		public FeedItem CityModel
		{
			get { return GetValue<FeedItem>(); }
			set { SetValue(value); }
		}

		[DependsOn(nameof(CityModel))]
		public string City => CityModel?.City ?? " ";

		[DependsOn(nameof(CityModel))]
		public string Name => CityModel?.Name ?? " "; 

		[DependsOn(nameof(CityModel))]
		public string Image => CityModel?.Image; 

		public ICommand ViewDetailsCommand = new NavigateCommand<CityDetailsViewModel>();
	}
}
