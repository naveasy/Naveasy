using Microsoft.Extensions.Logging;
using Naveasy.Common;
using Naveasy.Extensions;

namespace Naveasy.Core.Processors;

public class NavigationPageNavigationProcessor(IApplicationProvider applicationProvider, 
                                               IPageFactory pageFactory, 
                                               ILogger<NavigationPageNavigationProcessor> logger) 
    : IPageNavigationProcessor
{
    public bool CanHandle<TViewModel>()
    {
        if (applicationProvider.MainPage is null or FlyoutPage)
        {
            return false;
        }
        var viewType = pageFactory.ResolveViewType(typeof(TViewModel));
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
            var pageToNavigate = pageFactory.ResolvePage(typeof(TViewModel));

            await MvvmHelpers.OnInitializeAsync(pageToNavigate, parameters);

            if (applicationProvider.HasNavigationPage)
            {
                await navigation.PushAsync(pageToNavigate);
            }
            else
            {
                await navigation.PushAsync(new NaveasyNavigationPage(pageToNavigate));
            }

            MvvmHelpers.OnNavigatedFrom(leavingPage, parameters);
            
            await MvvmHelpers.OnInitializedAsync(pageToNavigate, parameters);
            parameters.GetNavigationParametersInternal().Add(KnownInternalParameters.NavigationMode, NavigationMode.New);
            await MvvmHelpers.OnNavigatedTo(pageToNavigate, parameters);

            return new NavigationResult(true);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, ex.Message);
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

            var pageToNavigate = pageFactory.ResolvePage(typeof(TViewModel));

            await MvvmHelpers.OnInitializeAsync(pageToNavigate, parameters);

            Application.Current!.Windows[0].Page = new NaveasyNavigationPage(pageToNavigate);

            foreach (var destroyPage in pagesToRemove)
            {
                MvvmHelpers.OnNavigatedFrom(destroyPage, parameters);
                MvvmHelpers.DestroyPage(destroyPage);
            }
            
            await MvvmHelpers.OnInitializedAsync(pageToNavigate, parameters);
            parameters.GetNavigationParametersInternal().Add(KnownInternalParameters.NavigationMode, NavigationMode.New);
            await MvvmHelpers.OnNavigatedTo(pageToNavigate, parameters);

            return new NavigationResult(true);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, ex.Message);
            return new NavigationResult(ex);
        }
    }
}