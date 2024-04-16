using Naveasy.Bootstrapping;
using Naveasy.Extensions;

namespace Naveasy.Navigation;

public interface IPageFactory
{
    Page ResolvePage(Type viewModelType);
}

[Singleton]
public class PageFactory : IPageFactory
{
    private readonly IPageScopeService _pageScopeService;

    public PageFactory(IPageScopeService pageScopeService)
    {
        _pageScopeService = pageScopeService;
    }

    public Page ResolvePage(Type viewModelType)
    {
        var scope = _pageScopeService.BeginPageLifetimeScope();

        try
        {
            var vm = scope.ServiceProvider.GetRequiredService(viewModelType);
            var viewType = PageRegistry.ResolveViewType(viewModelType);
            var view = scope.ServiceProvider.GetRequiredService(viewType);

            if (view is not Page page)
                throw new ApplicationException($"View '{view}' is not a Page");

            page.BindingContext = vm;
            NavigationAttached.SetLifetimeScope(page, scope);

            page.ApplyBehaviors();

            return page;
        }
        catch (Exception ex)
        {
            throw new Exception($"Can't resolve a Page for viewModel: {viewModelType}", ex);
        }
    }
}