namespace Naveasy.Core.Processors;

public interface IPageNavigationProcessor
{
    bool CanHandle<T>();
    Task<INavigationResult> NavigateAsync<T>(INavigationParameters parameters = null, bool? animated = null);
    Task<INavigationResult> NavigateAbsoluteAsync<T>(INavigationParameters parameters = null, bool? animated = null);
}