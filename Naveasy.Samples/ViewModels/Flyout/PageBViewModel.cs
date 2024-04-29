using Microsoft.Extensions.Logging;
using Naveasy.Samples.Views;

namespace Naveasy.Samples.ViewModels.Flyout;

public class PageBViewModel : ViewModelBase
{
    public PageBViewModel(ILogger<PageBViewModel> logger) : base(logger)
    {
        Title = "Page B";
    }
}