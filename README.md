# Naveasy

[![Build](https://github.com/naveasy/Naveasy/actions/workflows/CI.yml/badge.svg)](https://github.com/naveasy/Naveasy/actions/workflows/CI.yml)
[![NuGet](https://img.shields.io/badge/Naveasy-%20nuget.org-%20%23097ABB?link=https%3A%2F%2Fwww.nuget.org%2Fpackages%2FNaveasy)](https://www.nuget.org/packages/Naveasy)


**Naveasy** is a lightweight, opinionated **ViewModel-to-ViewModel** navigation framework for **.NET MAUI**, built on top of `Microsoft.Extensions.DependencyInjection`. It removes the boilerplate around `NavigationPage`, `FlyoutPage`, page lifecycle, scoped DI, and typed navigation parameters so your app code stays in the ViewModel layer where it belongs.

It works with:
- .NET MAUI (currently targeting `net10.0` and the MAUI platform TFMs).
- `Microsoft.Extensions.DependencyInjection`.
- MAUI `NavigationPage`.
- MAUI `FlyoutPage` (see the included sample to see how it works).

> **No MAUI AppShell support.** Naveasy will not add Shell support until Microsoft fixes the issues listed at the bottom of this document.


---

## Table of contents

1. [Why Naveasy](#why-naveasy)
2. [Installation](#installation)
3. [Quick start](#quick-start)
4. [Registering pages](#registering-pages-for-navigation)
5. [The `INavigationService` API](#the-inavigationservice-api)
6. [Passing navigation parameters](#passing-navigation-parameters)
7. [Page / ViewModel lifecycle](#page--viewmodel-lifecycle)
8. [Detecting forward vs. back navigation](#detecting-forward-vs-back-navigation)
9. [Returning data on back navigation](#returning-data-on-back-navigation)
10. [`FlyoutPage` support](#flyoutpage-support)
11. [Page dialogs (`IPageDialogService`)](#page-dialogs-ipagedialogservice)
12. [`BindableBase` for ViewModels](#bindablebase-for-viewmodels)
13. [Scoped vs. transient pages](#scoped-vs-transient-pages)
14. [View / ViewModel naming convention](#view--viewmodel-naming-convention)
15. [`App.xaml.cs` — what NOT to do](#appxamlcs--what-not-to-do)
16. [Sample app](#sample-app)
17. [Known limitations](#known-limitations)
18. [Release Notes](./ReleaseNotes.md)

---

## Why Naveasy

Naveasy gives you:

- **Strongly-typed navigation** — you navigate by ViewModel type, never by string keys or page paths.
- **Convention-based view resolution** — `FooPageViewModel` is automatically matched with `FooPage` in the same assembly.
- **Full page-lifecycle pipeline** — `IInitialize`, `IInitializeAsync`, `INavigatedAware`, `IPageLifecycleAware`, and `IDisposable` are called automatically.
- **Automatic `NavigationPage` wrapping** — you never have to wrap pages manually.
- **First-class `FlyoutPage` support** with helpers to switch detail pages and toggle the flyout from a ViewModel.
- **Per-page DI scopes** — every navigated page gets its own `IServiceScope` (great for `DbContext`, per-page caches, etc.).
- **Hardware back button handling** on Android, including invoking `OnNavigatedTo` / `OnNavigatedFrom` on the right ViewModels.

---

## Installation

Install from [nuget.org](https://www.nuget.org/packages/Naveasy):

```powershell
dotnet add package Naveasy
```

Then add this `using` to your `MauiProgram.cs`:

```csharp
using Naveasy;
using Naveasy.Core;
```

---

## Quick start

### 1) Create a startup ViewModel

This is the first ViewModel Naveasy will navigate to when your app starts. It is a perfect place to put one-time logic such as: bootstrapping your services, checking credentials, calling your web API, and then deciding whether to navigate to a login page or to the main shell of your application.

> Naveasy is a **ViewModel-to-ViewModel** navigation framework. You always specify the **ViewModel** type when you navigate, never the page type.

```csharp
using Naveasy;

namespace MyApp.Views.Startup;

public class StartupPageViewModel : BindableBase, IPageLifecycleAware
{
    private readonly INavigationService _navigationService;

    public StartupPageViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    public void OnAppearing()
    {
        // Run any startup logic here (auth check, API ping, migrations, etc.)
        // then navigate to the next page absolutely (replacing the root).
        _ = _navigationService.NavigateAbsoluteAsync<LoginPageViewModel>();
    }

    public void OnDisappearing() { }
}
```

And its matching `StartupPage` (an empty `ContentPage` works fine — it is just a placeholder while the startup logic runs):

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="MyApp.Views.Startup.StartupPage" />
```

### 2) Configure Naveasy in `MauiProgram.cs`

Call `.UseNaveasy<TStartupViewModel>()` on the `MauiAppBuilder`. The generic argument is the **ViewModel** of the page you want to show first — Naveasy will create the `Window`, resolve the matching page, and wire everything up for you.

Register every page/ViewModel pair using `.AddTransientForNavigation<TPage, TPageViewModel>()` (or `.AddScopedForNavigation<…>()`):

```csharp
using Naveasy;
using Naveasy.Core;

namespace MyApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder()
            .UseMauiApp<App>()
            // The generic type (StartupPageViewModel) will be used to create a new window
            // and navigate to the corresponding page (StartupPage).
            .UseNaveasy<StartupPageViewModel>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services
            .AddTransientForNavigation<StartupPage, StartupPageViewModel>()
            .AddTransientForNavigation<LoginPage,   LoginPageViewModel>()
            .AddTransientForNavigation<HomePage,    HomePageViewModel>();

        return builder.Build();
    }
}
```

That's it — your app is now running on Naveasy.

---

## Registering pages for navigation

Two extension methods on `IServiceCollection` register both the page and the ViewModel **and** map them together inside Naveasy's internal `PageRegistry`:

```csharp
public static IServiceCollection AddTransientForNavigation<TView, TViewModel>(this IServiceCollection self)
    where TView : IView;

public static IServiceCollection AddScopedForNavigation<TView, TViewModel>(this IServiceCollection self)
    where TView : IView;
```

Rules:

- Every page must be registered. Registering only the ViewModel is **not** enough — Naveasy needs to know how to resolve the matching page type for that ViewModel.
- Registering the same ViewModel type twice throws `ArgumentException`. Each `TViewModel` maps to exactly one `TView`.
- You don't need to manually `AddTransient<TView>()` or `AddTransient<TViewModel>()` — the helper does it for you.

---

## The `INavigationService` API

Inject `INavigationService` into any ViewModel via constructor injection.

```csharp
public interface INavigationService
{
    bool IsFlyoutOpen { get; set; }

    Task<INavigationResult> GoBackAsync(INavigationParameters parameters = null, bool? animated = null);
    Task<INavigationResult> GoBackToRootAsync(INavigationParameters parameters = null, bool? animated = null);

    Task<INavigationResult> NavigateAsync<TViewModel>(INavigationParameters parameters = null, bool? animated = null);
    Task<INavigationResult> NavigateAndPopPreviousAsync<TViewModel>(INavigationParameters parameters = null, bool? animated = null);
    Task<INavigationResult> NavigateAbsoluteAsync<TViewModel>(INavigationParameters parameters = null, bool? animated = null);

    Task<INavigationResult> NavigateFlyoutAbsoluteAsync<TFlyoutViewModel, TDetailViewModel>(
        INavigationParameters flyoutParameters = null,
        INavigationParameters detailParameters = null,
        bool? animated = null);

    Task<INavigationResult> NavigateFlyoutDetailAbsoluteAsync<TDetailViewModel>(
        INavigationParameters detailParameters = null,
        bool? animated = null);
}
```

### What each method does

| Method | Behavior |
| --- | --- |
| `NavigateAsync<TVm>()` | Pushes a new page on top of the current `NavigationPage` stack. If the app has no `NavigationPage` yet, Naveasy creates one for you. |
| `NavigateAndPopPreviousAsync<TVm>()` | Pushes a new page **and removes the previous page** from the stack — the previous page is destroyed (its `IDisposable.Dispose()` runs) and the user cannot navigate back to it. Useful for "next step" wizards (e.g. `EnterPhone → EnterCode → Done`) or for replacing a transitional page (loading / OTP / EULA) with the next real screen. |
| `NavigateAbsoluteAsync<TVm>()` | Replaces the current root with a fresh page (and a fresh `NavigationPage` if needed). Destroys all pages currently on the stack. This is what you typically use for `Login → MainApp` or `SignOut → Login` transitions. |
| `NavigateFlyoutAbsoluteAsync<TFlyoutVm, TDetailVm>()` | Replaces the root with a `FlyoutPage` whose Flyout is `TFlyoutVm` and whose Detail is `TDetailVm`. |
| `NavigateFlyoutDetailAbsoluteAsync<TDetailVm>()` | When the app's root is already a `FlyoutPage`, replaces only the **Detail** with a fresh navigation rooted on `TDetailVm`. Existing detail pages are destroyed. |
| `GoBackAsync()` | Pops one page from the current navigation stack. |
| `GoBackToRootAsync()` | Pops everything back to the root page on the current navigation stack. |
| `IsFlyoutOpen` | Reads / sets `FlyoutPage.IsPresented` when the root is a `FlyoutPage`. Returns `false` (and is a no-op when set) for non-flyout roots. |

### `INavigationResult`

Every `INavigationService` call returns:

```csharp
public interface INavigationResult
{
    bool Success { get; }
    Exception Exception { get; }
}
```

Naveasy **does not throw** on navigation errors — exceptions are captured in the result. Check `result.Success` and inspect `result.Exception` when needed:

```csharp
var result = await _navigationService.NavigateAsync<DetailsPageViewModel>();
if (!result.Success)
{
    _logger.LogError(result.Exception, "Navigation to DetailsPage failed");
}
```

> Every overload accepts an optional `animated` argument. Passing `null` (the default) lets MAUI use its own platform default, `true` forces an animated transition, and `false` disables it (useful for fast unit-test-driven flows or for `Splash → Login` transitions where you don't want a slide animation).

---

## Passing navigation parameters

Naveasy uses `INavigationParameters` (a typed `IDictionary<string, object>`) to pass data between ViewModels. You almost never have to deal with raw string keys — use the included extensions to pass strongly-typed objects.

### Building parameters

```csharp
var client  = new ClientModel(id: 1, name: "Contributor User");
var product = new ProductModel(id: 2, name: "Windows Phone 11");

// Build a parameter set keyed by full type name:
var parameters = client.ToNavigationParameter()
                       .Including(product);

await _navigationService.NavigateAsync<DetailsPageViewModel>(parameters);
```

> ⚠️ Each parameter is keyed by the runtime type's `FullName`. **Do not** add two parameters of the exact same type — the second `Add` will throw. If you need multiple values of the same type, wrap them in a small composite type.

If you want to send a value under a base-class or interface key (so the receiver can call `GetValue<TBase>()`), pass the desired `Type` explicitly:

```csharp
IShippingMethod ground = new GroundShipping(...);

// Stored under typeof(IShippingMethod).FullName, not GroundShipping's full name.
var parameters = ground.ToNavigationParameter(typeof(IShippingMethod));
// Or, when chaining:
parameters = new ClientModel(...).ToNavigationParameter()
                                 .Including(ground, typeof(IShippingMethod));
```

### Reading parameters in the next ViewModel

`INavigationParameters` exposes two type-safe overloads of `GetValue<T>`:

```csharp
public override void OnInitialize(INavigationParameters parameters)
{
    // No key needed — looks up by typeof(T).FullName,
    // which is what ToNavigationParameter()/Including() use.
    Client  = parameters.GetValue<ClientModel>();
    Product = parameters.GetValue<ProductModel>();

    // Or by explicit key if you added entries manually:
    var token = parameters.GetValue<string>("AuthToken");
}
```

When a key isn't found, `GetValue<T>()` returns `default(T)` — so reference types come back as `null` and value types come back as `0` / `false` / etc. Always treat parameters as optional unless you control both ends of the call.

If you'd rather use string keys, just treat `NavigationParameters` like the dictionary it is:

```csharp
var p = new NavigationParameters { { "id", 42 }, { "tab", "Overview" } };
await _navigationService.NavigateAsync<DetailsPageViewModel>(p);
```

Naveasy will also perform best-effort conversions when the parameter's runtime type doesn't match the requested type — `enum` parsing from name **or** integer value, and any conversion supported by `IConvertible` (e.g. `"42" → int`, `42 → string`).

---

## Page / ViewModel lifecycle

When you navigate to a ViewModel, Naveasy walks a well-defined lifecycle pipeline. Each hook is exposed as a small, single-purpose interface — implement only the ones you actually need on a given ViewModel.

| Interface | Method | When it fires |
| --- | --- | --- |
| `IInitialize` | `void OnInitialize(INavigationParameters)` | **Once**, the first time the page is created, just before navigation. Ideal place to read typed navigation parameters and populate the ViewModel. |
| `IInitializeAsync` | `Task OnInitializeAsync(INavigationParameters)` | **Once**, the first time the page is created (async variant). Awaited before the page is pushed — use it for I/O-bound bootstrap work (loading from disk, calling a web API, etc.). |
| `INavigatedAware` | `void OnNavigatedTo(INavigationParameters)` | Every time the page becomes the active page (forward navigation **and** when a page on top is popped). |
| `INavigatedAware` | `void OnNavigatedFrom(INavigationParameters)` | Every time the page leaves the active position (forward and back). |
| `IPageLifecycleAware` | `void OnAppearing()` / `void OnDisappearing()` | Mirrors MAUI's `Page.Appearing` / `Page.Disappearing` events on the ViewModel. |
| `IDisposable` | `void Dispose()` | Called when the page is destroyed (popped and not coming back, replaced by an absolute navigation, or pushed-over by `NavigateAndPopPreviousAsync`). The per-page `IServiceScope` is disposed at the same time, so every `Scoped` service of that page is released. |

### Forward navigation: exact order of calls

When `NavigateAsync<TVm>()` (or any other forward-navigation method) runs against a freshly created destination page, Naveasy executes the hooks in this exact order:

1. Destination → `IInitialize.OnInitialize(parameters)` *(sync, first creation only)*
2. Destination → `await IInitializeAsync.OnInitializeAsync(parameters)` *(first creation only)*
3. The page is pushed onto the navigation stack.
4. Source (leaving page) → `INavigatedAware.OnNavigatedFrom(parameters)`
5. Internal `NavigationMode.New` is added to `parameters`.
6. Destination → `INavigatedAware.OnNavigatedTo(parameters)`
7. MAUI fires `Appearing` on the destination → `IPageLifecycleAware.OnAppearing()` runs.

### Back navigation: exact order of calls

When the user goes back — either via `GoBackAsync()`, `GoBackToRootAsync()`, the system back gesture, or the Android hardware back button — Naveasy runs:

1. Internal `NavigationMode.Back` is added to `parameters`.
2. Leaving (popped) page → `INavigatedAware.OnNavigatedFrom(parameters)`
3. Revealed (previous) page → `INavigatedAware.OnNavigatedTo(parameters)`
4. Leaving page → `IDisposable.Dispose()` and its per-page DI scope is disposed.

Note that **`IInitialize` / `IInitializeAsync` do NOT run on the revealed page** — they only run once per page instance, on creation.

### Deprecated lifecycle interfaces

For backward compatibility Naveasy still ships two **obsolete** interfaces:

- `IInitialized` — replaced by `IInitialize`.
- `IInitializedAsync` — replaced by `IInitializeAsync`.

They will be removed in a future major version. Migration is a straight rename:

| Old (obsolete)                              | New                                         |
| ------------------------------------------- | ------------------------------------------- |
| `IInitialized.OnInitialized(parameters)`    | `IInitialize.OnInitialize(parameters)`      |
| `IInitializedAsync.OnInitializedAsync(...)` | `IInitializeAsync.OnInitializeAsync(...)`   |

### A reusable `ViewModelBase`

Because most ViewModels want most of those hooks, the recommended pattern is a base class like the one used in the sample:

```csharp
using Microsoft.Extensions.Logging;
using Naveasy;

public class ViewModelBase : BindableBase, IInitialize, IInitializeAsync, INavigatedAware, IDisposable
{
    private string? _title;
    public string? Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public static ILogger Logger { get; set; } = null!;

    public virtual void OnInitialize(INavigationParameters parameters)             { }
    public virtual Task OnInitializeAsync(INavigationParameters parameters)        => Task.CompletedTask;
    public virtual void OnNavigatedTo(INavigationParameters navigationParameters)  { }
    public virtual void OnNavigatedFrom(INavigationParameters navigationParameters){ }
    public virtual void Dispose()                                                  { }
}
```

Add `IPageLifecycleAware` on individual ViewModels when you actually need `OnAppearing` / `OnDisappearing`.

---

## Detecting forward vs. back navigation

`INavigationParameters` carries an internal `NavigationMode` that tells you whether the page was reached by a forward push or by a back pop. Read it from `OnNavigatedTo` / `OnNavigatedFrom`:

```csharp
using Naveasy.Core;
using Naveasy.Extensions;

public override void OnNavigatedTo(INavigationParameters parameters)
{
    switch (parameters.GetNavigationMode())
    {
        case NavigationMode.New:
            // Came from a NavigateAsync / NavigateAbsoluteAsync call
            break;
        case NavigationMode.Back:
            // Reached after a GoBackAsync / GoBackToRootAsync / hardware back
            break;
    }
}
```

`GetNavigationMode()` throws `ArgumentNullException` if it is called from a place that did **not** receive navigation parameters from Naveasy (for example, a hand-rolled `new NavigationParameters()` you built yourself). In `OnNavigatedTo` / `OnNavigatedFrom`, however, the navigation mode is always set by the framework.

---

## Returning data on back navigation

`GoBackAsync` and `GoBackToRootAsync` both accept an `INavigationParameters` argument that is forwarded to the revealed page's `OnNavigatedTo`. This is the recommended way to send a result back to the previous ViewModel — no events, no `TaskCompletionSource`, no static state.

Sending the result from the closing page:

```csharp
private async Task ConfirmAsync()
{
    var selection = new ProductModel(id: 7, name: "Selected item");

    var result = selection.ToNavigationParameter();
    await _navigationService.GoBackAsync(result);
}
```

Reading the result on the page that is revealed:

```csharp
public override void OnNavigatedTo(INavigationParameters parameters)
{
    base.OnNavigatedTo(parameters);

    if (parameters.GetNavigationMode() == NavigationMode.Back)
    {
        var picked = parameters.GetValue<ProductModel>();
        if (picked is not null)
            SelectedProduct = picked;
    }
}
```

---

## `FlyoutPage` support

Naveasy fully supports MAUI's `FlyoutPage`. Just declare your flyout page as a normal MAUI `FlyoutPage` in XAML and register the pair:

```csharp
builder.Services
    .AddTransientForNavigation<MyFlyoutPage,  MyFlyoutPageViewModel>()
    .AddTransientForNavigation<FeaturePageA,  FeaturePageAViewModel>()
    .AddTransientForNavigation<FeaturePageB,  FeaturePageBViewModel>();
```

Bootstrapping a `FlyoutPage` as the root of your app (typical post-login flow):

```csharp
await _navigationService.NavigateFlyoutAbsoluteAsync<MyFlyoutPageViewModel, FeaturePageAViewModel>();
```

Switching the detail page from inside the flyout's ViewModel:

```csharp
public class MyFlyoutPageViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    public MyFlyoutPageViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        NavigateCommand    = new Command<string>(DoNavigate);
        CloseFlyoutCommand = new Command(() => IsFlyoutPresented = false);
    }

    public bool IsFlyoutPresented { get; set; } // bound TwoWay to FlyoutPage.IsPresented
    public ICommand NavigateCommand { get; }
    public ICommand CloseFlyoutCommand { get; }

    private void DoNavigate(string target) => _ = target switch
    {
        "PageA"   => _navigationService.NavigateFlyoutDetailAbsoluteAsync<FeaturePageAViewModel>(),
        "PageB"   => _navigationService.NavigateFlyoutDetailAbsoluteAsync<FeaturePageBViewModel>(),
        "SignOut" => _navigationService.NavigateAbsoluteAsync<LoginPageViewModel>(),
        _         => Task.FromResult<INavigationResult>(new NavigationResult(true))
    };
}
```

`NavigateFlyoutDetailAbsoluteAsync<TDetailVm>()` replaces only the **detail** side of the FlyoutPage (and starts a fresh navigation stack on it). The flyout side stays alive. If the user taps the same detail page that's already active, Naveasy detects it and simply closes the flyout instead of recreating the page.

You can also toggle the flyout open/closed from any ViewModel:

```csharp
_navigationService.IsFlyoutOpen = true;  // open
_navigationService.IsFlyoutOpen = false; // close
```

> If the root isn't a `FlyoutPage`, `IsFlyoutOpen` simply returns `false` and ignores assignments.

When the active root is a `FlyoutPage`, `NavigateAsync<TVm>()` will also do the right thing: it pushes the new page on top of the flyout's detail `NavigationPage`, and closes the flyout if it's open.

---

## Page dialogs (`IPageDialogService`)

Naveasy ships a thin wrapper around MAUI's `DisplayAlert` / `DisplayActionSheet` so you can call them from a ViewModel without touching `Page`:

```csharp
public interface IPageDialogService
{
    Task<bool> DisplayAlertAsync(string title, string message, string acceptButton, string cancelButton);
    Task<bool> DisplayAlertAsync(string title, string message, string acceptButton, string cancelButton, FlowDirection flowDirection);

    Task DisplayAlertAsync(string title, string message, string cancelButton);
    Task DisplayAlertAsync(string title, string message, string cancelButton, FlowDirection flowDirection);

    Task<string> DisplayActionSheetAsync(string title, string cancelButton, string destroyButton, params string[] otherButtons);
    Task<string> DisplayActionSheetAsync(string title, string cancelButton, string destroyButton, FlowDirection flowDirection, params string[] otherButtons);
}
```

It's automatically registered by `UseNaveasy<T>()` — just inject it:

```csharp
public class DeleteItemViewModel
{
    private readonly IPageDialogService _dialogs;

    public DeleteItemViewModel(IPageDialogService dialogs) => _dialogs = dialogs;

    public async Task ConfirmDeleteAsync()
    {
        var ok = await _dialogs.DisplayAlertAsync(
            title: "Delete",
            message: "Are you sure?",
            acceptButton: "Yes",
            cancelButton: "No");

        if (ok) { /* delete */ }
    }
}
```

---

## `BindableBase` for ViewModels

Naveasy includes a small `INotifyPropertyChanged` helper class so you don't have to bring in another MVVM library if you don't want to:

```csharp
public class BindableBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null);
    protected bool SetProperty<T>(ref T storage, T value, Action onChanged, [CallerMemberName] string propertyName = null);
    protected void RaisePropertyChanged([CallerMemberName] string propertyName = null);
}
```

Use it in any ViewModel:

```csharp
public class LoginPageViewModel : BindableBase
{
    private string? _username;
    public string? Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }
}
```

You are free to use CommunityToolkit.Mvvm or any other MVVM framework instead — Naveasy doesn't require `BindableBase`.

---

## Scoped vs. transient pages

Naveasy creates a **child `IServiceScope` per navigated page** and stores it on an attached property of the page. Scoped services injected into your page or ViewModel constructor (`AddScoped<...>()`) live exactly as long as that page lives. The scope is disposed when the page is destroyed (popped without coming back, or replaced by an absolute navigation), which automatically disposes everything resolved through it.

Choose between:

- `AddTransientForNavigation<TView, TViewModel>()` — a fresh `TView` / `TViewModel` instance each time you navigate.
- `AddScopedForNavigation<TView, TViewModel>()` — the view and ViewModel are bound to the per-page scope. Use this when you also register other `AddScoped` services that should share the same lifetime as the page.

> Singletons (`AddSingleton<...>()`) work as usual — there's one instance for the whole app. `INavigationService`, `IPageDialogService`, and `IApplicationProvider` are themselves registered as singletons.

---

## View / ViewModel naming convention

Naveasy maps ViewModels to Views by **class-name convention** (and the registry built by `AddTransientForNavigation` / `AddScopedForNavigation`):

> A ViewModel named `FooPageViewModel` is expected to have a matching View named `FooPage` **in the same assembly**.

If you don't follow that, you'll get an exception like:

> `ViewModel MyApp.Views.FooPageViewModel does not have a matching view MyApp.Views.FooPage in the same assembly.`

So always:

- Suffix your ViewModel class with `ViewModel`.
- Keep the View and ViewModel in the same assembly.
- Register both with `AddTransientForNavigation<TView, TViewModel>()`.

You **cannot** point one ViewModel at a different View name — the registration is the source of truth.

---

## `App.xaml.cs` — what NOT to do

> ⚠️ Do **not** override `CreateWindow()` in your `App.xaml.cs`.

Naveasy v3 already overrides it for you, internally, so it can:

- create the initial `Window`,
- hook lifecycle events,
- wrap your first page in a `NavigationPage` when needed,
- install the hardware-back-button handler on Android.

If you previously overrode `CreateWindow()` in older versions of Naveasy, remove the override and move any logic into the **startup ViewModel** (the one you passed to `UseNaveasy<T>()`).

A clean `App.xaml.cs` is enough:

```csharp
public partial class App : Application
{
    public App() => InitializeComponent();
}
```

> ⚠️ Do **not** wrap your pages in a `NavigationPage` yourself. Naveasy will create one for you whenever the destination requires it.

---

## Sample app

The [`Naveasy.Samples`](./Naveasy.Samples) project shows real-world usage of every feature:

| Scenario | Where to look |
| --- | --- |
| App startup + initial routing via `IPageLifecycleAware.OnAppearing` | `Views/Splash/SplashPageViewModel.cs` |
| `NavigateAbsoluteAsync` for `Login → App` (ContentPage **or** FlyoutPage root) | `Views/Login/LoginPageViewModel.cs` |
| Passing typed parameters with `.ToNavigationParameter().Including(...)` | `Views/Feature1/FeaturePage1ViewModel.cs` |
| Reading typed parameters in `OnInitialize` | `Views/Feature2/FeaturePage2ViewModel.cs` |
| `FlyoutPage` setup with TwoWay-bound `IsPresented` | `Views/Flyout/MyFlyoutPage.xaml` + `MyFlyoutPageViewModel.cs` |
| Switching flyout detail with `NavigateFlyoutDetailAbsoluteAsync` | `Views/Flyout/MyFlyoutPageViewModel.cs` |
| Opening / closing the flyout from a ViewModel via `IsFlyoutOpen` | `Views/FeatureA/FeaturePageAViewModel.cs` |
| `GoBackAsync` / `GoBackToRootAsync` / `SignOut` | `Views/FeatureD/PageDViewModel.cs` |
| `ViewModelBase` implementing every lifecycle hook with logging | `Views/ViewModelBase.cs` |
| `MauiProgram` wiring (`UseNaveasy<TStartupVm>()` + registrations) | `MauiProgram.cs` |

Run it on any supported MAUI target to walk through the flows.

---

## Known limitations

There is **no support for MAUI AppShell** until Microsoft truly fixes the following issues:

- https://github.com/dotnet/maui/issues/7354
- https://github.com/dotnet/maui/issues/21814
- https://github.com/dotnet/maui/issues/21816

---

## Contributing

Feel free to open issues and pull requests — contributions are welcome.

This library was inspired by the original .NET Foundation version of PRISM (which is no longer a free library).

