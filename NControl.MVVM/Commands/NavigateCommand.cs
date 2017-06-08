using System;
using System.Threading.Tasks;

namespace NControl.Mvvm
{

	public abstract class BaseNavigateCommand<TViewModel>: AsyncCommand 
		where TViewModel : BaseViewModel
	{
		public BaseNavigateCommand(PresentationMode presentationMode = PresentationMode.Default,
			Func<object, bool> canExecuteFunc = null, Action<bool> presentedCallback = null) : 
			base(async (param)=> {
				
				// Present viewmodel
				await MvvmApp.Current.Presenter.ShowViewModelAsync<TViewModel>(
					presentationMode, presentedCallback, true, param);
			
			}, canExecuteFunc) 
		{
		}
	}

	public class NavigateCommand<TViewModel> : BaseNavigateCommand<TViewModel>
		where TViewModel : BaseViewModel
	{
		public NavigateCommand(Action<bool> presentedCallback = null,
			Func<object, bool> canExecuteFunc = null) : 
			base(PresentationMode.Default, canExecuteFunc, presentedCallback)
		{
		}	
	}

	public class NavigateModalCommand<TViewModel> : BaseNavigateCommand<TViewModel>
		where TViewModel : BaseViewModel
	{
		public NavigateModalCommand(Action<bool> presentedCallback = null,
			Func<object, bool> canExecuteFunc = null) : 
			base(PresentationMode.Modal, canExecuteFunc, presentedCallback)
		{
		}	
	}

	public class NavigatePopupCommand<TViewModel> : BaseNavigateCommand<TViewModel>
		where TViewModel : BaseViewModel
	{
		public NavigatePopupCommand(Action<bool> presentedCallback = null, 
           Func<object, bool> canExecuteFunc = null) : 
				base(PresentationMode.Popup, canExecuteFunc, presentedCallback)
		{
		}	
	}

}
