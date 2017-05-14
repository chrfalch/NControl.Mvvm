using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NControl.XAnimation
{
    public interface IXPackage
    {
        int ElementCount { get; }
        VisualElement GetElement(int index);
    }

    public interface IXTransformation<TReturnType> where TReturnType : class
    {
		/// <summary>
		/// Adds a rotate transformation
		/// </summary>
		TReturnType Rotate(double rotation);

		/// <summary>
		/// Fade to the specified opacity (between 0.0 -> 1.0 where 1.0 is non-transparent).
		/// </summary>
		TReturnType Opacity(double opacity);

		/// <summary>
		/// Sets the scale factor
		/// </summary>
		TReturnType Scale(double scale);

		/// <summary>
		/// Translate the view x and y pixels
		/// </summary>
		TReturnType Translate(double x, double y);

		/// <summary>
		/// Resets the transformation
		/// </summary>
		TReturnType Reset();

		/// <summary>
		/// Transforms according to the animation
		/// </summary>
		TReturnType Set();

		/// <summary>
		/// Adds a new transformation to the sequence of transformations
		/// </summary>
		TReturnType Then();

		/// <summary>
		/// Transforms the color of the item
		/// </summary>
		TReturnType Color(Color color);

		/// <summary>
		/// Transforms the frame of the item
		/// </summary>
		TReturnType Frame(Rectangle rect);

		/// <summary>
		/// Transforms the frame of the item
		/// </summary>
        TReturnType Frame(double x, double y, double width, double height);

		/// <summary>
		/// Interpolate the animation along the values from 0.0 -> 1.0
		/// </summary>
		void Interpolate(double value);
	}

    public interface IXTimeable<TReturnType>: IXTransformation<TReturnType> where TReturnType : class
    {
		/// <summary>
		/// Sets the duration for the current animation in milliseconds.
		/// </summary>      
		TReturnType Duration(long milliseconds);
    }

    public interface IXTimeable: IXTimeable<IXTimeable>
    {
        
    }

    public interface IXAnimation<TReturnType>: IXTimeable<TReturnType> where TReturnType : class
    {
        /// <summary>
        /// Sets the easing function 
        /// </summary>
        TReturnType Easing(EasingFunction easing);

        /// <summary>
        /// Creates a custom easing curve. See more here: http://cubic-bezier.com
        /// </summary>
        TReturnType Easing(Point start, Point end);

        /// <summary>
        /// Creates a custom easing curve. See more here: http://cubic-bezier.com
        /// </summary>
        TReturnType Easing(double startX, double startY, double endX, double endY);

        /// <summary>
        /// Creates a custom easing curve. See more here: http://cubic-bezier.com
        /// </summary>
        TReturnType Easing(EasingFunctionBezier easingFunction);

        /// <summary>
        /// Saves state for the current animation 
        /// </summary>
        void Save();

        /// <summary>
        /// Pops the current state and restores
        /// </summary>
        void Restore();

        /// <summary>
        /// Runs this animation
        /// </summary>
        void Run(Action completed = null, bool reverse = false);

        /// <summary>
        /// Runs the animation in reverse.
        /// </summary>
        void RunReverse(Action completed = null);

        /// <summary>
        /// Runs the animation async
        /// </summary>
        Task RunAsync(bool reverse = false);
    }

    public interface IXAnimation: IXAnimation<IXAnimation>
    {        
    }
}
