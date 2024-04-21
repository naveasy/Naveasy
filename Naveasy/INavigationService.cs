namespace Naveasy;

public interface INavigationService
{
    Task<INavigationResult> GoBackAsync(INavigationParameters parameters = null, bool? animated = null);
    Task<INavigationResult> GoBackToRootAsync(INavigationParameters parameters = null, bool? animated = null);
    Task<INavigationResult> NavigateAsync<TViewModel>(INavigationParameters parameters = null, bool? animated = null);
    Task<INavigationResult> NavigateAndPopPreviousAsync<TViewModel>(INavigationParameters parameters = null, bool? animated = null);
    Task<INavigationResult> NavigateAbsoluteAsync<TViewModel>(INavigationParameters parameters = null, bool? animated = null);
}