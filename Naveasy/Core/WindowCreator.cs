using Naveasy.Common;

namespace Naveasy.Core;

internal class WindowCreator() : IWindowCreator
{
    private Page _page;

    public Window CreateWindow(Application app, IActivationState activationState)
    {
        var serviceProvider = activationState!.Context.Services;
        var pageFactory = serviceProvider.GetRequiredService<IPageFactory>();
        var initialNavigationTypeProvider = serviceProvider.GetRequiredService<InitialNavigationTypeProvider>();

        _page = pageFactory.ResolvePage(initialNavigationTypeProvider.InitialNavigationViewModelType);

        var window = new NaveasyWindow(new NavigationPage(_page));
        window.Created += OnWindowCreated;
        return window;
    }

    private void OnWindowCreated(object sender, EventArgs e)
    {
        var parameters = new NavigationParameters { { KnownInternalParameters.NavigationMode, NavigationMode.New } };
        MvvmHelpers.OnInitializeAsync(_page, parameters).ConfigureAwait(false);
        MvvmHelpers.OnNavigatedTo(_page, parameters).ConfigureAwait(false);
    }
}
