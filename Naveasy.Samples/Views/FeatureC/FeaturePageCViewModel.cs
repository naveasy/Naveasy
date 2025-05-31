namespace Naveasy.Samples.Views.FeatureC;

public class FeaturePageCViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    public FeaturePageCViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        Title = "Page C";
        NavigateCommand = new Command(Navigate);
    }

    public ICommand NavigateCommand { get; }

    private void Navigate()
    {
        _navigationService.NavigateAsync<FeaturePageDViewModel>();
    }
}