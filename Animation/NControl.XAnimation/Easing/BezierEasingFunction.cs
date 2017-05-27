using System;
using Xamarin.Forms;

namespace NControl.XAnimation
{
	public sealed class EasingFunctionBezier
	{
		public Point Start { get; private set; }
		public Point End { get; private set; }
        public EasingFunctionBezier(Point start, Point end)
		{
			Start = start;
			End = end;
		}

		public EasingFunctionBezier(double startX, double startY, double endX, double endY) :
			this(new Point(startX, startY), new Point(endX, endY))
		{
		}
	}
}
