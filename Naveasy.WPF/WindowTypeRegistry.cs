using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace Naveasy.WPF;

public static class WindowTypeRegistry
{
    private static readonly Dictionary<Type, Type> WindowModelToWindowDictionary = [];

    public static IServiceCollection AddTransientForNavigation<TView, TViewModel>(this IServiceCollection self) where TView: Window
    {
        var windowType = typeof(TView);
        var viewModelType = typeof(TViewModel);

        if (WindowModelToWindowDictionary.ContainsKey(viewModelType))
        {
            throw new ArgumentException($"The ViewModel {viewModelType.FullName} is already registered and cannot be registered again.");
        }

        WindowModelToWindowDictionary.Add(viewModelType, windowType);

        if (self.All(x => x.ServiceType != windowType))
        {
            self.AddTransient(windowType);
        }

        return self.AddTransient(viewModelType);
    }

    public static Type ResolveWindowType(Type viewModelType)
    {
        return WindowModelToWindowDictionary[viewModelType];
    }
}