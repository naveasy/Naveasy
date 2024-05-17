# Naveasy
It it works with:
- .NET MAUI
- Microsoft.Extensions.DependencyInjection
- MAUI NavigatonPage.
- MAUI FlyoutPage (check the samples to see how it works).

[![Build](https://github.com/naveasy/Naveasy/actions/workflows/CI.yml/badge.svg)](https://github.com/naveasy/Naveasy/actions/workflows/CI.yml)

## Get Started

1) Install it from nuget.org [![Static Badge](https://img.shields.io/badge/Naveasy-%20nuget.org-%20%23097ABB?link=https%3A%2F%2Fwww.nuget.org%2Fpackages%2FNaveasy)](https://www.nuget.org/packages/Naveasy)


2) Add the following namespace to your MauiProgram.cs:  
```using Naveasy;```

3) Call `.UseNaveasy()` on your AppBuilder and then register your Views with it's corresponding ViewModels the your Service Collection by calling `AddTransientForNavigation` like described bellow.
```csharp
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
            
        builder.UseMauiApp<App>()
               .UseNaveasy();

        builder.Services
            .AddTransientForNavigation<LoginPage, LoginPageViewModel>()
            .AddTransientForNavigation<HomePage, HomePageViewModel>()
            .AddTransientForNavigation<ProductsPage, ProductsPageViewModel>()
            .AddTransientForNavigation<DetailsPage, DetailsPageViewModel>();

        return builder.Build();
    }
}
```

4) Change your App.xaml.cs to assign a new NavigationPage to the MainPage and then use the `NavigationService` to navigate to the page you want's to.

```csharp
public partial class App : Application
{
    public App(INavigationService navigationService)
    {
        InitializeComponent();

        //If you wanna navigate to page wraping it into a MAUI's NavigationPage use the Naveasy's generice interface
        //INavgiationPage<YourPageViewModel> like in the example bellow.
        navigationService.NavigateAsync<INavgiationPage<LoginPageViewModel>>();

        //otherwise use it like this:
        //navigationService.NavigateAsync<LoginPageViewModel>();
    }
}
```

You can refer to the sample that we have here on this repo to have a more in depth understanding of how you can navigate from one page to another and also how to handle the various page lifecycle events.
You can optionaly implement the following handy base class wich provides the various page lifecicle events that you would care about:

```csharp
public class ViewModelBase : BindableBase, IInitialize, IInitializeAsync, INavigatedAware, IDestructible
{
    public virtual void OnInitialize(INavigationParameters parameters)
    {
    }

    public virtual Task OnInitializeAsync(INavigationParameters parameters)
    {
        return Task.CompletedTask;
    }

    public virtual void OnNavigatedFrom(INavigationParameters navigationParameters)
    {
    }

    public virtual void OnNavigatedTo(INavigationParameters navigationParameters)
    {
    }

    public virtual void Destroy()
    {
    }
}
```

Fell free to contribute.

There's No support for MAUI AppShell until MSFT trully fixes the following issues:
https://github.com/dotnet/maui/issues/7354
https://github.com/dotnet/maui/issues/21814
https://github.com/dotnet/maui/issues/21816

This library was inpired on the Dotnet Foundation version of [PRISM](https://github.com/PrismLibrary/Prism/releases/tag/DNF).
