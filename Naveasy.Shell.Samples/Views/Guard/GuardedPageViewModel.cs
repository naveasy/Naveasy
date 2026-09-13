namespace Naveasy.Shell.Samples.Views.Guard;

/// <summary>
/// Shows IConfirmNavigation: while there are unsaved changes, every way out of the page - the Shell back
/// button, the iOS swipe back gesture, the Android hardware back button and GoBackAsync - asks for confirmation.
/// </summary>
public class GuardedPageViewModel : ViewModelBase, IConfirmNavigation
{
    private readonly INavigationService _navigationService;
    private readonly IPageDialogService _pageDialogService;
    private bool _hasUnsavedChanges = true;

    public GuardedPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
    {
        Title = "Unsaved changes";
        _navigationService = navigationService;
        _pageDialogService = pageDialogService;

        SaveCommand = new Command(() => HasUnsavedChanges = false);
        GoBackCommand = new Command(async () => await _navigationService.GoBackAsync());
    }

    public bool HasUnsavedChanges
    {
        get => _hasUnsavedChanges;
        set => SetProperty(ref _hasUnsavedChanges, value);
    }

    public ICommand SaveCommand { get; }

    public ICommand GoBackCommand { get; }

    public async Task<bool> CanNavigateAsync(INavigationParameters parameters)
    {
        if (!HasUnsavedChanges)
            return true;

        return await _pageDialogService.DisplayAlertAsync(
            "Unsaved changes",
            "Leave the page and lose the changes?",
            "Leave",
            "Stay");
    }
}
