using Naveasy.Bootstrapping;

namespace Naveasy.Navigation;

public interface IPageDialogService
{
    Task<bool> DisplayAlertAsync(string title, string message, string acceptButton, string cancelButton);
    Task<bool> DisplayAlertAsync(string title, string message, string acceptButton, string cancelButton, FlowDirection flowDirection);
    Task DisplayAlertAsync(string title, string message, string cancelButton);
    Task DisplayAlertAsync(string title, string message, string cancelButton, FlowDirection flowDirection);
    Task<string> DisplayActionSheetAsync(string title, string cancelButton, string destroyButton, params string[] otherButtons);
    Task<string> DisplayActionSheetAsync(string title, string cancelButton, string destroyButton, FlowDirection flowDirection, params string[] otherButtons);
}

[Singleton]
public class PageDialogService : IPageDialogService
{
	private readonly IApplicationProvider _applicationProvider;

	public PageDialogService(IApplicationProvider applicationProvider)
	{
		_applicationProvider = applicationProvider;
	}

	public Task<bool> DisplayAlertAsync(string title, string message, string acceptButton, string cancelButton)
	{
		return DisplayAlertAsync(title, message, acceptButton, cancelButton, FlowDirection.MatchParent);
	}

	public Task<bool> DisplayAlertAsync(string title, string message, string acceptButton, string cancelButton,
		FlowDirection flowDirection)
	{
		return _applicationProvider.MainPage.DisplayAlert(title, message, acceptButton, cancelButton, flowDirection);
	}

	public Task DisplayAlertAsync(string title, string message, string cancelButton)
	{
		return _applicationProvider.MainPage.DisplayAlert(title, message, cancelButton);
	}

	public Task DisplayAlertAsync(string title, string message, string cancelButton, FlowDirection flowDirection)
	{
		return _applicationProvider.MainPage.DisplayAlert(title, message, cancelButton, flowDirection);
	}

	public Task<string> DisplayActionSheetAsync(string title, string cancelButton, string destroyButton, params string[] otherButtons)
	{
		return _applicationProvider.MainPage.DisplayActionSheet(title, cancelButton, destroyButton, otherButtons);
	}

	public Task<string> DisplayActionSheetAsync(string title, string cancelButton, string destroyButton, FlowDirection flowDirection,
		params string[] otherButtons)
	{
		return _applicationProvider.MainPage.DisplayActionSheet(title, cancelButton, destroyButton, flowDirection, otherButtons);
	}
}