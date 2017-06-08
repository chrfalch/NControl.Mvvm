using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Linq;

namespace NControl.Mvvm
{
	public static class TransitionExtensions
	{
		#region Private Members

		/// <summary>
		/// Transition elements
		/// </summary>
		static List<TransitionInfo> _transitionElements =>
			MvvmApp.Current.Get(() => new List<TransitionInfo>());

		#endregion

		#region Public Members

		public static VisualElement SetTransitionIdentifier(
			this VisualElement element, string identifier, TransitionTarget target)
		{
			CleanList();
			AddToList(new TransitionInfo(identifier, element, target));
			return element;
		}

		public static View SetTransitionIdentifier(
			this View element, string identifier, TransitionTarget target)
		{
			CleanList();
			AddToList(new TransitionInfo(identifier, element, target));
			return element;
		}

		public static IEnumerable<VisualElement> GetElementsForIdentifier(
			string identifier, TransitionTarget target)
		{
			CleanList();

			return _transitionElements
				.Where(ti => 
					   ti.TransitionId == identifier &&
					   ti.Target == target)
				.Select(ti => ti.GetElement());
		}

		public static IEnumerable<string> GetAvailableTransactionIdentifiers()
		{
			CleanList();

			return _transitionElements				
				.Select(ti => ti.TransitionId)
				.Distinct();
		}

		#endregion

		#region Private Members

		static void CleanList()
		{
			lock (_transitionElements)
			{
				var toRemove = new List<TransitionInfo>();
				foreach (var ti in _transitionElements)
					if (!ti.IsAlive)
						toRemove.Add(ti);

				foreach (var tito in toRemove)
					_transitionElements.Remove(tito);
			}
		}

		static void AddToList(TransitionInfo ti)
		{
			lock (_transitionElements)
			{
				// remove
				var itemToRemove = _transitionElements.FirstOrDefault(
					oti => oti.TransitionId == ti.TransitionId &&
					oti.Target == ti.Target);

				_transitionElements.Remove(itemToRemove);
				_transitionElements.Add(ti);
			}
		}

		#endregion
	}

	public enum TransitionTarget
	{
		Source,
		Target,
	}

	public class TransitionInfo
	{
		public string TransitionId { get; private set; }
		public WeakReference Element { get; private set; }
		public TransitionTarget Target { get;private set;}

		public TransitionInfo(string transitionId, VisualElement element, 
		                      TransitionTarget target)
		{
			TransitionId = transitionId;
			Element = new WeakReference(element);
			Target = target;
		}

		public bool IsAlive => Element.IsAlive;

		public VisualElement GetElement()
		{			
			if (Element.IsAlive && Element.Target != null)
				return Element.Target as VisualElement;

			return null;
		}
	}
}
