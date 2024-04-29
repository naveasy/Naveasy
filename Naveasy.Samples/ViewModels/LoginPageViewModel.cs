using Microsoft.Extensions.Logging;
using Naveasy.Samples.Models;
using Naveasy.Samples.ViewModels.Flyout;
using Naveasy.Samples.Views;

namespace Naveasy.Samples.ViewModels;

public class LoginPageViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    public LoginPageViewModel(INavigationService navigationService, ILogger<LoginPageViewModel> logger) : base(logger)
    {
        _navigationService = navigationService;
        LoginCommand = new Command<string>(Login);
    }

    public Command<string> LoginCommand { get; }

    private void Login(string parameter)
    {
        var id = 1;
        var model = new ModelA()
        {
            Name = "Parameter from Login Page"
        };

        var navigationParameters = id.ToNavigationParameter().Including(model);

        if (parameter == "ContentPage")
        {
            _navigationService.NavigateAbsoluteAsync<HomeContentPageViewModel>(navigationParameters);
        }
        else
        {
            _navigationService.NavigateFlyoutAbsoluteAsync<HomeFlyoutPageViewModel, ProductsPageViewModel>(navigationParameters);
        }
    }
}