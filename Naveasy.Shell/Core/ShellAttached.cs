namespace Naveasy.Shell.Core;

/// <summary>
/// Attached state Naveasy keeps on the pages it drives through Shell.
/// </summary>
public static class ShellAttached
{
    private static readonly BindableProperty IsInitializedProperty =
        BindableProperty.CreateAttached("IsInitialized", typeof(bool), typeof(ShellAttached), false);

    private static readonly BindableProperty IsModalProperty =
        BindableProperty.CreateAttached("IsModal", typeof(bool), typeof(ShellAttached), false);

    /// <summary>
    /// <c>true</c> once IInitialize/IInitializeAsync ran for the page, so that reselecting a tab never
    /// initializes an already created page a second time.
    /// </summary>
    public static bool GetIsInitialized(BindableObject bindable) => (bool)bindable.GetValue(IsInitializedProperty);

    public static void SetIsInitialized(BindableObject bindable, bool value) => bindable.SetValue(IsInitializedProperty, value);

    /// <summary>
    /// <c>true</c> for pages Naveasy pushed modally. Shell does not consistently move those pages to
    /// Navigation.ModalStack (dotnet/maui#12162), so they are tracked here instead.
    /// </summary>
    public static bool GetIsModal(BindableObject bindable) => (bool)bindable.GetValue(IsModalProperty);

    public static void SetIsModal(BindableObject bindable, bool value) => bindable.SetValue(IsModalProperty, value);
}
