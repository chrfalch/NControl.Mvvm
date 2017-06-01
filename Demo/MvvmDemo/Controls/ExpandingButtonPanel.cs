using System;
using System.Windows.Input;
using Xamarin.Forms;
using NControl.Controls;
using NControl.Mvvm;
using NControl.XAnimation;
using System.Collections.Generic;

namespace MvvmDemo
{
	public class ExpandingButtonPanel: RelativeLayout
	{
		const int circleSize = 44;
		readonly RoundCornerView _circleControl;
		readonly ExtendedButton _button;
		readonly HorizontalStackLayout _container;
		readonly List<ButtonBarItem> _children = new List<ButtonBarItem>();

		public ExpandingButtonPanel()
		{
			IsClippedToBounds = true;

			// Add circle control
			HeightRequest = circleSize * 2;
			HorizontalOptions = LayoutOptions.FillAndExpand;

			_circleControl = new RoundCornerView
			{
				BackgroundColor = Color.Black.MultiplyAlpha(0.5),
				Opacity = 0.5,
				CornerRadius = circleSize / 2,
			};

			_container = new HorizontalStackLayout { 				
				IsVisible = false,
				Margin = 8,
				Spacing = 24,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
			};

			_button = new ExtendedButton
			{
				BindingContext = this,
				BorderWidth = 0,
				BackgroundColor = Color.Transparent,
				BorderRadius = circleSize / 2,
				FontFamily = FontMaterialDesignLabel.FontName,
				Text = FontMaterialDesignLabel.MDChevronUp,
				FontSize = 36,
				TextColor = Color.White,
				Command = new Command(() => Toggle()),
			};

			Children.Add(_circleControl, () => new Rectangle(
				(Width / 2) - (circleSize / 2), (Height) - (circleSize + 8),
				circleSize, circleSize));

			Children.Add(_container, () => new Rectangle(
				0, Height - circleSize, Width, circleSize));

			Children.Add(_button, () => new Rectangle(
				(Width / 2) - (circleSize / 2), (Height) - (circleSize + 8),
				circleSize, circleSize));
			
		}

		void Toggle()
		{
			bool animateOut = _button.Rotation.Equals(0.0);

			if (animateOut)
			{
				_container.Children.Clear();
				foreach (var child in _children)
					_container.Children.Add(new RoundButton
					{
						Text = child.Icon,
						Command = new Command(() =>
						{
							if (child.Command != null && child.Command.CanExecute(null))
							{
								// Hide
								Toggle();
								child.Command.Execute(null);
							}
						})
					});

				_container.IsVisible = true;
			}

			var easing = animateOut ? EasingFunctions.EaseIn : EasingFunctions.EaseOut;

			var animation = new XAnimationPackage(_container)
				.Set((t) => t.SetTranslation(0, animateOut ? Height - (circleSize * 0.8) : 0))
				.Add((t) => t
					 .SetEasing(easing)
				     .SetTranslation(0, animateOut ? 0 : Height - (circleSize * 0.8))) as XAnimationPackage;
			
			animation.Animate(() =>
			{
				if (!animateOut)
					_container.IsVisible = false;
			});

			animation = new XAnimationPackage(_button)
				.Add((t) => t
					.SetEasing(easing)
					.SetRotation(animateOut ? 180 : 0.0)
				     .SetTranslation(0, animateOut ? -(circleSize - 8) : 0)) as XAnimationPackage;

			animation.Animate();

			animation = new XAnimationPackage(_circleControl)
				.Add((t) => t
					.SetEasing(easing)
					.SetScale(animateOut ? 20 : 1.0)) as XAnimationPackage;
			
			animation.Animate();
		}

		/// <summary>
		/// Children
		/// </summary>
		/// <value>The buttons.</value>
		public IList<ButtonBarItem> Buttons
		{
			get { return _children; }
		}
	}

	public class ButtonBarItem
	{
		public string Icon { get; set; }
		public ICommand Command { get; set; }
		public ButtonBarItem()
		{

		}

		public ButtonBarItem(string icon, ICommand command = null)
		{
			Icon = icon;
			Command = command;
		}
	}
}
