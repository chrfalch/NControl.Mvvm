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

		public override async Task InitializeAsync()
		{
			await base.InitializeAsync();

			MvvmApp.Current.MessageHub.Subscribe<MyMessage>(this, async message => {

					IsRunningAsyncCommand = true;

					for (var i = 0; i< 10; i++)
					{
						await Task.Delay(150);
						NumberValue++;
					}

					//IsBusy = false;
					IsRunningAsyncCommand = false;

					await MvvmApp.Current.Presenter.ShowMessageAsync(Title, message.Message, "OK");
			});
		}

		public Command ClickMeCommand
		{
			get
			{
				return GetOrCreateCommand((obj) =>
				{
					MvvmApp.Current.MessageHub.Publish(new MyMessage("This is the message"));
				});
			}
		}

		public bool IsRunningAsyncCommand
		{
			get { return GetValue<bool>();}
			set { SetValue(value);}
		}

		public ICommand CountAsyncCommand
		{
			get
			{
				return GetOrCreateCommandAsync(async (arg) => {

					//IsBusy = true;
					//IsBusyText = "Counting...";
					//IsBusySubTitle = "the numbers in a sequence from zero to 10 stopping on 10.";
					IsRunningAsyncCommand = true;

					for (var i = 0; i < 10; i++)
					{
						await Task.Delay(150);
						NumberValue++;
					}

					//IsBusy = false;
					IsRunningAsyncCommand = false;
				});
			}
		}

		public int NumberValue
		{
			get { return GetValue<int>(); }
			set { SetValue(value); }
		}

		//[OnMessage(typeof(MyMessage))]
		public AsyncCommand<MyMessage> HandleMyMessage
		{
			get
			{
				return GetOrCreateCommandAsync(async (MyMessage arg) => {
					await MvvmApp.Current.Presenter.ShowMessageAsync(Title, arg.Message, "OK");
				});
			}
		}

		public ICommand PushNewAboutCommand
		{
			get
			{
				return GetOrCreateCommandAsync(async _=> 
				                               await MvvmApp.Current.Presenter.ShowViewModelAsync<FeedViewModel>());
			}
		}
		public override ICommand CloseCommand
		{
			get
			{
				return GetOrCreateCommandAsync(async (arg) => 
                   await MvvmApp.Current.Presenter.DismissViewModelAsync(PresentationMode, true));
			}
		}
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

