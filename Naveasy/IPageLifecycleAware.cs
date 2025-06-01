namespace Naveasy;

public interface IPageLifecycleAware
{
    /// <summary>
    /// Gets called when your page is appearing
    /// </summary>
    void OnAppearing();

    /// <summary>
    /// Gets called when your page is disappearing
    /// </summary>
    void OnDisappearing();
}