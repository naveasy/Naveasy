using Naveasy.Extensions;
using Naveasy.Navigation;

namespace Naveasy.Common;

public static class MvvmHelpers
{
	private static void DestroyChildren(IView page)
	{
		switch (page)
		{
			case FlyoutPage flyout:
				DestroyPage(flyout.Flyout);
				DestroyPage(flyout.Detail);
				break;
			case TabbedPage tabbedPage:
				foreach (var item in tabbedPage.Children.Reverse())
				{
					DestroyPage(item);
				}
				break;
			case NavigationPage navigationPage:
				foreach (var item in navigationPage.Navigation.NavigationStack.Reverse())
				{
					DestroyPage(item);
				}
				break;
		}
	}
	
	public static void InvokeViewAndViewModelAction<T>(object view, Action<T> action) where T : class
	{
		if (view is T viewAsT)
		{
			action(viewAsT);
		}

		if (view is BindableObject {BindingContext: T viewModelAsT})
		{
			action(viewModelAsT);
		}
	}

	public static async Task InvokeViewAndViewModelActionAsync<T>(object view, Func<T, Task> action) where T : class
	{
		if (view is T viewAsT)
		{
			await action(viewAsT);
		}

		if (view is BindableObject {BindingContext: T viewModelAsT})
		{
			await action(viewModelAsT);
		}
	}
	
	public static void OnNavigatedFrom(object page, INavigationParameters parameters)
	{
		if (page != null)
			InvokeViewAndViewModelAction<INavigatedAware>(page, v => v.OnNavigatedFrom(parameters));
	}
	
	public static void OnNavigatedTo(object page, INavigationParameters parameters)
	{
		if (page != null)
			InvokeViewAndViewModelAction<INavigatedAware>(page, v => v.OnNavigatedTo(parameters));
	}
	
	public static async Task OnInitializedAsync(object page, INavigationParameters parameters)
	{
		if (page is null) return;

		InvokeViewAndViewModelAction<IInitialize>(page, v => v.OnInitialize(parameters));
		await InvokeViewAndViewModelActionAsync<IInitializeAsync>(page, async v => await v.OnInitializeAsync(parameters));
	}
	
	public static void DestroyPage(IView view)
	{
		try
		{
			DestroyChildren(view);

			InvokeViewAndViewModelAction<IDestructible>(view, v => v.Destroy());

			if(view is Page page)
			{
				page.Behaviors?.Clear();
				page.BindingContext = null;
			}
		}
		catch (Exception ex)
		{
			throw new Exception($"Cannot destroy {view}.", ex);
		}
	}
	
	public static void HandleSystemGoBack(IView previousPage, IView currentPage)
	{
		var parameters = new NavigationParameters();
		parameters.GetNavigationParametersInternal().Add(KnownInternalParameters.NavigationMode, NavigationMode.Back);
		OnNavigatedFrom(previousPage, parameters);
		OnNavigatedTo(GetOnNavigatedToTargetFromChild(currentPage), parameters);
		DestroyPage(previousPage);
	}
	
	public static Page GetPreviousPage(Page currentPage, System.Collections.Generic.IReadOnlyList<Page> navStack)
	{
		Page previousPage = null;

		int currentPageIndex = GetCurrentPageIndex(currentPage, navStack);
		int previousPageIndex = currentPageIndex - 1;
		if (navStack.Count >= 0 && previousPageIndex >= 0)
			previousPage = navStack[previousPageIndex];

		return previousPage;
	}

	public static int GetCurrentPageIndex(Page currentPage, System.Collections.Generic.IReadOnlyList<Page> navStack)
	{
		int stackCount = navStack.Count;
		for (int x = 0; x < stackCount; x++)
		{
			var view = navStack[x];
			if (view == currentPage)
				return x;
		}

		return stackCount - 1;
	}
	
	public static Page GetOnNavigatedToTargetFromChild(IView target)
	{
		Page child = null;

		if (target is FlyoutPage flyout)
			child = flyout.Detail;
		else if (target is TabbedPage tabbed)
			child = tabbed.CurrentPage;
		else if (target is NavigationPage np)
			child = np.Navigation.NavigationStack.Last();

		if (child != null)
			target = GetOnNavigatedToTargetFromChild(child);

		if (target is Page page)
			return page;

		return null;
	}
}