using Microsoft.Extensions.Logging;

namespace Naveasy.Samples.Views;

public class ViewModelBase() : BindableBase, IInitialize, IInitializeAsync, INavigatedAware, IDisposable
{
    private string? _title;

    public string? Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public static ILogger Logger { get; set; }

    public virtual void OnInitialize(INavigationParameters parameters)
    {
        Logger.LogDebug($"Initialized {Title}");
    }

    public virtual Task OnInitializeAsync(INavigationParameters parameters)
    {
        Logger.LogDebug($"InitializedAsync {Title}");
        return Task.CompletedTask;
    }

    public virtual void OnNavigatedFrom(INavigationParameters navigationParameters)
    {
        Logger.LogDebug($"NavigatedFrom {Title}");
    }

    public virtual void OnNavigatedTo(INavigationParameters navigationParameters)
    {
        Logger.LogDebug($"NavigatedTo {Title}");
    }

    public virtual void Dispose()
    {
        Logger.LogDebug($"Destroyed {Title}");
    }
}