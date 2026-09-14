using Microsoft.Extensions.Logging;
using Naveasy.Extensions;

namespace Naveasy.Shell.Samples.Views;

public class ViewModelBase : BindableBase, IInitialize, IInitializeAsync, INavigatedAware, IPageLifecycleAware, IDisposable
{
    private string? _title;

    public string? Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public static ILogger Logger { get; set; } = null!;

    public virtual void OnInitialize(INavigationParameters parameters)
    {
        //Logger.LogDebug("Initialize {Title} ({NavigationMode})", Title, parameters.GetNavigationMode());
    }

    public virtual Task OnInitializeAsync(INavigationParameters parameters)
    {
        Logger.LogDebug("InitializeAsync {Title} ({NavigationMode})", Title, parameters.GetNavigationMode());

        return Task.CompletedTask;
    }

    public virtual void OnNavigatedTo(INavigationParameters navigationParameters)
    {
        Logger.LogInformation("NavigatedTo {Title} ({NavigationMode})", Title, navigationParameters.GetNavigationMode());
    }

    public virtual void OnNavigatedFrom(INavigationParameters navigationParameters)
    {
        Logger.LogDebug("NavigatedFrom {Title}", Title);
    }

    public virtual void OnAppearing()
    {
        Logger.LogDebug("Appearing {Title}", Title);
    }

    public virtual void OnDisappearing()
    {
        Logger.LogDebug("Disappearing {Title}", Title);
    }

    public virtual void Dispose()
    {
        Logger.LogWarning("Disposed {Title}", Title);
    }
}