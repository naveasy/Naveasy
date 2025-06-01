namespace Naveasy;

/// <summary>
/// This gets called only once, when the view gets created for the first time
/// </summary>
/// <param name="navigationParameters"></param>
public interface IInitializeAsync
{
    Task OnInitializeAsync(INavigationParameters parameters);
}

/// <summary>
/// This gets called only once, when the view gets created for the first time
/// </summary>
/// <param name="navigationParameters"></param>
public interface IInitialize
{
    void OnInitialize(INavigationParameters parameters);
}

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