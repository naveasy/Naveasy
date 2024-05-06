using Microsoft.Extensions.Logging;
using Naveasy.Samples.Models;
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
        var primitive = 2.5d;
        var model = new ModelA()
        {
            Id = 1,
            Name = "Parameter from Login Page"
        };

        var navigationParameters = model.ToNavigationParameter().Including(primitive);

        if (parameter == "ContentPage")
        {
            _navigationService.NavigateAbsoluteAsync<INavigationPage<Page0ViewModel>>(navigationParameters);
        }
        else
        {
            _navigationService.NavigateFlyoutAbsoluteAsync<MyFlyoutPageViewModel, INavigationPage<PageAViewModel>>(navigationParameters);
        }
    }
}