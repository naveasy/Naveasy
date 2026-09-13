using Naveasy.Shell.Samples.Services;

namespace Naveasy.Shell.Samples.Views.Reports;

public class SummaryPageViewModel : ViewModelBase
{
    public SummaryPageViewModel(IOrderService orderService)
    {
        Title = "Summary";
        Total = orderService.GetOrders().Sum(order => order.Total);
    }

    public decimal Total { get; }
}
