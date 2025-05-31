using System.Windows.Input;
using Naveasy.Samples.Views.FeatureC;

namespace Naveasy.Samples.Views.FeatureB;

public class FeaturePageBViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    public FeaturePageBViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        Title = "Page B";
        NavigateCommand = new Command(Navigate);
        BackCommand = new Command(Back);
    }

    public ICommand NavigateCommand { get; }
    public ICommand BackCommand { get; }

    private void Navigate()
    {
        _navigationService.NavigateAsync<FeaturePageCViewModel>();
    }

    private void Back()
    {
        _navigationService.GoBackAsync();
    }
}