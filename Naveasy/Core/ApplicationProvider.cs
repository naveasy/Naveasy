namespace Naveasy.Core;

public interface IApplicationProvider
{
    INavigation Navigation { get; }
    Page MainPage { get; }
}

public class ApplicationProvider : IApplicationProvider
{
    public Page MainPage => Application.Current?.MainPage;

    public INavigation Navigation => MainPage is FlyoutPage flyoutPage
        ? flyoutPage.Detail.Navigation
        : MainPage.Navigation;
}