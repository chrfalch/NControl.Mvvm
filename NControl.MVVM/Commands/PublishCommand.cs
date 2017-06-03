using System;
using System.Threading.Tasks;

namespace NControl.Mvvm
{	
	public class PublishCommand<TMessageType> : AsyncCommand 
		where TMessageType : class
	{
		public PublishCommand(Func<object, TMessageType> messageFunc,		   
		   Func<object, bool> canExecuteFunc = null) :
			base((param) =>
			{
				MvvmApp.Current.MessageHub.Publish(messageFunc(param));
				return Task.FromResult(true);

			}, canExecuteFunc)
		{ }
	}
}
