namespace Naveasy.Samples.Views.FeatureD;

public class FeaturePageDViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    public FeaturePageDViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        Title = "Page D";
        NavigateBackCommand = new Command(NavigateBack);
        NavigateBackToRootCommand = new Command(NavigateToRoot);
        SignOutCommand = new Command(SignOut);
    }

    public ICommand NavigateBackCommand { get; }
    public ICommand NavigateBackToRootCommand { get; set; }

    public ICommand SignOutCommand { get; }

    private void NavigateBack()
    {
        _navigationService.GoBackAsync();
    }

    private void NavigateToRoot()
    {
        _navigationService.GoBackToRootAsync();
    }

    private void SignOut()
    {
        _navigationService.NavigateAbsoluteAsync<LoginPageViewModel>();
    }
}