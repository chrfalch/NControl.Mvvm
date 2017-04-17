using System;
using Xamarin.Forms;
using NControl.Controls;

namespace NControl.Mvvm
{
	public enum IconType
	{
		MaterialDesign,
		Fontawesome
	}

	public class IconLabel: HorizontalStackLayout
	{
		readonly Label _textLabel;
		readonly Label _iconLabel;

		public IconLabel()
		{
			_textLabel = new Label{
				BindingContext = this,
				VerticalTextAlignment = TextAlignment.Center,
				LineBreakMode = LineBreakMode.TailTruncation,

			}.BindTo(Label.TextProperty, nameof(Text))
			 .BindTo(Label.TextColorProperty, nameof(TextColor));

			_iconLabel = new Label
			{
				FontFamily = FontMaterialDesignLabel.FontName,
				FontSize = 18,
				BindingContext = this,
			}.BindTo(Label.TextProperty, nameof(Icon))
			 .BindTo(Label.TextColorProperty, nameof(TextColor));

			Children.Add(_iconLabel);
			Children.Add(_textLabel);
		}

		/// <summary>
		/// The Text property.
		/// </summary>
		public static BindableProperty TextProperty = BindableProperty.Create(
			nameof(Text), typeof(string), typeof(IconLabel), null,
			BindingMode.OneWay);

		/// <summary>
		/// Gets or sets the Text of the IconLabel instance.
		/// </summary>
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		/// <summary>
		/// The TextColor property.
		/// </summary>
		public static BindableProperty TextColorProperty = BindableProperty.Create(
			nameof(TextColor), typeof(Color), typeof(IconLabel), Config.TextColor,
			BindingMode.OneWay);

		/// <summary>
		/// Gets or sets the TextColor of the IconLabel instance.
		/// </summary>
		public Color TextColor
		{
			get { return (Color)GetValue(TextColorProperty); }
			set { SetValue(TextColorProperty, value); }
		}

		/// <summary>
		/// The Icon property.
		/// </summary>
		public static BindableProperty IconProperty = BindableProperty.Create(
			nameof(Icon), typeof(string), typeof(IconLabel), null,
			BindingMode.OneWay);

		/// <summary>
		/// Gets or sets the Icon of the IconLabel instance.
		/// </summary>
		public string Icon
		{
			get { return (string)GetValue(IconProperty); }
			set { SetValue(IconProperty, value); }
		}

		/// <summary>
		/// The IconType property.
		/// </summary>
		public static BindableProperty IconTypeProperty = BindableProperty.Create(
			nameof(IconType), typeof(IconType), typeof(IconLabel), IconType.MaterialDesign,
			BindingMode.OneWay, null, propertyChanged: (bindable, oldValue, newValue) =>
			{
				var ctrl = (IconLabel)bindable;
				ctrl._iconLabel.FontFamily = (IconType)newValue == IconType.Fontawesome ?
					FontAwesomeLabel.FontAwesomeName : FontMaterialDesignLabel.FontName;
			});

		/// <summary>
		/// Gets or sets the IconType of the IconLabel instance.
		/// </summary>
		public IconType IconType
		{
			get { return (IconType)GetValue(IconTypeProperty); }
			set { SetValue(IconTypeProperty, value); }
		}
	}
}
