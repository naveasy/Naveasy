using Microsoft.Extensions.Logging;
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
    private readonly ILogger<PageFactory> _logger;

    public PageFactory(IPageScopeService pageScopeService, ILogger<PageFactory> logger)
    {
        _pageScopeService = pageScopeService;
        _logger = logger;
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
            var errMessage = $"Unable to create a Page or ViewModel for {viewModelType}: {ex.Message}";
            _logger?.LogError(ex, errMessage);
            throw new Exception(errMessage, ex);
        }
    }

    public Type ResolveViewType(Type viewModelType)
    {
        return PageRegistry.ResolveViewType(viewModelType);
    }
}