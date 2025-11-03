namespace Naveasy.Samples.Views.FeatureA;

public class FeaturePageAViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    public FeaturePageAViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        Title = "Page A";
        NavigateCommand = new Command(Navigate);
        OpenFlyoutCommand = new Command(OpenFlyout);
    }

    public ICommand NavigateCommand { get; }
    public ICommand OpenFlyoutCommand { get; }

    private void Navigate()
    {
        _navigationService.NavigateAsync<FeaturePageBViewModel>();
    }

    private void OpenFlyout()
    {
        if (!_navigationService.IsFlyoutOpen)
        {
            _navigationService.IsFlyoutOpen = true;
        }
    }
}