using System;
using Xamarin.Forms;

namespace NControl.XAnimation
{
	public enum EasingFunction
	{
		Linear,
		EaseIn,
		EaseOut,
		EaseInOut,
		Custom,
	}

	/// <summary>
	/// Contains information about an animation 
	/// </summary>
	public class XAnimationInfo
	{
		static int Counter = 0;

		public long Delay { get; set; }
		public long Duration { get; set; }
		public int AnimationId { get; set; }

		public double Scale { get; set; }
		public double Rotate { get; set; }
		public double TranslationX { get; set; }
		public double TranslationY { get; set; }
		public double Opacity { get; set; }

		public Color Color { get; set; }

		public EasingFunction Easing { get; set; }
		public EasingFunctionBezier EasingBezier { get; set; }

		/// <summary>
		/// Set to true to indicate that we should not run this as an animation, just
		/// apply transformations directly.
		/// </summary>
		public bool OnlyTransform { get; set; }

		public XAnimationInfo(): this(null)
		{
			
		}

		public XAnimationInfo(XAnimationInfo prevAnimationInfo) : this(prevAnimationInfo, true)
		{
		}

		public XAnimationInfo(XAnimationInfo prevAnimationInfo, bool keepTransforms)
		{
			AnimationId = Counter++;

			// Delay should always be reset
			Delay = 0;

			// Duration should be inherited
			Duration = prevAnimationInfo != null ? prevAnimationInfo.Duration : 250;

			if (keepTransforms && prevAnimationInfo != null)
			{
				Scale = prevAnimationInfo.Scale;
				Rotate = prevAnimationInfo.Rotate;
				TranslationX = prevAnimationInfo.TranslationX;
				TranslationY = prevAnimationInfo.TranslationY;
				Opacity = prevAnimationInfo.Opacity;
				Color = prevAnimationInfo.Color;
			}
			else
			{
				Reset();
			}
		}

		public void Reset()
		{
			Scale = 1;
			Rotate = 0;
			TranslationX = 0;
			TranslationY = 0;
			Opacity = 1;
			Easing = EasingFunction.EaseIn;
			EasingBezier = new EasingFunctionBezier(new Point(0, 0), new Point(1.0, 1.0));
		}

		public override string ToString()
		{
			return string.Format(OnlyTransform ? "Set" : "Animate" + ": [#{3}: Delay={0}, Duration={1}, Repeat={2}, Scale={4}," + 
			                     " Rotate={5}, TranslationX={6}, TranslationY={7}, Opacity={8}]", Delay, Duration, false, AnimationId, 
			                     Scale, Rotate, TranslationX, TranslationY, Opacity);
		}
	}

	public sealed class EasingFunctionBezier
	{
		public Point Start { get; private set; }
		public Point End { get; private set; }
		public EasingFunctionBezier(Point start, Point end)
		{
			Start = start;
			End = end;
		}
	}
}
