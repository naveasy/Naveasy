using Microsoft.Extensions.Logging;
using Naveasy.Core;

namespace Naveasy.Samples;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()

            //The generic type specified here the line bellow builder.UseNaveasy<T>()
            //will be used to create a new window and navigate to it SplashPageViewModel
            .UseNaveasy<SplashPageViewModel>() 
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services
            .AddTransientForNavigation<SplashPage, SplashPageViewModel>()
            .AddTransientForNavigation<LoginPage, LoginPageViewModel>()
            .AddTransientForNavigation<MyFlyoutPage, MyFlyoutPageViewModel>()
            .AddTransientForNavigation<FeaturePage3, MyPage3ViewModel>()
            .AddTransientForNavigation<FeaturePage1, FeaturePage1ViewModel>()
            .AddTransientForNavigation<FeaturePage2, FeaturePage2ViewModel>()
            .AddTransientForNavigation<FeaturePageA, FeaturePageAViewModel>()
            .AddTransientForNavigation<FeaturePageB, FeaturePageBViewModel>()
            .AddTransientForNavigation<FeaturePageC, FeaturePageCViewModel>()
            .AddTransientForNavigation<FeaturePageD, FeaturePageDViewModel>();

#if DEBUG
        builder.Logging
            .SetMinimumLevel(LogLevel.Trace)
            .AddDebug();
#endif

        return builder.Build();
    }
}