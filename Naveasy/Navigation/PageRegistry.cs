using System.Reflection;
using Naveasy.Bootstrapping;

namespace Naveasy.Navigation;

public interface IPageRegistry
{
    Type ResolveViewType(Type viewModelType);
}

[Singleton]
public class PageRegistry : IPageRegistry
{
    public Type ResolveViewType(Type viewModelType)
    {
        var vmTypeName = viewModelType.FullName;

        if (!vmTypeName!.EndsWith("ViewModel"))
            throw new Exception($"ViewModel ${viewModelType} does not follow convention.");

        var vmAssemblyName = viewModelType.GetTypeInfo().Assembly.FullName;

        var viewName = vmTypeName.Substring(0, vmTypeName.Length - "ViewModel".Length);
        var viewTypeName = $"{viewName}, {vmAssemblyName}";
        var result = Type.GetType(viewTypeName);

        if (result == null)
            throw new Exception($"ViewModel ${viewModelType} does not have a matching view ${viewTypeName} in the same assembly.");

        return result;
    }
}