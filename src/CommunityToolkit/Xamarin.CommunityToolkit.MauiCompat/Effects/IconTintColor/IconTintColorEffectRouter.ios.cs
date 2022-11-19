﻿using System;using Microsoft.Extensions.Logging;
using System.ComponentModel;
using UIKit;
using Xamarin.CommunityToolkit.Effects;
using Microsoft.Maui; using Microsoft.Maui.Controls; using Microsoft.Maui.Graphics; using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using Effects = Xamarin.CommunityToolkit.iOS.Effects;

[assembly: ExportEffect(typeof(Effects.IconTintColorEffectRouter), nameof(IconTintColorEffectRouter))]

namespace Xamarin.CommunityToolkit.iOS.Effects
{
	using Xamarin.CommunityToolkit.MauiCompat; public class IconTintColorEffectRouter : Microsoft.Maui.Controls.Platform.PlatformEffect
	{
		protected override void OnAttached()
			=> ApplyTintColor();

		protected override void OnDetached()
			=> ClearTintColor();

		protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
		{
			base.OnElementPropertyChanged(args);

			if (!args.PropertyName?.Equals(IconTintColorEffect.TintColorProperty.PropertyName) is true &&
				!args.PropertyName?.Equals(Microsoft.Maui.Controls.Image.SourceProperty.PropertyName) is true &&
				!args.PropertyName?.Equals(Microsoft.Maui.Controls.ImageButton.SourceProperty.PropertyName) is true)
				return;

			ApplyTintColor();
		}

		void ApplyTintColor()
		{
			if (Control == null || Element == null || !(Element is VisualElement))
				return;

			var color = IconTintColorEffect.GetTintColor(Element);

			switch (Control)
			{
				case UIImageView imageView:
					SetUIImageViewTintColor(imageView, color);
					break;
				case UIButton button:
					SetUIButtonTintColor(button, color);
					break;
			}
		}

		void ClearTintColor()
		{
			switch (Control)
			{
				case UIImageView imageView:
					Element.PropertyChanged -= ImageViewTintColorPropertyChanged;
					if (imageView.Image != null)
					{
						imageView.Image = imageView.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
					}

					break;
				case UIButton button:
					Element.PropertyChanged -= ButtonTintColorPropertyChanged;
					if (button.ImageView?.Image != null)
					{
						var originalImage = button.CurrentImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
						button.SetImage(originalImage, UIControlState.Normal);
					}

					break;
			}
		}

		void SetUIImageViewTintColor(UIImageView imageView, Color color)
		{
			if (imageView.Image == null)
			{
				Element.PropertyChanged += ImageViewTintColorPropertyChanged;
			}
			else
			{
				imageView.Image = imageView.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
				imageView.TintColor = color.ToUIColor();
			}
		}

		void ImageViewTintColorPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Image.IsLoadingProperty.PropertyName)
			{
				var element = (Image)Element;

				if (!element.IsLoading)
				{
					ApplyTintColor();
				}
			}
		}

		void SetUIButtonTintColor(UIButton button, Color color)
		{
			if (button.ImageView.Image == null)
			{
				Element.PropertyChanged += ButtonTintColorPropertyChanged;
			}
			else
			{
				var templatedImage = button.CurrentImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);

				button.SetImage(null, UIControlState.Normal);

				button.TintColor = color.ToUIColor();
				button.ImageView.TintColor = color.ToUIColor();
				button.SetImage(templatedImage, UIControlState.Normal);
			}
		}

		void ButtonTintColorPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == ImageButton.IsLoadingProperty.PropertyName)
			{
				var element = (ImageButton)Element;
				if (!element.IsLoading)
				{
					ApplyTintColor();
				}
			}
		}
	}
}