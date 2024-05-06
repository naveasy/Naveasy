using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Naveasy.Samples.Views;

namespace Naveasy.Samples.ViewModels;

public class PageAViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    public PageAViewModel(ILogger<PageAViewModel> logger, INavigationService navigationService) : base(logger)
    {
        _navigationService = navigationService;
        Title = "Page A";
        NavigateCommand = new Command(Navigate);
    }

    public ICommand NavigateCommand { get; }

    private void Navigate()
    {
        _navigationService.NavigateAsync<PageBViewModel>();
    }
}