using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public interface IContentSizeProvider
	{
		/// <summary>
		/// Contents size
		/// </summary>
		Size ContentSize { get; }
	}
}
