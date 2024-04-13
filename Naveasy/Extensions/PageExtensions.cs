using Naveasy.Behaviors;

namespace Naveasy.Extensions;

public static class PageExtensions
{
    public static void ApplyBehaviors(this Page page)
    {
        if(page is NavigationPage)
            page.Behaviors.Add(new NavigationPageSystemGoBackBehavior());
		
        page.Behaviors.Add(new PageLifecycleAwareBehavior());
    }
}