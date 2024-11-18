using Microsoft.Extensions.Logging;
using Naveasy.Common;
using Naveasy.Extensions;

namespace Naveasy.Core.Processors;

public class FlyoutPageNavigationProcessor(IApplicationProvider applicationProvider, IPageFactory pageFactory, ILogger<FlyoutPageNavigationProcessor> logger) : IFlyoutPageNavigationProcessor
{
    public bool CanHandle<TViewModel>()
    {
        if (applicationProvider.MainPage == null)
        {
            return false;
        }

        var viewModelType = MvvmHelpers.GetINavigationPageGenericType<TViewModel>();
        var viewType = pageFactory.ResolveViewType(viewModelType);
        var isTargetingFlyout = viewType.IsSubclassOf(typeof(FlyoutPage)) || viewType == typeof(FlyoutPage);

        return isTargetingFlyout || applicationProvider.MainPage is FlyoutPage;
    }

    public async Task<INavigationResult> NavigateAsync<TViewModel>(INavigationParameters parameters = null, bool? animated = null)
    {
        try
        {
            parameters ??= new NavigationParameters();

            var flyoutPage = (FlyoutPage)applicationProvider.MainPage;
            var viewModelType = MvvmHelpers.GetINavigationPageGenericType<TViewModel>();
            var pageToNavigate = pageFactory.ResolvePage(viewModelType);

            await MvvmHelpers.OnInitializeAsync(pageToNavigate, parameters);

            if (!flyoutPage.IsPresented && flyoutPage.Detail is NavigationPage detailNavigationPage)
            {
                var leavingPage = flyoutPage.Detail is NavigationPage leavingNavigationPage
                    ? leavingNavigationPage.CurrentPage
                    : flyoutPage.Detail;

                if (MvvmHelpers.IsINavigationPage<TViewModel>())
                {
                    await detailNavigationPage.Navigation.PushAsync(new NavigationPage(pageToNavigate));
                }
                else
                {
                    await detailNavigationPage.Navigation.PushAsync(pageToNavigate);
                }
                MvvmHelpers.OnNavigatedFrom(leavingPage, parameters);
            }
            else
            {
                if (flyoutPage.Detail is NavigationPage navigationPage)
                {
                    await navigationPage.Navigation.PushAsync(pageToNavigate);
                }
                else
                {
                    var pagesToRemove = flyoutPage.Detail is NavigationPage leavingDetail
                        ? leavingDetail.Navigation.NavigationStack.ToList()
                        : [flyoutPage.Detail];

                    flyoutPage.Detail = MvvmHelpers.IsINavigationPage<TViewModel>() 
                        ? new NavigationPage(pageToNavigate) 
                        : pageToNavigate;

                    foreach (var destroyPage in pagesToRemove)
                    {
                        MvvmHelpers.OnNavigatedFrom(destroyPage, parameters);
                        MvvmHelpers.DestroyPage(destroyPage);
                    }
                }

                flyoutPage.IsPresented = false;
            }

            await MvvmHelpers.OnInitializedAsync(pageToNavigate, parameters);
            parameters.GetNavigationParametersInternal().Add(KnownInternalParameters.NavigationMode, NavigationMode.New);
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
        try
        {
            parameters ??= new NavigationParameters();

            var flyoutPage = (FlyoutPage)applicationProvider.MainPage;
            var leavingDetail = flyoutPage.Detail;
            var pagesToRemove = leavingDetail.Navigation.NavigationStack.ToList();
            //var pagesToRemove = navigation.NavigationStack.Count > 0
            //    ? navigation.NavigationStack.ToList()
            //    : [applicationProvider.MainPage];

            pagesToRemove.Reverse();

            var viewModelType = MvvmHelpers.GetINavigationPageGenericType<TViewModel>();
            var pageToNavigate = pageFactory.ResolvePage(viewModelType);

            await MvvmHelpers.OnInitializeAsync(pageToNavigate, parameters);

            Application.Current!.MainPage = MvvmHelpers.IsINavigationPage<TViewModel>()
                ? new NavigationPage(pageToNavigate)
                : pageToNavigate;

            foreach (var destroyPage in pagesToRemove)
            {
                MvvmHelpers.OnNavigatedFrom(destroyPage, parameters);
                MvvmHelpers.DestroyPage(destroyPage);
            }

            MvvmHelpers.OnNavigatedFrom(leavingDetail, parameters);
            MvvmHelpers.DestroyPage(leavingDetail);

            await MvvmHelpers.OnInitializedAsync(pageToNavigate, parameters);
            parameters.GetNavigationParametersInternal().Add(KnownInternalParameters.NavigationMode, NavigationMode.New);
            await MvvmHelpers.OnNavigatedTo(pageToNavigate, parameters);

            return new NavigationResult(true);
        }
        catch (Exception ex)
        {
            return new NavigationResult(ex);
        }
    }

    public async Task<INavigationResult> NavigateFlyoutAbsoluteAsync<TFlyoutViewModel, TDetailViewModel>(INavigationParameters flyoutParameters = null, INavigationParameters detailParameters = null, bool? animated = null)
    {
        try
        {
            flyoutParameters ??= new NavigationParameters();
            detailParameters ??= new NavigationParameters();

            var pagesToRemove = applicationProvider.MainPage is NavigationPage leavingNavigationPage
                ? leavingNavigationPage.Navigation.NavigationStack.ToList()
                : [applicationProvider.MainPage];

            pagesToRemove.Reverse();

            var detailsViewModelType = MvvmHelpers.GetINavigationPageGenericType<TDetailViewModel>();
            var flyoutPage = pageFactory.ResolvePage(typeof(TFlyoutViewModel));
            var detailPage = pageFactory.ResolvePage(detailsViewModelType);

            if (flyoutPage is not FlyoutPage flyout)
            {
                throw new InvalidCastException($"To navigate to a FlyoutPage your {flyoutPage.GetType().FullName} must inherit from {typeof(FlyoutPage).FullName}");
            }

            await MvvmHelpers.OnInitializeAsync(flyoutPage, flyoutParameters);
            await MvvmHelpers.OnInitializeAsync(detailPage, detailParameters);

            flyout.Detail = MvvmHelpers.IsINavigationPage<TDetailViewModel>()
                ? new NavigationPage(detailPage)
                : detailPage;

            Application.Current!.MainPage = flyoutPage;

            foreach (var destroyPage in pagesToRemove)
            {
                MvvmHelpers.OnNavigatedFrom(destroyPage, flyoutParameters);
                MvvmHelpers.DestroyPage(destroyPage);
            }

            flyoutParameters.GetNavigationParametersInternal().Add(KnownInternalParameters.NavigationMode, NavigationMode.New);
            detailParameters.GetNavigationParametersInternal().Add(KnownInternalParameters.NavigationMode, NavigationMode.New);

            await MvvmHelpers.OnInitializedAsync(flyoutPage, flyoutParameters);
            await MvvmHelpers.OnInitializedAsync(detailPage, detailParameters);

            await MvvmHelpers.OnNavigatedTo(flyoutPage, flyoutParameters);
            await MvvmHelpers.OnNavigatedTo(detailPage, detailParameters);

            return new NavigationResult(true);
        }
        catch (Exception ex)
        {
            return new NavigationResult(ex);
        }
    }
}