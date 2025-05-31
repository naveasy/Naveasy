using Microsoft.Maui.LifecycleEvents;
using Naveasy.Core;
using Naveasy.Core.Processors;

namespace Naveasy;

public static class Program
{
    public static MauiAppBuilder UseNaveasy<TViewModel>(this MauiAppBuilder builder)
    {
        RegisterServices(builder, typeof(TViewModel));

        builder.ConfigureContainer(new DefaultServiceProviderFactory());

        builder.ConfigureLifecycleEvents(lifecycle =>
        {
#if ANDROID
            lifecycle.AddAndroid(android =>
            {
                android.OnBackPressed(activity =>
                {
                    if (NavigationService.CurrentNavigationSource == NavigationSource.NavigationService)
                        return true;
                    
                    var serviceProvider = IPlatformApplication.Current!.Services;
                    var navigationService = (NavigationService)serviceProvider.GetRequiredService<INavigationService>();

                    navigationService.GoBackAsync();
                    return false;
                });
            });
#endif
        });
        return builder;
    }

    private static void RegisterServices(MauiAppBuilder builder, Type initialViewModelType)
    {
        builder.Services
            .AddSingleton(new InitialNavigationTypeProvider(initialViewModelType))
            .AddSingleton<IWindowCreator, WindowCreator>()
            .AddSingleton<IPageFactory, PageFactory>()
            .AddSingleton<IApplicationProvider, ApplicationProvider>()
            .AddSingleton<IPageScopeService>(sp => new PageScopeService(sp.CreateScope()))
            .AddSingleton<INavigationService, NavigationService>()
            .AddSingleton<IPageNavigationProcessor, NavigationPageNavigationProcessor>()
            .AddSingleton<IPageNavigationProcessor, FlyoutPageNavigationProcessor>()
            .AddSingleton<IPageDialogService, PageDialogService>();
    }
}