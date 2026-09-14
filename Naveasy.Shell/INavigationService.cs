namespace Naveasy.Shell;

/// <summary>
/// ViewModel to ViewModel navigation on top of .NET MAUI Shell.
/// Every ViewModel is mapped to a Shell route by <see cref="ShellRouteRegistry"/>, so pages are never
/// referenced directly and routes never leak into the ViewModels.
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Gets or sets whether the Shell flyout is presented.
    /// </summary>
    bool IsFlyoutOpen { get; set; }

    /// <summary>
    /// Gets or sets how the Shell flyout behaves.
    /// </summary>
    FlyoutBehavior FlyoutBehavior { get; set; }

    /// <summary>
    /// Pushes the page mapped to <typeparamref name="TViewModel"/> on top of the current Shell navigation stack.
    /// </summary>
    Task<INavigationResult> NavigateAsync<TViewModel>(INavigationParameters parameters = null, bool animated = true);

    /// <summary>
    /// Pushes the page mapped to <typeparamref name="TViewModel"/> modally, on top of everything else.
    /// </summary>
    Task<INavigationResult> NavigateModalAsync<TViewModel>(INavigationParameters parameters = null, bool animated = true);

    /// <summary>
    /// Navigates to the absolute route of <typeparamref name="TViewModel"/>, resetting the navigation stack.
    /// </summary>
    Task<INavigationResult> NavigateAbsoluteAsync<TViewModel>(INavigationParameters parameters = null, bool animated = true);

    /// <summary>
    /// Pushes the page mapped to <typeparamref name="TViewModel"/> and removes the page it was pushed from.
    /// </summary>
    Task<INavigationResult> NavigateAndPopPreviousAsync<TViewModel>(INavigationParameters parameters = null, bool animated = true);

    /// <summary>
    /// Selects the Shell item (tab or flyout item) that hosts <typeparamref name="TViewModel"/>.
    /// </summary>
    Task<INavigationResult> SelectTabAsync<TViewModel>(INavigationParameters parameters = null);

    /// <summary>
    /// Navigates back one page.
    /// </summary>
    Task<INavigationResult> GoBackAsync(INavigationParameters parameters = null, bool animated = true);

    /// <summary>
    /// Navigates back to the root page of the current Shell section.
    /// </summary>
    Task<INavigationResult> GoBackToRootAsync(INavigationParameters parameters = null, bool animated = true);
}
