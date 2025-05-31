using Microsoft.Extensions.Logging;
using Naveasy.Samples.Views.Login;

namespace Naveasy.Samples.Views.Splash;

public class SplashPageViewModel : ViewModelBase, IPageLifecycleAware
{
    private readonly INavigationService _navigationService;

    public SplashPageViewModel(INavigationService navigationService, ILoggerFactory loggerFactory)
    {
        Title = "Splash Page";
        _navigationService = navigationService;
        ViewModelBase.Logger = loggerFactory.CreateLogger("[-INF-] Naveasy Sample");
    }


    public void OnAppearing()
    {
        _navigationService.NavigateAbsoluteAsync<LoginPageViewModel>();
    }

    public void OnDisappearing()
    {
    }
}