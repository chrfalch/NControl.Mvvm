using System;
using System.Threading.Tasks;
using System.Windows.Input;
using NControl.Mvvm;
using Xamarin.Forms;

namespace MvvmDemo
{
	public class AboutViewModel : BaseViewModel
	{
		public AboutViewModel()
		{
			Title = "About";
		}

		#region Properties

		public bool IsRunningAsyncCommand
		{
			get { return GetValue<bool>(); }
			set { SetValue(value); }
		}

		public int NumberValue
		{
			get { return GetValue<int>(); }
			set { SetValue(value); }
		}

		#endregion

		#region Commands

		public ICommand ClickMeCommand => GetCommand(() => new PublishCommand<MyMessage>((param)=> 
			new MyMessage("This is the message")));

		public ICommand CountAsyncCommand => GetOrCreateCommandAsync(async (arg) =>
		{			
			IsRunningAsyncCommand = true;

			for (var i = 0; i < 10; i++)
			{
				await Task.Delay(150);
				NumberValue++;
			}

			IsRunningAsyncCommand = false;

		});

		[OnMessage(typeof(MyMessage))]
		public ICommand HandleMyMessage => GetOrCreateCommandAsync(
			async (MyMessage arg) =>
			await MvvmApp.Current.Presenter.ShowMessageAsync(Title, arg.Message, "OK")
		);

		public ICommand PushNewAboutCommand => GetCommand(() => new NavigateCommand<FeedViewModel>());
		public override ICommand CloseCommand => GetCommand(() => new DismissCommand(PresentationMode, () => true));

		#endregion
	}

	public class MyMessage
	{
		public string Message { get; private set;}
		public MyMessage  (string message)
		{
			Message = message;
		}
	}
}

