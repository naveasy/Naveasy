using Microsoft.Extensions.Logging;
using Naveasy.Samples.Views;

namespace Naveasy.Samples.ViewModels.Flyout;

public class PageAViewModel : ViewModelBase
{
    public PageAViewModel(ILogger<PageAViewModel> logger) : base(logger)
    {
        Title = "Page A";
    }
}