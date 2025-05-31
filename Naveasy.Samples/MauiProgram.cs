using Microsoft.Extensions.Logging;
using Naveasy.Core;
using Naveasy.Samples.Views;
using Naveasy.Samples.Views.Feature1;
using Naveasy.Samples.Views.Feature2;
using Naveasy.Samples.Views.Feature3;
using Naveasy.Samples.Views.FeatureA;
using Naveasy.Samples.Views.FeatureB;
using Naveasy.Samples.Views.FeatureC;
using Naveasy.Samples.Views.FeatureD;
using Naveasy.Samples.Views.Flyout;
using Naveasy.Samples.Views.Login;
using Naveasy.Samples.Views.Splash;

namespace Naveasy.Samples;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseNaveasy<SplashPageViewModel>()
            .UseMauiApp<App>()
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
