using System.ComponentModel;
using System.Windows;
//using Microsoft.Extensions.Logging;

namespace Naveasy.WPF;

public interface INavigationService
{
    void Show<TViewModel>(NavigationParameters? navigationParameters = null);
    void ShowSingle<TViewModel>(NavigationParameters? navigationParameters = null);
    object? ShowModal<TViewModel>(NavigationParameters? navigationParameters = null);
    void Close(object viewModel);
    void SetDefaultParentWindow(Window defaultParentWindow);
}

public interface IStartupNavigationService
{
    Window GetSingleWindow<TViewModel>(NavigationParameters? navigationParameters = null);
}

public class NavigationService(IWindowFactory windowFactory, IServiceProvider serviceProvider/*, ILogger<NavigationService> logger*/)
    : INavigationService, IStartupNavigationService
{
    private readonly Dictionary<string, Window> _singletonWindows = [];
    private readonly List<Window> _allWindows = [];
    private Window? _defaultParentWindow;

    public void Show<TViewModel>(NavigationParameters? navigationParameters = null)
    {
        var window = GetWindow<TViewModel>(isSingle: false, navigationParameters);
        window.Show();
        window.Activate();
    }

    public void ShowSingle<TViewModel>(NavigationParameters? navigationParameters = null)
    {
        var window = GetWindow<TViewModel>(isSingle: true, navigationParameters);
        window.Show();
        window.Activate();
    }

    public object? ShowModal<TViewModel>(NavigationParameters? navigationParameters = null)
    {
        var window = GetWindow<TViewModel>(isSingle: true, navigationParameters);
        window.ShowDialog();

        if (window.DataContext is IModalResult viewModel)
        {
            return viewModel.ModalResult;
        }
        return null;
    }

    public void SetDefaultParentWindow(Window defaultParentWindow)
    {
        _defaultParentWindow = defaultParentWindow;
    }

    public Window GetSingleWindow<TViewModel>(NavigationParameters? navigationParameters = null)
    {
        return GetWindow<TViewModel>(isSingle: true, navigationParameters);
    }

    public void Close(object viewModel)
    {
        var window = _allWindows.SingleOrDefault(x => x.DataContext == viewModel);
        window?.Close();
    }

    private Window GetWindow<TViewModel>(bool isSingle, NavigationParameters? navigationParameters = null)
    {
        var viewModel = serviceProvider.GetService(typeof(TViewModel));
        var parameters = navigationParameters ?? new NavigationParameters();
        var viewModelType = viewModel!.GetType();
        var viewModelTypeName = viewModelType.FullName!;

        if (_singletonWindows.TryGetValue(viewModelTypeName, out var existingWindow))
            return existingWindow;

        if (viewModel is IInitialize iInitialize)
        {
            iInitialize.OnInitialize(parameters);
        }

        var window = windowFactory.CreateWindow(viewModelType);

        InitializeWindowControl(viewModel, isSingle, parameters, viewModelTypeName, window);
        return window;
    }

    private void InitializeWindowControl<TViewModel>(TViewModel viewModel, bool isSingle, NavigationParameters parameters, string viewModelTypeName, Window window)
    {
        window.DataContext = viewModel;
        window.Owner = _defaultParentWindow;
        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        window.Closed += OnClosed;
        window.Closing += OnClosing;

        if (parameters.Height.HasValue)
            window.Height = parameters.Height.Value;

        if (parameters.Width.HasValue)
            window.Width = parameters.Width.Value;

        window.WindowStartupLocation = parameters.WindowStartupLocation ?? WindowStartupLocation.CenterScreen;

        if (parameters.ResizeMode.HasValue)
        {
            window.ResizeMode = parameters.ResizeMode.Value;
        }

        if (parameters.SizeToContent.HasValue)
            window.SizeToContent = parameters.SizeToContent.Value;

        if (!string.IsNullOrWhiteSpace(parameters.Title))
        {
            window.Title = parameters.Title;
        }

        if (isSingle)
        {
            _singletonWindows.Add(viewModelTypeName, window);
        }

        _allWindows.Add(window);

        return;

        void OnClosing(object? sender, CancelEventArgs args)
        {
            if (viewModel is INavigationAware navigationAware)
            {
                navigationAware.IsClosing = true;
                navigationAware.OnClosing(args);

                if (args.Cancel)
                {
                    navigationAware.IsClosing = false;
                }
            }
        }

        void OnClosed(object? sender, EventArgs args)
        {
            window.Closing -= OnClosing;
            window.Closed -= OnClosed;

            _allWindows.Remove(window);

            if (window.DataContext is IDisposable disposableViewModel)
            {
                disposableViewModel.Dispose();
                //logger.LogTrace("{ViewModelType} was disposed.", disposableViewModel.GetType().Name);
            }

            if (window is IDisposable disposableWindow)
            {
                disposableWindow.Dispose();
                //logger.LogTrace("{ViewType} was disposed.", window.GetType().Name);
            }

            if (_singletonWindows.ContainsKey(viewModelTypeName))
            {
                _singletonWindows.Remove(viewModelTypeName);
            }
        }
    }
}