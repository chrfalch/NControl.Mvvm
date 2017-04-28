using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace NControl.XAnimation
{
	public class XAnimatableLayout: Layout<View>
	{
		#region Private Members

		readonly List<XAnimationPackage> _animations = new List<XAnimationPackage>();
		readonly List<Func<XAnimatableLayout, XAnimationPackage>> _animationCallback = 
			new List<Func<XAnimatableLayout, XAnimationPackage>>();
		
		readonly ViewCollection _children;
		readonly Dictionary<View, Tuple<Func<XAnimatableLayout, Rectangle>, Func<XAnimatableLayout, Rectangle>>> _innerDict =
					new Dictionary<View, Tuple<Func<XAnimatableLayout, Rectangle>, Func<XAnimatableLayout, Rectangle>>>();
		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		public XAnimatableLayout()
		{			
			VerticalOptions = HorizontalOptions = LayoutOptions.FillAndExpand;
			_children = new ViewCollection(_innerDict, this);
			SizeChanged += XAnimatableLayout_SizeChanged;
		}

		#region Properties

		public static BindableProperty InterpolationProperty = BindableProperty.Create(
			nameof(Interpolation), typeof(double), typeof(XAnimatableLayout),
			0.0, BindingMode.OneWay);


		/// <summary>
		/// Gets or sets the time interpolation.
		/// </summary>
		public double Interpolation
		{
			get { return (double)GetValue(InterpolationProperty); }
			set {
				if (value < 0.0)
					SetValue(InterpolationProperty, 0.0);
				else if (value > 1.0)
                    SetValue(InterpolationProperty, 1.0);
				else
                    SetValue(InterpolationProperty, value);

				InvalidateLayout();
			}
		}

		/// <summary>
		/// Runs the animations in this instance
		/// </summary>
		public void Animate(Action completed = null)
		{
			XAnimationPackage.RunAll(_animations, completed);
		}

		/// <summary>
		/// Returns the list of children
		/// </summary>
		public new IElementList Children { get { return _children; } }

		/// <summary>
		/// Adds an animation callback. 
		/// </summary>
		/// <param name="callback">Callback.</param>
		public void AddAnimation(Func<XAnimatableLayout, XAnimationPackage> callback)
		{
			_animationCallback.Add(callback);
			_animations.Add(callback(this));
			InvalidateLayout();
		}

		#endregion

		/// <summary>
		/// Layouts the children.
		/// </summary>
		protected override void LayoutChildren(double x, double y, double width, double height)
		{
			foreach (var animation in _animations)
				animation.Interpolate(Interpolation);
			
			//foreach (var childItem in _innerDict)
			//{
			//	// Interpolate
			//	var start = childItem.Value.Item1(this);
			//	var end = childItem.Value.Item2(this);

			//	var currentBounds = new Rectangle(
			//		start.X + ((end.X - start.X) * Interpolation),
			//		start.Y + ((end.Y - start.Y) * Interpolation),
			//		start.Width + ((end.Width - start.Width) * Interpolation),
			//		start.Height + ((end.Height - start.Height) * Interpolation));
				
			//	childItem.Key.Layout(currentBounds);
			//}
		}

		/// <summary>
		/// Add to base child collection
		/// </summary>
		void BaseAdd(View item)
		{
			base.Children.Add(item);
            InvalidateLayout();
		}

		void XAnimatableLayout_SizeChanged(object sender, EventArgs e)
		{
			_animations.Clear();
			foreach (var animationCallback in _animationCallback)
			{
				_animations.Add(animationCallback(this));
			}

			InvalidateLayout();
		}

		/// <summary>
		/// View collection with rect functions
		/// </summary>
		class ViewCollection : IElementList
		{
			readonly Dictionary<View, Tuple<Func<XAnimatableLayout, Rectangle>, Func<XAnimatableLayout, Rectangle>>> _innerDict;
			readonly XAnimatableLayout _parent;

			public ViewCollection(Dictionary<View, Tuple<Func<XAnimatableLayout, Rectangle>, Func<XAnimatableLayout, Rectangle>>> innerDict, XAnimatableLayout parent)
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

			public void Add(View view, Func<XAnimatableLayout, Rectangle> start, Func<XAnimatableLayout, Rectangle> end)
			{
				if (start == null)
					throw new ArgumentNullException(nameof(start));

				if (end == null)
					throw new ArgumentNullException(nameof(end));

				_innerDict.Add(view, new Tuple<Func<XAnimatableLayout, Rectangle>, Func<XAnimatableLayout, Rectangle>>(start, end));

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
	}

	/// <summary>
	/// List interface
	/// </summary>
	public interface IElementList : IList<View>
	{		
		void Add(View view, Func<XAnimatableLayout, Rectangle> start, Func<XAnimatableLayout, Rectangle> end);
	}

}
