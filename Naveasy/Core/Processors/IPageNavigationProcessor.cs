namespace Naveasy.Core.Processors;

public interface IPageNavigationProcessor
{
    bool CanHandle<TViewModel>();
    Task<INavigationResult> NavigateAsync<TViewModel>(INavigationParameters parameters = null, bool? animated = null);
    Task<INavigationResult> NavigateAbsoluteAsync<TViewModel>(INavigationParameters parameters = null, bool? animated = null);
}

public interface IFlyoutPageNavigationProcessor : IPageNavigationProcessor
{
    Task<INavigationResult> NavigateFlyoutAbsoluteAsync<TFlyoutViewModel, TDetailViewModel>(INavigationParameters flyoutParameters = null, INavigationParameters detailParameters = null, bool? animated = null);
}