using System.Windows;

namespace Naveasy.WPF;
public class NavigationParameters : Dictionary<string, object>
{
    public string? Title { get; set; }
    public double? Height { get; set; }
    public double? Width { get; set; }
    public WindowStartupLocation? WindowStartupLocation { get; set; }
    public ResizeMode? ResizeMode { get; set; }
    public SizeToContent? SizeToContent { get; set; }

    public NavigationParameters SetValue<T>(T value)
    {
        Add(typeof(T).FullName!, value!);
        return this;
    }

    public T? GetValue<T>()
    {
        var key = typeof(T).FullName!;
        if (!ContainsKey(key))
            return default(T);

        return (T)this[key];
    }
}

public static class ExtensionsOfNavigationParameters
{
    public static NavigationParameters ToNavigationParameter(this object value, Type genericType)
    {
        return new NavigationParameters{ { genericType.FullName ?? throw new InvalidOperationException(), value } };
    }

    public static NavigationParameters ToNavigationParameter(this object value)
    {
        return new NavigationParameters { { value.GetType().FullName ?? throw new InvalidOperationException(), value } };
    }

    public static IDictionary<string, object> SetValue<T>(this IDictionary<string, object> navigationParameters, T value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        navigationParameters.Add(typeof(T).FullName ?? throw new InvalidOperationException(), value);
        return navigationParameters;
    }

    public static NavigationParameters Including(this NavigationParameters self, object value)
    {
        self.Add(value.GetType().FullName ?? throw new InvalidOperationException(), value);
        return self;
    }

    public static NavigationParameters Including(this NavigationParameters self, object value, Type genericType)
    {
        self.Add(genericType.FullName ?? throw new InvalidOperationException(), value);
        return self;
    }
}
