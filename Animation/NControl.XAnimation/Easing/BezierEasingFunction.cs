using System;
using System.Collections.Generic;
using System.Linq;
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

		Point GetCasteljauPoint(int r, int i, double t, IEnumerable<Point> points)
		{
			if (r == 0) return points.ElementAt(i);

			Point p1 = GetCasteljauPoint(r - 1, i, t, points);
			Point p2 = GetCasteljauPoint(r - 1, i + 1, t, points);

			return new Point((int)((1 - t) * p1.X + t * p2.X), (int)((1 - t) * p1.Y + t * p2.Y));
		}

		public EasingFunctionBezier Slice(double start, double end)
		{
			var pt1 = GetCasteljauPoint(1, 0, start, new Point[] { Start, End });
			var pt2 = GetCasteljauPoint(1, 0, end, new Point[] { Start, End });
			return new EasingFunctionBezier(pt1, pt2);
		}

		public Point Interpolate(double time)
		{
			//return GetCasteljauPoint(1, 0, time, new Point[] { Start, End });

			//return new Point((int)((1 - time) * Start.X + time * End.X), 
			//                 (int)((1 - time) * Start.Y + time * End.Y));
			return new Point(
				X(time, 0.0, 1.0, Start.X, End.X),
				Y(time, 0.0, 1.0, Start.Y, End.Y));
		}

		// Parametric functions for drawing a degree 3 Bezier curve.
		static double X(double t,
			double x0, double x1, double x2, double x3)
		{
			return (double)(
				x0 * Math.Pow((1 - t), 3) +
				x1 * 3 * t * Math.Pow((1 - t), 2) +
				x2 * 3 * Math.Pow(t, 2) * (1 - t) +
				x3 * Math.Pow(t, 3)
			);
		}

		static double Y(double t,
			double y0, double y1, double y2, double y3)
		{
			return (double)(
				y0 * Math.Pow((1 - t), 3) +
				y1 * 3 * t * Math.Pow((1 - t), 2) +
				y2 * 3 * Math.Pow(t, 2) * (1 - t) +
				y3 * Math.Pow(t, 3)
			);
		}

		public override string ToString()
		{
			return string.Format("[EasingFunctionBezier: Start={0}, End={1}]", Start, End);
		}
	}
}
