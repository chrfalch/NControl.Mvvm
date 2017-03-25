using System;
using Xamarin.Forms;
using NControl.XAnimation;

namespace XAnimationDemo
{
	public class XAnimationDemoPage4: ContentPage
	{
		public XAnimationDemoPage4()
		{
			var box = new BoxView
			{
				BackgroundColor = Color.Aqua,
				HeightRequest = 44,
			};

			Content = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				Spacing = 14,
				Padding = 24,
				Children = {
					new Grid{
						Children = {
							new Label{
								Text = "Hello World",
								HorizontalTextAlignment = TextAlignment.Center,
								VerticalTextAlignment = TextAlignment.Center,
							},
							box,
						}
					},

					new Button{
						Text = "Out",
						Command = new Command(()=>{

							new XAnimationPackage(box)
								.Opacity(0.0)
								.Duration(500)
								.Easing(EasingFunction.EaseOut)
								.Animate()
								.Run();

						}),
					},

					new Button{
						Text = "In",
						Command = new Command(()=>{

							new XAnimationPackage(box)
								.Opacity(0)
								.Set()
								.Duration(500)
								.Opacity(1)
								.Animate()
								.Run();

						}),
					}
				},
			};
		}
	}
}
