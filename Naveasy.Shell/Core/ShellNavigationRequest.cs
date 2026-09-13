using System.Runtime.CompilerServices;

namespace Naveasy.Shell.Core;

/// <summary>
/// The navigation that <see cref="INavigationService"/> asked Shell to perform, waiting to be picked up by the
/// page factory (presentation mode) and by the lifecycle coordinator (parameters).
/// </summary>
public sealed class ShellNavigationRequest
{
    public INavigationParameters Parameters { get; init; }

    public bool IsModal { get; init; }

    public bool Animated { get; init; } = true;
}

/// <summary>
/// Holds the pending navigation request per Shell instance. It is not static state: an app can run more than one
/// window - and therefore more than one Shell - on Windows and Mac Catalyst.
/// </summary>
public sealed class ShellNavigationRequestStore
{
    private readonly ConditionalWeakTable<MauiShell, ShellNavigationRequest> _pending = [];

    public void SetPending(MauiShell shell, ShellNavigationRequest request)
    {
        if (shell is null) return;

        _pending.Remove(shell);
        _pending.Add(shell, request);
    }

    public ShellNavigationRequest Peek(MauiShell shell)
    {
        if (shell is null) return null;

        return _pending.TryGetValue(shell, out var request) ? request : null;
    }

    public ShellNavigationRequest Consume(MauiShell shell)
    {
        var request = Peek(shell);
        Clear(shell);

        return request;
    }

    public void Clear(MauiShell shell)
    {
        if (shell is null) return;

        _pending.Remove(shell);
    }
}
