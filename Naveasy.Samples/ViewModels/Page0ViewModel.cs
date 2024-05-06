using System.Text.Json;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Naveasy.Samples.Models;
using Naveasy.Samples.Views;

namespace Naveasy.Samples.ViewModels;

public class Page0ViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    public Page0ViewModel(INavigationService navigationService, ILogger<Page0ViewModel> logger): base(logger)
    {
        _navigationService = navigationService;
        Title = "Page 0";
        NavigateCommand = new Command(Navigate);
    }

    private string? _text;
    public string? Text
    {
        get => _text;
        set => SetProperty(ref _text, value);
    }

    public ICommand NavigateCommand { get; }

    public override void OnInitialize(INavigationParameters parameters)
    {
        base.OnInitialize(parameters);
        var model = parameters.GetValue<ModelA>();
        var primitive = parameters.GetValue<double>();

        Text = $"Received from navigation parameters. \n\n primitive: {primitive} \n\nObject:\n{JsonSerializer.Serialize(model)}";
    }

    private void Navigate()
    {
        _navigationService.NavigateAsync<Page1ViewModel>();
    }
}