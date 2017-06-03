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
			var fromView = fromContainer.GetContentsView();
			var fromTransitionCandidates = new Dictionary<string, List<View>>();
			GetTransitionCandidates(fromView, fromTransitionCandidates);

			// Try to find the thing we're clicking on in the to view
			var toView = GetContentsView();
			var toTransitionCandidates = new Dictionary<string, List<View>>();
			GetTransitionCandidates(toView, toTransitionCandidates);

			var transformationList = new List<XInterpolationPackage>();

			// We might have more than one candidate, lets ask the new view if
			// it prefers a specific one, or just do the default behaviour
			if (toTransitionCandidates.Any() && fromTransitionCandidates.Any())
			{
				transformationList.AddRange(
					GetResolvedCandidateTransitionInAnimations(fromTransitionCandidates, 
					                                           toTransitionCandidates));
			}

			toView.Animate("Animate", (d) =>
			{
				foreach (var transformation in transformationList)
					transformation.Interpolate(d);
				
			}, 0.0, 1.0, length:TransitionDuration, easing: Easing.CubicInOut);
				
			var animations = new List<XAnimationPackage>(new XAnimationPackage[]{
				new XAnimationPackage(_overlay)
					.SetDuration(TransitionDuration)
					.Set((transform) => transform.SetOpacity(0.0))
					//.Add((transform) => transform.SetOpacity(1.0))
					as XAnimationPackage
				});

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
			var fromView = GetContentsView();
			var fromTransitionCandidates = new Dictionary<string, List<View>>();
						GetTransitionCandidates(fromView, fromTransitionCandidates);

			// Try to find the thing we're moving into
			var toView = toContainer.GetContentsView();
			var toTransitionCandidates = new Dictionary<string, List<View>>();
			GetTransitionCandidates(toView, toTransitionCandidates);

			var transformationList = new List<XInterpolationPackage>();

			// We might have more than one candidate, lets ask the new view if
			// it prefers a specific one, or just do the default behaviour
			if (toTransitionCandidates.Any() && fromTransitionCandidates.Any())
			{
				transformationList.AddRange(
					GetResolvedCandidateTransitionInAnimations(toTransitionCandidates, 
					                                           fromTransitionCandidates));
			}

			toView.Animate("Animate", (d) =>
			{
				foreach (var transformation in transformationList)
					transformation.Interpolate(d);
				
			}, 1.0, 0.0, length:TransitionDuration, easing: Easing.CubicInOut);
				
			var animations = new List<XAnimationPackage>(new XAnimationPackage[]{
				new XAnimationPackage(_overlay)
					.SetDuration(TransitionDuration)
					.Add((transform) => transform.SetOpacity(0.0)) as XAnimationPackage
				});

			// Additional animations?
			if (_container.Content is IXViewAnimatable)
				return (_container.Content as IXViewAnimatable).TransitionOut(
					toContainer, this, animations, presentationMode);

			return animations;
		}

		#region Private Members

		void GetTransitionCandidates(View view, Dictionary<string, List<View>> dict)
		{
			if (view is ContentView)
			{
				if ((view as ContentView).Content != null)
					GetTransitionCandidates((view as ContentView).Content, dict);
			}
			else if (view is ILayoutController)
			{
				foreach (var child in (view as ILayoutController).Children)
					if (child is View)
						GetTransitionCandidates(child as View, dict);
			}
			else if (view is ListViewEx)
			{
				if((view as ListViewEx).SelectedCell is ViewCell)					
					GetTransitionCandidates(((view as ListViewEx).SelectedCell as ViewCell).View, dict);
			}

			var transitionIdentifier = view.GetTransitionIdentifier();
			if (!string.IsNullOrEmpty(transitionIdentifier))
			{
				if (!dict.ContainsKey(transitionIdentifier))
					dict.Add(transitionIdentifier, new List<View>());

				dict[transitionIdentifier].Add(view);
			}
		}

		IEnumerable<XInterpolationPackage> GetResolvedCandidateTransitionInAnimations(
			Dictionary<string, List<View>> fromTransitionCandidates, Dictionary<string, List<View>> toTransitionCandidates)
		{
			var transformationList = new List<XInterpolationPackage>();

			// Enum list of pairs
			foreach (var toTransitionCandidateKey in toTransitionCandidates.Keys)
			{
				if (fromTransitionCandidates.ContainsKey(toTransitionCandidateKey))
				{
					var fromList = fromTransitionCandidates[toTransitionCandidateKey];
					var toList = toTransitionCandidates[toTransitionCandidateKey];

					// Now we can start to match and mate, lets just take the first one from
					// both lists, TODO: we can create some logic to let the toView handle
					// this selection
					var fromView = fromList.FirstOrDefault();
					var toView = toList.FirstOrDefault();
					if (fromView == null || toView == null)
						continue;

					// Now we can create a transition between these two items
					var fromRect = GetScreenCoordinates(fromView);
					var toRect = GetScreenCoordinates(toView);
					if (toRect.Width.Equals(0)) toRect.Width = fromRect.Width;
					if (toRect.Height.Equals(0)) toRect.Height = fromRect.Height;

					var startRect = new Rectangle(toView.Bounds.X, toView.Bounds.Y, fromRect.Width, fromRect.Height);
					System.Diagnostics.Debug.WriteLine(toView + " " + new Rectangle(fromRect.X - toRect.X, 
					                                                                fromRect.Y - toRect.Y, 
					                                                                startRect.Width, startRect.Height)
					                                   + " => " + toRect);

					var transformation = new XInterpolationPackage(toView);
					transformation.Set()
					              .SetRectangle(startRect)
					              .SetTranslation(fromRect.X - toRect.X, fromRect.Y - toRect.Y);
					
					//transformation.Add()
					//		 .SetEasing(EasingFunctions.EaseInOut)
					//		 .SetRectangle(toRect)
					//              .SetTranslation(0, 0);

					transformationList.Add(transformation);
				}
			}

			return transformationList;
		}

		Rectangle GetScreenCoordinates(VisualElement view)
		{
			var environmentProvider = Container.Resolve<IEnvironmentProvider>();
			return environmentProvider.GetLocationOnScreen(view);

		}
		#endregion
	}
}
