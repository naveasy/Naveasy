using System.ComponentModel;
using Naveasy.Core;

namespace Naveasy;

public interface INavigationParameters : IDictionary<string, object>
{
    T GetValue<T>(string key);
    T GetValue<T>();
}

public class NavigationParameters : Dictionary<string, object>, INavigationParameters, INavigationParametersInternal
{
	private readonly Dictionary<string, object> _internalParameters = new Dictionary<string, object>();
	
	void INavigationParametersInternal.Add(string key, object value)
	{
		_internalParameters.Add(key, value);
	}

	bool INavigationParametersInternal.ContainsKey(string key)
	{
		return _internalParameters.ContainsKey(key);
	}

	T INavigationParametersInternal.GetValue<T>(string key)
	{
		return _internalParameters.GetValue<T>(key);
	}

    public T GetValue<T>(string key)
    {
        return (T)this[key];
    }

    public T GetValue<T>()
    {
        var key = typeof(T).FullName!;
        if (!ContainsKey(key))
            return default(T);

        return (T)this[key];
    }
}

public static class ExtensionsOfNavigationParameters
{
    public static NavigationParameters ToNavigationParameter(this object value, Type genericType)
    {
        return new NavigationParameters { { genericType.FullName ?? throw new InvalidOperationException(), value } };
    }

    public static NavigationParameters ToNavigationParameter(this object value)
    {
        return new NavigationParameters { { value.GetType().FullName ?? throw new InvalidOperationException(), value } };
    }

    public static NavigationParameters Including(this NavigationParameters self, object value)
    {
        self.Add(value.GetType().FullName ?? throw new InvalidOperationException(), value);
        return self;
    }

    public static NavigationParameters Including(this NavigationParameters self, object value, Type genericType)
    {
        self.Add(genericType.FullName ?? throw new InvalidOperationException(), value);
        return self;
    }
}

public static class ParamExtensions
{
	/// <summary>
	/// Searches <paramref name="parameters"/> for <paramref name="key"/>
	/// </summary>
	/// <typeparam name="T">The type of the parameter to return</typeparam>
	/// <param name="parameters">A collection of parameters to search</param>
	/// <param name="key">The key of the parameter to find</param>
	/// <returns>A matching value of <typeparamref name="T"/> if it exists</returns>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static T GetValue<T>(this IEnumerable<KeyValuePair<string, object>> parameters, string key) =>
		(T)GetValue(parameters, key, typeof(T));

	/// <summary>
	/// Searches <paramref name="parameters"/> for value referenced by <paramref name="key"/>
	/// </summary>
	/// <param name="parameters">A collection of parameters to search</param>
	/// <param name="key">The key of the parameter to find</param>
	/// <param name="type">The type of the parameter to return</param>
	/// <returns>A matching value of <paramref name="type"/> if it exists</returns>
	/// <exception cref="InvalidCastException">Unable to convert the value of Type</exception>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static object GetValue(this IEnumerable<KeyValuePair<string, object>> parameters, string key, Type type)
	{
		foreach (var kvp in parameters)
		{
			if (string.Compare(kvp.Key, key, StringComparison.Ordinal) == 0)
			{
				if (TryGetValueInternal(kvp, type, out var value))
					return value;

				throw new InvalidCastException($"Unable to convert the value of Type '{kvp.Value.GetType().FullName}' to '{type.FullName}' for the key '{key}' ");
			}
		}

		return GetDefault(type);
	}
	
	private static bool TryGetValueInternal(KeyValuePair<string, object> kvp, Type type, out object value)
	{
		value = GetDefault(type);
		var valueAsString = kvp.Value is string str ? str : kvp.Value?.ToString();
		var success = false;
		if (kvp.Value == null)
		{
			success = true;
		}
		else if (kvp.Value.GetType() == type)
		{
			success = true;
			value = kvp.Value;
		}
		else if (type.IsAssignableFrom(kvp.Value.GetType()))
		{
			success = true;
			value = kvp.Value;
		}
		else if (type.IsEnum && !string.IsNullOrEmpty(valueAsString))
		{
			if (Enum.IsDefined(type, valueAsString))
			{
				success = true;
				value = Enum.Parse(type, valueAsString);
			}
			else if (int.TryParse(valueAsString, out var numericValue))
			{
				success = true;
				value = Enum.ToObject(type, numericValue);
			}
		}

		if (!success && type.GetInterface("System.IConvertible") != null)
		{
			success = true;
			value = Convert.ChangeType(kvp.Value, type);
		}

		return success;
	}
	
	private static object GetDefault(Type type)
	{
		if (type.IsValueType)
		{
			return Activator.CreateInstance(type);
		}
		return null;
	}
}