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
    }

    private string? _text;

    public string? Text
    {
        get => _text;
        set => SetProperty(ref _text, value);
    }

    private bool _isFlyoutPresented;
    public bool IsFlyoutPresented
    {
        get => _isFlyoutPresented;
        set
        {
            _isFlyoutPresented = value;
            RaisePropertyChanged();
        }
    }

    public ICommand NavigateCommand { get; }

    public override void OnInitialize(INavigationParameters parameters)
    {
        base.OnInitialize(parameters);

        var model = parameters.GetValue<ModelA>();
        var id = parameters.GetValue<int>();

        Text = $"{model.Name} {Environment.NewLine} id = {id} from navigation parameters.";
    }

    private void DoNavigate(string targetPage)
    {
        switch (targetPage)
        {
            case "PageA":
                _navigationService.NavigateAsync<INavigationPage<PageAViewModel>>();
                break;
            case "PageB":
                _navigationService.NavigateAsync<INavigationPage<PageBViewModel>>();
                break;
            case "PageC":
                _navigationService.NavigateAsync<INavigationPage<PageCViewModel>>();
                break;
            case "PageD":
                _navigationService.NavigateAsync<PageDViewModel>();
                break;
            case "SignOut":
                _navigationService.NavigateAbsoluteAsync<LoginPageViewModel>();
                break;
        }
    }
}