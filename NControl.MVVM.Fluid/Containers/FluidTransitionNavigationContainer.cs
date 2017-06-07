using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NControl.XAnimation;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public class FluidTransitionNavigationContainer : FluidNavigationContainer
	{
		public const uint TransitionDuration = 1250;
		IEnvironmentProvider _environmentProvider;

		public FluidTransitionNavigationContainer()
		{
			BackgroundColor = Color.Transparent;
		}

		public FluidTransitionNavigationContainer(bool hasTransparentStatusbar)
		{
			HasTransparentStatusbar = hasTransparentStatusbar;
		}

		public override void OnContentSet(View content)
		{
			base.OnContentSet(content);
			content.BackgroundColor = Color.Transparent;
		}

		View _overlay;

		public override View GetOverlayView()
		{
			if (_overlay == null)
			{
				_overlay = new BoxView
				{
					BackgroundColor = Config.ViewBackgroundColor,
				};
			}

			return _overlay;
		}

		public override IEnumerable<XAnimationPackage> TransitionIn(
			INavigationContainer fromContainer, PresentationMode presentationMode)
		{
			// Try to find the thing we're clicking on in the from view
			var fromTransitionCandidates = GetTransitionCandidates(TransitionTarget.Source);

			// Try to find the thing we're clicking on in the to view
			var toView = GetContentsView();
			var toTransitionCandidates = GetTransitionCandidates(TransitionTarget.Target);

			var transformationList = new List<XInterpolationPackage>();

			// We might have more than one candidate, lets ask the new view if
			// it prefers a specific one, or just do the default behaviour
			if (toTransitionCandidates.Any() && fromTransitionCandidates.Any())
			{
				transformationList.AddRange(GetTransitionsFromCandidates(
					fromTransitionCandidates, toTransitionCandidates));

				if (transformationList.Any())
				{
					toView.Animate("Animate", (d) =>
					{
						foreach (var transformation in transformationList)
							transformation.Interpolate(d);

					}, 0.0, 1.0, length: TransitionDuration, easing: Easing.CubicInOut);
				}
			}

			var animations = new List<XAnimationPackage>(
				new XAnimationPackage[]{
				new XAnimationPackage(_overlay)
					.SetDuration(TransitionDuration)
					.Set((transform) => transform.SetOpacity(0.0))
				//	.Add((transform) => transform.SetOpacity(0.4))
					as XAnimationPackage
				}
			);

			// Additional animations?
			if (_container.Content is IXViewAnimatable)
				return (_container.Content as IXViewAnimatable).TransitionIn(
					fromContainer, this, animations, presentationMode);

			return animations;
		}

		public override IEnumerable<XAnimationPackage> TransitionOut(
			INavigationContainer toContainer, PresentationMode presentationMode)
		{
			// Try to find the thing we're moving away from					
			//var fromTransitionCandidates = GetTransitionCandidates(TransitionTarget.Source);

			//// Try to find the thing we're moving into
			//var toView = toContainer.GetContentsView();
			//var toTransitionCandidates = GetTransitionCandidates(TransitionTarget.Target);

			//var transformationList = new List<XInterpolationPackage>();

			//// We might have more than one candidate, lets ask the new view if
			//// it prefers a specific one, or just do the default behaviour
			//if (toTransitionCandidates.Any() && fromTransitionCandidates.Any())
			//{
			//	transformationList.AddRange(GetTransitionsFromCandidates(
			//			toTransitionCandidates, fromTransitionCandidates));

			//	if (transformationList.Any())
			//	{
			//		toView.Animate("Animate", (d) =>
			//		{
			//			foreach (var transformation in transformationList)
			//				transformation.Interpolate(d);

			//		}, 1.0, 0.0, length: TransitionDuration, easing: Easing.CubicInOut);
			//	}
			//}
				
			var animations = new List<XAnimationPackage>(
				//new XAnimationPackage[]{
				//	new XAnimationPackage(_overlay)
				//		.SetDuration(TransitionDuration)
				//		.Add((transform) => transform.SetOpacity(0.0)) as XAnimationPackage
				//	}
			);

			// Additional animations?
			if (_container.Content is IXViewAnimatable)
				return (_container.Content as IXViewAnimatable).TransitionOut(
					toContainer, this, animations, presentationMode);

			return animations;
		}

		#region Private Members

		Dictionary<string, List<VisualElement>> GetTransitionCandidates(TransitionTarget target)
		{
			var dict = new Dictionary<string, List<VisualElement>>();
			var transitionIdentifiers = TransitionExtensions.GetAvailableTransactionIdentifiers();
			foreach (var transitionIdentifier in transitionIdentifiers)
			{
				var tis = TransitionExtensions.GetElementsForIdentifier(transitionIdentifier, target);
				foreach (var ti in tis)
				{
					if (!dict.ContainsKey(transitionIdentifier))
						dict.Add(transitionIdentifier, new List<VisualElement>());

					dict[transitionIdentifier].Add(ti);
				}
			}

			return dict;

		}

		IEnumerable<XInterpolationPackage> GetTransitionsFromCandidates(
			Dictionary<string, List<VisualElement>> fromTransitionCandidates, 
			Dictionary<string, List<VisualElement>> toTransitionCandidates)
		{
			var transformationList = new List<XInterpolationPackage>();

			// Enum list of pairs
			foreach (var toTransitionCandidateKey in toTransitionCandidates.Keys)
			{
				if (fromTransitionCandidates.ContainsKey(toTransitionCandidateKey))
				{
					var fromList = fromTransitionCandidates[toTransitionCandidateKey];
					var toList = toTransitionCandidates[toTransitionCandidateKey];

					if (toList.Count > 1)
					{
						throw new ArgumentException("Target transition candidates should only contain one candiate, " +
													"now it contains " + toList.Count);
					}

					// Now we can start to match and mate, lets just take the first one from
					// both lists, TODO: we can create some logic to let the toView handle
					// this selection
					var toView = toList.FirstOrDefault();
					var fromView = fromList.FirstOrDefault();

					if (fromView == null || toView == null)
						continue;

					// Now we can create a transition between these two items
					var fromRect = GetScreenCoordinates(fromView);
					var toRect = toView.Bounds;

					if (toRect.Width.Equals(0)) toRect.Width = fromRect.Width;
					if (toRect.Height.Equals(0)) toRect.Height = fromRect.Height;

					var newLoc = GetLocalCoordinates(toView, fromRect);

					var transformation = new XInterpolationPackage(toView);
					transformation.Set().SetRectangle(newLoc);

					transformation.Add()
								  .SetEasing(EasingFunctions.EaseInOut)
								  .SetRectangle(toRect);

					transformationList.Add(transformation);
				}
			}

			return transformationList;
		}

		Rectangle GetScreenCoordinates(VisualElement element)
		{
			var native = EnvironmentProvider.GetLocationOnScreen(element);
			return new Rectangle(native, element.Bounds.Size);
		}

		Rectangle GetLocalCoordinates(VisualElement element, Rectangle rect)
		{
			return new Rectangle(EnvironmentProvider.GetLocalLocation(element, rect.Location),
			                     rect.Size);
		}

		IEnvironmentProvider EnvironmentProvider
		{
			get
			{
				if (_environmentProvider == null)
					_environmentProvider = Container.Resolve<IEnvironmentProvider>();

				return _environmentProvider;
			}
		}
		#endregion
	}
}
