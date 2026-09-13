using Naveasy.Shell.Samples.Services;

namespace Naveasy.Shell.Samples.Views.Reports;

public class ChartPageViewModel : ViewModelBase
{
    public ChartPageViewModel(IOrderService orderService)
    {
        Title = "Customers";
        Customers = orderService.GetOrders().Select(order => order.Customer).ToList();
    }

    public IReadOnlyList<string> Customers { get; }
}
