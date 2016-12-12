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

		public ICommand CountAsyncCommand
		{
			get
			{
				return GetOrCreateCommandAsync(async (arg) => {
					for (var i = 0; i < 10; i++)
					{
						await Task.Delay(150);
						NumberValue++;
					}
				});
			}
		}

		public int NumberValue
		{
			get { return GetValue<int>(); }
			set { SetValue(value); }
		}

		[OnMessage(typeof(MyMessage))]
		public AsyncCommand<MyMessage> HandleMyMessage
		{
			get
			{
				return GetOrCreateCommandAsync(async (MyMessage arg) => {
					await MvvmApp.Current.Presenter.ShowMessageAsync(Title, arg.Message, "OK");
				});
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

