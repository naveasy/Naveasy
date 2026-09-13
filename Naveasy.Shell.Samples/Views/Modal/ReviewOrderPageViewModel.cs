namespace Naveasy.Shell.Samples.Views.Modal;

/// <summary>
/// Pushed with NavigateModalAsync: Naveasy sets Shell.PresentationMode on the page before Shell shows it.
/// GoBackAsync closes it, and the data given to GoBackAsync reaches the page underneath.
/// </summary>
public class ReviewOrderPageViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private OrderModel? _order;

    public ReviewOrderPageViewModel(INavigationService navigationService)
    {
        Title = "Review";
        _navigationService = navigationService;
        CloseCommand = new Command(async () => await _navigationService.GoBackAsync());
    }

    public OrderModel? Order
    {
        get => _order;
        set => SetProperty(ref _order, value);
    }

    public ICommand CloseCommand { get; }

    public override void OnInitialize(INavigationParameters parameters)
    {
        base.OnInitialize(parameters);

        Order = parameters.GetValue<OrderModel>();
    }
}
