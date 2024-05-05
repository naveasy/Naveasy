using Microsoft.Extensions.Logging;
using Naveasy.Samples.Views;
using System.Windows.Input;

namespace Naveasy.Samples.ViewModels.Flyout;

public class PageCViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    public PageCViewModel(ILogger<PageCViewModel> logger, INavigationService navigationService) : base(logger)
    {
        _navigationService = navigationService;
        Title = "Page C";
        NavigateCommand = new Command(Navigate);
    }

    public ICommand NavigateCommand { get; }

    private void Navigate()
    {
        _navigationService.NavigateAsync<PageDViewModel>();
    }
}