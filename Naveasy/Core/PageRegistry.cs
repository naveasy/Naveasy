using System.Reflection;

namespace Naveasy.Core;

public static class PageRegistry
{
    private static readonly Dictionary<Type, Type> _pageDictionary = [];

    public static Type ResolveViewType(Type viewModelType)
    {
        if (_pageDictionary.TryGetValue(viewModelType, out var viewType))
        {
            return viewType;
        }

        var vmTypeName = viewModelType.FullName;

        if (!vmTypeName!.EndsWith("ViewModel"))
            throw new Exception($"ViewModel ${viewModelType} does not follow convention.");

        var vmAssemblyName = viewModelType.GetTypeInfo().Assembly.FullName;

        var viewName = vmTypeName.Substring(0, vmTypeName.Length - "ViewModel".Length);
        var viewTypeName = $"{viewName}, {vmAssemblyName}";
        var result = Type.GetType(viewTypeName);

        if (result == null)
            throw new Exception($"ViewModel ${viewModelType} does not have a matching view ${viewTypeName} in the same assembly.");

        return result;
    }

    public static IServiceCollection AddTransientForNavigation<TView, TViewModel>(this IServiceCollection self) where TView : IView
    {
        var viewType = typeof(TView);
        var viewModelType = typeof(TViewModel);

        if (!_pageDictionary.TryAdd(viewModelType, viewType))
        {
            throw new ArgumentException($"The ViewModel {viewModelType.FullName} was already registered and cannot be registered twice.");
        }

        if (self.All(x => x.ServiceType != viewType))
        {
            self.AddTransient(viewType);
        }

        if (self.All(x => x.ServiceType != viewModelType))
        {
            self.AddTransient(viewModelType);
        }

        return self;
    }

    public static IServiceCollection AddScopedForNavigation<TView, TViewModel>(this IServiceCollection self) where TView : IView
    {
        var viewType = typeof(TView);
        var viewModelType = typeof(TViewModel);

        if (!_pageDictionary.TryAdd(viewModelType, viewType))
        {
            throw new ArgumentException($"The ViewModel {viewModelType.FullName} was already registered and cannot be registered twice.");
        }

        if (self.All(x => x.ServiceType != viewType))
        {
            self.AddScoped(viewType);
        }

        if (self.All(x => x.ServiceType != viewModelType))
        {
            self.AddScoped(viewModelType);
        }

        return self;
    }
}