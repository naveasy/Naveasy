using Naveasy.Behaviors;

namespace Naveasy.Shell.Extensions;

public static class ShellPageExtensions
{
    /// <summary>
    /// Adds the behaviors Naveasy needs on a page driven by Shell. Shell raises its own navigation events,
    /// so only the IPageLifecycleAware bridge is required here.
    /// </summary>
    public static void ApplyBehaviors(this Page page)
    {
        if (page.Behaviors.Any(behavior => behavior is PageLifecycleAwareBehavior))
            return;

        page.Behaviors.Add(new PageLifecycleAwareBehavior());
    }
}
