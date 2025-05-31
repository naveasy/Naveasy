using System.Windows.Input;
using Naveasy.Samples.Views.Feature2;

namespace Naveasy.Samples.Views.Feature1;

public class FeaturePage1ViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    public FeaturePage1ViewModel(INavigationService navigationService)
    {
        Title = "Page 1";
        _navigationService = navigationService;
        NavigateCommand = new Command(Navigate);
    }

    public ICommand NavigateCommand { get; }

    private void Navigate()
    {
        _navigationService.NavigateAsync<FeaturePage2ViewModel>();
    }
}