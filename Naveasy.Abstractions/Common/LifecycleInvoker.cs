using Naveasy.Core;

namespace Naveasy.Common;

/// <summary>
/// Invokes the Naveasy lifecycle hooks on a View and on its BindingContext.
/// It is navigation-stack agnostic: it is shared by the NavigationPage based engine (Naveasy)
/// and by the Shell based engine (Naveasy.Shell).
/// </summary>
public static class LifecycleInvoker
{
    /// <summary>
    /// Invokes <paramref name="action"/> on the view and on its BindingContext when they implement <typeparamref name="T"/>.
    /// </summary>
    public static void InvokeViewAndViewModelAction<T>(object view, Action<T> action) where T : class
    {
        if (view is T viewAsT)
        {
            action(viewAsT);
        }

        if (view is BindableObject { BindingContext: T viewModelAsT })
        {
            action(viewModelAsT);
        }
    }

    /// <summary>
    /// Invokes the asynchronous <paramref name="action"/> on the view and on its BindingContext when they implement <typeparamref name="T"/>.
    /// </summary>
    public static async Task InvokeViewAndViewModelActionAsync<T>(object view, Func<T, Task> action) where T : class
    {
        if (view is T viewAsT)
        {
            await action(viewAsT);
        }

        if (view is BindableObject { BindingContext: T viewModelAsT })
        {
            await action(viewModelAsT);
        }
    }

    /// <summary>
    /// Raises <see cref="IInitialize"/> and <see cref="IInitializeAsync"/>. Must be called only once per page.
    /// </summary>
    public static async Task OnInitializeAsync(object page, INavigationParameters parameters)
    {
        if (page is null) return;

        InvokeViewAndViewModelAction<IInitialize>(page, v => v.OnInitialize(parameters));
        await InvokeViewAndViewModelActionAsync<IInitializeAsync>(page, v => v.OnInitializeAsync(parameters));
    }

    /// <summary>
    /// Raises <see cref="INavigatedAware.OnNavigatedTo"/>.
    /// </summary>
    public static void OnNavigatedTo(object page, INavigationParameters parameters)
    {
        if (page is null) return;

        InvokeViewAndViewModelAction<INavigatedAware>(page, v => v.OnNavigatedTo(parameters));
    }

    /// <summary>
    /// Raises <see cref="INavigatedAware.OnNavigatedFrom"/>.
    /// </summary>
    public static void OnNavigatedFrom(object page, INavigationParameters parameters)
    {
        if (page is null) return;

        InvokeViewAndViewModelAction<INavigatedAware>(page, v => v.OnNavigatedFrom(parameters));
    }

    /// <summary>
    /// Disposes the View, the ViewModel and the hosted pages of <paramref name="view"/>, releases the page
    /// lifetime scope and detaches the page from its BindingContext.
    /// </summary>
    /// <remarks>
    /// Whatever the page lifetime scope created is disposed by the scope, which is also how a singleton survives:
    /// the container only disposes the instances it owns, and a singleton belongs to the root provider. Anything
    /// created outside of the scope is disposed here, as told by <see cref="NavigationAttached.GetDisposalPolicy"/>.
    /// </remarks>
    public static void DestroyPage(IView view)
    {
        if (view is null) return;

        try
        {
            DestroyHostedPages(view);

            var page = view as Page;
            var scope = page is not null ? NavigationAttached.GetLifetimeScope(page) : null;

            // A page Naveasy never created has no policy, and then nothing else owns its View and its ViewModel.
            var policy = (page is not null ? NavigationAttached.GetDisposalPolicy(page) : null)
                         ?? (scope is null ? PageDisposalPolicy.ViewAndViewModel : PageDisposalPolicy.None);

            if (policy.HasFlag(PageDisposalPolicy.View) && view is IDisposable disposableView)
                disposableView.Dispose();

            if (policy.HasFlag(PageDisposalPolicy.ViewModel) && view is BindableObject { BindingContext: IDisposable disposableViewModel })
                disposableViewModel.Dispose();

            if (page is null)
                return;

            // Destroying the same page twice must be harmless: the scope is gone and nothing is left to dispose.
            NavigationAttached.SetDisposalPolicy(page, PageDisposalPolicy.None);
            NavigationAttached.ClearLifetimeScope(page);
            scope?.Dispose();

            page.Behaviors?.Clear();
            page.BindingContext = null;
        }
        catch (Exception ex)
        {
            throw new Exception($"Cannot destroy {view}.", ex);
        }
    }

    /// <summary>
    /// Destroys the pages hosted by <paramref name="view"/>. The children of a TabbedPage, the two halves of a
    /// FlyoutPage and the stack of a NavigationPage leave the navigation together with their host, and each of
    /// them has its own ViewModel and its own page lifetime scope.
    /// </summary>
    private static void DestroyHostedPages(IView view)
    {
        switch (view)
        {
            case FlyoutPage flyoutPage:
                DestroyPage(flyoutPage.Flyout);
                DestroyPage(flyoutPage.Detail);
                break;

            case TabbedPage tabbedPage:
                foreach (var child in tabbedPage.Children.Reverse())
                    DestroyPage(child);
                break;

            case NavigationPage navigationPage:
                foreach (var child in navigationPage.Navigation.NavigationStack.Reverse())
                    DestroyPage(child);
                break;
        }
    }
}
