using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Naveasy.Navigation;
using Naveasy.Samples.Views;

namespace Naveasy.Samples.ViewModels;

public class ProductsPageViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly ILogger<ProductsPageViewModel> _logger;

    public ProductsPageViewModel(INavigationService navigationService, ILogger<ProductsPageViewModel> logger)
    {
        _navigationService = navigationService;
        _logger = logger;
        DetailsCommand = new Command(OnDetails);
    }

    public ICommand DetailsCommand { get; }

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

    private void OnDetails()
    {
        _navigationService.NavigateAsync<DetailsPageViewModel>();
    }

    public override void Destroy()
    {
        _logger.LogInformation($"Passed through {GetType().Name}.Destroy()");
    }
}