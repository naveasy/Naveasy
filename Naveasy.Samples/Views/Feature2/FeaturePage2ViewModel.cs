using System.Windows.Input;
using Naveasy.Samples.Models;
using Naveasy.Samples.Views.Feature3;

namespace Naveasy.Samples.Views.Feature2;

public class FeaturePage2ViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    public FeaturePage2ViewModel(INavigationService navigationService)
    {
        Title = "Page 2";
        _navigationService = navigationService;
        NavigateCommand = new Command(Navigate);
    }

    public ICommand NavigateCommand { get; }

    private void Navigate()
    {
        var primitive = 2.5d;
        var model = new ModelA()
        {
            Id = 1,
            Name = "Parameter from Login Page"
        };

        var navigationParameters = model.ToNavigationParameter()
                                        .Including(primitive);
            //You can include as many parameter as you want using the .Including() method
            //Take care to do not add parameters with the same type cause on next page you
            //will use the type of the parameter to get it's values.
            //If you need multiple parameters of the same type create your own complex type like
            // the ModelA that's being instantiated on line 23,


        _navigationService.NavigateAsync<MyPage3ViewModel>(navigationParameters);
    }
}