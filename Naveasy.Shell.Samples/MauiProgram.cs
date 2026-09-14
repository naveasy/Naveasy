using Microsoft.Extensions.Logging;
using Naveasy.Shell.Samples.Services;

namespace Naveasy.Shell.Samples;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            // Creates the window with AppShell and drives the Naveasy lifecycle from the Shell navigation events.
            .UseNaveasyShell<AppShell>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services
            // Pages declared as a ShellContent in AppShell.xaml: Shell owns the page creation, so only the
            // mapping is registered here. When no explicit route is declared, the name of the Page/View is used
            // as the route by default, which is why the Route attribute of those ShellContent is LoginPage and
            // HomePage.
            .AddShellContentForNavigation<LoginPage, LoginPageViewModel>()
            .AddShellContentForNavigation<HomePage, HomePageViewModel>()
            // An explicit route is only needed when the Route attribute in the XAML differs from the View name.
            .AddShellContentForNavigation<OrdersPage, OrdersPageViewModel>("orderspage")
            // Pages reached by pushing a route. The route defaults to the name of the View.
            .AddTransientForNavigation<OrderDetailPage, OrderDetailPageViewModel>()
            .AddTransientForNavigation<ReviewOrderPage, ReviewOrderPageViewModel>()
            .AddTransientForNavigation<GuardedPage, GuardedPageViewModel>()
            .AddTransientForNavigation<ReportsPage, ReportsPageViewModel>()
            // Pages hosted by another page - the children of the TabbedPage - have no route of their own.
            .AddChildForNavigation<SummaryPage, SummaryPageViewModel>()
            .AddChildForNavigation<ChartPage, ChartPageViewModel>()
            // Scoped pages get one dependency injection scope per page, disposed when the page is popped.
            .AddScopedForNavigation<CustomerPage, CustomerPageViewModel>();

        builder.Services.AddSingleton<IOrderService, OrderService>();

#if DEBUG
        builder.Logging
            .SetMinimumLevel(LogLevel.Trace)
            .AddDebug();
#endif

        var app = builder.Build();

        ViewModelBase.Logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Naveasy.Shell.Samples");

        return app;
    }
}
