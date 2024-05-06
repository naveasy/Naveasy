namespace Naveasy;

public interface INavigationResult
{
    bool Success { get; }
    Exception Exception { get; }
}

public class NavigationResult : INavigationResult
{
    public NavigationResult(bool success)
    {
        Success = success;
    }

    public NavigationResult(Exception exception)
    {
        Exception = exception;
    }

    public bool Success { get; }
    public Exception Exception { get; }
}