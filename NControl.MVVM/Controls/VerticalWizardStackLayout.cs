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
			Spacing = Config.DefaultLargeSpacing;
            Padding = Config.DefaultLargePadding;
        }
    }
}

