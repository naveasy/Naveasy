using System.Windows;
//using Microsoft.Extensions.Logging;

namespace Naveasy.WPF;

public interface IWindowFactory
{
    Window CreateWindow<TViewModel>();
    Window CreateWindow(Type viewModelType);
}

public class WindowFactory(IServiceProvider serviceProvider/*, ILogger<WindowFactory> logger*/) : IWindowFactory
{
    public Window CreateWindow<TViewModel>()
    {
        return CreateWindow(typeof(TViewModel));
    }

    public Window CreateWindow(Type viewModelType)
    {
        try
        {
            var view = serviceProvider.GetService(WindowTypeRegistry.ResolveWindowType(viewModelType));

            if (view is not Window window)
                throw new ApplicationException($"View '{view}' is not a Window");

            return window;
        }
        catch (Exception exception)
        {
            //logger.LogError(exception, "Could not resolve page or view model for view model type {ViewModelType}.", viewModelType.Name);
            throw new Exception($"Could not resolve page or view model for view model type {viewModelType.Name}.", exception);
        }
    }
}