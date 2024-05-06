namespace Naveasy.Core;

public class NavigationAttached
{
    private static readonly BindableProperty LifetimeScopeProperty = BindableProperty.CreateAttached("LifetimeScope", typeof(IServiceScope), typeof(Page), null);

    public static IServiceScope GetLifetimeScope(BindableObject bindable)
    {
        return (IServiceScope) bindable.GetValue(LifetimeScopeProperty);
    }

    public static void SetLifetimeScope(BindableObject bindable, IServiceScope value)
    {
        bindable.SetValue(LifetimeScopeProperty, value);
    }

    public static void ClearLifetimeScope(BindableObject bindable)
    {
        bindable.ClearValue(LifetimeScopeProperty);
    }
}