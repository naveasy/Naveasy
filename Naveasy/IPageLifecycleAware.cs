namespace Naveasy;

public interface IPageLifecycleAware
{
    void OnAppearing();
    void OnDisappearing();
}