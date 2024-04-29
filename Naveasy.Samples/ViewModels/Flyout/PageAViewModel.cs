using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Naveasy.Samples.Services;
using Naveasy.Samples.Views;

namespace Naveasy.Samples.ViewModels.Flyout;

public class PageAViewModel : ViewModelBase
{
    private readonly IFlyoutService _flyoutService;

    public PageAViewModel(ILogger<PageAViewModel> logger, IFlyoutService flyoutService) : base(logger)
    {
        _flyoutService = flyoutService;
        Title = "Page A";
        ToggleFlyoutMenuCommand = new Command(ToggleFlyoutMenu);
    }

    public ICommand ToggleFlyoutMenuCommand { get; }

    private void ToggleFlyoutMenu()
    {
        _flyoutService.SetIsPresented(true);
    }
}