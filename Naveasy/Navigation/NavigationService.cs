using Naveasy.Bootstrapping;
using Naveasy.Common;
using Naveasy.Extensions;

namespace Naveasy.Navigation;

internal enum NavigationSource
{
	NavigationService,
	System
}

public interface INavigationService
{
	Task<INavigationResult> GoBackAsync(INavigationParameters parameters = null, bool? animated = null);
	Task<INavigationResult> GoBackToRootAsync(INavigationParameters parameters = null, bool? animated = null);
	Task<INavigationResult> NavigateAsync<T>(INavigationParameters parameters = null, bool? animated = null);
	Task<INavigationResult> NavigateAndPopPreviousAsync<T>(INavigationParameters parameters = null, bool? animated = null);
	Task<INavigationResult> NavigateAndPopAllPreviousAsync<T>(INavigationParameters parameters = null, bool? animated = null);
}

[Singleton]
public class NavigationService : INavigationService
{
	private readonly IApplicationProvider _applicationProvider;
	private readonly IPageFactory _pageFactory;

	public NavigationService(IApplicationProvider applicationProvider,
		IPageFactory pageFactory)
	{
		_applicationProvider = applicationProvider;
		_pageFactory = pageFactory;
	}

	internal NavigationSource CurrentNavigationSource { get; private set; } = NavigationSource.System;

	public async Task<INavigationResult> GoBackAsync(INavigationParameters parameters = null, bool? animated = null)
	{
		var result = new NavigationResult();
		Page page = null;

		try
		{
			parameters ??= new NavigationParameters();
			var navigation = _applicationProvider.Navigation;
			page = _applicationProvider.MainPage is NavigationPage navPage ? navPage.CurrentPage : _applicationProvider.MainPage;
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

				result.Success = true;
				return result;
			}
		}
		catch (Exception ex)
		{
			result.Exception = ex;
			return result;
		}
		finally
		{
			CurrentNavigationSource = NavigationSource.System;
		}

		result.Exception = new Exception($"Can't navigate back from {page}");
		return result;
	}

	public async Task<INavigationResult> GoBackToRootAsync(INavigationParameters parameters = null,
		bool? animated = null)
	{
		var result = new NavigationResult();
		try
		{
			parameters ??= new NavigationParameters();

			parameters.GetNavigationParametersInternal()
				.Add(KnownInternalParameters.NavigationMode, NavigationMode.Back);

			var navigation = _applicationProvider.MainPage.Navigation;
			List<Page> pagesToDestroy = navigation.NavigationStack.ToList();
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

			result.Success = true;
			return result;
		}
		catch (Exception ex)
		{
			result.Exception = ex;
			return result;
		}
		finally
		{
			CurrentNavigationSource = NavigationSource.System;
		}
	}

	public async Task<INavigationResult> NavigateAsync<T>(INavigationParameters parameters = null, bool? animated = null)
	{
		var result = new NavigationResult();
		try
		{
			parameters ??= new NavigationParameters();

			var navigation = _applicationProvider.MainPage.Navigation;
			var leavingPage = navigation.NavigationStack.LastOrDefault();

			var pageToNavigate = _pageFactory.ResolvePage(typeof(T));

			await MvvmHelpers.OnInitializedAsync(pageToNavigate, parameters);
			await navigation.PushAsync(pageToNavigate);

			MvvmHelpers.OnNavigatedFrom(leavingPage, parameters);
			
			parameters.GetNavigationParametersInternal()
				.Add(KnownInternalParameters.NavigationMode, NavigationMode.New);
			MvvmHelpers.OnNavigatedTo(pageToNavigate, parameters);

			result.Success = true;
			return result;
		}
		catch (Exception ex)
		{
			result.Exception = ex;
			return result;
		}
	}

	public async Task<INavigationResult> NavigateAndPopPreviousAsync<T>(INavigationParameters parameters = null, bool? animated = null)
	{
		var result = new NavigationResult();
		try
		{
			parameters ??= new NavigationParameters();

			var navigation = _applicationProvider.MainPage.Navigation;
			var pageToRemove = navigation.NavigationStack.LastOrDefault();

			var pageToNavigate = _pageFactory.ResolvePage(typeof(T));

			await PushPipeline(pageToNavigate, parameters, pageToRemove, () => navigation.PushAsync(pageToNavigate));

			result.Success = true;
			return result;
		}
		catch (Exception ex)
		{
			result.Exception = ex;
			return result;
		}
	}

	public async Task<INavigationResult> NavigateAndPopAllPreviousAsync<T>(INavigationParameters parameters = null, bool? animated = null)
	{
		var result = new NavigationResult();
		try
		{
			parameters ??= new NavigationParameters();

			var navigation = _applicationProvider.MainPage.Navigation;
			var pagesToRemove = navigation.NavigationStack.ToList();

			var pageToNavigate = _pageFactory.ResolvePage(typeof(T));

			await PushPipeline(pageToNavigate, parameters, pagesToRemove, () => navigation.PushAsync(pageToNavigate));

			result.Success = true;
			return result;
		}
		catch (Exception ex)
		{
			result.Exception = ex;
			return result;
		}
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

			var navigation = _applicationProvider.MainPage.Navigation;
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

	private async Task PushPipeline(Page page, INavigationParameters parameters, List<Page> leavingPages, Func<Task> navAction)
	{
		try
		{
			CurrentNavigationSource = NavigationSource.NavigationService;
			await MvvmHelpers.OnInitializedAsync(page, parameters);

			await navAction();

			var navigation = _applicationProvider.MainPage.Navigation;
			foreach (var destroyPage in leavingPages)
			{
				navigation.RemovePage(destroyPage);
				MvvmHelpers.OnNavigatedFrom(destroyPage, parameters);
				MvvmHelpers.DestroyPage(destroyPage);
			}

			parameters.GetNavigationParametersInternal().Add(KnownInternalParameters.NavigationMode, NavigationMode.New);
			MvvmHelpers.OnNavigatedTo(page, parameters);
		}
		finally
		{
			CurrentNavigationSource = NavigationSource.System;
		}
	}
}