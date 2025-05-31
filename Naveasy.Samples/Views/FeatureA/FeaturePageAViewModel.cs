using System.Windows.Input;
using Naveasy.Samples.Views.FeatureB;

namespace Naveasy.Samples.Views.FeatureA;

public class FeaturePageAViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    public FeaturePageAViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        Title = "Page A";
        NavigateCommand = new Command(Navigate);
    }

    public ICommand NavigateCommand { get; }

    private void Navigate()
    {
        _navigationService.NavigateAsync<FeaturePageBViewModel>();
    }
}