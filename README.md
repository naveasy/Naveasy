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

1) Install it from nuget.org [![Static Badge](https://img.shields.io/badge/Naveasy-%20nuget.org-%20%23097ABB?link=https%3A%2F%2Fwww.nuget.org%2Fpackages%2FNaveasy)](https://www.nuget.org/packages/Naveasy)


2) Add the following namespace to your MauiProgram.cs:  
```using Naveasy;```

3) On your MauiAppBuilder add the invoke `.UseNaveasy<YourStartup_Page_VIEW_MODEL>();`
4) Register your `Page` and it's corresponding `PageViewModel` on `builder.Services.AddTransientForNavigation<Page, PageViewModel>` like described bellow.
```csharp
using Microsoft.Extensions.Logging;
using Naveasy.Core;

namespace Naveasy.Samples;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseNaveasy<SplashPageViewModel>() //The generic type specified in builder.UseNaveasy<T>()
                                               //will be used to create a new window and navigate to it SplashPageViewModel
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services
            //Register your types here using Microsoft.Extensions.DependecyInjection's container
            .AddTransientForNavigation<SplashPage, SplashPageViewModel>()
            .AddTransientForNavigation<LoginPage, LoginPageViewModel>();

        #if DEBUG
            builder.Logging
                .SetMinimumLevel(LogLevel.Trace)
                .AddDebug();
        #endif

        return builder.Build();
    }
}
```

4) Do NOT override the `CreateWindow()` on your `App.xaml.cs`
    `protected override Window CreateWindow(IActivationState? activationState)`
- Naveasy v3 already does to it for you during the startup navigation phase.
  Your App.xaml.cs file can be as clean as in the example bellow so you'll probably never look back to it to bootstrap your application.

```csharp
public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }
}
```

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
