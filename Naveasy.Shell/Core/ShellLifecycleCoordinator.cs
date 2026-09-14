using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Naveasy.Common;
using Naveasy.Core;
using Naveasy.Extensions;

namespace Naveasy.Shell.Core;

/// <summary>
/// Bridges the Shell navigation events to the Naveasy lifecycle: IInitialize/IInitializeAsync, INavigatedAware,
/// IConfirmNavigation and the disposal of the pages that leave the navigation stack.
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
            await ActivateAsync(state, currentPage, PrepareParameters(null, NavigationMode.New));
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Naveasy could not activate the root page {Page}.", currentPage);
        }
        finally
        {
            state.PagesBeforeNavigation = GetPages(shell);
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
        state.PagesBeforeNavigation = GetPages(shell);
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

            await ActivateAsync(state, currentPage, parameters);

            DestroyRemovedPages(shell, state, currentPage);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Naveasy could not complete the navigation to {Page}.", currentPage);
        }
        finally
        {
            state.NavigatingFrom = null;
            state.PendingQuery = [];
            state.PagesBeforeNavigation = GetPages(shell);
        }
    }

    private async void OnShellLoaded(object sender, EventArgs e)
    {
        if (sender is MauiShell shell)
            await EnsureCurrentPageActivatedAsync(shell);
    }

    private async Task ActivateAsync(ShellState state, Page page, INavigationParameters parameters)
    {
        if (page is null || ReferenceEquals(state.ActivatedPage, page))
            return;

        // Assigned before the first await so that a concurrent activation of the same page is a no-op.
        state.ActivatedPage = page;

        _pageFactory.EnsureViewModelAttached(page);

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

    private static void DestroyRemovedPages(MauiShell shell, ShellState state, Page currentPage)
    {
        var remainingPages = GetPages(shell);

        // A page can show up more than once in the snapshot - Shell does not always move a modal page out of
        // NavigationStack (dotnet/maui#12162) - and it must never be destroyed twice.
        foreach (var page in state.PagesBeforeNavigation.Distinct())
        {
            if (ReferenceEquals(page, currentPage) || remainingPages.Contains(page))
                continue;

            LifecycleInvoker.DestroyPage(page);
        }
    }

    /// <summary>
    /// Every page that is alive anywhere on the Shell. Shell.Navigation only exposes the stack of the section the
    /// app is on, so the stack of every other section is read as well: switching tabs must not destroy the pages
    /// of the section left behind, but an absolute navigation that empties that stack must. The first item of
    /// NavigationStack is always null (dotnet/maui#12162) and modal pages may live in either stack, so both are
    /// read and nulls are discarded.
    /// </summary>
    private static List<Page> GetPages(MauiShell shell)
    {
        var pages = new List<Page>();
        var navigation = shell.Navigation;

        if (navigation is not null)
        {
            pages.AddRange(navigation.NavigationStack.Where(page => page is not null));
            pages.AddRange(navigation.ModalStack.Where(page => page is not null));
        }

        foreach (var item in shell.Items)
        {
            foreach (var section in item.Items)
                pages.AddRange(section.Stack.Where(page => page is not null));
        }

        return pages;
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
        public List<Page> PagesBeforeNavigation { get; set; } = [];

        public IReadOnlyList<KeyValuePair<string, object>> PendingQuery { get; set; } = [];

        public Page NavigatingFrom { get; set; }

        public Page ActivatedPage { get; set; }
    }
}
