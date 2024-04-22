namespace Naveasy;

public interface IInitializeAsync
{
    Task OnInitializeAsync(INavigationParameters parameters);
}

public interface IInitialize
{
    void OnInitialize(INavigationParameters parameters);
}