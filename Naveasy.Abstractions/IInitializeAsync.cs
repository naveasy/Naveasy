namespace Naveasy;

/// <summary>
/// This gets called only once, when the view gets created for the first time
/// </summary>
public interface IInitializeAsync
{
    Task OnInitializeAsync(INavigationParameters parameters);
}

/// <summary>
/// This gets called only once, when the view gets created for the first time
/// </summary>
public interface IInitialize
{
    void OnInitialize(INavigationParameters parameters);
}
