namespace Naveasy.Samples.Views.FeatureD;

public class FeaturePageDViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    public FeaturePageDViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        Title = "Page D";
        NavigateCommand = new Command(Navigate);
        SignOutCommand = new Command(SignOut);
    }

    public ICommand NavigateCommand { get; }

    public ICommand SignOutCommand { get; }

    private void Navigate()
    {
        _navigationService.GoBackToRootAsync();
    }

    private void SignOut()
    {
        _navigationService.NavigateAbsoluteAsync<LoginPageViewModel>();
    }
}