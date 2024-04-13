namespace Naveasy.Extensions;

public static class NavigatorFrameworkExtensions
{
    public static MauiAppBuilder UseNavigator(this MauiAppBuilder builder)
    {
        _ = new NavigatorFrameworkAppBuilder(builder);
        return builder;
    }
}