using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public class VerticalSeparator : BoxView
	{
		public VerticalSeparator()
		{			
			HeightRequest = Config.DefaultBorderSize;
			Color = Config.BorderColor;
		}
	}

	public class VerticalLightSeparator : BoxView
	{
		public VerticalLightSeparator()
		{			
			HeightRequest = Config.DefaultBorderSize * 0.5;
			Color = Config.BorderColor;
		}
	}
}
