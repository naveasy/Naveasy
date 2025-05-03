namespace Naveasy.WPF;

public static class WindowTypeRegistry
{
    public static Dictionary<Type, Type> WindowModelToWindowDictionary { get; } = [];

    public static Type ResolveWindowType(Type viewModelType)
    {
        return WindowModelToWindowDictionary[viewModelType];
    }
}