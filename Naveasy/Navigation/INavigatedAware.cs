namespace Naveasy.Navigation;

public interface INavigatedAware
{
    void OnNavigatedFrom(INavigationParameters navigationParameters);
    void OnNavigatedTo(INavigationParameters navigationParameters);
}