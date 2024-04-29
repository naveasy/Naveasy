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

    public virtual void OnInitialize(INavigationParameters parameters)
    {
        logger.LogInformation($"{GetType().Name} Initialized");
    }

    public virtual Task OnInitializeAsync(INavigationParameters parameters)
    {
        logger.LogInformation($"{GetType().Name} InitializedAsync");
        return Task.CompletedTask;
    }

    public virtual void OnNavigatedFrom(INavigationParameters navigationParameters)
    {
        logger.LogInformation($"{GetType().Name} NavigatedFrom");
    }

    public virtual void OnNavigatedTo(INavigationParameters navigationParameters)
    {
        logger.LogInformation($"{GetType().Name}NavigatedTo");
    }

    public virtual void Destroy()
    {
        logger.LogInformation($"{GetType().Name} Destroyed");
    }
}