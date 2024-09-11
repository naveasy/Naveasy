using Microsoft.Extensions.Logging;
using Naveasy.Common;
using Naveasy.Extensions;

namespace Naveasy.Core.Processors;

public class NavigationPageNavigationProcessor(IApplicationProvider applicationProvider, 
                                               IPageFactory pageFactory)
    : IPageNavigationProcessor
{
    public bool CanHandle<TViewModel>()
    {
        if (applicationProvider.MainPage is null or FlyoutPage)
        {
            return false;
        }
        var viewModelType = MvvmHelpers.GetINavigationPageGenericType<TViewModel>();
        var viewType = pageFactory.ResolveViewType(viewModelType);
        var result = viewType.IsSubclassOf(typeof(ContentPage)) || viewType == typeof(ContentPage);
        return result;
    }

    public async Task<INavigationResult> NavigateAsync<TViewModel>(INavigationParameters parameters = null, bool? animated = null)
    {
        try
        {
            parameters ??= new NavigationParameters();

            var navigation = applicationProvider.Navigation;
            var leavingPage = navigation.NavigationStack.LastOrDefault();

            var viewModelType = MvvmHelpers.GetINavigationPageGenericType<TViewModel>();
            var pageToNavigate = pageFactory.ResolvePage(viewModelType);

            await MvvmHelpers.OnInitializedAsync(pageToNavigate, parameters);

            if (MvvmHelpers.IsINavigationPage<TViewModel>())
            {
                await navigation.PushAsync(new NavigationPage(pageToNavigate));
            }
            else
            {
                await navigation.PushAsync(pageToNavigate);
            }

            MvvmHelpers.OnNavigatedFrom(leavingPage, parameters);

            parameters.GetNavigationParametersInternal().Add(KnownInternalParameters.NavigationMode, NavigationMode.New);
            MvvmHelpers.OnNavigatedTo(pageToNavigate, parameters);

            return new NavigationResult(true);
        }
        catch (Exception ex)
        {
            return new NavigationResult(ex);
        }
    }

    public async Task<INavigationResult> NavigateAbsoluteAsync<TViewModel>(INavigationParameters parameters = null, bool? animated = null)
    {
        try
        {
            parameters ??= new NavigationParameters();

            var navigation = applicationProvider.Navigation;
            
            var pagesToRemove = navigation.NavigationStack.Count > 0
                ? navigation.NavigationStack.ToList()
                : [applicationProvider.MainPage];

            pagesToRemove.Reverse();

            var viewModelType = MvvmHelpers.GetINavigationPageGenericType<TViewModel>();
            var pageToNavigate = pageFactory.ResolvePage(viewModelType);

            await MvvmHelpers.OnInitializedAsync(pageToNavigate, parameters);


            Application.Current!.MainPage = MvvmHelpers.IsINavigationPage<TViewModel>()
                ? new NavigationPage(pageToNavigate)
                : pageToNavigate;

            foreach (var destroyPage in pagesToRemove)
            {
                MvvmHelpers.OnNavigatedFrom(destroyPage, parameters);
                MvvmHelpers.DestroyPage(destroyPage);
            }

            parameters.GetNavigationParametersInternal().Add(KnownInternalParameters.NavigationMode, NavigationMode.New);
            MvvmHelpers.OnNavigatedTo(pageToNavigate, parameters);

            return new NavigationResult(true);
        }
        catch (Exception ex)
        {
            return new NavigationResult(ex);
        }
    }
}