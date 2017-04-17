using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NControl.Mvvm
{
	public interface IPerformanceTimer
	{
	IDisposable BeginTimer(object sender, string message = null, [CallerMemberName] string methodName = null);
		void BeginSection(object sender, string message = null, [CallerMemberName] string methodName = null);
		void EndSection();
	}

	public class PerformanceTimerMock : IPerformanceTimer
	{
		public void BeginSection(object sender, string message, string methodName){}
		public void EndSection() {}
		public IDisposable BeginTimer(object sender, string message, string methodName)
		{
			return new DisposableMock();
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
		int _level = 0;

		public static void Init()
		{
			_current = new PerformanceTimer();
		}

		PerformanceTimer()
		{
			StartTime = DateTime.Now;
		}

		public IDisposable BeginTimer(object sender, string message = null, [CallerMemberName] string methodName = "Unknown")
		{
			var timingElement = new TimingElement
			{
				Level = _level,
				Text = message,
				StartTime = DateTime.Now,
				Reference = new WeakReference<object>(sender),
				Caller = methodName,
			};

			_elements.Add(timingElement);

			return timingElement;
		}

		public void BeginSection(object sender, string message = null, [CallerMemberName] string methodName = "Unknown")
		{
			_level++;
		}

		public void EndSection()
		{
			_level--;
		}

		DateTime StartTime { get; set; }

		public override string ToString()
		{
			var retVal = "";

			foreach (var e in _elements)
			{
				var indentation = new string(' ', e.Level * 2);
				object target = null;
				if (e.Reference != null)
					e.Reference.TryGetTarget(out target);
				
				retVal += $"{(e.EndTime - e.StartTime).TotalMilliseconds, 10}:{indentation}[{target?.GetType().Name}.{e.Caller}]: {e.Text}\n";
			}

			return retVal;
		}
	}

	public class TimingElement: IDisposable
	{		
		public bool IsSection { get; set; }
		public WeakReference<object> Reference { get; set; }
		public string Caller { get; set; }
		public int Level { get; set; }
		public string Text { get; set; }
		public DateTime StartTime { get; set; }
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
