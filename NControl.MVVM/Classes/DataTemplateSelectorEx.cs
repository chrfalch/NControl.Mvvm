using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public class DataTemplateSelectorEx: DataTemplateSelector		
	{
		Func<object, DataTemplate> _getTemplateFunction;

		public DataTemplateSelectorEx(Func<object, DataTemplate> getTemplateFunction)
		{
			_getTemplateFunction = getTemplateFunction;
		}

		/// <summary>
		/// Override to implement selector
		/// </summary>
		/// <returns>The select template.</returns>
		/// <param name="item">Item.</param>
		/// <param name="container">Container.</param>
		protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
		{
			return _getTemplateFunction(item);
		}
	}
}
