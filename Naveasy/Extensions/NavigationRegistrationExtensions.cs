namespace Naveasy.Extensions;

public static class NavigationRegistrationExtensions
{
    public static IServiceCollection RegisterForNavigation<TView>(this IServiceCollection container, string name = null)
        where TView : Page =>
        container.RegisterForNavigation(typeof(TView), null, name);

    public static IServiceCollection RegisterForNavigation<TView, TViewModel>(this IServiceCollection container, string name = null)
        where TView : Page =>
        container.RegisterForNavigation(typeof(TView), typeof(TViewModel), name);

    public static IServiceCollection RegisterForNavigation(this IServiceCollection container, Type view, Type viewModel, string name = null)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));

        if (string.IsNullOrEmpty(name))
            name = view.Name;

        container.AddTransient(view);

        if (viewModel != null)
            container.AddTransient(viewModel);

        return container;
    }
}