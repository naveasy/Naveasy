namespace Naveasy;

public interface INavigatedAware
{
    void OnNavigatedFrom(INavigationParameters navigationParameters);
    void OnNavigatedTo(INavigationParameters navigationParameters);
}