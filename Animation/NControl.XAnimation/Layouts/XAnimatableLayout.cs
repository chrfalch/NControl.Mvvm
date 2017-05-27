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

        readonly List<XInterpolationPackage> _animations = new List<XInterpolationPackage>();

        readonly Dictionary<View, Action<XInterpolationPackage, XAnimatableLayout>> _innerDict;
        readonly ViewCollection _children;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public XAnimatableLayout()
        {
            _innerDict = new Dictionary<View, Action<XInterpolationPackage, XAnimatableLayout>>();
            _children = new ViewCollection(_innerDict, this);
            SizeChanged += XAnimatableLayout_SizeChanged;
        }

        #region Properties

        /// <summary>
        /// Interpolation property
        /// </summary>
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
        /// Returns the child collection
        /// </summary>
        public new IElementList Children
        {
            get { return _children; }
        }

        /// <summary>
        /// Runs the animations in this instance
        /// </summary>
        public void Animate(Action completed = null, Easing easing = null,
                           uint rate = 16, uint length = 500)
        {
            var start = Interpolation;
            this.Animate("Animate", (d) =>
            {
                Interpolation = d;
            }, start, 1.0, rate, length, easing ?? Easing.CubicOut, (d, b) =>
            {
                if (d.Equals(1.0))
                    completed?.Invoke();
            });
        }

        /// <summary>
        /// Runs the animations in this instance in reverse
        /// </summary>
        public void Reverse(Action completed = null, Easing easing = null,
                           uint rate = 16, uint length = 500)
        {
            var end = Interpolation;
            this.Animate("Animate", (d) =>
            {
                Interpolation = d;
            }, end, 0.0, rate, length, easing ?? Easing.CubicOut, (d, b) =>
            {
                if (d.Equals(1.0))
                    completed?.Invoke();
            });
        }

        #endregion

        internal void BaseAdd(View view)
        {
			base.Children.Add(view);
			InvalidateLayout();
		}    

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
            // Layouting is done as part of the measure method. 
		}

        /// <summary>
        /// Update animations when size of the view changes.
        /// </summary>
        void XAnimatableLayout_SizeChanged(object sender, EventArgs e)
		{
			_animations.Clear();

            foreach (var key in _innerDict.Keys)
            {
                var animationCallback = _innerDict[key];
                var animation = new XInterpolationPackage(key);
                animationCallback(animation, this);
                _animations.Add(animation);
            }				

			InvalidateLayout();
		}
	}

	/// <summary>
	/// View collection with rect functions
	/// </summary>
	class ViewCollection : IElementList
	{
		readonly Dictionary<View, Action<XInterpolationPackage, XAnimatableLayout>> _innerDict;
		readonly XAnimatableLayout _parent;

		public ViewCollection(Dictionary<View, Action<XInterpolationPackage, XAnimatableLayout>> innerDict, XAnimatableLayout parent)
		{
			_innerDict = innerDict;
			_parent = parent;
		}

		public View this[int index]
		{
			get { return _innerDict.Keys.ElementAt(index); }
			set { throw new NotSupportedException(); }
		}

		public int Count { get { return _innerDict.Count(); } }

		public bool IsReadOnly { get { return true; } }

		public void Add(View item)
		{
			_parent.BaseAdd(item);
		}

		public void Add(View view, Action<XInterpolationPackage, XAnimatableLayout> animation)
		{
			if (animation == null)
				throw new ArgumentNullException(nameof(animation));

			_innerDict.Add(view, animation);
			_parent.BaseAdd(view);
		}

		public void Clear()
		{
			throw new NotSupportedException();
		}

		public bool Contains(View item)
		{
			return _innerDict.ContainsKey(item);
		}

		public void CopyTo(View[] array, int arrayIndex)
		{
			if (array.Length - arrayIndex < Count)
				throw new ArgumentException("Destination array was not long enough. " +
											"Check destIndex and length, and the array's lower bounds.");

			foreach (var item in this)
			{
				array[arrayIndex] = item;
				arrayIndex++;
			}
		}

		public IEnumerator<View> GetEnumerator()
		{
			return _innerDict.Keys.OfType<View>().GetEnumerator();
		}

		public int IndexOf(View item)
		{
			return _innerDict.Keys.ToList().IndexOf(item);
		}

		public void Insert(int index, View item)
		{
			throw new NotSupportedException();
		}

		public bool Remove(View item)
		{
			throw new NotSupportedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	/// <summary>
	/// List interface
	/// </summary>
	public interface IElementList : IList<View>
	{
        void Add(View view, Action<XInterpolationPackage, XAnimatableLayout> transformPackage);
	}

}
	
