using Microsoft.Maui.LifecycleEvents;
using Naveasy.Navigation;

namespace Naveasy.Extensions;

public static class ContainerProvider
{
    public static IServiceProvider Current { get; set; }
}

public class NavigatorFrameworkAppBuilder
{
    private readonly MauiAppBuilder _mauiAppBuilder;

    internal NavigatorFrameworkAppBuilder(MauiAppBuilder builder)
    {
        _mauiAppBuilder = builder;
        RegisterServices();
        
        _mauiAppBuilder.ConfigureContainer(new DefaultServiceProviderFactory());
		
        _mauiAppBuilder.ConfigureLifecycleEvents(lifecycle =>
        {
#if ANDROID
            lifecycle.AddAndroid(android =>
            {
                android.OnBackPressed(_ =>
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    var serviceProvider = MauiApplication.Current!.Services;
#pragma warning restore CS0618 // Type or member is obsolete
                    if(serviceProvider is null)
                        return true;

                    var navigation = serviceProvider.GetRequiredService<INavigationService>();
                    navigation.GoBackAsync();

                    return false;
                });
            });
#endif
        });
    }

    private void RegisterServices()
    {
        var serviceCollection = _mauiAppBuilder.Services;

        serviceCollection.AddSingleton<IPageFactory, PageFactory>()
            .AddSingleton<IPageScopeService>(sp => new PageScopeService(sp.CreateScope()))
            .AddSingleton<INavigationService, NavigationService>()
            .AddSingleton<IApplicationProvider, ApplicationProvider>();
    }
}