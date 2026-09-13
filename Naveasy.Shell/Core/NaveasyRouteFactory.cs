namespace Naveasy.Shell.Core;

/// <summary>
/// Creates the page of a Shell route through Naveasy, so that the ViewModel, the page lifetime scope and the
/// Naveasy behaviors are in place before Shell shows the page.
/// </summary>
internal sealed class NaveasyRouteFactory : RouteFactory
{
    private readonly ShellRouteRegistration _registration;

    public NaveasyRouteFactory(ShellRouteRegistration registration) => _registration = registration;

    public override Element GetOrCreate() => GetOrCreate(IPlatformApplication.Current?.Services);

    public override Element GetOrCreate(IServiceProvider services)
    {
        var serviceProvider = services ?? IPlatformApplication.Current?.Services;

        if (serviceProvider is null)
            throw new InvalidOperationException(ShellErrorMessages.NoActiveShell());

        var pageFactory = (ShellPageFactory)serviceProvider.GetRequiredService<IShellPageFactory>();

        return pageFactory.CreatePage(_registration);
    }
}
