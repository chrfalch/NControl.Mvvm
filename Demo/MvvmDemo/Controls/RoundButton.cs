using System;
using NControl.Mvvm;
using NControl.Controls;
using Xamarin.Forms;

namespace MvvmDemo
{
	public class RoundButton: ExtendedButton
	{
		public RoundButton()
		{
			BorderWidth = 2;
			HeightRequest = 28;
			WidthRequest = 28;
			BorderColor = Color.White;
			FontFamily = FontMaterialDesignLabel.FontName;
			TextColor = Color.White;
		}

		protected override void OnSizeAllocated(double width, double height)
		{
			base.OnSizeAllocated(width, height);

			if (width > height)
				HeightRequest = width;
			else if (height > width)
				WidthRequest = height;
			else
				BorderRadius = (int)(Math.Max(width, height) / 2.0);
		}
	}
}
