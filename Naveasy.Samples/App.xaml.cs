using Naveasy.Samples.ViewModels;

namespace Naveasy.Samples;

public partial class App : Application
{
    public App(INavigationService navigationService)
    {
        InitializeComponent();
        navigationService.NavigateAsync<LoginPageViewModel>();
    }
}
