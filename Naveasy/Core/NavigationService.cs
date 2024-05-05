using Naveasy.Common;
using Naveasy.Core.Processors;
using Naveasy.Extensions;

namespace Naveasy.Core;

internal enum NavigationSource
{
    NavigationService,
    System
}

public class NavigationService : INavigationService
{
    private readonly IApplicationProvider _applicationProvider;
    private readonly IPageFactory _pageFactory;
    private readonly IEnumerable<IPageNavigationProcessor> _pageNavigationProcessors;

    public NavigationService(IApplicationProvider applicationProvider, IPageFactory pageFactory, IEnumerable<IPageNavigationProcessor> pageNavigationProcessors)
    {
        _applicationProvider = applicationProvider;
        _pageFactory = pageFactory;
        _pageNavigationProcessors = pageNavigationProcessors;
    }

    internal NavigationSource CurrentNavigationSource { get; private set; } = NavigationSource.System;
    public async Task<INavigationResult> GoBackAsync(INavigationParameters parameters = null, bool? animated = null)
    {
        Page page = null;

        try
        {
            parameters ??= new NavigationParameters();
            
            var navigation = _applicationProvider.Navigation;

            page = _applicationProvider.MainPage is NavigationPage navPage 
                ? navPage.CurrentPage 
                : _applicationProvider.MainPage;

            parameters.GetNavigationParametersInternal()
                .Add(KnownInternalParameters.NavigationMode, NavigationMode.Back);

            CurrentNavigationSource = NavigationSource.NavigationService;
            var poppedPage = await DoPop(navigation, animated ?? true);
            var previousPage = navigation.NavigationStack.LastOrDefault();

            if (poppedPage != null)
            {
                MvvmHelpers.OnNavigatedFrom(poppedPage, parameters);
                MvvmHelpers.OnNavigatedTo(previousPage, parameters);
                MvvmHelpers.DestroyPage(poppedPage);

                return new NavigationResult(true);
            }
        }
        catch (Exception ex)
        {
            return new NavigationResult(ex);
        }
        finally
        {
            CurrentNavigationSource = NavigationSource.System;
        }

        return new NavigationResult(new Exception($"Can't navigate back from {page}"));
    }

    public async Task<INavigationResult> GoBackToRootAsync(INavigationParameters parameters = null, bool? animated = null)
    {
        try
        {
            parameters ??= new NavigationParameters();

            parameters.GetNavigationParametersInternal()
                .Add(KnownInternalParameters.NavigationMode, NavigationMode.Back);

            var navigation = _applicationProvider.Navigation;

            var pagesToDestroy = navigation.NavigationStack.ToList();

            pagesToDestroy.Reverse();
            var root = pagesToDestroy.Last();

            pagesToDestroy.Remove(root);

            CurrentNavigationSource = NavigationSource.NavigationService;

            await navigation.PopToRootAsync(animated ?? true);

            foreach (var destroyPage in pagesToDestroy)
            {
                MvvmHelpers.OnNavigatedFrom(destroyPage, parameters);
                MvvmHelpers.DestroyPage(destroyPage);
            }

            MvvmHelpers.OnNavigatedTo(root, parameters);

            return new NavigationResult(true);
        }
        catch (Exception ex)
        {
            return new NavigationResult(ex);
        }
        finally
        {
            CurrentNavigationSource = NavigationSource.System;
        }
    }

    public async Task<INavigationResult> NavigateAsync<TViewModel>(INavigationParameters parameters = null, bool? animated = null)
    {
        CurrentNavigationSource = NavigationSource.NavigationService;

        var result = await _pageNavigationProcessors.Single(x => x.CanHandle<TViewModel>())
                                                    .NavigateAsync<TViewModel>(parameters, animated);
        
        CurrentNavigationSource = NavigationSource.System;
        return result;
    }

    public async Task<INavigationResult> NavigateAndPopPreviousAsync<TViewModel>(INavigationParameters parameters = null, bool? animated = null)
    {
        try
        {
            parameters ??= new NavigationParameters();

            var navigation = _applicationProvider.Navigation;
            var pageToRemove = navigation.NavigationStack.LastOrDefault();

            var pageToNavigate = _pageFactory.ResolvePage(typeof(TViewModel));

            await PushPipeline(pageToNavigate, parameters, pageToRemove, () => navigation.PushAsync(pageToNavigate));

            return new NavigationResult(true);
        }
        catch (Exception ex)
        {
            return new NavigationResult(ex);
        }
    }

    public async Task<INavigationResult> NavigateAbsoluteAsync<TViewModel>(INavigationParameters parameters = null, bool? animated = null)
    {
        CurrentNavigationSource = NavigationSource.NavigationService;

        var result = await _pageNavigationProcessors.Single(x => x.CanHandle<TViewModel>())
                                                    .NavigateAbsoluteAsync<TViewModel>(parameters, animated);

        CurrentNavigationSource = NavigationSource.System;
        return result;
    }

    public async Task<INavigationResult> NavigateFlyoutAbsoluteAsync<TFlyoutViewModel, TDetailViewModel>(INavigationParameters flyoutParameters = null, INavigationParameters detailParameters = null, bool? animated = null)
    {
        CurrentNavigationSource = NavigationSource.NavigationService;

        var processor = (IFlyoutPageNavigationProcessor)_pageNavigationProcessors.Single(x => x.CanHandle<TFlyoutViewModel>());

        var result = await processor.NavigateFlyoutAbsoluteAsync<TFlyoutViewModel, TDetailViewModel>(flyoutParameters, detailParameters, animated);

        CurrentNavigationSource = NavigationSource.System;
        return result;
    }

    private Task<Page> DoPop(INavigation navigation, bool animated)
    {
        return navigation.PopAsync(animated);
    }

    private async Task PushPipeline(Page page, INavigationParameters parameters, Page leavingPage, Func<Task> navAction)
    {
        try
        {
            CurrentNavigationSource = NavigationSource.NavigationService;
            await MvvmHelpers.OnInitializedAsync(page, parameters);

            await navAction();

            var navigation = _applicationProvider.Navigation;
            navigation.RemovePage(leavingPage);
            MvvmHelpers.OnNavigatedFrom(leavingPage, parameters);

            parameters.GetNavigationParametersInternal()
                .Add(KnownInternalParameters.NavigationMode, NavigationMode.New);
            MvvmHelpers.OnNavigatedTo(page, parameters);
        }
        finally
        {
            CurrentNavigationSource = NavigationSource.System;
        }
    }
}