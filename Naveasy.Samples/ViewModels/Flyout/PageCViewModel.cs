using Microsoft.Extensions.Logging;
using Naveasy.Samples.Views;

namespace Naveasy.Samples.ViewModels.Flyout;

public class PageCViewModel : ViewModelBase
{
    public PageCViewModel(ILogger<PageCViewModel> logger) : base(logger)
    {
        Title = "Page C";
    }
}