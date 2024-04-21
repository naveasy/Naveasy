using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Naveasy.Samples.Models;
using Naveasy.Samples.Views;

namespace Naveasy.Samples.ViewModels;

public class HomeFlyoutPageViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    public HomeFlyoutPageViewModel(INavigationService navigationService, ILogger<HomeFlyoutPageViewModel> logger) : base(logger)
    {
        _navigationService = navigationService;
        NavigateCommand = new Command<string>(DoNavigate);
        SignOutCommand = new Command(SignOut);
    }

    private string? _text;
    public string? Text
    {
        get => _text;
        set => SetProperty(ref _text, value);
    }

    public ICommand NavigateCommand { get; }
    public ICommand SignOutCommand { get; }

    public override void OnInitialize(INavigationParameters parameters)
    {
        var model = parameters.GetValue<ModelA>();
        var id = parameters.GetValue<int>();

        Text = $"{model.Name} {Environment.NewLine} id = {id} from navigation parameters.";
    }

    private void DoNavigate(string targetPage)
    {
        _navigationService.NavigateAsync<ProductsPageViewModel>();
    }

    private void SignOut()
    {
        _navigationService.NavigateAbsoluteAsync<LoginPageViewModel>();
    }
}