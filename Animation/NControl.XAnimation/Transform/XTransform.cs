using System;
using Xamarin.Forms;

namespace NControl.XAnimation
{
	/// <summary>
	/// Contains information about an animation 
	/// </summary>
	public class XTransform
	{		
        #region Private Members

		static int Counter = 0;
        long _duration = 0;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
		public XTransform(): this(null)
		{
			
		}

        /// <summary>
        /// Constructor from previous info
        /// </summary>
        /// <param name="prevAnimationInfo">Previous animation info.</param>
		public XTransform(XTransform prevAnimationInfo) : this(prevAnimationInfo, true)
		{
		}

        /// <summary>
        /// Constructor from previous info
        /// </summary>
        /// <param name="prevAnimationInfo">Previous animation info.</param>
        /// <param name="keepTransforms">If set to <c>true</c> keep transforms.</param>
		public XTransform(XTransform prevAnimationInfo, bool keepTransforms)
		{
			AnimationId = Counter++;

			// Delay should always be reset
			Delay = 0;

			// Duration should be inherited
			Duration = prevAnimationInfo != null && !prevAnimationInfo.OnlyTransform ?
				prevAnimationInfo.Duration : 250;

			if (keepTransforms && prevAnimationInfo != null)
			{
				Scale = prevAnimationInfo.Scale;
				Rotation = prevAnimationInfo.Rotation;
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

		#endregion

		#region Public Properties

		public long Delay { get; internal set; }
		public long Duration
		{
			get { return OnlyTransform ? 0 : _duration; }
			internal set { _duration = value; }
		}
		public int AnimationId { get; internal set; }

		public double Scale { get; internal set; }
		public double Rotation { get; internal set; }
		public double TranslationX { get; internal set; }
		public double TranslationY { get; internal set; }
		public double Opacity { get; internal set; }

		public bool AnimateColor { get; internal set; }
		public Color Color { get; internal set; }

		public bool AnimateRectangle { get; internal set; }
		public Rectangle Rectangle { get; internal set; }

		public EasingFunctionBezier Easing { get; internal set; }

		/// <summary>
		/// Set to true to indicate that we should not run this as an animation, just
		/// apply transformations directly.
		/// </summary>
		public bool OnlyTransform { get; internal set; }

        #endregion

        #region Public Members

        /// <summary>
        /// Resets this instance
        /// </summary>
        public void Reset()
		{
			Scale = 1;
			Rotation = 0;
			TranslationX = 0;
			TranslationY = 0;
			Opacity = 1;
			AnimateColor = false;
			Easing = EasingFunctions.Linear;
			Rectangle = Rectangle.Zero;
			AnimateRectangle = false;
		}

		/// <summary>
		/// Rotates the view around the z axis
		/// </summary>
        public XTransform SetRotation(double rotation)
		{
			Rotation = rotation;
			return this;
		}

		/// <summary>
		/// Fade to the specified opacity (between 0.0 -> 1.0 where 1.0 is non-transparent).
		/// </summary>
		/// <param name="opacity">Opacity.</param>
		public XTransform SetOpacity(double opacity)
		{
			Opacity = opacity;
			return this;
		}

		/// <summary>
		/// Sets the easing.
		/// </summary>
		public XTransform SetEasing(EasingFunctionBezier easing)
		{
			Easing = easing;
			return this;
		}

		/// <summary>
		/// Sets the easing.
		/// </summary>
		public XTransform SetEasing(Point start, Point end)
		{
			Easing = EasingFunctions.Custom(start, end);
			return this;
		}

		/// <summary>
		/// Sets the easing.
		/// </summary>
		public XTransform SetEasing(double startX, double startY, double endX, double endY)
		{
			Easing = EasingFunctions.Custom(startX, startY, endX, endY);
			return this;
		}

		/// <summary>
		/// Sets the scale factor
		/// </summary>
		/// <returns>The scale.</returns>
		/// <param name="scale">Scale.</param>
		public XTransform SetScale(double scale)
		{
			Scale = scale;
			return this;
		}

		/// <summary>
		/// Translate the view x and y pixels
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public XTransform SetTranslation(double x, double y)
		{
			TranslationX = x;
			TranslationY = y;
			return this;
		}

		public XTransform SetColor(Color color)
		{
			AnimateColor = true;
			Color = color;
			return this;
		}

		public XTransform SetRectangle(Rectangle rect)
		{
			AnimateRectangle = true;
			Rectangle = rect;
			return this;
		}

        public XTransform SetOnlyTransform(bool isTransformOnly)
        {
            OnlyTransform = isTransformOnly;
            return this;
        }

		public XTransform SetRectangle(double x, double y, double width, double height)
		{
			return SetRectangle(new Rectangle(x, y, width, height));
		}

        public XTransform SetDuration(long duration)
        {
            Duration = duration;
            return this;
        }

		#endregion

		#region Overridden Members

		public override string ToString()
		{
			return string.Format(OnlyTransform ? "Set" : "Animate" + 
			                     ": Delay={0}, Duration={1}, AnimationId={2}, Scale={3}, " + 
			                     "Rotation={4}, TranslationX={5}, TranslationY={6}, Opacity={7}, " + 
			                     "AnimateColor={8}, Color={9}, AnimateRectangle={10}, " + 
			                     "Rectangle={11}, OnlyTransform={12}]", Delay, Duration, 
			                     AnimationId, Scale, Rotation, TranslationX, TranslationY, Opacity, 
			                     AnimateColor, Color, AnimateRectangle, Rectangle, OnlyTransform);
		}

        #endregion

        #region Static Members

        /// <summary>
        /// Clones an animation info object
        /// </summary>
        public static XTransform FromAnimationInfo(XTransform info)
		{
			return new XTransform
			{
				Duration = info.Duration,
				AnimateColor = info.AnimateColor,
				AnimationId = info.AnimationId,
				AnimateRectangle = info.AnimateRectangle,
				Color = info.Color,
				Delay = info.Delay,
				Easing = info.Easing,
				OnlyTransform = info.OnlyTransform,
				Opacity = info.Opacity,
				Rectangle = info.Rectangle,
				Rotation = info.Rotation,
				Scale = info.Scale,
				TranslationX = info.TranslationX,
				TranslationY = info.TranslationY,
			};
		}

		public static XTransform FromAnimationInfoAndElement(VisualElement element, XTransform animationInfo)
		{
			return new XTransform
			{
				Duration = animationInfo.Duration,
				Delay = animationInfo.Delay,
				//Easing = animationInfo.Easing,
				//EasingBezier = animationInfo.EasingBezier,
				OnlyTransform = animationInfo.OnlyTransform,
				Rotation = element.Rotation,
				TranslationX = element.TranslationX,
				TranslationY = element.TranslationY,
				Scale = element.Scale,
				Opacity = element.Opacity,
				AnimateColor = animationInfo.AnimateColor,
				Color = element.BackgroundColor,
				AnimateRectangle = animationInfo.AnimateRectangle,
				Rectangle = new Xamarin.Forms.Rectangle(element.X, element.Y, element.Width, element.Height),
			};
		}

        #endregion
	}
}
