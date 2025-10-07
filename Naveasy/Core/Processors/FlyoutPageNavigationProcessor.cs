using Microsoft.Extensions.Logging;
using Naveasy.Common;
using Naveasy.Extensions;

namespace Naveasy.Core.Processors;

public class FlyoutPageNavigationProcessor(IApplicationProvider applicationProvider, IPageFactory pageFactory, ILogger<FlyoutPageNavigationProcessor> logger) 
    : IFlyoutPageNavigationProcessor
{
    public bool CanHandle<TViewModel>()
    {
        if (applicationProvider.MainPage == null)
        {
            return false;
        }

        var viewType = pageFactory.ResolveViewType(typeof(TViewModel));
        var isTargetingFlyout = viewType.IsSubclassOf(typeof(FlyoutPage)) || viewType == typeof(FlyoutPage);

        return isTargetingFlyout || applicationProvider.MainPage is FlyoutPage;
    }

    public async Task<INavigationResult> NavigateAsync<TViewModel>(INavigationParameters parameters = null, bool? animated = null)
    {
        try
        {
            parameters ??= new NavigationParameters();

            var flyoutPage = (FlyoutPage)applicationProvider.MainPage;
            var detailNavigationPage = flyoutPage.Detail as NavigationPage;

            if (flyoutPage.IsPresented &&
                flyoutPage.Detail is NavigationPage detailNavPage &&
                detailNavPage.RootPage.BindingContext is TViewModel)
            {
                //This is to prevent navigating to the same page again when the detail is already showing the requested ViewModel
                flyoutPage.IsPresented = false;
                MvvmHelpers.OnNavigatedFrom(flyoutPage.Flyout, parameters);
                await MvvmHelpers.OnNavigatedTo(detailNavPage.CurrentPage, parameters);
                return new NavigationResult(true);
            }

            var pageToNavigate = pageFactory.ResolvePage(typeof(TViewModel));

            await MvvmHelpers.OnInitializeAsync(pageToNavigate, parameters);

            if (!flyoutPage.IsPresented && detailNavigationPage != null)
            {
                if (applicationProvider.HasNavigationPage)
                {
                    await detailNavigationPage.Navigation.PushAsync(pageToNavigate);
                }
                else
                {
                    await detailNavigationPage.Navigation.PushAsync(new NaveasyNavigationPage(pageToNavigate));
                }
                MvvmHelpers.OnNavigatedFrom(detailNavigationPage, parameters);
            }
            else
            {
                if (detailNavigationPage != null)
                {
                    await detailNavigationPage.Navigation.PushAsync(pageToNavigate);
                }
                else
                {
                    var pageToRemove = flyoutPage.Detail;

                    flyoutPage.Detail = applicationProvider.HasNavigationPage
                        ? pageToNavigate
                        : new NaveasyNavigationPage(pageToNavigate);

                    MvvmHelpers.OnNavigatedFrom(pageToRemove, parameters);
                    MvvmHelpers.DestroyPage(pageToRemove);
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
            logger?.LogError(ex, ex.Message);
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

            pagesToRemove.Reverse();

            var pageToNavigate = pageFactory.ResolvePage(typeof(TViewModel));

            await MvvmHelpers.OnInitializeAsync(pageToNavigate, parameters);

            Application.Current!.Windows[0].Page = new NaveasyNavigationPage(pageToNavigate);

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
            logger?.LogError(ex, ex.Message);
            return new NavigationResult(ex);
        }
    }

    public async Task<INavigationResult> NavigateFlyoutDetailAbsoluteAsync<TDetailViewModel>(INavigationParameters detailParameters = null, bool? animated = null)
    {
        try
        {
            var flyoutPage = (FlyoutPage)applicationProvider.MainPage;

            if (flyoutPage.IsPresented &&
                flyoutPage.Detail is NavigationPage detailNavPage &&
                detailNavPage.RootPage.BindingContext is TDetailViewModel)
            {
                //This is to prevent navigating to the same page again when the detail is already showing the requested ViewModel
                flyoutPage.IsPresented = false;
                MvvmHelpers.OnNavigatedFrom(flyoutPage.Flyout, detailParameters);
                await MvvmHelpers.OnNavigatedTo(detailNavPage.CurrentPage, detailParameters);
                return new NavigationResult(true);
            }

            detailParameters ??= new NavigationParameters();

            var pagesToRemove = flyoutPage.Detail is NavigationPage leavingNavigationPage
                ? leavingNavigationPage.Navigation.NavigationStack.ToList()
                : [flyoutPage.Detail];

            pagesToRemove.Reverse();

            var detailPage = pageFactory.ResolvePage(typeof(TDetailViewModel));

            await MvvmHelpers.OnInitializeAsync(detailPage, detailParameters);

            flyoutPage.Detail = new NaveasyNavigationPage(detailPage);

            //Application.Current!.Windows[0].Page = flyoutPage;

            foreach (var destroyPage in pagesToRemove)
            {
                MvvmHelpers.OnNavigatedFrom(destroyPage, detailParameters);
                MvvmHelpers.DestroyPage(destroyPage);
            }

            detailParameters.GetNavigationParametersInternal().Add(KnownInternalParameters.NavigationMode, NavigationMode.New);

            await MvvmHelpers.OnInitializedAsync(detailPage, detailParameters);

            await MvvmHelpers.OnNavigatedTo(detailPage, detailParameters);

            return new NavigationResult(true);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, ex.Message);
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

            var flyoutPage = pageFactory.ResolvePage(typeof(TFlyoutViewModel));
            var detailPage = pageFactory.ResolvePage(typeof(TDetailViewModel));

            if (flyoutPage is not FlyoutPage flyout)
            {
                throw new InvalidCastException($"To navigate to a FlyoutPage your {flyoutPage.GetType().FullName} must inherit from {typeof(FlyoutPage).FullName}");
            }

            await MvvmHelpers.OnInitializeAsync(flyoutPage, flyoutParameters);
            await MvvmHelpers.OnInitializeAsync(detailPage, detailParameters);

            flyout.Detail = new NaveasyNavigationPage(detailPage);

            Application.Current!.Windows[0].Page = flyoutPage;

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
            logger?.LogError(ex, ex.Message);
            return new NavigationResult(ex);
        }
    }
}