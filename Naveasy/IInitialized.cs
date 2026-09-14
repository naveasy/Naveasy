namespace Naveasy;

[Obsolete("This interface will be removed on future versions. Use Naveasy.IInitializeAsync instead.")]
public interface IInitializedAsync
{
    [Obsolete("This method will be removed on future versions. Please implement Naveasy.IInitializeAsync and use Task OnInitialize(INavigationParameters parameters).")]
    Task OnInitializedAsync(INavigationParameters parameters);
}

[Obsolete("This interface will be removed on future versions. Use Naveasy.IInitialize instead.")]
public interface IInitialized
{
    [Obsolete("This method will be removed on future versions. Please implement Naveasy.IInitializeAsync and use Task OnInitialize(INavigationParameters parameters).")]
    void OnInitialized(INavigationParameters parameters);
}
