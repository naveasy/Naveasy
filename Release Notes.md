# Breaking changes on v3

## 1) the interface IDestructible has been removed
- replace the old `IDestructible` with the built-in `System.IDisposable` and Naveasy will call Dispose for your ViewModel when the page get's popped from the navigation stack.

## 2) the interface INavigationPage has been removed
- The interface `INavigationPage` has been removed because it's no longer needed. Naveasy will automatically create one for you when needed.

## 3) .UseMauiApp<T>() is now generic
Now when involking `.UseMauiApp<App>()` on your `MauiAppBuilder` you'll have to specify the VM type of the first page you want to navigate to.
```csharp
var builder = MauiApp.CreateBuilder();
    builder.UseMauiApp<App>()
           .UseNaveasy<SplashPageViewModel>(); // The ViewModel Type specified here will be use to
                                              //create a window and navigate immediately to it.
```

 Bellow it's a more complete example of how your Program.cs should look like:
```csharp
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseNaveasy<SplashPageViewModel>() // in this example SplashPageViewModel represents your initial Page+VM
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            }); 

        //Register your types here using dotnet default's DI framework
        builder.Services.AddTransientForNavigation<SplashPage, SplashPageViewModel>()
                        .AddTransientForNavigation<MyFlyoutMenuPage, MyFlyoutMenuPageViewModel>()
                        .AddTransientForNavigation<MyDetailsPage, MyDetailsPageViewModel>();
        return builder.Build();
    }
}
```

## 4) You should NOT override CreateWindow() on App.xaml.cs
- On Naveasy **v2** we where suggesting you to implement your navigation inside your App.xaml.cs like in the example bellow:

```csharp
public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var serviceProvider = activationState!.Context.Services;
        var accessTokenProvider = serviceProvider.GetRequiredService<IAccessTokenProvider>();
        var navigationService = serviceProvider.GetRequiredService<INavigationService>();
        var authenticationService = serviceProvider.GetRequiredService<IAuthenticationService>();

        authenticationService.ObserveLogout
            .ThrottleFirst(TimeSpan.FromSeconds(1))
            .ObserveOnUIDispatcher()
            .Subscribe(_ =>
            {
                navigationService.NavigateAbsoluteAsync<INavigationPage<LoginPageViewModel>>();
            });

        accessTokenProvider.Initialize()
            .ObserveOnUIDispatcher()
            .Subscribe(_ =>
            {
                if (accessTokenProvider.Token == null)
                {
                    navigationService.NavigateAsync<LoginPageViewModel>();
                }
                else
                {
                    navigationService.NavigateFlyoutAbsoluteAsync<MyFlyoutMenuPageViewModel, INavigationPage<MyDetailsPageViewModel>>();
                }
            });

        return new Window(new NavigationPage(new SplashPage()));
    }
}
```
Now you should not override `protected override Window CreateWindow(IActivationState? activationState)` on `App.xaml.cs` like in the example above that was using the **old** Naveasy v2.

Now instead you should completely remove the `CreateWindow(...)` method and move tha entire logic it had inside to your initialization PageViewModel, in this example `SplashPageViewModel`
### Your App.xaml.cs should have almost nothing, like in the example bellow:

```csharp
public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }
}
```

Your startup page view model (i.e: `SplashPageViewModel` in this example) should implement Naveasy's interface `IPageLifecycleAware` and then you'll be able to execute that same logic that was previously on your `App.xaml.cs` the OnAppearing you of your startup page VM (i.e: `SplashPageViewModel` in this example).
Just like in bollow's example:
```csharp
public class SplashPageViewModel(IAccessTokenProvider accessTokenProvider,
                                 IAuthenticationService authenticationService,
                                 INavigationService navigationService)
                                 : ViewModelBase, IPageLifecycleAware
{
    public void OnAppearing()
    {
        authenticationService.ObserveLogout
            .ThrottleFirst(TimeSpan.FromSeconds(1))
            .ObserveOnUIDispatcher()
            .Subscribe(_ =>
            {
                navigationService.NavigateAbsoluteAsync<LoginPageViewModel>();
            });

        accessTokenProvider.Initialize()
            .ObserveOnUIDispatcher()
            .Subscribe(_ =>
            {
                if (accessTokenProvider.Token == null)
                {
                    navigationService.NavigateAsync<LoginPageViewModel>();
                }
                else
                {
                    navigationService.NavigateFlyoutAbsoluteAsync<MyFlyoutMenuPageViewModel, MyDetailsPageViewModel>();
                }
            });
    }

    public void OnDisappearing()
    { }
}
```




  
