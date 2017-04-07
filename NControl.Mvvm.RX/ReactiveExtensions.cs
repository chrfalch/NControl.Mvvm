using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Input;
using System.Reactive.Subjects;
using System.Collections.Generic;
using System.Reactive.Disposables;

namespace NControl.Mvvm
{
	/// <summary>
	/// Contains Rx Extension Classes
	/// </summary>
	public static class RxExtensions
	{
		/// <summary>
		/// Returns an observable sequence of the value of a property when <paramref name="source"/> raises 
		/// <seealso cref="INotifyPropertyChanged.PropertyChanged"/> for the given property.
		/// </summary>
		/// <typeparam name="T">The type of the source object. Type must implement <seealso cref="INotifyPropertyChanged"/>.</typeparam>
		/// <typeparam name="TProperty">The type of the property that is being observed.</typeparam>
		/// <param name="source">The object to observe property changes on.</param>
		/// <param name="property">An expression that describes which property to observe.</param>
		/// <returns>Returns an observable sequence of the property values as they change.</returns>
		public static IObservable<TProperty> OnPropertyChanges<T, TProperty>(this T source, Expression<Func<T, TProperty>> property)
			where T : INotifyPropertyChanged
		{
			return  Observable.Create<TProperty>(o=>
				{
					var propertyName = property.GetPropertyInfo().Name;
					var propertySelector = property.Compile();

					return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
						handler => handler.Invoke,
						h => source.PropertyChanged += h,
						h => source.PropertyChanged -= h)
							.Where(e => e.EventArgs.PropertyName == propertyName)
							.Select(e => propertySelector(source))
							.Subscribe(o);
				});
		}

		/// <summary>
		/// Gets property information for the specified <paramref name="property"/> expression.
		/// </summary>
		/// <typeparam name="TSource">Type of the parameter in the <paramref name="property"/> expression.</typeparam>
		/// <typeparam name="TValue">Type of the property's value.</typeparam>
		/// <param name="property">The expression from which to retrieve the property information.</param>
		/// <returns>Property information for the specified expression.</returns>
		/// <exception cref="ArgumentException">The expression is not understood.</exception>
		public static PropertyInfo GetPropertyInfo<TSource, TValue>(this Expression<Func<TSource, TValue>> property)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}

			var body = property.Body as MemberExpression;
			if (body == null)
			{
				throw new ArgumentException("Expression is not a property", "property");
			}

			var propertyInfo = body.Member as PropertyInfo;
			if (propertyInfo == null)
			{
				throw new ArgumentException("Expression is not a property", "property");
			}

			return propertyInfo;
		}
	}

	/// <summary>
	/// I reactive command.
	/// </summary>
	public interface IReactiveCommand : ICommand
	{
		/// <summary>
		/// Gets the command executed stream.
		/// </summary>
		/// <value>The command executed stream.</value>
		IObservable<object> CommandExecutedStream { get; }

		/// <summary>
		/// Gets the command execeptions stream.
		/// </summary>
		/// <value>The command execeptions stream.</value>
		IObservable<Exception> CommandExeceptionsStream { get; }

	}

	/// <summary>
	/// Reactive command.
	/// </summary>
	public class ReactiveCommand : IReactiveCommand, IDisposable
	{
		#region Private Members

		/// <summary>
		/// The command executed subject.
		/// </summary>
		private Subject<object> commandExecutedSubject = new Subject<object>();

		/// <summary>
		/// The command execeptions subject stream.
		/// </summary>
		private Subject<Exception> commandExeceptionsSubjectStream = new Subject<Exception>();

		/// <summary>
		/// The can execute obs.
		/// </summary>
		private IObservable<bool> canExecuteObs;

		/// <summary>
		/// The can execute latest.
		/// </summary>
		private bool canExecuteLatest = true;

		/// <summary>
		/// The disposables.
		/// </summary>
		private CompositeDisposable disposables = new CompositeDisposable();

		#endregion

		#region Events

		/// <summary>
		/// Occurs when can execute changed.
		/// </summary>
		public event EventHandler CanExecuteChanged;

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="NControl.MVVM.ReactiveCommand"/> class.
		/// </summary>
		public ReactiveCommand()
		{
			RaiseCanExecute(true);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NControl.MVVM.ReactiveCommand"/> class.
		/// </summary>
		/// <param name="initPredicate">Init predicate.</param>
		/// <param name="initialCondition">If set to <c>true</c> initial condition.</param>
		public ReactiveCommand(IObservable<bool> predicate)
		{
			if (predicate != null)
			{
				canExecuteObs = predicate;
				SetupSubscriptions();
			}
			predicate.Do (m => RaiseCanExecute (m));
		}

		/// <summary>
		/// Determines whether this instance can execute the specified parameter.
		/// </summary>
		/// <returns><c>true</c> if this instance can execute the specified parameter; otherwise, <c>false</c>.</returns>
		/// <param name="parameter">Parameter.</param>
		bool ICommand.CanExecute(object parameter)
		{
			return canExecuteLatest;
		}

		/// <Docs>To be added.</Docs>
		/// <attribution license="cc4" from="Microsoft" modified="false"></attribution>
		/// <summary>
		/// Execute the specified parameter.
		/// </summary>
		/// <param name="parameter">Parameter.</param>
		public void Execute(object parameter)
		{
			commandExecutedSubject.OnNext(parameter);
		}

		/// <summary>
		/// Gets the command executed stream.
		/// </summary>
		/// <value>The command executed stream.</value>
		public IObservable<object> CommandExecutedStream
		{
			get { return this.commandExecutedSubject.AsObservable(); }
		}

		/// <summary>
		/// Gets the command execeptions stream.
		/// </summary>
		/// <value>The command execeptions stream.</value>
		public IObservable<Exception> CommandExeceptionsStream
		{
			get { return this.commandExeceptionsSubjectStream.AsObservable(); }
		}

		/// <summary>
		/// Releases all resource used by the <see cref="NControl.MVVM.ReactiveCommand"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="NControl.MVVM.ReactiveCommand"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="NControl.MVVM.ReactiveCommand"/> in an unusable state. After
		/// calling <see cref="Dispose"/>, you must release all references to the <see cref="NControl.MVVM.ReactiveCommand"/>
		/// so the garbage collector can reclaim the memory that the <see cref="NControl.MVVM.ReactiveCommand"/> was occupying.</remarks>
		public void Dispose()
		{
			disposables.Dispose();
		}

		#region Protected and Private Members

		/// <summary>
		/// Raises the can execute changed.
		/// </summary>
		/// <param name="e">E.</param>
		protected virtual void RaiseCanExecuteChanged(EventArgs e)
		{
			var handler = this.CanExecuteChanged;

			if (handler != null)
			{
				handler(this, e);
			}
		}

		/// <summary>
		/// Raises the can execute.
		/// </summary>
		/// <param name="value">If set to <c>true</c> value.</param>
		private void RaiseCanExecute(bool value)
		{
			canExecuteLatest = value;
			this.RaiseCanExecuteChanged(EventArgs.Empty);
		}

		/// <summary>
		/// Setups the subscriptions.
		/// </summary>
		private void SetupSubscriptions()
		{

			disposables = new CompositeDisposable();
			disposables.Add(this.canExecuteObs.Subscribe(
				//OnNext
				x =>
				{
					RaiseCanExecute(x);
				},
				//onError
				commandExeceptionsSubjectStream.OnNext
			));
		}

		#endregion
	}

}

