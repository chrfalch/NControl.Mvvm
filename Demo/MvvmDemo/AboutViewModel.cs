using System;
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

		[OnMessage(typeof(MyMessage))]
		public Command<MyMessage> HandleMyMessage
		{
			get
			{
				return GetOrCreateCommand<MyMessage>(async (obj) => {
					await MvvmApp.Current.Presenter.ShowMessageAsync(Title, obj.Message, "OK");
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

