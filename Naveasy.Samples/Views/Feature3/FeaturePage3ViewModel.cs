using System.Text.Json;
using System.Windows.Input;
using Naveasy.Samples.Models;
using Naveasy.Samples.Views.FeatureA;

namespace Naveasy.Samples.Views.Feature3;

public class MyPage3ViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    public MyPage3ViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        Title = "Page 3";
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
        _navigationService.NavigateAsync<FeaturePageAViewModel>();
    }
}