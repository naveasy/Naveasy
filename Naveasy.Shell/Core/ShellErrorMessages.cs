namespace Naveasy.Shell.Core;

/// <summary>
/// Every message follows the same shape: what failed, why it failed and how to fix it.
/// </summary>
internal static class ShellErrorMessages
{
    public static string PendingNavigation(string route) =>
        $"Navigation to '{route}' failed because a previous navigation is still pending. This happens when an " +
        "IConfirmNavigation guard has not completed, or when a guard itself triggers navigation. Do not navigate " +
        "from inside CanNavigateAsync; await the current navigation before starting a new one.";

    public static string RouteNotRegistered(Type viewModelType)
    {
        var viewName = ConventionalViewName(viewModelType);

        return $"No route is registered for ViewModel '{viewModelType.FullName}'. Register it in MauiProgram with " +
               $"services.AddTransientForNavigation<{viewName}, {viewModelType.Name}>() - or, for a page declared in " +
               $"AppShell.xaml, services.AddShellContentForNavigation<{viewName}, {viewModelType.Name}>(\"<the Route= value used in XAML>\").";
    }

    public static string NotRoutable(Type viewModelType) =>
        $"ViewModel '{viewModelType.FullName}' is registered as a child page - the child of a TabbedPage, for " +
        "instance - and has no route of its own. Navigate to the page that hosts it, or register it with " +
        "AddTransientForNavigation/AddScopedForNavigation to give it a route.";

    public static string ConventionFailed(Type viewModelType) =>
        $"Cannot infer the View for ViewModel '{viewModelType.FullName}' because its name does not end with " +
        "'ViewModel'. Rename it to '<ViewName>ViewModel' or register the pair explicitly with " +
        "AddTransientForNavigation<TView, TViewModel>().";

    public static string ConventionalViewNotFound(Type viewModelType, string expectedViewTypeName) =>
        $"Cannot infer the View for ViewModel '{viewModelType.FullName}' because '{expectedViewTypeName}' does not " +
        "exist in the same assembly. Register the pair explicitly with AddTransientForNavigation<TView, TViewModel>().";

    public static string ViewIsNotAPage(Type viewType, Type viewModelType) =>
        $"'{viewType.FullName}' is registered for ViewModel '{viewModelType.FullName}' but it is not a Page. Shell can " +
        "only navigate to types deriving from Page (for example ContentPage or TabbedPage).";

    public static string DuplicatedRegistration(Type viewModelType, string existingRoute) =>
        $"ViewModel '{viewModelType.FullName}' is already registered for route '{existingRoute}' and cannot be " +
        "registered twice. Remove the duplicated AddTransientForNavigation/AddScopedForNavigation call.";

    public static string DuplicatedRoute(string route, Type viewModelType) =>
        $"Route '{route}' is already registered for ViewModel '{viewModelType.FullName}'. Give one of the pages a " +
        "different route by passing it to AddTransientForNavigation/AddScopedForNavigation.";

    public static string ScopedShellContent(Type viewType) =>
        $"View '{viewType.FullName}' cannot be registered as scoped because .NET MAUI resolves ShellContent pages from " +
        "the root service provider. Register the View with AddShellContentForNavigation (transient); the ViewModel may " +
        "still be scoped.";

    public static string NoActiveShell() =>
        "There is no active Shell. Naveasy.Shell requires a Shell created by UseNaveasyShell<TShell>() - make sure " +
        "UseNaveasyShell is called in MauiProgram, that TShell derives from Shell, and that App.CreateWindow is not " +
        "overridden.";

    public static string PageCreationFailed(Type viewType, Type viewModelType, string route) =>
        $"Unable to create page '{viewType?.FullName}' or ViewModel '{viewModelType?.FullName}' for route '{route}'. " +
        "Check that both types and all of their constructor dependencies are registered in the service container. " +
        "See the inner exception for the failing dependency.";

    public static string CannotGoBackFromRoot(string route) =>
        $"Cannot navigate back from '{route}' because it is the root page of the current Shell section. Use " +
        "NavigateAbsoluteAsync<TViewModel>() to switch the Shell item instead.";

    public static string ConventionalViewName(Type viewModelType)
    {
        var name = viewModelType.Name;

        return name.EndsWith("ViewModel", StringComparison.Ordinal)
            ? name[..^"ViewModel".Length]
            : "TView";
    }
}
