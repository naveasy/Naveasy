namespace Naveasy.Shell.Core;

/// <summary>
/// Creates the application window with the Shell registered through UseNaveasyShell and hands it to the
/// lifecycle coordinator. Overriding App.CreateWindow bypasses this and therefore bypasses Naveasy.
/// </summary>
internal sealed class ShellWindowCreator : IWindowCreator
{
    public Window CreateWindow(Application app, IActivationState activationState)
    {
        var services = activationState.Context.Services;
        var shellTypeProvider = services.GetRequiredService<InitialShellTypeProvider>();
        var shell = (MauiShell)services.GetRequiredService(shellTypeProvider.ShellType);

        if (shellTypeProvider.ShellViewModelType is not null)
            shell.BindingContext = services.GetRequiredService(shellTypeProvider.ShellViewModelType);

        services.GetRequiredService<ShellLifecycleCoordinator>().Attach(shell);

        return new Window(shell);
    }
}
