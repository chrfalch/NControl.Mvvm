using System;
using Xamarin.Forms;

namespace NControl.XAnimation
{
	public static class EasingFunctions
	{
		public static EasingFunctionBezier Linear = new EasingFunctionBezier(0, 0, 1, 1);
		public static EasingFunctionBezier Ease = new EasingFunctionBezier(.25, .1, .25, 1);
		public static EasingFunctionBezier EaseIn = new EasingFunctionBezier(.42, 0, 1, 1);
		public static EasingFunctionBezier EaseOut = new EasingFunctionBezier(0, 0, .58, 1);
		public static EasingFunctionBezier EaseInOut = new EasingFunctionBezier(.42, 0, .58, 1);
		public static EasingFunctionBezier Custom(double startX, double startY, double endX, double endY)
		{
			return new EasingFunctionBezier(startX, startY, endX, endY);
		}

		public static EasingFunctionBezier Custom(Point start, Point end)
		{
			return new EasingFunctionBezier(start, end);
		}

	}
}
