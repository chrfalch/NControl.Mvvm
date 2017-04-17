using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NControl.Mvvm
{
	public class PerformanceTimer
	{
		public static PerformanceTimer Current;

		List<TimingElement> _elements = new List<TimingElement>();
		int _level = 0;

		public static void Init()
		{
			Current = new PerformanceTimer();
		}

		PerformanceTimer()
		{
			StartTime = DateTime.Now;
			CurrentTime = StartTime;
		}

		public void AddTimer(object sender, string message = null, [CallerMemberName] string methodName = "Unknown")
		{
			_elements.Add(new TimingElement
			{
				Level = _level,
				Text = message,
				Time = DateTime.Now,
				SincePrevious = DateTime.Now - CurrentTime,
				SinceStart = DateTime.Now - StartTime,
				Reference = new WeakReference<object>(sender),
				Caller = methodName,
			});

			CurrentTime = DateTime.Now;
		}

		public void BeginSection(object sender, string message = null, [CallerMemberName] string methodName = "Unknown")
		{
			_level++;
			AddTimer(sender, message, methodName);
			_level++;
		}

		public void EndSection()
		{
			_level-=2;
		}

		DateTime StartTime { get; set; }
		DateTime CurrentTime { get; set; }

		public override string ToString()
		{
			var retVal = "";

			foreach (var e in _elements)
			{
				var indentation = new string(' ', e.Level * 2);
				var idx = _elements.IndexOf(e);
				object target = null;
				if (e.Reference != null)
					e.Reference.TryGetTarget(out target);
				
				if (idx < _elements.Count - 1)
					retVal += $"{_elements.ElementAt(idx + 1).SincePrevious.TotalMilliseconds, 10}:{indentation}[{target?.GetType().Name}.{e.Caller}]: {e.Text}\n";
				else
					retVal += $"{"",10}:{indentation}[{target?.GetType().Name}.{e.Caller}]: {e.Text}\n";
			}

			return retVal;
		}
	}

	public class TimingElement
	{		
		public WeakReference<object> Reference { get; set; }
		public string Caller { get; set; }
		public int Level { get; set; }
		public string Text { get; set; }
		public DateTime Time { get; set; }
		public TimeSpan SinceStart { get; set; }
		public TimeSpan SincePrevious { get; set; }
	}
}
