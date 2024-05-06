using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Naveasy.Samples.Views;

namespace Naveasy.Samples.ViewModels;

public class PageDViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    public PageDViewModel(ILogger<PageDViewModel> logger, INavigationService navigationService) : base(logger)
    {
        _navigationService = navigationService;
        Title = "Page D";
        NavigateCommand = new Command(Navigate);
        SignOutCommand = new Command(SignOut);
    }

    public ICommand NavigateCommand { get; }

    public ICommand SignOutCommand { get; }

    private void Navigate()
    {
        _navigationService.GoBackToRootAsync();
    }

    private void SignOut()
    {
        _navigationService.NavigateAbsoluteAsync<LoginPageViewModel>();
    }
}