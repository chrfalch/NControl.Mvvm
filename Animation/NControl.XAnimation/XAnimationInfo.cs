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
		long _duration;
		long _delay;

		static int Counter = 0;

		public long Delay { get { return _delay * (XAnimationPackage.SlowAnimations ? 5 : 1); } set { _delay = value; } }
		public long Duration { get { return _duration * (XAnimationPackage.SlowAnimations ? 5 : 1); } set { _duration = value; } }
		public int AnimationId { get; set; }

		public double Scale { get; set; }
		public double Rotate { get; set; }
		public double TranslationX { get; set; }
		public double TranslationY { get; set; }
		public double Opacity { get; set; }

		public bool AnimateColor { get; set; }
		public Color Color { get; set; }

		public bool AnimateRectangle { get; set; }
		public Rectangle Rectangle {get;set;}

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
			AnimateColor = false;
			Easing = EasingFunction.Linear;
			EasingBezier = new EasingFunctionBezier(new Point(0, 0), new Point(1.0, 1.0));
			Rectangle = Rectangle.Zero;
			AnimateRectangle = false;
		}

		public override string ToString()
		{
			return string.Format(OnlyTransform ? "Set" : "Animate" + ": [#{3}: Delay={0}, Duration={1}, Repeat={2}, Scale={4}," + 
			                     " Rotate={5}, TranslationX={6}, TranslationY={7}, Opacity={8}, Color={9}, Easing={10}]", Delay, Duration, false, AnimationId, 
			                     Scale, Rotate, TranslationX, TranslationY, Opacity, AnimateColor ? Color.ToString() : "NO", Easing);
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

		public EasingFunctionBezier(double startX, double startY, double endX, double endY):
			this(new Point(startX, startY), new Point(endX, endY))
		{			
		}
	}
}
