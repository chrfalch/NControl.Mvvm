using System;

namespace NControl.Mvvm
{
	public class DismissCommand : AsyncCommand
	{
		public DismissCommand(PresentationMode presentationMode,
		   Func<bool> resultFunc = null,
		   Func<object, bool> canExecuteFunc = null) :
		base(async (param) =>
		{
				// Present viewmodel
				await MvvmApp.Current.Presenter.DismissViewModelAsync(
				presentationMode, resultFunc == null ? false : resultFunc());

		}, canExecuteFunc)
		{ }
	}
}
