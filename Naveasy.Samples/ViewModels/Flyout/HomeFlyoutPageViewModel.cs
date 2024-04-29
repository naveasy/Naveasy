using System.Reactive.Disposables;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Naveasy.Samples.Models;
using Naveasy.Samples.Services;
using Naveasy.Samples.Views;

namespace Naveasy.Samples.ViewModels.Flyout;

public class HomeFlyoutPageViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IFlyoutService _flyoutService;
    private readonly CompositeDisposable _disposables = [];

    public HomeFlyoutPageViewModel(INavigationService navigationService, IFlyoutService flyoutService, ILogger<HomeFlyoutPageViewModel> logger) : base(logger)
    {
        _navigationService = navigationService;
        _flyoutService = flyoutService;
        NavigateCommand = new Command<string>(DoNavigate);
        SignOutCommand = new Command(SignOut);

        var subscription = _flyoutService.ObserveIsPresented.Subscribe(x =>
        {
            IsFlyoutPresented = x;
        });

        _disposables.Add(subscription);
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

        _flyoutService.SetIsPresented(false);
    }

    private void SignOut()
    {
        _navigationService.NavigateAbsoluteAsync<LoginPageViewModel>();
    }

    public override void Destroy()
    {
        _disposables.Dispose();
        base.Destroy();
    }
}