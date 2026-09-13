namespace Naveasy.Shell.Samples.Services;

public interface IOrderService
{
    IReadOnlyList<OrderModel> GetOrders();

    OrderModel? GetOrder(int id);
}

public sealed class OrderService : IOrderService
{
    private readonly OrderModel[] _orders =
    [
        new(1, "Contributor User", 149.90m),
        new(2, "Maui Developer", 89.50m),
        new(3, "Shell Enthusiast", 320.00m)
    ];

    public IReadOnlyList<OrderModel> GetOrders() => _orders;

    public OrderModel? GetOrder(int id) => _orders.FirstOrDefault(order => order.Id == id);
}
