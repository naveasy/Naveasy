using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Naveasy.Samples.Views;

namespace Naveasy.Samples.ViewModels;

public class ProductsPageViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    public ProductsPageViewModel(INavigationService navigationService, ILogger<ProductsPageViewModel> logger) : base(logger)
    {
        Title = "Products";
        _navigationService = navigationService;
        DetailsCommand = new Command(OnDetails);
    }

    public ICommand DetailsCommand { get; }

    private void OnDetails()
    {
        _navigationService.NavigateAsync<ProductDetailsPageViewModel>();
    }
}