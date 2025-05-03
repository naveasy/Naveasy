using System.ComponentModel;

namespace Naveasy.WPF;

public interface INavigationAware
{
    bool IsClosing { get; set; }
    void OnClosing(CancelEventArgs cancel);
}
