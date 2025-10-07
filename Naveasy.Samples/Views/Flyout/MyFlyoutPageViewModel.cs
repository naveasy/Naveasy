namespace Naveasy.Samples.Views.Flyout;

public class MyFlyoutPageViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    public MyFlyoutPageViewModel(INavigationService navigationService)
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

    private void DoNavigate(string targetPage)
    {
        switch (targetPage)
        {
            case "FeaturePageA":
                _navigationService.NavigateFlyoutDetailAbsoluteAsync<FeaturePageAViewModel>();
                break;
            case "FeaturePageB":
                _navigationService.NavigateFlyoutDetailAbsoluteAsync<FeaturePageBViewModel>();
                break;
            case "FeaturePageC":
                _navigationService.NavigateFlyoutDetailAbsoluteAsync<FeaturePageCViewModel>();
                break;
            case "FeaturePageD":
                _navigationService.NavigateFlyoutDetailAbsoluteAsync<FeaturePageDViewModel>();
                break;
            case "FeaturePage1":
                _navigationService.NavigateFlyoutDetailAbsoluteAsync<FeaturePage1ViewModel>();
                break;
            case "FeaturePage2":
                _navigationService.NavigateFlyoutDetailAbsoluteAsync<FeaturePage2ViewModel>();
                break;
            case "SignOut":
                _navigationService.NavigateFlyoutDetailAbsoluteAsync<LoginPageViewModel>();
                break;
        }
    }
}