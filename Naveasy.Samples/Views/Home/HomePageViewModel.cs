using Naveasy.Navigation;
using Naveasy.Samples.Models;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Naveasy.Samples.Views.Products;

namespace Naveasy.Samples.Views.Home;

public class HomePageViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly ILogger<HomePageViewModel> _logger;

    public HomePageViewModel(INavigationService navigationService, ILogger<HomePageViewModel> logger)
    {
        _navigationService = navigationService;
        _logger = logger;
        ProductsCommand = new Command(OnProducts);
    }

    private string? _text;
    public string? Text
    {
        get => _text;
        set => SetProperty(ref _text, value);
    }

    public ICommand ProductsCommand { get; }

    public override void OnInitialize(INavigationParameters parameters)
    {
        var model = parameters.GetValue<ModelA>();
        var id = parameters.GetValue<int>();

        Text = $"{model.Name} {Environment.NewLine} id = {id} from navigation parameters.";
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

    private void OnProducts()
    {
        _navigationService.NavigateAsync<ProductsPageViewModel>();
    }

    public override void Destroy()
    {
        _logger.LogInformation($"Passed through {GetType().Name}.Destroy()");
    }
}