namespace Naveasy.Shell.Core;

/// <summary>
/// How a page mapped by <see cref="ShellRouteRegistry"/> is created and reached.
/// </summary>
public enum ShellRouteKind
{
    /// <summary>
    /// The page is created by Naveasy when its route is navigated to.
    /// </summary>
    Route,

    /// <summary>
    /// The page is declared as a ShellContent in the Shell visual hierarchy: Shell creates it and the route
    /// already exists, so it can only be reached by absolute navigation.
    /// </summary>
    ShellContent,

    /// <summary>
    /// The page is hosted by another page - a TabbedPage child, for instance - and cannot be navigated to.
    /// </summary>
    Child
}

/// <summary>
/// A ViewModel to View to Shell route mapping created by <see cref="ShellRouteRegistry"/>.
/// </summary>
public sealed class ShellRouteRegistration
{
    internal ShellRouteRegistration(Type viewModelType, Type viewType, string route, ShellRouteKind kind)
    {
        ViewModelType = viewModelType;
        ViewType = viewType;
        Route = route;
        Kind = kind;
    }

    public Type ViewModelType { get; }

    public Type ViewType { get; }

    public string Route { get; }

    public ShellRouteKind Kind { get; }

    /// <summary>
    /// <c>false</c> for pages hosted by another page, which have no route of their own.
    /// </summary>
    public bool IsRoutable => Kind is not ShellRouteKind.Child;
}
