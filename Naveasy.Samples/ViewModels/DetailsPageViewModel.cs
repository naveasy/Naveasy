using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Naveasy.Navigation;
using Naveasy.Samples.Views;

namespace Naveasy.Samples.ViewModels;

public class DetailsPageViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly ILogger<ProductsPageViewModel> _logger;

    public DetailsPageViewModel(INavigationService navigationService, ILogger<ProductsPageViewModel> logger)
    {
        _navigationService = navigationService;
        _logger = logger;
        LogoutCommand = new Command(OnLogout);
    }

    public ICommand LogoutCommand { get; }

    public override void OnInitialize(INavigationParameters parameters)
    {
        _logger.LogInformation($"Passed through {GetType().Name}.OnInitialize()");
    }

    public override Task OnInitializeAsync(INavigationParameters parameters)
    {
        _logger.LogInformation($"Passed through {GetType().Name}.OnInitializeAsync()");
        return Task.CompletedTask;
    }

    public override void OnNavigatedFrom(INavigationParameters navigationParameters)
    {
        _logger.LogInformation($"Passed through {GetType().Name}.OnNavigatedFrom()");
    }

    public override void OnNavigatedTo(INavigationParameters navigationParameters)
    {
        _logger.LogInformation($"Passed through {GetType().Name}.OnNavigatedTo()");
    }

    private void OnLogout()
    {
        _navigationService.NavigateAbsoluteAsync<LoginPageViewModel>();
    }

    public override void Destroy()
    {
        _logger.LogInformation($"Passed through {GetType().Name}.Destroy()");
    }
}