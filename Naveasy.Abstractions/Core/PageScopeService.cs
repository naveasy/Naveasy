namespace Naveasy.Core;

public interface IPageScopeService : IDisposable
{
    IServiceScope BeginPageLifetimeScope();
} 

public class PageScopeService : IPageScopeService
{
    private IServiceScope _rootScope;

    internal PageScopeService(IServiceScope rootScope)
    {
        _rootScope = rootScope;
    }
	
    public IServiceScope BeginPageLifetimeScope()
    {
        return _rootScope.ServiceProvider.CreateScope();
    }
	
    public void Dispose()
    {
        _rootScope?.Dispose();
    }
}