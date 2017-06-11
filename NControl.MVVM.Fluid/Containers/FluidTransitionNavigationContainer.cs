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
		public const uint TransitionDuration = 250;
		IViewHelperProvider _viewHelperProvider;
		readonly List<XInterpolationPackage> _transformationList = 
			new List<XInterpolationPackage>();

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
			_transformationList.Clear();

			// Try to find the thing we're clicking on in the from view
			var fromTransitionCandidates = GetTransitionCandidates(fromContainer.GetBaseView(),
			                                                       TransitionTarget.Source);

			// Try to find the thing we're clicking on in the to view
			var toTransitionCandidates = GetTransitionCandidates(GetBaseView(), TransitionTarget.Target);

			// We might have more than one candidate, lets ask the new view if
			if (toTransitionCandidates.Any() && fromTransitionCandidates.Any())
			{
				_transformationList.AddRange(GetTransitionsFromCandidates(
					fromTransitionCandidates, toTransitionCandidates));

				if (_transformationList.Any())
				{
					this.Animate("Animate", (d) =>
					{
						foreach (var transformation in _transformationList)
							transformation.Interpolate(d);

					}, 0.0, 1.0, length: TransitionDuration, easing: Easing.CubicOut);
				}
			}

			var animations = new List<XAnimationPackage>(
				new XAnimationPackage[]{
				new XAnimationPackage(_overlay)
					.SetDuration(TransitionDuration)
					.Set((transform) => transform.SetOpacity(0.0))
					.Add((transform) => transform.SetOpacity(0.4))
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
			if (_transformationList.Any())
			{
				this.Animate("Animate", (d) =>
				{
					foreach (var transformation in _transformationList)
						transformation.Interpolate(d);

				}, 1.0, 0.0, length: TransitionDuration, easing: Easing.CubicOut);
			}
				
			var animations = new List<XAnimationPackage>(
				new XAnimationPackage[]{
					new XAnimationPackage(_overlay)
						.SetDuration(TransitionDuration)
						.Add((transform) => transform.SetOpacity(0.0)) as XAnimationPackage
					}
			);

			// Additional animations?
			if (_container.Content is IXViewAnimatable)
				return (_container.Content as IXViewAnimatable).TransitionOut(
					toContainer, this, animations, presentationMode);

			return animations;
		}

		#region Private Members

		/// <summary>
		/// Returns a list of candidate views transition operations
		/// </summary>
		Dictionary<string, List<VisualElement>> GetTransitionCandidates(VisualElement expectedParent, 
		                                                                TransitionTarget target)
		{
			var dict = new Dictionary<string, List<VisualElement>>();
			var transitionIdentifiers = TransitionExtensions.GetAvailableTransactionIdentifiers();
			foreach (var transitionIdentifier in transitionIdentifiers)
			{
				var tis = TransitionExtensions.GetElementsForIdentifier(transitionIdentifier, target);
				foreach (var ti in tis)
				{
					// Make sure we have the parent we want
					var parent = ti.Parent;
					var correctParent = false;
					while (parent != null)
					{
						if (parent == expectedParent)
						{
							correctParent = true;
							break;
						}

						if (parent.Parent is Cell)
							parent = parent.Parent.Parent;
						else
							parent = parent.Parent;
					}

					if (!correctParent || parent == null)
						continue;

					// Add to result
					if (!dict.ContainsKey(transitionIdentifier))
						dict.Add(transitionIdentifier, new List<VisualElement>());

					dict[transitionIdentifier].Add(ti);
				}
			}

			return dict;
		}

		/// <summary>
		/// Builds a list of transitions from the list of candidates
		/// </summary>
		/// <returns>The transitions from candidates.</returns>
		/// <param name="fromTransitionCandidates">From transition candidates.</param>
		/// <param name="toTransitionCandidates">To transition candidates.</param>
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

					var startRect = GetLocalCoordinates(toView, fromRect);

					var transformation = new XInterpolationPackage(toView);
					transformation.Set().SetRectangle(startRect);

					//transformation.Add()
					//			  .SetEasing(EasingFunctions.EaseInOut)
					//			  .SetRectangle(toRect);

					// Let view override
					if (_container.Content is IXViewTransitionable)
						transformationList.AddRange((_container.Content as IXViewTransitionable).OverrideTransition(
							toTransitionCandidateKey, fromView, toView, startRect, toRect, transformation));

					else
						transformationList.Add(transformation);
				}
			}

			return transformationList;
		}

		/// <summary>
		/// Returns screen coordinates for a given element
		/// </summary>
		/// <returns>The screen coordinates.</returns>
		/// <param name="element">Element.</param>
		Rectangle GetScreenCoordinates(VisualElement element)
		{
			var native = ViewHelperProvider.GetLocationOnScreen(element);
			return new Rectangle(native, element.Bounds.Size);
		}

		/// <summary>
		/// Returns local coordinate conversion 
		/// </summary>
		/// <returns>The local coordinates.</returns>
		Rectangle GetLocalCoordinates(VisualElement element, Rectangle rect)
		{
			return new Rectangle(ViewHelperProvider.GetLocalLocation(element, rect.Location),
			                     rect.Size);
		}

		/// <summary>
		/// Returns the environment provider
		/// </summary>
		/// <value>The environment provider.</value>
		IViewHelperProvider ViewHelperProvider
		{
			get
			{
				if (_viewHelperProvider == null)
					_viewHelperProvider = Container.Resolve<IViewHelperProvider>();

				return _viewHelperProvider;
			}
		}
		#endregion
	}
}
