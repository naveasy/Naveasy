namespace Naveasy.Shell.Samples.Views.Home;

public class HomePageViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    public HomePageViewModel(INavigationService navigationService)
    {
        Title = "Home";
        _navigationService = navigationService;

        SelectOrdersTabCommand = new Command(async () => await _navigationService.SelectTabAsync<OrdersPageViewModel>());
        OpenGuardedPageCommand = new Command(async () => await _navigationService.NavigateAsync<GuardedPageViewModel>());
        OpenReportsCommand = new Command(async () => await _navigationService.NavigateAsync<ReportsPageViewModel>());
        OpenFlyoutCommand = new Command(() => _navigationService.IsFlyoutOpen = true);
        OpenDeepLinkCommand = new Command(async () => await OpenDeepLinkAsync());
        SignOutCommand = new Command(async () => await _navigationService.NavigateAbsoluteAsync<LoginPageViewModel>());
    }

    public ICommand SelectOrdersTabCommand { get; }

    public ICommand OpenGuardedPageCommand { get; }

    public ICommand OpenReportsCommand { get; }

    public ICommand OpenFlyoutCommand { get; }

    public ICommand OpenDeepLinkCommand { get; }

    public ICommand SignOutCommand { get; }

    /// <summary>
    /// Navigates through the raw Shell API, the way an external deep link reaches the app. Naveasy delivers the
    /// values of the query string to the ViewModel as regular navigation parameters.
    /// </summary>
    private static Task OpenDeepLinkAsync() => MauiShell.Current.GoToAsync("//main/orders/OrderDetailPage?orderId=2");

    public override void OnInitialize(INavigationParameters parameters)
    {
        base.OnInitialize(parameters);
    }

    public override void OnNavigatedTo(INavigationParameters navigationParameters)
    {
        base.OnNavigatedTo(navigationParameters);

        // The flyout is disabled on the login page, so it is turned back on once the user is signed in.
        _navigationService.FlyoutBehavior = FlyoutBehavior.Flyout;
    }
}
