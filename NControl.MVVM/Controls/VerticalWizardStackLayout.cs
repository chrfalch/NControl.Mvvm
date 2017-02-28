using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
    /// <summary>
    /// Vertical wizard stack layout.
    /// </summary>
    public class VerticalWizardStackLayout: VerticalStackLayout
    {
        public VerticalWizardStackLayout()
        {
            VerticalOptions = LayoutOptions.Center;        
			Spacing = MvvmApp.Current.Sizes.Get(Config.DefaultLargeSpacing);
            Padding = MvvmApp.Current.Sizes.Get(Config.DefaultLargePadding);
        }
    }
}

