namespace Naveasy.Core;
internal class InitialNavigationTypeProvider(Type initialNavigationViewModelType)
{
    public Type InitialNavigationViewModelType { get; } = initialNavigationViewModelType;
}
