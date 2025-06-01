namespace Naveasy;

public interface INavigatedAware
{
    void OnNavigatedTo(INavigationParameters navigationParameters);

    void OnNavigatedFrom(INavigationParameters navigationParameters);
}