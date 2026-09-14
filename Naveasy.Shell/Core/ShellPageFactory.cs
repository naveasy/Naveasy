using Microsoft.Extensions.Logging;
using Naveasy.Core;
using Naveasy.Shell.Extensions;

namespace Naveasy.Shell.Core;

public interface IShellPageFactory
{
    /// <summary>
    /// Creates the page mapped to <paramref name="viewModelType"/> with its own dependency injection scope.
    /// </summary>
    Page CreatePage(Type viewModelType);

    /// <summary>
    /// Attaches the registered ViewModel, the page lifetime scope and the Naveasy behaviors to a page that was
    /// created by Shell itself - a ShellContent declared in the Shell visual hierarchy, for instance.
    /// </summary>
    void EnsureViewModelAttached(Page page);
}

internal sealed class ShellPageFactory : IShellPageFactory
{
    private readonly IPageScopeService _pageScopeService;
    private readonly ShellNavigationRequestStore _requestStore;
    private readonly ILogger<ShellPageFactory> _logger;

    public ShellPageFactory(IPageScopeService pageScopeService, ShellNavigationRequestStore requestStore, ILogger<ShellPageFactory> logger)
    {
        _pageScopeService = pageScopeService;
        _requestStore = requestStore;
        _logger = logger;
    }

    public Page CreatePage(Type viewModelType) => CreatePage(ShellRouteRegistry.GetRegistration(viewModelType));

    internal Page CreatePage(ShellRouteRegistration registration)
    {
        var scope = _pageScopeService.BeginPageLifetimeScope();

        try
        {
            var page = BuildPage(scope, registration);

            ApplyPendingPresentationMode(page);

            return page;
        }
        catch (Exception ex)
        {
            scope.Dispose();

            var message = ShellErrorMessages.PageCreationFailed(registration.ViewType, registration.ViewModelType, registration.Route);
            _logger?.LogError(ex, message);

            throw new InvalidOperationException(message, ex);
        }
    }

    public void EnsureViewModelAttached(Page page)
    {
        if (page is null)
            return;

        if (page is TabbedPage tabbedPage)
        {
            foreach (var child in tabbedPage.Children)
                EnsureViewModelAttached(child);
        }

        if (!ShellRouteRegistry.TryGetRegistrationByView(page.GetType(), out var registration))
            return;

        if (page.BindingContext is not null && registration.ViewModelType.IsInstanceOfType(page.BindingContext))
        {
            // The page may have been given its ViewModel outside of Naveasy - in XAML, for instance - and then
            // no page lifetime scope owns either of them.
            if (NavigationAttached.GetDisposalPolicy(page) is null)
                NavigationAttached.SetDisposalPolicy(page, GetDisposalPolicy(registration, isViewModelOwnedByScope: false));

            page.ApplyBehaviors();
            return;
        }

        var scope = NavigationAttached.GetLifetimeScope(page);

        if (scope is null)
        {
            scope = _pageScopeService.BeginPageLifetimeScope();
            NavigationAttached.SetLifetimeScope(page, scope);
        }

        // Shell - or XAML - created this page, so the scope that resolves the ViewModel knows nothing about the
        // View itself and Naveasy has to dispose it when the page leaves the navigation stack.
        NavigationAttached.SetDisposalPolicy(page, GetDisposalPolicy(registration, isViewModelOwnedByScope: true));

        try
        {
            page.BindingContext = scope.ServiceProvider.GetRequiredService(registration.ViewModelType);
            page.ApplyBehaviors();
        }
        catch (Exception ex)
        {
            var message = ShellErrorMessages.PageCreationFailed(registration.ViewType, registration.ViewModelType, registration.Route);
            _logger?.LogError(ex, message);

            throw new InvalidOperationException(message, ex);
        }
    }

    private Page BuildPage(IServiceScope scope, ShellRouteRegistration registration)
    {
        var viewModel = scope.ServiceProvider.GetRequiredService(registration.ViewModelType);

        if (scope.ServiceProvider.GetRequiredService(registration.ViewType) is not Page page)
            throw new InvalidOperationException(ShellErrorMessages.ViewIsNotAPage(registration.ViewType, registration.ViewModelType));

        page.BindingContext = viewModel;
        NavigationAttached.SetLifetimeScope(page, scope);

        // Both came out of the page lifetime scope, which disposes what it created and leaves singletons alone.
        NavigationAttached.SetDisposalPolicy(page, PageDisposalPolicy.None);

        page.ApplyBehaviors();

        if (page is TabbedPage tabbedPage)
        {
            foreach (var child in tabbedPage.Children)
                EnsureViewModelAttached(child);
        }

        return page;
    }

    /// <summary>
    /// What Naveasy must dispose on a page it did not fully create. A singleton is owned by the container for the
    /// whole life of the app and is therefore left out.
    /// </summary>
    private static PageDisposalPolicy GetDisposalPolicy(ShellRouteRegistration registration, bool isViewModelOwnedByScope)
    {
        var policy = PageDisposalPolicy.None;

        if (registration.ViewLifetime != ServiceLifetime.Singleton)
            policy |= PageDisposalPolicy.View;

        if (!isViewModelOwnedByScope && registration.ViewModelLifetime != ServiceLifetime.Singleton)
            policy |= PageDisposalPolicy.ViewModel;

        return policy;
    }

    private void ApplyPendingPresentationMode(Page page)
    {
        var request = _requestStore.Peek(MauiShell.Current);

        if (request is not { IsModal: true })
            return;

        MauiShell.SetPresentationMode(page, request.Animated
            ? PresentationMode.ModalAnimated
            : PresentationMode.ModalNotAnimated);

        ShellAttached.SetIsModal(page, true);
    }
}
