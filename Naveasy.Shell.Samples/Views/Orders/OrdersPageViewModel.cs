using System.Collections.ObjectModel;
using Naveasy.Extensions;
using Naveasy.Shell.Samples.Services;

namespace Naveasy.Shell.Samples.Views.Orders;

public class OrdersPageViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IOrderService _orderService;
    private string? _lastReview;

    public OrdersPageViewModel(INavigationService navigationService, IOrderService orderService)
    {
        Title = "Orders";
        _navigationService = navigationService;
        _orderService = orderService;

        OpenOrderCommand = new Command<OrderModel>(async order => await OpenOrderAsync(order));
    }

    public ObservableCollection<OrderModel> Orders { get; } = [];

    public ICommand OpenOrderCommand { get; }

    public string? LastReview
    {
        get => _lastReview;
        set
        {
            if (SetProperty(ref _lastReview, value))
                RaisePropertyChanged(nameof(HasReview));
        }
    }

    public bool HasReview => !string.IsNullOrEmpty(LastReview);

    public override void OnInitialize(INavigationParameters parameters)
    {
        base.OnInitialize(parameters);

        foreach (var order in _orderService.GetOrders())
            Orders.Add(order);
    }

    public override void OnNavigatedTo(INavigationParameters navigationParameters)
    {
        base.OnNavigatedTo(navigationParameters);

        if (navigationParameters.GetNavigationMode() != NavigationMode.Back)
            return;

        // Data handed back by the page that was just popped.
        var review = navigationParameters.GetValue<OrderReviewModel>();

        if (review is not null)
            LastReview = $"Order {review.OrderId} was {(review.Approved ? "approved" : "rejected")}";
    }

    private Task OpenOrderAsync(OrderModel? order)
    {
        if (order is null)
            return Task.CompletedTask;

        return _navigationService.NavigateAsync<OrderDetailPageViewModel>(order.ToNavigationParameter());
    }
}
