using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Naveasy.Samples.Views;

namespace Naveasy.Samples.ViewModels;

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