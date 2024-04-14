using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Naveasy.Navigation;
using Naveasy.Samples.Models;
using Naveasy.Samples.Views.Home;

namespace Naveasy.Samples.Views.Login;

public class LoginPageViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly ILogger<LoginPageViewModel> _logger;

    public LoginPageViewModel(INavigationService navigationService, ILogger<LoginPageViewModel> logger)
    {
        _navigationService = navigationService;
        _logger = logger;
        LoginCommand = new Command(Login);
    }

    public ICommand LoginCommand { get; }

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

    private void Login()
    {
        var id = 1;
        var model = new ModelA()
        {
            Name = "Parameter from Login Page"
        };

        var navigationParameters = id.ToNavigationParameter().Including(model);

        _navigationService.NavigateAbsoluteAsync<HomePageViewModel>(navigationParameters);
    }

    public override void Destroy()
    {
        _logger.LogInformation($"Passed through {GetType().Name}.Destroy()");
    }
}