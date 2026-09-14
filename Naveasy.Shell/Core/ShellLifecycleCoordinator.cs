using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Naveasy.Common;
using Naveasy.Core;
using Naveasy.Extensions;

namespace Naveasy.Shell.Core;

/// <summary>
/// Bridges the Shell navigation events to the Naveasy lifecycle: IInitialize/IInitializeAsync, INavigatedAware,
/// IConfirmNavigation and the disposal of the pages Shell no longer keeps.
/// </summary>
public sealed class ShellLifecycleCoordinator
{
    private readonly IShellPageFactory _pageFactory;
    private readonly ShellNavigationRequestStore _requestStore;
    private readonly ILogger<ShellLifecycleCoordinator> _logger;
    private readonly ConditionalWeakTable<MauiShell, ShellState> _states = [];

    public ShellLifecycleCoordinator(IShellPageFactory pageFactory, ShellNavigationRequestStore requestStore, ILogger<ShellLifecycleCoordinator> logger)
    {
        _pageFactory = pageFactory;
        _requestStore = requestStore;
        _logger = logger;
    }

    /// <summary>
    /// Starts driving the Naveasy lifecycle for <paramref name="shell"/>. It is called by the window creator for
    /// the Shell registered through UseNaveasyShell, and is public for apps that build their Shell differently.
    /// </summary>
    public void Attach(MauiShell shell)
    {
        ArgumentNullException.ThrowIfNull(shell);

        Unsubscribe(shell);

        shell.Navigating += OnNavigating;
        shell.Navigated += OnNavigated;
        shell.Loaded += OnShellLoaded;
    }

    /// <summary>
    /// Stops driving the Naveasy lifecycle for <paramref name="shell"/>.
    /// </summary>
    public void Detach(MauiShell shell)
    {
        if (shell is null)
            return;

        Unsubscribe(shell);

        if (_states.TryGetValue(shell, out var state))
        {
            foreach (var page in state.TrackedPages)
                page.Unloaded -= OnTrackedPageUnloaded;

            state.TrackedPages.Clear();
        }

        _states.Remove(shell);
    }

    /// <summary>
    /// Runs the lifecycle of the page Shell is currently showing when it has not been activated yet.
    /// Shell coalesces the navigation events raised while the app is starting, so the root page is activated here.
    /// </summary>
    public async Task EnsureCurrentPageActivatedAsync(MauiShell shell)
    {
        ArgumentNullException.ThrowIfNull(shell);

        var state = _states.GetOrCreateValue(shell);
        var currentPage = shell.CurrentPage;

        if (currentPage is null || ReferenceEquals(state.ActivatedPage, currentPage))
            return;

        try
        {
            await ActivateAsync(shell, state, currentPage, PrepareParameters(null, NavigationMode.New));
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Naveasy could not activate the root page {Page}.", currentPage);
        }
    }

    private void Unsubscribe(MauiShell shell)
    {
        shell.Navigating -= OnNavigating;
        shell.Navigated -= OnNavigated;
        shell.Loaded -= OnShellLoaded;
    }

    private async void OnNavigating(object sender, ShellNavigatingEventArgs e)
    {
        if (sender is not MauiShell shell)
            return;

        var state = _states.GetOrCreateValue(shell);
        state.NavigatingFrom = shell.CurrentPage;

        // Only the target state of this event carries the query string of a deep link.
        state.PendingQuery = ShellQueryString.Parse(e.Target);

        // GetDeferral returns null when the navigation cannot be cancelled - the very first navigation, for instance.
        var deferral = e.CanCancel ? e.GetDeferral() : null;

        try
        {
            if (state.NavigatingFrom is null)
                return;

            // The guard gets a copy: the parameters of the request still belong to the caller and are only
            // given their navigation mode when the navigation actually completes.
            var parameters = CopyWithMode(_requestStore.Peek(shell)?.Parameters, IsBack(e.Source) ? NavigationMode.Back : NavigationMode.New);
            ShellQueryString.MergeInto(parameters, state.PendingQuery);

            if (await CanNavigateAsync(state.NavigatingFrom, parameters))
                return;

            if (e.CanCancel)
                e.Cancel();

            // The navigation was vetoed, so its parameters must not leak into the next one.
            _requestStore.Clear(shell);
            state.PendingQuery = [];
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Naveasy could not evaluate the navigation guards of {Page}.", state.NavigatingFrom);
        }
        finally
        {
            deferral?.Complete();
        }
    }

    private async void OnNavigated(object sender, ShellNavigatedEventArgs e)
    {
        if (sender is not MauiShell shell)
            return;

        var state = _states.GetOrCreateValue(shell);
        var request = _requestStore.Consume(shell);
        var isBack = IsBack(e.Source);
        var parameters = PrepareParameters(request?.Parameters, isBack ? NavigationMode.Back : NavigationMode.New);

        // A deep link carries its values in the query string. They reach the ViewModel as navigation parameters,
        // on top of the IQueryAttributable pipeline of Shell, which keeps working untouched.
        ShellQueryString.MergeInto(parameters, state.PendingQuery);

        var currentPage = shell.CurrentPage;
        var previousPage = state.NavigatingFrom;

        try
        {
            if (previousPage is not null && !ReferenceEquals(previousPage, currentPage))
                InvokeOnNavigatedFrom(previousPage, parameters);

            await ActivateAsync(shell, state, currentPage, parameters);

            DestroyDisconnectedPages(shell, state);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Naveasy could not complete the navigation to {Page}.", currentPage);
        }
        finally
        {
            state.NavigatingFrom = null;
            state.PendingQuery = [];
        }
    }

    private async void OnShellLoaded(object sender, EventArgs e)
    {
        if (sender is MauiShell shell)
            await EnsureCurrentPageActivatedAsync(shell);
    }

    /// <summary>
    /// Shell drops the page of a ShellContent it created through the service container as soon as the app moves
    /// to another Shell item, and it does so while handling this very event. The pages are swept right after, so
    /// that a page Shell threw away is disposed instead of silently outliving its ViewModel and its scope.
    /// </summary>
    private void OnTrackedPageUnloaded(object sender, EventArgs e)
    {
        if (sender is not Page page)
            return;

        var dispatcher = page.Dispatcher;

        if (dispatcher is null)
            SweepEveryShell();
        else
            dispatcher.Dispatch(SweepEveryShell);
    }

    private void SweepEveryShell()
    {
        try
        {
            foreach (var pair in _states)
                DestroyDisconnectedPages(pair.Key, pair.Value);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Naveasy could not dispose the pages Shell has disconnected.");
        }
    }

    private async Task ActivateAsync(MauiShell shell, ShellState state, Page page, INavigationParameters parameters)
    {
        if (page is null || ReferenceEquals(state.ActivatedPage, page))
            return;

        // Assigned before the first await so that a concurrent activation of the same page is a no-op.
        state.ActivatedPage = page;

        _pageFactory.EnsureViewModelAttached(page);
        Track(state, page);

        foreach (var target in GetLifecycleTargets(page))
        {
            if (!ShellAttached.GetIsInitialized(target))
            {
                ShellAttached.SetIsInitialized(target, true);
                await LifecycleInvoker.OnInitializeAsync(target, parameters);
            }

            LifecycleInvoker.OnNavigatedTo(target, parameters);
        }
    }

    /// <summary>
    /// Remembers a page so that it can be disposed once Shell stops holding it.
    /// </summary>
    private void Track(ShellState state, Page page)
    {
        if (state.TrackedPages.Contains(page))
            return;

        state.TrackedPages.Add(page);
        page.Unloaded += OnTrackedPageUnloaded;
    }

    /// <summary>
    /// Disposes every page Naveasy activated that Shell no longer holds: a page popped from a navigation stack,
    /// a modal page that was closed, and the page of a ShellContent that Shell disconnected.
    /// </summary>
    private void DestroyDisconnectedPages(MauiShell shell, ShellState state)
    {
        if (state.TrackedPages.Count == 0)
            return;

        var alivePages = GetAlivePages(shell);

        for (var index = state.TrackedPages.Count - 1; index >= 0; index--)
        {
            var page = state.TrackedPages[index];

            if (alivePages.Contains(page))
                continue;

            state.TrackedPages.RemoveAt(index);
            page.Unloaded -= OnTrackedPageUnloaded;

            if (ReferenceEquals(state.ActivatedPage, page))
                state.ActivatedPage = null;

            DestroyPageTree(page);
        }
    }

    private static void DestroyPageTree(Page page)
    {
        // The children of a TabbedPage have a ViewModel and a scope of their own.
        if (page is TabbedPage tabbedPage)
        {
            foreach (var child in tabbedPage.Children.Reverse())
                DestroyPageTree(child);
        }

        LifecycleInvoker.DestroyPage(page);
    }

    private static void InvokeOnNavigatedFrom(Page page, INavigationParameters parameters)
    {
        foreach (var target in GetLifecycleTargets(page))
            LifecycleInvoker.OnNavigatedFrom(target, parameters);
    }

    private static async Task<bool> CanNavigateAsync(Page page, INavigationParameters parameters)
    {
        var canNavigate = true;

        foreach (var target in GetLifecycleTargets(page))
        {
            await LifecycleInvoker.InvokeViewAndViewModelActionAsync<IConfirmNavigation>(target, async guard =>
            {
                if (canNavigate)
                    canNavigate = await guard.CanNavigateAsync(parameters);
            });
        }

        return canNavigate;
    }

    /// <summary>
    /// Every page Shell is still holding.
    /// </summary>
    /// <remarks>
    /// Shell.Navigation only exposes the stack of the section the app is on, so every section is read instead:
    /// switching tabs must not dispose the pages of the section left behind. The root page of a ShellContent is
    /// never on a stack - the first item of a stack is always null (dotnet/maui#12162) and the page itself is
    /// cached by the ShellContent - so it is read from the ShellContent, which is also how a page Shell
    /// disconnected becomes visible to Naveasy.
    /// </remarks>
    private static HashSet<Page> GetAlivePages(MauiShell shell)
    {
        var pages = new HashSet<Page>();

        AddPages(pages, shell.Navigation?.NavigationStack);
        AddPages(pages, shell.Navigation?.ModalStack);

        foreach (var item in shell.Items)
        {
            foreach (var section in item.Items)
            {
                AddPages(pages, section.Stack);

                foreach (var content in section.Items)
                {
                    if (((IShellContentController)content).Page is { } contentPage)
                        pages.Add(contentPage);
                }
            }
        }

        return pages;
    }

    private static void AddPages(HashSet<Page> pages, IEnumerable<Page> source)
    {
        if (source is null)
            return;

        foreach (var page in source)
        {
            if (page is not null)
                pages.Add(page);
        }
    }

    /// <summary>
    /// A page receives the lifecycle hooks, and a TabbedPage forwards them to the tab that is currently selected.
    /// </summary>
    private static IEnumerable<Page> GetLifecycleTargets(Page page)
    {
        if (page is null)
            yield break;

        yield return page;

        if (page is TabbedPage { CurrentPage: { } currentChild })
            yield return currentChild;
    }

    private static bool IsBack(ShellNavigationSource source) =>
        source is ShellNavigationSource.Pop or ShellNavigationSource.PopToRoot;

    private static INavigationParameters CopyWithMode(INavigationParameters parameters, NavigationMode mode)
    {
        var copy = new NavigationParameters();

        if (parameters is not null)
        {
            foreach (var parameter in parameters)
                copy.Add(parameter.Key, parameter.Value);
        }

        copy.GetNavigationParametersInternal().Add(KnownInternalParameters.NavigationMode, mode);

        return copy;
    }

    private static INavigationParameters PrepareParameters(INavigationParameters parameters, NavigationMode mode)
    {
        var result = parameters ?? new NavigationParameters();
        var internalParameters = result.GetNavigationParametersInternal();

        // The caller may reuse the same instance for more than one navigation. The mode of a previous navigation
        // cannot be overwritten, so a copy is made instead of reporting a stale navigation mode.
        if (internalParameters.ContainsKey(KnownInternalParameters.NavigationMode))
        {
            var copy = new NavigationParameters();

            foreach (var parameter in result)
                copy.Add(parameter.Key, parameter.Value);

            result = copy;
            internalParameters = copy.GetNavigationParametersInternal();
        }

        internalParameters.Add(KnownInternalParameters.NavigationMode, mode);

        return result;
    }

    private sealed class ShellState
    {
        public List<Page> TrackedPages { get; } = [];

        public IReadOnlyList<KeyValuePair<string, object>> PendingQuery { get; set; } = [];

        public Page NavigatingFrom { get; set; }

        public Page ActivatedPage { get; set; }
    }
}
