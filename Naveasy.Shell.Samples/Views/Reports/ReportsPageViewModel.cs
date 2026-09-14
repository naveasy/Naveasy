namespace Naveasy.Shell.Samples.Views.Reports;

/// <summary>
/// The ViewModel of a real TabbedPage pushed as a Shell route. Naveasy resolves the ViewModel of the TabbedPage
/// and of each one of its children, and forwards the lifecycle to the tab that is currently selected.
/// </summary>
public class ReportsPageViewModel : ViewModelBase
{
    public ReportsPageViewModel()
    {
        Title = "Reports";
    }
}
