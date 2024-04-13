using Naveasy.Bootstrapping;

namespace Naveasy;

public interface IApplicationProvider
{
    INavigation Navigation { get; }
    Page MainPage { get; }
}

[Singleton]
public class ApplicationProvider : IApplicationProvider
{
    public Page MainPage => Application.Current!.MainPage;

    public INavigation Navigation => MainPage.Navigation;
}