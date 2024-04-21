using Naveasy.Extensions;

namespace Naveasy.Core;

public interface IPageFactory
{
    Page ResolvePage(Type viewModelType);
    Type ResolveViewType(Type viewModelType);
}

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
            var viewType = ResolveViewType(viewModelType);
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

    public Type ResolveViewType(Type viewModelType)
    {
        return PageRegistry.ResolveViewType(viewModelType);
    }
}