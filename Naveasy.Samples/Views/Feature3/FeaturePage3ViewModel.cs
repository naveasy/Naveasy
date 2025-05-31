namespace Naveasy.Samples.Views.Feature3;

public class MyPage3ViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    public MyPage3ViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        Title = "Page 3";
        NavigateCommand = new Command(Navigate);
    }

    public ICommand NavigateCommand { get; }

    private void Navigate()
    {
        _navigationService.NavigateAsync<FeaturePageAViewModel>();
    }
}