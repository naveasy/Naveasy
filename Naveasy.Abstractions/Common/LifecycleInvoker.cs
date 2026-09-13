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
    /// Disposes the View and the ViewModel, releases the page lifetime scope and detaches the page from its BindingContext.
    /// </summary>
    /// <remarks>
    /// When the page has a Naveasy lifetime scope, the scope is what disposes the View and the ViewModel: the
    /// service container already tracks every IDisposable it created, transient ones included, and disposing them
    /// here as well would call Dispose twice.
    /// </remarks>
    public static void DestroyPage(IView view)
    {
        if (view is null) return;

        try
        {
            var page = view as Page;
            var scope = page is not null ? NavigationAttached.GetLifetimeScope(page) : null;

            if (scope is null)
                InvokeViewAndViewModelAction<IDisposable>(view, v => v.Dispose());

            if (page is null)
                return;

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
}
