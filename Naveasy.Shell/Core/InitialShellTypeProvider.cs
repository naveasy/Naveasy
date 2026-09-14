namespace Naveasy.Shell.Core;

/// <summary>
/// The Shell - and its optional ViewModel - the app was bootstrapped with through UseNaveasyShell.
/// </summary>
internal sealed class InitialShellTypeProvider
{
    public InitialShellTypeProvider(Type shellType, Type shellViewModelType)
    {
        ShellType = shellType;
        ShellViewModelType = shellViewModelType;
    }

    public Type ShellType { get; }

    public Type ShellViewModelType { get; }
}
