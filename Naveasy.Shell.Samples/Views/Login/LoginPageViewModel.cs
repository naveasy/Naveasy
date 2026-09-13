namespace Naveasy.Shell.Samples.Views.Login;

public class LoginPageViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    public LoginPageViewModel(INavigationService navigationService)
    {
        Title = "Sign in";
        _navigationService = navigationService;
        SignInCommand = new Command(async () => await SignInAsync());
    }

    public ICommand SignInCommand { get; }

    private async Task SignInAsync()
    {
        // An absolute navigation switches the Shell item and resets the navigation stack, so the user
        // cannot go back to the login page.
        await _navigationService.NavigateAbsoluteAsync<HomePageViewModel>();
    }
}
