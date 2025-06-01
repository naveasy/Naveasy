# Naveasy
It it works with:
- .NET MAUI
- Microsoft.Extensions.DependencyInjection
- MAUI NavigatonPage.
- MAUI FlyoutPage (check the samples to see how it works).

[![Build](https://github.com/naveasy/Naveasy/actions/workflows/CI.yml/badge.svg)](https://github.com/naveasy/Naveasy/actions/workflows/CI.yml)

## Version 3 has breaking changes
- [View Release Notes](./ReleaseNotes.md)

## Get Started

### 1) Install it from nuget.org [![Static Badge](https://img.shields.io/badge/Naveasy-%20nuget.org-%20%23097ABB?link=https%3A%2F%2Fwww.nuget.org%2Fpackages%2FNaveasy)](https://www.nuget.org/packages/Naveasy)


### 2) Add the following namespace to your MauiProgram.cs:  
```using Naveasy;```

### 3) Create a new `ContentPage` and it's corresponding `ViewModel`
   - Inside this `ViewModel` ask for an instance of `INavigationService` on it's `class` contructor and store it in a private field;
   - Make this `PageViewModel` to implemnt `Naveasy.IPageLifecycleAware`
   - Inside the `void OnAppearing()` method of this VM use the `INavigationService` instance that you've got to navigate to other page of your choice.
   - Tip.: You car you this `StartupPageViewMode` to implement custom logic like quering you web API's or checking credential and ect conditionaly navigate to login page or another page if the user is alrealy logged-in.
   - in example bellow I've named it `StartupPage` & `StartupPageViewMode`);

### 4) Configure Naveasy on your MauiProgram.cs
On call the generic method `.UseNaveasy<TViewModel>();` with the type of your `StartupPageViewModel`.
    - Pay attention that you should specify here the Type of your startup page `ViewModel` NOT the `Page` ok cause Naveasy is a `ViewModel` to `ViewModel` navigation framework.
    - The same concept explained abote must be followed when you'll use the `INavigationService` on your other Pages, use the `ViewModel` type rather the the `Page` type to perform navigation. 
    - Register your `Page` and it's corresponding `PageViewModel` on `builder.Services` using Navaeasy's `.AddTransientForNavigation<TPage, TPageViewModel>` like described bellow.
```csharp
using Naveasy.Core;

namespace Naveasy.Samples;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder()
            .UseMauiApp<App>()
            .UseNaveasy<StartupPageViewModel>() //The generic type specified in builder.UseNaveasy<T>()
                                               //will be used to create a new window and navigate to it StartupPageViewModel

            //Register your types here using Microsoft.Extensions.DependecyInjection's container
            .AddTransientForNavigation<StartupPage, StartupPageViewModel>()
            .AddTransientForNavigation<LoginPage, LoginPageViewModel>();

            .ConfigureFonts(fonts =>
            { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });
        return builder.Build();
    }
}
```

# IMPORTANT
6) You should not `override CreateWindow()` on your `App.xaml.cs`;
    
- Naveasy v3 already does to it for internally because we need to hook up our own events to make Naveasy work properly.
- If you the the `protected override Window CreateWindow(IActivationState? activationState)` method on your App.xaml.cs, go there and remove it.
    - any custom logic that you might already have the if you were using Naveasy versions older then v3, move your custom logic the `ViewModel` created on step `#3` of this documentation.
- Your App.xaml.cs file can be as clean as in the example bellow so you'll probably never look back to it to bootstrap your application.

```csharp
public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }
}
```

## Optional
You can refer to the sample that we have here on this repo to have a more in depth understanding of how you can navigate from one page to another and also how to handle the various page lifecycle events.
You can optionaly implement the following handy base class which provides the various page lifecicle events that you would care about:

```csharp
public class ViewModelBase : BindableBase, IInitialize, IInitializeAsync, INavigatedAware, IDisposable
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

    public virtual void Dispose()
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
