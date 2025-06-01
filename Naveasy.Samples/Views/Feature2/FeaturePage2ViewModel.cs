namespace Naveasy.Samples.Views.Feature2;

public class FeaturePage2ViewModel : ViewModelBase
{
    private string? _primitive;
    private readonly INavigationService _navigationService;
    private ProductModel _product = null!;
    private ClientModel _client = null!;

    public FeaturePage2ViewModel(INavigationService navigationService)
    {
        Title = "Page 2";
        _navigationService = navigationService;
        NavigateCommand = new Command(Navigate);
    }

    public string? Primitive
    {
        get => _primitive;
        set => SetProperty(ref _primitive, value);
    }


    public ClientModel Client
    {
        get => _client;
        set => SetProperty(ref _client, value);
    }

    public ProductModel Product
    {
        get => _product;
        set => SetProperty(ref _product, value);
    }

    public ICommand NavigateCommand { get; }

    public override void OnInitialize(INavigationParameters parameters)
    {
        Product = parameters.GetValue<ProductModel>();
        Client = parameters.GetValue<ClientModel>();
    }

    public override void OnNavigatedTo(INavigationParameters navigationParameters)
    {
        base.OnNavigatedTo(navigationParameters);
    }

    public override void OnNavigatedFrom(INavigationParameters navigationParameters)
    {
        base.OnNavigatedFrom(navigationParameters);
    }

    private void Navigate()
    {
        _navigationService.NavigateAsync<MyPage3ViewModel>();
    }
}