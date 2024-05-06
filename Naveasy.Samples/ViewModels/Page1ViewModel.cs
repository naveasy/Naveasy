using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Naveasy.Samples.Views;

namespace Naveasy.Samples.ViewModels;

public class Page1ViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    public Page1ViewModel(INavigationService navigationService, ILogger<Page1ViewModel> logger) : base(logger)
    {
        Title = "Page 1";
        _navigationService = navigationService;
        NavigateCommand = new Command(Navigate);
    }

    public ICommand NavigateCommand { get; }

    private void Navigate()
    {
        _navigationService.NavigateAsync<Page2ViewModel>();
    }
}