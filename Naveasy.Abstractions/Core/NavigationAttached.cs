namespace Naveasy.Core;

/// <summary>
/// What Naveasy has to dispose itself when a page leaves the navigation stack.
/// </summary>
/// <remarks>
/// A View or a ViewModel created by the page lifetime scope is not listed here: the scope disposes every
/// <see cref="IDisposable"/> it created, and it never disposes a singleton, which belongs to the root provider.
/// </remarks>
[Flags]
public enum PageDisposalPolicy
{
    /// <summary>
    /// The page lifetime scope created the View and the ViewModel, so disposing the scope is enough.
    /// </summary>
    None = 0,

    /// <summary>
    /// The View was created outside of the page lifetime scope - by Shell or by XAML, for instance.
    /// </summary>
    View = 1,

    /// <summary>
    /// The ViewModel was created outside of the page lifetime scope.
    /// </summary>
    ViewModel = 2,

    /// <summary>
    /// Neither the View nor the ViewModel is owned by a page lifetime scope.
    /// </summary>
    ViewAndViewModel = View | ViewModel
}

public class NavigationAttached
{
    private static readonly BindableProperty LifetimeScopeProperty = BindableProperty.CreateAttached("LifetimeScope", typeof(IServiceScope), typeof(Page), null);

    // Declared as object so that the absence of a value can be told apart from PageDisposalPolicy.None: a page
    // Naveasy never created has no policy at all, and then nothing else owns its View and its ViewModel.
    private static readonly BindableProperty DisposalPolicyProperty = BindableProperty.CreateAttached("DisposalPolicy", typeof(object), typeof(Page), null);

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

    /// <summary>
    /// What Naveasy must dispose on this page, or <c>null</c> when the page was not created by Naveasy.
    /// </summary>
    public static PageDisposalPolicy? GetDisposalPolicy(BindableObject bindable)
    {
        return bindable.GetValue(DisposalPolicyProperty) as PageDisposalPolicy?;
    }

    public static void SetDisposalPolicy(BindableObject bindable, PageDisposalPolicy value)
    {
        bindable.SetValue(DisposalPolicyProperty, value);
    }

    public static void ClearDisposalPolicy(BindableObject bindable)
    {
        bindable.ClearValue(DisposalPolicyProperty);
    }
}
