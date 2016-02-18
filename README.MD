# NControl.Mvvm

This small library is a simple Mvvm framework for Xamarin.Forms based 
around the [NControl](https://github.com/chrfalch/NControl) and
[NControl.Controls](https://github.com/chrfalch/NControl.Controls) libraries also created and distributed by yours truly.

The library is built to make it easy to build mobile applications with
Xamarin.Forms without having to write the same boilerplate code over and
over again. 

> Disclaimer: This is [my](https://github.com/chrfalch) library and it is
written with the tools and packages I use when developing my mobile apps for
my customers.

## Dependencies

Since this library is built upon [NGraphics](https://github.com/chrfalch/NGraphics), [NControl](https://github.com/chrfalch/NControl) and [NControl.Controls](https://github.com/chrfalch/NControl.Controls) which are open source Nuget packages.

In addition the library uses a few other projects that I have chosen to use - but these can be configured if you want other implementations:

- [SimpleInjector](https://simpleinjector.org/index.html) - Fast and efficient Dependency Injection library
- [Reactive Extensions](https://rx.codeplex.com) - Reactive Extensions for those who like it
- [ReactiveProperty](https://github.com/runceel/ReactiveProperty) - Easy extensions to handle properties and commands

## Requirements
The library is based on the [Mvvm pattern](https://en.wikipedia.org/wiki/Model–view–viewmodel) first described by Microsoft. This document will not try to explain the pattern.

When bulding this library there were a few simple requirements on my list:

- The library should significantly increase the speed of setting up a new mobile app
- Remove boring boilerplate code
- Be based on dependency injection
- View model-to-view model navigation with support for different navigation types
- Use the async-await pattern as much as possible

## Getting Started
Getting started with NControl.Mvvm is simple and only consists of a few steps before you are ready to run:

- Create a new Xamarin.Forms/PCL based cross platform app for iOS/Android
- Add the package NControl.Mvvm to all projects

### Create a new app that inherits from the base MvvmApp class in the PCL project

```csharp
public class DemoMvvmApp: MvvmApp
{
	public DemoMvvmApp (IMvvmPlatform platform): base(platform)
	{
	}
}
```

### Update your startup code
Update the ```AppDelegate.cs``` file for iOS:

```csharp
public override bool FinishedLaunching (UIApplication app, NSDictionary options)
{
	global::Xamarin.Forms.Forms.Init ();
	LoadApplication (new DemoMvvmApp (new TouchPlatform()));
	return base.FinishedLaunching (app, options);
}
```

Update the code in your ```MainActivity.cs``` file for Android:

```csharp
protected override void OnCreate (Bundle bundle)
{
	base.OnCreate (bundle);
	global::Xamarin.Forms.Forms.Init (this, bundle);
	LoadApplication (new DemoMvvmApp (new DroidPlatform()));
}
```

### Create your first view and view model

####Viewmodel:

```csharp
public class CompanyViewModel: BaseViewModel
{
}
```

####View:

```csharp
public class CompanyView: BaseContentsView<CompanyViewModel>
{
	protected override View CreateContents ()
	{
		return new Label { Text = "Hello World" };
	}
}
```

### Register your views and startup page in your app class

```csharp
public class DemoMvvmApp: MvvmApp
{
	public DemoMvvmApp (IMvvmPlatform platform): base(platform)
	{
	}
	
	protected override void RegisterViews ()
	{
		ViewContainer.RegisterView<CompanyViewModel, CompanyView> ();
	}

	protected override Xamarin.Forms.Page GetMainPage ()
	{
		return new NavigationPage(Container.Resolve<CompanyView> ());
	}
}
```

> Note: You don't need to wrap your main page into a NavigationPage, but it makes navigation easier.

Your app should now be ready to run, and you should be ready to read more about how to bind the different parts of the app together.

### Dependency Injection
To ensure loose coupling between the different parts of the application and testability all setup and dependencies between the different objects in the app is set up using a dependency injection library. 

In the NControl.Mvvm library none of the views or view models are constructed using the ```new``` operator - they are automatically instantiated by the injection container using the ```Resolve<>``` method. 

### Navigation
Navigation in an Mvvm app should always be from one ViewModel to another ViewModel. The type of navigation (modal, push, popup) should only be given as indications in the code and it should be the presenter's responsibility to present views in a consistent way on screen.

A presenter interface has been defined, and a default implementation using a combination of NControl.Controls and Xamarin.Forms' own Navigation system is provided in the ```DefaultPresenter``` class which can be overriden.

### BaseModel
The ```BaseModel``` class is used as a base class for objects that should be able to participate in data binding. This means both view models and models used as carriers for the view model. Often a service or rest client returns simple objects that does not implement property notification and you might want your view model to transform the data into a more complex object that can participate in data binding.

#### Properties
One of the most boring tasks in building an Mvvm app is defining bindable properties with fields for storage. A base class that has built in support for storage is provided, and the syntax for defining a property is really simple:

```csharp
public string MyProperty
{
	get { return GetValue<string>(); }
	set { SetValue<string>(value); }
}
```

Providing default values can be done with this ```GetValue<>()``` override:

```GetValue<string>(()=> "My Default Value")```

Backing fields has been replaced with a dictionary.


### ViewModels
ViewModels serves as the transformation layer in an Mvvm app, and should take data from services and clients (again reading from databases or api endpoints) and transform these so that they can be properly bound to one or more UI elements in a view.

NControl.Mvvm contains base classes for view models that is easy to work with and easy to extend.

#### Construction of view models
Declare all dependencies as parameters in the constructor and save them in private readonly fields in the class. This is not up for discussion. The dependency container will take care of resolving dependencies.

```csharp
public class MyViewModel: BaseViewModel
{
	private readonly ILoggingService _loggingService;
	
	public MyViewModel(ILoggingService loggingService)
	{
		_loggingService = loggingService;
	}
}
```

#### Initialization
Initialization in a view model often requires performing async calls to service methods. Do not call any async methods in the constructor. This is why the ```BaseViewModel``` has built in methods for performing async initialization:

```csharp
public class MyViewModel: BaseViewModel
{
	... constructor and private fields ...
	
	public override async Task InitializeAsync ()
	{
		await base.InitializeAsync();
		
		await LoadDataAndPopulatePropertiesAsync();
	}
	
}
```

If a view model requires a parameter to initialize itself make sure you derrive your view models from the generic base class to get type safe initalization and navigation:

```csharp
public class MyViewModel: BaseViewModel<MyParameterType>
{
	... constructor and private fields ...
	
	public override async Task InitializeAsync (MyParameterType parameter)
	{
		await base.InitializeAsync();
		
		await LoadDataAndPopulatePropertiesAsync(parameter);
	}	
}
```

#### Commands
Another boring task is defining commands with state in our view models. This has been replaced with the convinient family of ```GetOrCreateCommand``` functions:

```csharp
public Command MyCommand
{
	get { 
		return GetOrCreateCommand(async ()=> {
			await DoSomethingThatTakesSomeTime();
		});
	}
}
```

This method also exists as a generic function:

```GetOrCreateCommand<CommandParameterType>(async (parameter)=> {})```

The method even has an overload that takes a delegate that returns the state of the command:

```csharp
GetOrCreateCommand(async ()=>{
		... do something ...
	}, 
	()=> GetSomethingIsTrueOrFalse())
```

#### DependsOnAttribute
Properties can be decorated with the ```DependsOn``` attribute that automatically raises the correct property notifications. The attribute works with both regular properties and command properties. When used on command properties they will raise the ChangeCanExecute method on the command updating UI that is bound to the command. 

> Note: Command dependency is implemented in the ```BaseViewModel``` class and not in the ```BaseModel``` - a model does not have commands.

A quick example of how to use the dependency:

```csharp
public string Username
{
	get { return GetValue<string>(); }
	set { SetValue<string>(value); }
}

public string Password
{
	get { return GetValue<string>(); }
	set { SetValue<string>(value); }
}

[DependsOn(nameof(Username), nameof(Password))]
public Command LoginCommand
{
	get {
		return GetOrCreateCommand(async ()=>{
			await LoginService.AuthenticateAsync(Username, Password);
		},
		()=> !string.IsNullOrEmpty(Username) && 
			!string.IsNullOrEmpty(Password));
	}
}
```

#### ExecuteOnChangeAttribute
In the previous example you saw how we can use attributes to create dependencies in your view model. If you want to execute a task whenever a property changes, you can do this by creating a command and decorating it with the ```ExecuteOnChange``` attribute:

```csharp
public string Query
{
	get { return GetValue<string>(); }
	set { SetValue<string>(value); }
}

[ExecuteOnChange(nameof(Query))]
public Command SearchCommand
{
	get {
		return GetOrCreateCommand(async ()=>{

			... perform search and update the ui ...

		});
	}
}
```

### Views
Views can be declared in Xaml (and now that we have the new compile option for our Xaml we even get some sort of type checking) but I prefer to build my user interfaces in a fluent way using code.

> Note: By describing the user interface in code with as few lines as possible we are forced to think about our views in a more conceptual way. I always try to hide code that talks about pixels and colors in separate control classes that can easily be reused in my views.

#### Binding
The most important part of building the UI is its bindings to the view model. NControl.Mvvm contains a few extensions that makes this a simple task as well (remember that I don't like to repeat myself. Whops..)

Look at this example that sets up a list view (using the included ListViewEx class that exposes the item selected event as a command) and binds it to a view model with a list of companies:

```csharp
return new StackLayout {
	Orientation = StackOrientation.Vertical,
		Children = {					
		new ListViewEx{
			ItemsSource = ViewModel.Companies,
			ItemSelectedCommand = ViewModel.SelectCompanyCommand,
			ItemTemplate = new DataTemplate(typeof(TextCell))
				.BindTo(TextCell.TextProperty, NameOf<Company>(cw => cw.Name))
			}
		}
	};
```

The only special stuff here is the ```BindTo``` extension method which exists for both DataTemplates and other controls like labels and textfields. 

In addition the library contains a simple ```NameOf``` method in the base view classes that can be used to make binding type safe. This method uses the view model as its default generic parameter, but can also be used for other types like the items in an enumerable property.