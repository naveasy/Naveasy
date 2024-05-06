using Microsoft.Extensions.Logging;
using Naveasy.Core;
using Naveasy.Extensions;
using Naveasy.Samples.ViewModels;
using Naveasy.Samples.Views;

namespace Naveasy.Samples;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseNaveasy()
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services
            .AddTransientForNavigation<LoginPage, LoginPageViewModel>()
            .AddTransientForNavigation<MyFlyoutPage, MyFlyoutPageViewModel>()
            .AddTransientForNavigation<Page0, Page0ViewModel>()
            .AddTransientForNavigation<Page1, Page1ViewModel>()
            .AddTransientForNavigation<Page2, Page2ViewModel>()
            .AddTransientForNavigation<PageA, PageAViewModel>()
            .AddTransientForNavigation<PageB, PageBViewModel>()
            .AddTransientForNavigation<PageC, PageCViewModel>()
            .AddTransientForNavigation<PageD, PageDViewModel>();

#if DEBUG
        builder.Logging
            .SetMinimumLevel(LogLevel.Trace)
            .AddDebug();
#endif

        return builder.Build();
    }
}
