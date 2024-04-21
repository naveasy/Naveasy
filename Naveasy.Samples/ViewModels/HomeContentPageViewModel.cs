using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Naveasy.Samples.Models;
using Naveasy.Samples.Views;

namespace Naveasy.Samples.ViewModels;

public class HomeContentPageViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly ILogger<HomeContentPageViewModel> _logger;

    public HomeContentPageViewModel(INavigationService navigationService, ILogger<HomeContentPageViewModel> logger): base(logger)
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

    private void OnProducts()
    {
        _navigationService.NavigateAsync<ProductsPageViewModel>();
    }
}