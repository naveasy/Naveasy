using System.Reactive.Subjects;

namespace Naveasy.Samples.Services;

public interface IFlyoutService
{
    IObservable<bool> ObserveIsPresented { get; }
    void SetIsPresented(bool isPresented);
    void Dispose();
}

public class FlyoutService : IDisposable, IFlyoutService
{
    private readonly BehaviorSubject<bool> _isPresentedSubject = new BehaviorSubject<bool>(false);

    public IObservable<bool> ObserveIsPresented=> _isPresentedSubject;

    public void SetIsPresented(bool isPresented)
    {
        _isPresentedSubject.OnNext(isPresented);
    }

    public void Dispose()
    {
        _isPresentedSubject.Dispose();
    }
}