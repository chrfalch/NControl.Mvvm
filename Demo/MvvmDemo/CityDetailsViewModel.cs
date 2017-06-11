using System;
using System.Threading.Tasks;
using NControl.Mvvm;

namespace MvvmDemo
{
	public class CityDetailsViewModel: BaseViewModel<FeedItem>
	{
		public override Task InitializeAsync(FeedItem parameter)
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
		public string Description => CityModel?.Description ?? " "; 

		[DependsOn(nameof(CityModel))]
		public string Image => CityModel?.Image;
	}
}
