namespace Naveasy.Shell;

/// <summary>
/// Implemented by a View or a ViewModel that can veto a navigation away from its page.
/// It is evaluated for every navigation source, including the Shell back button, the iOS swipe back
/// gesture and the Android hardware back button.
/// </summary>
/// <remarks>
/// The implementation must not start a navigation of its own: Shell is holding a navigation deferral while
/// <see cref="CanNavigateAsync"/> runs and any GoToAsync issued from here fails.
/// </remarks>
public interface IConfirmNavigation
{
    /// <summary>
    /// Returns <c>true</c> to let the navigation continue, or <c>false</c> to cancel it.
    /// </summary>
    Task<bool> CanNavigateAsync(INavigationParameters parameters);
}
