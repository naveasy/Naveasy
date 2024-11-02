namespace Naveasy;

public interface IInitializeAsync
{
    Task OnInitializeAsync(INavigationParameters parameters);
}

public interface IInitialize
{
    void OnInitialize(INavigationParameters parameters);
}

public interface IInitializedAsync
{
    Task OnInitializedAsync(INavigationParameters parameters);
}

public interface IInitialized
{
    void OnInitialized(INavigationParameters parameters);
}