namespace Naveasy.Core;

public interface INavigationParametersInternal
{
    void Add(string key, object value);
    bool ContainsKey(string key);
    T GetValue<T>(string key);
}