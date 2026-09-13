using Naveasy.Shell.Core;

namespace Naveasy.Shell;

/// <summary>
/// Displays alerts and action sheets on the page Shell is currently showing.
/// </summary>
internal sealed class ShellPageDialogService : IPageDialogService
{
    public Task<bool> DisplayAlertAsync(string title, string message, string acceptButton, string cancelButton) =>
        DisplayAlertAsync(title, message, acceptButton, cancelButton, FlowDirection.MatchParent);

    public Task<bool> DisplayAlertAsync(string title, string message, string acceptButton, string cancelButton, FlowDirection flowDirection) =>
        GetCurrentPage().DisplayAlertAsync(title, message, acceptButton, cancelButton, flowDirection);

    public Task DisplayAlertAsync(string title, string message, string cancelButton) =>
        GetCurrentPage().DisplayAlertAsync(title, message, cancelButton);

    public Task DisplayAlertAsync(string title, string message, string cancelButton, FlowDirection flowDirection) =>
        GetCurrentPage().DisplayAlertAsync(title, message, cancelButton, flowDirection);

    public Task<string> DisplayActionSheetAsync(string title, string cancelButton, string destroyButton, params string[] otherButtons) =>
        GetCurrentPage().DisplayActionSheetAsync(title, cancelButton, destroyButton, otherButtons);

    public Task<string> DisplayActionSheetAsync(string title, string cancelButton, string destroyButton, FlowDirection flowDirection, params string[] otherButtons) =>
        GetCurrentPage().DisplayActionSheetAsync(title, cancelButton, destroyButton, flowDirection, otherButtons);

    private static Page GetCurrentPage()
    {
        var shell = MauiShell.Current ?? throw new InvalidOperationException(ShellErrorMessages.NoActiveShell());

        return shell.CurrentPage ?? shell;
    }
}
