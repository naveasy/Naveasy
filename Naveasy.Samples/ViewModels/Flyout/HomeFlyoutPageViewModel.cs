using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Naveasy.Samples.Models;
using Naveasy.Samples.Views;

namespace Naveasy.Samples.ViewModels.Flyout;

public class HomeFlyoutPageViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    public HomeFlyoutPageViewModel(INavigationService navigationService, ILogger<HomeFlyoutPageViewModel> logger) : base(logger)
    {
        _navigationService = navigationService;
        NavigateCommand = new Command<string>(DoNavigate);
        SignOutCommand = new Command(SignOut);
    }

    private string? _text;
    public string? Text
    {
        get => _text;
        set => SetProperty(ref _text, value);
    }

    public ICommand NavigateCommand { get; }
    public ICommand SignOutCommand { get; }

    public override void OnInitialize(INavigationParameters parameters)
    {
        var model = parameters.GetValue<ModelA>();
        var id = parameters.GetValue<int>();

        Text = $"{model.Name} {Environment.NewLine} id = {id} from navigation parameters.";
    }

    private void DoNavigate(string targetPage)
    {
        switch (targetPage)
        {
            case "PageA":
                _navigationService.NavigateAsync<PageAViewModel>();
                break;
            case "PageB":
                _navigationService.NavigateAsync<PageBViewModel>();
                break;
            case "PageC":
                _navigationService.NavigateAsync<PageCViewModel>();
                break;
        }
    }

    private void SignOut()
    {
        _navigationService.NavigateAbsoluteAsync<LoginPageViewModel>();
    }
}