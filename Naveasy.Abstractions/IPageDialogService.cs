namespace Naveasy;

public interface IPageDialogService
{
    Task<bool> DisplayAlertAsync(string title, string message, string acceptButton, string cancelButton);
    Task<bool> DisplayAlertAsync(string title, string message, string acceptButton, string cancelButton, FlowDirection flowDirection);
    Task DisplayAlertAsync(string title, string message, string cancelButton);
    Task DisplayAlertAsync(string title, string message, string cancelButton, FlowDirection flowDirection);
    Task<string> DisplayActionSheetAsync(string title, string cancelButton, string destroyButton, params string[] otherButtons);
    Task<string> DisplayActionSheetAsync(string title, string cancelButton, string destroyButton, FlowDirection flowDirection, params string[] otherButtons);
}
