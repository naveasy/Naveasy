namespace Naveasy.Samples.Views.Login;

public class LoginPageViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    public LoginPageViewModel(INavigationService navigationService)
    {
        Title = "Login Page";
        _navigationService = navigationService;
        LoginCommand = new Command<string>(Login);
    }

    public Command<string> LoginCommand { get; }

    private void Login(string parameter)
    {
        if (parameter == "ContentPage")
        {
            _navigationService.NavigateAbsoluteAsync<FeaturePage1ViewModel>();
        }
        else
        {
            _navigationService.NavigateFlyoutAbsoluteAsync<MyFlyoutPageViewModel, FeaturePageAViewModel>();
        }
    }
}