using Naveasy.Common;
using Naveasy.Extensions;

namespace Naveasy.Core.Processors;

public class RootPageNavigationProcessor(IApplicationProvider applicationProvider, IPageFactory pageFactory) : IPageNavigationProcessor
{
    public bool CanHandle<TViewModel>()
    {
        var result = applicationProvider.MainPage == null;
        return result;
    }

    public async Task<INavigationResult> NavigateAsync<TViewModel>(INavigationParameters parameters = null, bool? animated = null)
    {
        try
        {
            parameters ??= new NavigationParameters();

            var viewModelType = MvvmHelpers.GetINavigationPageGenericType<TViewModel>();
            var pageToNavigate = pageFactory.ResolvePage(viewModelType);

            await MvvmHelpers.OnInitializeAsync(pageToNavigate, parameters);

            Application.Current!.MainPage = MvvmHelpers.IsINavigationPage<TViewModel>() 
                ? new NavigationPage(pageToNavigate) 
                : pageToNavigate;

            parameters.GetNavigationParametersInternal().Add(KnownInternalParameters.NavigationMode, NavigationMode.New);

            await MvvmHelpers.OnInitializedAsync(pageToNavigate, parameters);
            await MvvmHelpers.OnNavigatedTo(pageToNavigate, parameters);

            return new NavigationResult(true);
        }
        catch (Exception ex)
        {
            return new NavigationResult(ex);
        }
    }

    public async Task<INavigationResult> NavigateAbsoluteAsync<TViewModel>(INavigationParameters parameters = null, bool? animated = null)
    {
        return await NavigateAsync<TemplatedView>(parameters, animated);
    }
}