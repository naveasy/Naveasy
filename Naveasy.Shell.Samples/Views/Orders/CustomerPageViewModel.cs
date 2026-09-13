namespace Naveasy.Shell.Samples.Views.Orders;

/// <summary>
/// Registered with AddScopedForNavigation: the page gets its own dependency injection scope, and Dispose runs
/// when the page is popped from the Shell navigation stack.
/// </summary>
public class CustomerPageViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private string? _customer;

    public CustomerPageViewModel(INavigationService navigationService)
    {
        Title = "Customer";
        _navigationService = navigationService;
        GoBackCommand = new Command(async () => await _navigationService.GoBackAsync());
        GoBackToRootCommand = new Command(async () => await _navigationService.GoBackToRootAsync());
    }

    public string? Customer
    {
        get => _customer;
        set => SetProperty(ref _customer, value);
    }

    public ICommand GoBackCommand { get; }

    public ICommand GoBackToRootCommand { get; }

    public override void OnInitialize(INavigationParameters parameters)
    {
        base.OnInitialize(parameters);

        Customer = parameters.GetValue<OrderModel>()?.Customer;
    }
}
