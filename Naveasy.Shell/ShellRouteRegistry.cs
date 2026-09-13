using System.Reflection;
using Naveasy.Shell.Core;

namespace Naveasy.Shell;

/// <summary>
/// Maps ViewModels to Views and to Shell routes, and registers both in the service container.
/// </summary>
/// <remarks>
/// Pages are registered as transient or scoped only. A singleton page is not supported on purpose: a singleton
/// page re-pushed after an absolute route reset renders blank on Android (dotnet/maui#37113).
/// </remarks>
public static class ShellRouteRegistry
{
    private static readonly Dictionary<Type, ShellRouteRegistration> RegistrationsByViewModel = [];
    private static readonly Dictionary<Type, ShellRouteRegistration> RegistrationsByView = [];

    /// <summary>
    /// Registers a View and its ViewModel as transient and maps them to a Shell route.
    /// </summary>
    /// <param name="route">The Shell route. Defaults to the name of the View type.</param>
    public static IServiceCollection AddTransientForNavigation<TView, TViewModel>(this IServiceCollection self, string route = null)
        where TView : Page
        where TViewModel : class
    {
        var registration = Register(typeof(TViewModel), typeof(TView), route, ShellRouteKind.Route);

        self.TryAddService(typeof(TView), ServiceLifetime.Transient);
        self.TryAddService(typeof(TViewModel), ServiceLifetime.Transient);

        RegisterRoute(registration);

        return self;
    }

    /// <summary>
    /// Registers a ViewModel as transient, inferring its View by convention: 'FooPageViewModel' is rendered by 'FooPage'.
    /// </summary>
    /// <param name="route">The Shell route. Defaults to the name of the inferred View type.</param>
    public static IServiceCollection AddTransientForNavigation<TViewModel>(this IServiceCollection self, string route = null)
        where TViewModel : class
    {
        var viewType = ResolveViewTypeByConvention(typeof(TViewModel));
        var registration = Register(typeof(TViewModel), viewType, route, ShellRouteKind.Route);

        self.TryAddService(viewType, ServiceLifetime.Transient);
        self.TryAddService(typeof(TViewModel), ServiceLifetime.Transient);

        RegisterRoute(registration);

        return self;
    }

    /// <summary>
    /// Registers a View and its ViewModel as scoped - one scope per page - and maps them to a Shell route.
    /// </summary>
    /// <param name="route">The Shell route. Defaults to the name of the View type.</param>
    public static IServiceCollection AddScopedForNavigation<TView, TViewModel>(this IServiceCollection self, string route = null)
        where TView : Page
        where TViewModel : class
    {
        var registration = Register(typeof(TViewModel), typeof(TView), route, ShellRouteKind.Route);

        self.TryAddService(typeof(TView), ServiceLifetime.Scoped);
        self.TryAddService(typeof(TViewModel), ServiceLifetime.Scoped);

        RegisterRoute(registration);

        return self;
    }

    /// <summary>
    /// Registers a page that is declared as a ShellContent in the Shell visual hierarchy.
    /// The route is not registered with <see cref="Routing"/> because Shell already owns it; pass the very same
    /// value used in the Route attribute of the ShellContent.
    /// </summary>
    /// <remarks>
    /// The View is registered as transient because .NET MAUI resolves ShellContent pages from the root service
    /// provider. The ViewModel may still be scoped: Naveasy creates the page scope itself.
    /// </remarks>
    public static IServiceCollection AddShellContentForNavigation<TView, TViewModel>(this IServiceCollection self, string route)
        where TView : Page
        where TViewModel : class
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(route);

        if (self.Any(descriptor => descriptor.ServiceType == typeof(TView) && descriptor.Lifetime == ServiceLifetime.Scoped))
            throw new ArgumentException(ShellErrorMessages.ScopedShellContent(typeof(TView)));

        Register(typeof(TViewModel), typeof(TView), route, ShellRouteKind.ShellContent);

        self.TryAddService(typeof(TView), ServiceLifetime.Transient);
        self.TryAddService(typeof(TViewModel), ServiceLifetime.Transient);

        return self;
    }

    /// <summary>
    /// Registers a page that is hosted by another page - the children of a TabbedPage, for instance.
    /// No Shell route is registered because the page cannot be navigated to on its own; Naveasy only needs the
    /// mapping to give it its ViewModel, its page scope and the Naveasy lifecycle.
    /// </summary>
    public static IServiceCollection AddChildForNavigation<TView, TViewModel>(this IServiceCollection self)
        where TView : Page
        where TViewModel : class
    {
        Register(typeof(TViewModel), typeof(TView), typeof(TView).Name, ShellRouteKind.Child);

        self.TryAddService(typeof(TView), ServiceLifetime.Transient);
        self.TryAddService(typeof(TViewModel), ServiceLifetime.Transient);

        return self;
    }

    /// <summary>
    /// Returns the Shell route mapped to <paramref name="viewModelType"/>.
    /// </summary>
    public static string ResolveRoute(Type viewModelType)
    {
        var registration = GetRegistration(viewModelType);

        if (!registration.IsRoutable)
            throw new InvalidOperationException(ShellErrorMessages.NotRoutable(viewModelType));

        return registration.Route;
    }

    /// <summary>
    /// Returns the View type mapped to <paramref name="viewModelType"/>.
    /// </summary>
    public static Type ResolveViewType(Type viewModelType) => GetRegistration(viewModelType).ViewType;

    internal static ShellRouteRegistration GetRegistration(Type viewModelType)
    {
        if (RegistrationsByViewModel.TryGetValue(viewModelType, out var registration))
            return registration;

        throw new KeyNotFoundException(ShellErrorMessages.RouteNotRegistered(viewModelType));
    }

    internal static bool TryGetRegistrationByView(Type viewType, out ShellRouteRegistration registration)
    {
        while (viewType is not null && viewType != typeof(Page))
        {
            if (RegistrationsByView.TryGetValue(viewType, out registration))
                return true;

            viewType = viewType.BaseType;
        }

        registration = null;
        return false;
    }

    /// <summary>
    /// Infers the View type of a ViewModel by convention: 'FooPageViewModel' is rendered by 'FooPage'
    /// declared in the same assembly.
    /// </summary>
    internal static Type ResolveViewTypeByConvention(Type viewModelType)
    {
        var viewModelTypeName = viewModelType.FullName;

        if (viewModelTypeName is null || !viewModelTypeName.EndsWith("ViewModel", StringComparison.Ordinal))
            throw new ArgumentException(ShellErrorMessages.ConventionFailed(viewModelType));

        var viewName = viewModelTypeName[..^"ViewModel".Length];
        var viewTypeName = $"{viewName}, {viewModelType.GetTypeInfo().Assembly.FullName}";
        var viewType = Type.GetType(viewTypeName);

        if (viewType is null)
            throw new ArgumentException(ShellErrorMessages.ConventionalViewNotFound(viewModelType, viewName));

        return viewType;
    }

    private static ShellRouteRegistration Register(Type viewModelType, Type viewType, string route, ShellRouteKind kind)
    {
        if (RegistrationsByViewModel.TryGetValue(viewModelType, out var existing))
            throw new ArgumentException(ShellErrorMessages.DuplicatedRegistration(viewModelType, existing.Route));

        if (!typeof(Page).IsAssignableFrom(viewType))
            throw new ArgumentException(ShellErrorMessages.ViewIsNotAPage(viewType, viewModelType));

        route ??= viewType.Name;

        var duplicatedRoute = RegistrationsByViewModel.Values.FirstOrDefault(x => x.Route == route);

        if (duplicatedRoute is not null)
            throw new ArgumentException(ShellErrorMessages.DuplicatedRoute(route, duplicatedRoute.ViewModelType));

        var registration = new ShellRouteRegistration(viewModelType, viewType, route, kind);

        RegistrationsByViewModel.Add(viewModelType, registration);
        RegistrationsByView[viewType] = registration;

        return registration;
    }

    private static void RegisterRoute(ShellRouteRegistration registration) =>
        Routing.RegisterRoute(registration.Route, new NaveasyRouteFactory(registration));

    private static void TryAddService(this IServiceCollection self, Type serviceType, ServiceLifetime lifetime)
    {
        if (self.Any(descriptor => descriptor.ServiceType == serviceType))
            return;

        self.Add(new ServiceDescriptor(serviceType, serviceType, lifetime));
    }
}
