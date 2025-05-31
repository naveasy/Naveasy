using System.Windows.Input;
using Naveasy.Samples.Views.Feature1;
using Naveasy.Samples.Views.Feature2;
using Naveasy.Samples.Views.FeatureA;
using Naveasy.Samples.Views.FeatureB;
using Naveasy.Samples.Views.FeatureC;
using Naveasy.Samples.Views.FeatureD;
using Naveasy.Samples.Views.Login;

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
                _navigationService.NavigateAsync<FeaturePageAViewModel>();
                break;
            case "FeaturePageB":
                _navigationService.NavigateAsync<FeaturePageBViewModel>();
                break;
            case "FeaturePageC":
                _navigationService.NavigateAsync<FeaturePageCViewModel>();
                break;
            case "FeaturePageD":
                _navigationService.NavigateAsync<FeaturePageDViewModel>();
                break;
            case "FeaturePage1":
                _navigationService.NavigateAsync<FeaturePage1ViewModel>();
                break;
            case "FeaturePage2":
                _navigationService.NavigateAsync<FeaturePage2ViewModel>();
                break;
            case "SignOut":
                _navigationService.NavigateAbsoluteAsync<LoginPageViewModel>();
                break;
        }
    }
}