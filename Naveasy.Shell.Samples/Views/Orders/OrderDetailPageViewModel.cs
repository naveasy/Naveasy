using Naveasy.Shell.Samples.Services;

namespace Naveasy.Shell.Samples.Views.Orders;

/// <summary>
/// Receives the order in two ways, both of them through INavigationParameters: as a typed parameter sent by
/// NavigateAsync, and as the query string of a deep link such as "//main/orders/OrderDetailPage?orderId=2".
/// Implementing IQueryAttributable still works if the raw Shell query is needed.
/// </summary>
public class OrderDetailPageViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IOrderService _orderService;
    private OrderModel? _order;

    public OrderDetailPageViewModel(INavigationService navigationService, IOrderService orderService)
    {
        Title = "Order";
        _navigationService = navigationService;
        _orderService = orderService;

        ApproveCommand = new Command(async () => await GoBackWithReviewAsync(approved: true));
        RejectCommand = new Command(async () => await GoBackWithReviewAsync(approved: false));
        OpenCustomerCommand = new Command(async () => await _navigationService.NavigateAsync<CustomerPageViewModel>(Order.ToNavigationParameter()));
        ReviewCommand = new Command(async () => await _navigationService.NavigateModalAsync<ReviewOrderPageViewModel>(Order.ToNavigationParameter()));
    }

    public OrderModel? Order
    {
        get => _order;
        set => SetProperty(ref _order, value);
    }

    public ICommand ApproveCommand { get; }

    public ICommand RejectCommand { get; }

    public ICommand OpenCustomerCommand { get; }

    public ICommand ReviewCommand { get; }

    public override void OnInitialize(INavigationParameters parameters)
    {
        base.OnInitialize(parameters);

        Order = parameters.GetValue<OrderModel>() ?? GetOrderFromDeepLink(parameters);
        Title = Order is null ? "Order" : $"Order {Order.Id}";
    }

    /// <summary>
    /// A deep link only carries text, so the id arrives as the "orderId" navigation parameter.
    /// </summary>
    private OrderModel? GetOrderFromDeepLink(INavigationParameters parameters)
    {
        if (!parameters.TryGetValue("orderId", out var value) || !int.TryParse(value?.ToString(), out var orderId))
            return null;

        return _orderService.GetOrder(orderId);
    }

    private Task GoBackWithReviewAsync(bool approved)
    {
        if (Order is null)
            return _navigationService.GoBackAsync();

        var review = new OrderReviewModel(Order.Id, approved);

        return _navigationService.GoBackAsync(review.ToNavigationParameter());
    }

    public override void Dispose()
    {
        base.Dispose();
    }
}
