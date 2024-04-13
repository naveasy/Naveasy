namespace Naveasy.Navigation;

public interface IPageLifecycleAware
{
    void OnAppearing();
    void OnDisappearing();
}