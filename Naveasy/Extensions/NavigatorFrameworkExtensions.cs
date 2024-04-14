namespace Naveasy.Extensions;

public static class NavigatorFrameworkExtensions
{
    public static MauiAppBuilder UseNaveasy(this MauiAppBuilder builder)
    {
        _ = new NavigatorFrameworkAppBuilder(builder);
        return builder;
    }
}