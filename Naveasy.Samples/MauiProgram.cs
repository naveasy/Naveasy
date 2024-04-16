using Microsoft.Extensions.Logging;
using Naveasy.Extensions;
using Naveasy.Navigation;
using Naveasy.Samples.ViewModels;
using Naveasy.Samples.Views;

namespace Naveasy.Samples
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseNaveasy()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services
                .AddTransientForNavigation<LoginPage, LoginPageViewModel>()
                .AddTransientForNavigation<HomePage, HomePageViewModel>()
                .AddTransientForNavigation<ProductsPage, ProductsPageViewModel>()
                .AddTransientForNavigation<DetailsPage, DetailsPageViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
