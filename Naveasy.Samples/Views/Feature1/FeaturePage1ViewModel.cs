using System.Text.Json;

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

    public override void OnInitialize(INavigationParameters parameters)
    {
        base.OnInitialize(parameters);
    }

    public override void OnNavigatedTo(INavigationParameters navigationParameters)
    {
        base.OnNavigatedTo(navigationParameters);
    }

    public override void OnNavigatedFrom(INavigationParameters navigationParameters)
    {
        base.OnNavigatedFrom(navigationParameters);
    }


    private void Navigate()
    {
        var modelA = new ClientModel(id: 1, name: "Contributor User");
        var modelB = new ProductModel(id:2, name: "Windows Phone 11 [Joke]");

        var navigationParameters = modelA.ToNavigationParameter()
                                         .Including(modelB);
        //You can include as many parameter as you want using the .Including() method
        //Take care to do not add parameters with the same type cause on next page you
        //will use the type of the parameter to get it's values.
        //If you need multiple parameters of the same type create your own complex type like
        // the ModelA that's being instantiated on line 23,

        _navigationService.NavigateAsync<FeaturePage2ViewModel>(navigationParameters);
    }
}