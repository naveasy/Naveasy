using Naveasy.Core;
using Naveasy.Extensions;

namespace Naveasy.Common;

public static class MvvmHelpers
{
    public static void DestroyPage(IView view)
    {
        try
        {
            DestroyChildren(view);

            InvokeViewAndViewModelAction<IDisposable>(view, v => v.Dispose());

            if (view is Page page)
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
            case NaveasyNavigationPage navigationPage:
                foreach (var item in navigationPage.Navigation.NavigationStack.Reverse())
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
		=> LifecycleInvoker.InvokeViewAndViewModelAction(view, action);

	public static Task InvokeViewAndViewModelActionAsync<T>(object view, Func<T, Task> action) where T : class
		=> LifecycleInvoker.InvokeViewAndViewModelActionAsync(view, action);
	
	public static void OnNavigatedFrom(object page, INavigationParameters parameters)
		=> LifecycleInvoker.OnNavigatedFrom(page, parameters);
	
	public static async Task OnNavigatedTo(object page, INavigationParameters parameters)
	{
        if (page != null)
        {
            LifecycleInvoker.OnNavigatedTo(page, parameters);
            await Task.Delay(TimeSpan.FromMilliseconds(150));
        }
	}
	
	public static Task OnInitializeAsync(object page, INavigationParameters parameters)
		=> LifecycleInvoker.OnInitializeAsync(page, parameters);

    public static async Task OnInitializedAsync(object page, INavigationParameters parameters)
    {
        if (page is null) return;
        await Task.Delay(TimeSpan.FromMilliseconds(150));
#pragma warning disable CS0618 // The deprecated hooks are kept alive for backwards compatibility.
        InvokeViewAndViewModelAction<IInitialized>(page, v => v.OnInitialized(parameters));
        await InvokeViewAndViewModelActionAsync<IInitializedAsync>(page, async v => await v.OnInitializedAsync(parameters));
#pragma warning restore CS0618
    }
	
	public static async Task HandleSystemGoBack(IView previousPage, IView currentPage)
	{
		var parameters = new NavigationParameters();
		parameters.GetNavigationParametersInternal().Add(KnownInternalParameters.NavigationMode, NavigationMode.Back);
		OnNavigatedFrom(previousPage, parameters);
		await OnNavigatedTo(GetOnNavigatedToTargetFromChild(currentPage), parameters);
		DestroyPage(previousPage);
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
