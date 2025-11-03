namespace Naveasy;

public interface INavigationService
{
    bool IsFlyoutOpen { get; set; }
    Task<INavigationResult> GoBackAsync(INavigationParameters parameters = null, bool? animated = null);
    Task<INavigationResult> GoBackToRootAsync(INavigationParameters parameters = null, bool? animated = null);
    Task<INavigationResult> NavigateAsync<TViewModel>(INavigationParameters parameters = null, bool? animated = null);
    Task<INavigationResult> NavigateAndPopPreviousAsync<TViewModel>(INavigationParameters parameters = null, bool? animated = null);
    Task<INavigationResult> NavigateAbsoluteAsync<TViewModel>(INavigationParameters parameters = null, bool? animated = null);
    Task<INavigationResult> NavigateFlyoutAbsoluteAsync<TFlyoutViewModel, TDetailViewModel>(INavigationParameters flyoutParameters = null, INavigationParameters detailParameters = null, bool? animated = null);
    Task<INavigationResult> NavigateFlyoutDetailAbsoluteAsync<TDetailViewModel>(INavigationParameters detailParameters = null, bool? animated = null);
}