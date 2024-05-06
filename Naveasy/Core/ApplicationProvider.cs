namespace Naveasy.Core;

public interface IApplicationProvider
{
    INavigation Navigation { get; }
    Page MainPage { get; }
    List<Page> PagesToRemove { get; }
}

public class ApplicationProvider : IApplicationProvider
{
    public Page MainPage => Application.Current?.MainPage;

    public INavigation Navigation => MainPage is FlyoutPage flyoutPage
        ? flyoutPage.Detail is NavigationPage 
            ? flyoutPage.Detail.Navigation
            : null
        : MainPage.Navigation;

    public List<Page> PagesToRemove => MainPage switch
    {
        FlyoutPage x => x.Detail is NavigationPage
            ? x.Detail.Navigation.NavigationStack.ToList()
            : [x.Detail],
        NavigationPage x => x.Navigation.NavigationStack.ToList(),
        _ => [MainPage]
    };
}