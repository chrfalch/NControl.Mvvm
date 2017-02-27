using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using NControl.Mvvm.Droid;
using NControl.Controls;

[assembly:ExportRenderer(typeof(ExtendedEntry), typeof(ExtendedEntryRenderer))]
namespace NControl.Mvvm.Droid
{
	public class ExtendedEntryRenderer: EntryRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);

			if (Control != null)
			{								
				Control.SetPadding(10, 0, 0, 0);
				Control.Gravity = Android.Views.GravityFlags.CenterVertical;
			}
		}
	}
}
