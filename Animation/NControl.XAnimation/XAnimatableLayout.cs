using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace NControl.XAnimation
{
	public class XAnimatableLayout : Layout<View>
	{
		#region Private Members

		readonly List<XAnimationPackage> _animations = new List<XAnimationPackage>();
		readonly List<Func<XAnimatableLayout, XAnimationPackage>> _animationCallback =
			new List<Func<XAnimatableLayout, XAnimationPackage>>();

		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		public XAnimatableLayout()
		{
			//VerticalOptions = (base.HorizontalOptions = LayoutOptions.FillAndExpand);
			SizeChanged += XAnimatableLayout_SizeChanged;
		}

		#region Properties

		public static BindableProperty InterpolationProperty = BindableProperty.Create(
			nameof(Interpolation), typeof(double), typeof(XAnimatableLayout),
			0.0, BindingMode.TwoWay, propertyChanged: (bindable, oldValue, newValue) =>
			{
				var control = (XAnimatableLayout)bindable;

				if (newValue == oldValue)
					return;

				if ((double)newValue < 0.0)
					control.Interpolation = 0.0;
				else if ((double)newValue > 1.0)
					control.Interpolation = 1.0;
				else
					control.Interpolation = (double)newValue;

				control.InvalidateLayout();
			});


		/// <summary>
		/// Gets or sets the time interpolation.
		/// </summary>
		public double Interpolation
		{
			get { return (double)GetValue(InterpolationProperty); }
			set { SetValue(InterpolationProperty, value); }
		}

		/// <summary>
		/// Runs the animations in this instance
		/// </summary>
		public void Animate(Action completed = null)
		{
			var start = Interpolation;
			this.Animate("Animate", (d) =>
			{
				Interpolation = d;
			}, start, 1.0, 16, 500, Easing.CubicInOut, (d, b) => {
				if (d.Equals(1.0))
					 completed?.Invoke();
			});
		}

		/// <summary>
		/// Runs the animations in this instance in reverse
		/// </summary>
		public void Reverse(Action completed = null)
		{
			var end = Interpolation;
            this.Animate("Animate", (d) => {
				Interpolation = d;
			}, end, 0.0, 16, 500, Easing.CubicInOut, (d, b) => {
				if (d.Equals(1.0))
					completed?.Invoke();
			});

			//XAnimationPackage.RunAll(_animations, () =>
			//{
			//	completed?.Invoke();
			//	Interpolation = 0.0;
			//}, true);
		}

		/// <summary>
		/// Adds an animation callback. 
		/// </summary>
		/// <param name="callback">Callback.</param>
		public void AddAnimation(Func<XAnimatableLayout, XAnimationPackage> callback)
		{
			_animationCallback.Add(callback);
			_animations.Add(callback(this));
			InvalidateLayout();
		}

		#endregion

		/// <summary>
		/// Measure size
		/// </summary>
		/// <returns>The measure.</returns>
		/// <param name="widthConstraint">Width constraint.</param>
		/// <param name="heightConstraint">Height constraint.</param>
		protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
		{
			var retVal = base.OnMeasure(widthConstraint, heightConstraint);

			foreach (var animation in _animations)
				animation.Interpolate(Interpolation);

			var constraints = Rectangle.Zero;
			foreach (var element in Children)
			{				
				constraints.Left = Math.Min(constraints.Left, element.X);
				constraints.Top = Math.Min(constraints.Top, element.Y);
				constraints.Width = Math.Max(constraints.Width, element.X + element.Width);
				constraints.Height = Math.Max(constraints.Height, element.Y + element.Height);
			}

			retVal.Request = constraints.Size;
			retVal.Minimum = constraints.Size;

			return retVal;
		}

		/// <summary>
		/// Layouts the children.
		/// </summary>
		protected override void LayoutChildren(double x, double y, double width, double height)
		{

		}

		void XAnimatableLayout_SizeChanged(object sender, EventArgs e)
		{
			_animations.Clear();

			foreach (var animationCallback in _animationCallback)
				_animations.Add(animationCallback(this));

			InvalidateLayout();
		}
	}
}
	
