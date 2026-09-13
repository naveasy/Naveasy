namespace Naveasy.Shell.Core;

/// <summary>
/// Translates the ViewModel to ViewModel navigation API into Shell routes.
/// </summary>
/// <remarks>
/// Setup mistakes - a ViewModel without a route, or an app without a Shell - are thrown, because they are bugs
/// that must fail as early as possible. Navigation failures are returned as a failed <see cref="INavigationResult"/>.
/// </remarks>
internal sealed class ShellNavigationService : INavigationService
{
    private const string BackRoute = "..";

    private readonly ShellNavigationRequestStore _requestStore;

    public ShellNavigationService(ShellNavigationRequestStore requestStore) => _requestStore = requestStore;

    public bool IsFlyoutOpen
    {
        get => GetShell().FlyoutIsPresented;
        set => GetShell().FlyoutIsPresented = value;
    }

    public FlyoutBehavior FlyoutBehavior
    {
        get => GetShell().FlyoutBehavior;
        set => GetShell().FlyoutBehavior = value;
    }

    public Task<INavigationResult> NavigateAsync<TViewModel>(INavigationParameters parameters = null, bool animated = true) =>
        GoToAsync(ResolveRoute<TViewModel>(), parameters, animated, isModal: false);

    public Task<INavigationResult> NavigateModalAsync<TViewModel>(INavigationParameters parameters = null, bool animated = true) =>
        GoToAsync(ResolveRoute<TViewModel>(), parameters, animated, isModal: true);

    public Task<INavigationResult> NavigateAbsoluteAsync<TViewModel>(INavigationParameters parameters = null, bool animated = true) =>
        GoToAsync($"//{ResolveRoute<TViewModel>()}", parameters, animated, isModal: false);

    public Task<INavigationResult> NavigateAndPopPreviousAsync<TViewModel>(INavigationParameters parameters = null, bool animated = true) =>
        GoToAsync($"{BackRoute}/{ResolveRoute<TViewModel>()}", parameters, animated, isModal: false);

    public Task<INavigationResult> SelectTabAsync<TViewModel>(INavigationParameters parameters = null) =>
        NavigateAbsoluteAsync<TViewModel>(parameters, animated: false);

    public Task<INavigationResult> GoBackAsync(INavigationParameters parameters = null, bool animated = true)
    {
        var shell = GetShell();

        if (GetBackStackDepth(shell) == 0)
        {
            var location = shell.CurrentState?.Location?.ToString() ?? shell.CurrentPage?.GetType().Name;

            return Task.FromResult<INavigationResult>(
                new NavigationResult(new InvalidOperationException(ShellErrorMessages.CannotGoBackFromRoot(location))));
        }

        return GoToAsync(BackRoute, parameters, animated, isModal: false);
    }

    public Task<INavigationResult> GoBackToRootAsync(INavigationParameters parameters = null, bool animated = true)
    {
        var shell = GetShell();
        var depth = GetBackStackDepth(shell);

        if (depth == 0)
            return Task.FromResult<INavigationResult>(new NavigationResult(true));

        var route = string.Join("/", Enumerable.Repeat(BackRoute, depth));

        return GoToAsync(route, parameters, animated, isModal: false);
    }

    private static string ResolveRoute<TViewModel>() => ShellRouteRegistry.ResolveRoute(typeof(TViewModel));

    private static MauiShell GetShell() =>
        MauiShell.Current ?? throw new InvalidOperationException(ShellErrorMessages.NoActiveShell());

    /// <summary>
    /// How many pages can still be popped from the current Shell section. The first item of NavigationStack is
    /// always null (dotnet/maui#12162) and modal pages may live in either stack.
    /// </summary>
    private static int GetBackStackDepth(MauiShell shell)
    {
        var navigation = shell.Navigation;

        if (navigation is null)
            return 0;

        return navigation.NavigationStack.Count(page => page is not null) + navigation.ModalStack.Count;
    }

    private async Task<INavigationResult> GoToAsync(string route, INavigationParameters parameters, bool animated, bool isModal)
    {
        var shell = GetShell();

        _requestStore.SetPending(shell, new ShellNavigationRequest
        {
            Parameters = parameters,
            IsModal = isModal,
            Animated = animated
        });

        try
        {
            await shell.GoToAsync(route, animated);

            return new NavigationResult(true);
        }
        catch (InvalidOperationException ex)
        {
            _requestStore.Clear(shell);

            // Shell throws this when a navigation deferral is still pending, with a message that does not help
            // whoever is reading the log.
            return new NavigationResult(new InvalidOperationException(ShellErrorMessages.PendingNavigation(route), ex));
        }
        catch (Exception ex)
        {
            _requestStore.Clear(shell);

            return new NavigationResult(ex);
        }
    }
}
