namespace Naveasy;

public class NaveasyNavigationPage(Page page) : NavigationPage(page)
{
    protected sealed override bool OnBackButtonPressed()
    {
        var navigationService = IPlatformApplication.Current!.Services.GetRequiredService<INavigationService>();
        navigationService.GoBackAsync().ConfigureAwait(false);
        return true;
    }
}
