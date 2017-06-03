using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NControl.XAnimation
{
    public abstract class XElementContainer
    {
        #region Private Members

        /// <summary>
        /// List of elements we are animating
        /// </summary>
        readonly List<WeakReference<VisualElement>> _elements = new List<WeakReference<VisualElement>>();

		#endregion

		#region Static Properties

		/// <summary>
		/// Set to true to speed down animations
		/// </summary>
		public static bool SlowAnimations { get; set; }
		public static bool ShowSetTransforms { get; set; }

		#endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public XElementContainer(params VisualElement[] elements)
        {
            _elements.AddRange(elements.Select(el => new WeakReference<VisualElement>(el)));
        }

        #region IXPackage implementation

        /// <summary>
        /// Returns the number of elements
        /// </summary>
        /// <value>The element count.</value>
        public int ElementCount
        {
            get
            {
                return _elements.Count();
            }
        }

        /// <summary>
        /// Returns the elements that should be animated
        /// </summary>
        public VisualElement GetElement(int index)
        {
            VisualElement el2 = null;
            if (_elements.ElementAt(index).TryGetTarget(out el2))
                return el2;

            return null;
        }

		/// <summary>
		/// Enumerate elements
		/// </summary>
		public void EnumerateElements(Action<WeakReference<VisualElement>> callback)
		{
			foreach (var elementRef in _elements)
			{
				// Get element
				callback(elementRef);
			}
		}

        #endregion
    }      
}
