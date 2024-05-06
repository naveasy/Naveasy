using Microsoft.Extensions.Logging;

namespace Naveasy.Samples.Views;

public class ViewModelBase(ILogger logger) : BindableBase, IInitialize, IInitializeAsync, INavigatedAware, IDestructible
{
    private string? _title;

    public string? Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    private string PageName => GetType().Name.Replace("ViewModel", "");

    public virtual void OnInitialize(INavigationParameters parameters)
    {
        logger.LogDebug($"{PageName} Initialized");
    }

    public virtual Task OnInitializeAsync(INavigationParameters parameters)
    {
        logger.LogDebug($"{PageName} InitializedAsync");
        return Task.CompletedTask;
    }

    public virtual void OnNavigatedFrom(INavigationParameters navigationParameters)
    {
        logger.LogDebug($"{PageName} NavigatedFrom");
    }

    public virtual void OnNavigatedTo(INavigationParameters navigationParameters)
    {
        logger.LogDebug($"{PageName} NavigatedTo");
    }

    public virtual void Destroy()
    {
        logger.LogDebug($"{PageName} Destroyed");
    }
}