using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Naveasy.Samples.Views;

namespace Naveasy.Samples.ViewModels;

public class ProductDetailsPageViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    public ProductDetailsPageViewModel(INavigationService navigationService, ILogger<ProductsPageViewModel> logger) : base(logger)
    {
        _navigationService = navigationService;
        LogoutCommand = new Command(OnLogout);
    }

    public ICommand LogoutCommand { get; }

    private void OnLogout()
    {
        _navigationService.NavigateAbsoluteAsync<LoginPageViewModel>();
    }
}