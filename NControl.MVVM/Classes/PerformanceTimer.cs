using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NControl.Mvvm
{
	public interface IPerformanceTimer
	{
		IDisposable BeginTimer(object sender, string message = null, [CallerMemberName] string methodName = null, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = -1);
	}

	public class PerformanceTimerMock : IPerformanceTimer
	{
		public IDisposable BeginTimer(object sender, string message = null, [CallerMemberName] string methodName = null, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = -1)
		{
			return new DisposableMock();
		}

		public override string ToString()
		{
			return string.Empty;
		}
	}

	public class DisposableMock: IDisposable
	{
		public void Dispose() { }
	}

	public class PerformanceTimer: IPerformanceTimer
	{
		static IPerformanceTimer _current;
		public static IPerformanceTimer Current { get {
				return _current ?? new PerformanceTimerMock();
			} 
		}

		List<TimingElement> _elements = new List<TimingElement>();

		public static void Init()
		{
			_current = new PerformanceTimer();
		}

		PerformanceTimer()
		{
			StartTime = DateTime.Now;
		}

		public IDisposable BeginTimer(object sender, string message = null, [CallerMemberName] string methodName = null, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = -1)
		{
			var timingElement = new TimingElement
			{
				Text = message?? "",
				StartTime = DateTime.Now,
				Reference = new WeakReference<object>(sender),
				CallerMethodName = methodName,
				CallerPath = filePath,
				CallerLineNumber = lineNumber,
			};

			if (_elements.Any())
				_elements.Last().NextTimerTime = DateTime.Now;
			
			_elements.Add(timingElement);

			return timingElement;
		}

		DateTime StartTime { get; set; }

		public override string ToString()
		{
			var messageLength = _elements.Max((arg) =>
			{
				var length = (arg.Level * 2);
				object t = null;
				if (arg.Reference != null)
					arg.Reference.TryGetTarget(out t);
				length += (t?.GetType().Name + "." + arg.CallerMethodName).Length;
				length += arg.Text.Length + 1;
				return length;
			});

			messageLength+=2;

			var maxMessageSpacer = new string('=', messageLength);
			var maxMessageEmptySpacer = new string(' ', messageLength - "Message".Length);
			   
			var retVal = 
				$"{"Self"      , 10}\t{"Total"     , 10}\tMessage{maxMessageEmptySpacer}\t  Location\n" + 
				$"{"==========", 10}\t{"==========", 10}\t{maxMessageSpacer            }\t  ========\n";

			foreach (var e in _elements)
			{
				var indentation = new string(' ', e.Level * 2);
				object target = null;
				if (e.Reference != null)
					e.Reference.TryGetTarget(out target);

				var message = $"{indentation}[{target?.GetType().Name}.{e.CallerMethodName}] {e.Text}";
				var spacer = new string(' ', messageLength - message.Length);
				retVal += $"{((e.NextTimerTime == DateTime.MinValue ? e.EndTime : e.NextTimerTime) - e.StartTime).TotalMilliseconds, 10}\t" + 
					$"{(e.EndTime - e.StartTime).TotalMilliseconds, 10}\t" + 
					$"{message}" + 
					$"{spacer}\t" + 
					$"  {e.CallerPath}:{e.CallerLineNumber}\n";
			}

			return retVal;
		}
	}

	public class TimingElement: IDisposable
	{		
		public WeakReference<object> Reference { get; set; }
		public string CallerMethodName { get; set; }
		public string CallerPath { get; set; }
		public int CallerLineNumber { get; set; }
		public int Level { get; set; }
		public string Text { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime NextTimerTime { get; set; }
		public DateTime EndTime { get; set; }

		#region IDisposable Support
		bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					EndTime = DateTime.Now;
				}

				disposedValue = true;
			}
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);		
		}
		#endregion
	}
}
