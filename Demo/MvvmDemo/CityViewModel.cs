using System;
using NControl.Mvvm;
namespace MvvmDemo
{
	public class CityViewModel: BaseViewModel<FeedItem>
	{
		public override System.Threading.Tasks.Task InitializeAsync(FeedItem parameter)
		{
			CityModel = parameter;
			Title = string.Empty;
			return base.InitializeAsync(parameter);
		}

		public FeedItem CityModel
		{
			get { return GetValue<FeedItem>(); }
			set { SetValue(value); }
		}

		[DependsOn(nameof(CityModel))]
		public string City
		{
			get { return CityModel?.City; }
		}

		[DependsOn(nameof(CityModel))]
		public string Name
		{
			get { return CityModel?.Name; }
		}

		[DependsOn(nameof(CityModel))]
		public string Image
		{
			get { return CityModel?.Image; }
		}
	}
}
