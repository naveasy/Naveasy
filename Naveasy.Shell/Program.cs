using Naveasy.Core;
using Naveasy.Shell.Core;

namespace Naveasy.Shell;

public static class Program
{
    /// <summary>
    /// Bootstraps ViewModel to ViewModel navigation on top of <typeparamref name="TShell"/>.
    /// </summary>
    /// <remarks>
    /// App.CreateWindow must not be overridden: Naveasy creates the window with the Shell resolved from the
    /// service container.
    /// </remarks>
    public static MauiAppBuilder UseNaveasyShell<TShell>(this MauiAppBuilder builder)
        where TShell : MauiShell =>
        RegisterServices<TShell>(builder, shellViewModelType: null);

    /// <summary>
    /// Bootstraps ViewModel to ViewModel navigation on top of <typeparamref name="TShell"/>, bound to
    /// <typeparamref name="TShellViewModel"/>.
    /// </summary>
    public static MauiAppBuilder UseNaveasyShell<TShell, TShellViewModel>(this MauiAppBuilder builder)
        where TShell : MauiShell
        where TShellViewModel : class
    {
        builder.Services.AddSingleton<TShellViewModel>();

        return RegisterServices<TShell>(builder, typeof(TShellViewModel));
    }

    private static MauiAppBuilder RegisterServices<TShell>(MauiAppBuilder builder, Type shellViewModelType)
        where TShell : MauiShell
    {
        builder.Services
            .AddSingleton<TShell>()
            .AddSingleton(new InitialShellTypeProvider(typeof(TShell), shellViewModelType))
            .AddSingleton<IPageScopeService>(serviceProvider => new PageScopeService(serviceProvider.CreateScope()))
            .AddSingleton<ShellNavigationRequestStore>()
            .AddSingleton<IShellPageFactory, ShellPageFactory>()
            .AddSingleton<ShellLifecycleCoordinator>()
            .AddSingleton<INavigationService, ShellNavigationService>()
            .AddSingleton<IPageDialogService, ShellPageDialogService>()
            .AddSingleton<IWindowCreator, ShellWindowCreator>();

        builder.ConfigureContainer(new DefaultServiceProviderFactory());

        return builder;
    }
}
