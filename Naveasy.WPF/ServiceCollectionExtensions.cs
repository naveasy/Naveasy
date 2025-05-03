using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace Naveasy.WPF;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNaveasy(this IServiceCollection self)
    {
        return self.AddSingleton<INavigationService, NavigationService>()
            .AddSingleton<IWindowFactory, WindowFactory>();
    }

    public static IServiceCollection AddTransientForNavigation<TView, TViewModel>(this IServiceCollection self) where TView : Window
    {
        var windowType = typeof(TView);
        var viewModelType = typeof(TViewModel);

        if (WindowTypeRegistry.WindowModelToWindowDictionary.ContainsKey(viewModelType))
        {
            throw new ArgumentException($"The ViewModel {viewModelType.FullName} is already registered and cannot be registered again.");
        }

        WindowTypeRegistry.WindowModelToWindowDictionary.Add(viewModelType, windowType);

        if (self.All(x => x.ServiceType != windowType))
        {
            self.AddTransient(windowType);
        }

        return self.AddTransient(viewModelType);
    }
}