using System;
using Android.Graphics;

namespace NControl.XAnimation.Droid
{
	public class DroidBezierInterpolator: Java.Lang.Object, Android.Views.Animations.IInterpolator
	{
		#region Private Members

		PointF start;
		PointF end;
		PointF a = new PointF();
		PointF b = new PointF();
		PointF c = new PointF();
	
		#endregion

		public DroidBezierInterpolator(PointF start, PointF end)
		{
        	if (start.X < 0 || start.X > 1) 
			{
				throw new ArgumentException("startX value must be in the range [0, 1]");
			}
        	if (end.X < 0 || end.X > 1) 
			{
				throw new ArgumentException("endX value must be in the range [0, 1]");
			}

	        this.start = start;
	        this.end = end;
		}

		public DroidBezierInterpolator(float startX, float startY, float endX, float endY):
			this(new PointF(startX, startY), new PointF(endX, endY))
		{
		}

		public DroidBezierInterpolator(double startX, double startY, double endX, double endY):
			this((float)startX, (float)startY, (float)endX, (float)endY)
		{			
		}

		public float GetInterpolation(float time)
		{
			return GetBezierCoordinateY(GetXForTime(time));
		}

		protected float GetBezierCoordinateY(float time)
		{
			c.Y = 3 * start.Y;
			b.Y = 3 * (end.Y - start.Y) - c.Y;
			a.Y = 1 - c.Y - b.Y;
			return time * (c.Y + time * (b.Y + time * a.Y));
		}

		protected float GetXForTime(float time)
		{
			float x = time;
			float z;
			for (int i = 1; i < 14; i++)
			{
				z = getBezierCoordinateX(x) - time;
				if (Math.Abs(z) < 1e-3)
					break;
				
				x -= z / GetXDerivate(x);
			}
			return x;
		}

		float GetXDerivate(float t)
		{
			return c.X + t * (2 * b.X + 3 * a.X * t);
		}

		float getBezierCoordinateX(float time)
		{
			c.X = 3 * start.X;
			b.X = 3 * (end.X - start.X) - c.X;
			a.X = 1 - c.X - b.X;
			return time * (c.X + time * (b.X + time * a.X));
		}	
	}
}
