using System;
using System.Threading.Tasks;

namespace NControl.Mvvm
{

	public abstract class BasePresentCommand<TViewModel>: AsyncCommand 
		where TViewModel : BaseViewModel
	{
		public BasePresentCommand(PresentationMode presentationMode = PresentationMode.Default,
			Func<object, bool> canExecuteFunc = null, Action<bool> presentedCallback = null) : 
			base(async (param)=> {
				// Present viewmodel
				await MvvmApp.Current.Presenter.ShowViewModelAsync<TViewModel>(
					presentationMode, presentedCallback, true, param);
			
			}, canExecuteFunc) 
		{}
	}

	public class PresentCommand<TViewModel> : BasePresentCommand<TViewModel>
		where TViewModel : BaseViewModel
	{
		public PresentCommand(Action<bool> presentedCallback = null,
			Func<object, bool> canExecuteFunc = null) : 
			base(PresentationMode.Default, canExecuteFunc, presentedCallback)
		{
		}	
	}

	public class PresentModalCommand<TViewModel> : BasePresentCommand<TViewModel>
		where TViewModel : BaseViewModel
	{
		public PresentModalCommand(Action<bool> presentedCallback = null,
			Func<object, bool> canExecuteFunc = null) : 
			base(PresentationMode.Modal, canExecuteFunc, presentedCallback)
		{
		}	
	}

	public class PresentPopupCommand<TViewModel> : BasePresentCommand<TViewModel>
		where TViewModel : BaseViewModel
	{
		public PresentPopupCommand(Action<bool> presentedCallback = null, 
           Func<object, bool> canExecuteFunc = null) : 
				base(PresentationMode.Popup, canExecuteFunc, presentedCallback)
		{
		}	
	}

}
