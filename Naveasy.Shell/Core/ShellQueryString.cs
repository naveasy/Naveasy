namespace Naveasy.Shell.Core;

/// <summary>
/// Reads the query string of a Shell navigation - a deep link such as "//main/orders/OrderDetailPage?orderId=2" -
/// so that its values can be delivered as regular Naveasy navigation parameters.
/// </summary>
/// <remarks>
/// Only the state of the Navigating event carries the query string: the state of the Navigated event is rebuilt
/// from the Shell hierarchy and has no query. The location is a relative Uri, whose Query property throws, so the
/// original string is parsed instead.
/// </remarks>
internal static class ShellQueryString
{
    public static IReadOnlyList<KeyValuePair<string, object>> Parse(ShellNavigationState state) =>
        ParseLocation(state?.Location?.OriginalString);

    internal static IReadOnlyList<KeyValuePair<string, object>> ParseLocation(string location)
    {
        if (string.IsNullOrEmpty(location))
            return [];

        var queryStart = location.IndexOf('?');

        if (queryStart < 0 || queryStart == location.Length - 1)
            return [];

        var parameters = new List<KeyValuePair<string, object>>();

        foreach (var pair in location[(queryStart + 1)..].Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var separator = pair.IndexOf('=');

            if (separator <= 0)
                continue;

            var key = Uri.UnescapeDataString(pair[..separator]);
            var value = Uri.UnescapeDataString(pair[(separator + 1)..]);

            parameters.Add(new KeyValuePair<string, object>(key, value));
        }

        return parameters;
    }

    /// <summary>
    /// Copies the query values into <paramref name="parameters"/>. A value already there wins: parameters given
    /// to the navigation service are typed, and the query string is only text.
    /// </summary>
    public static void MergeInto(INavigationParameters parameters, IReadOnlyList<KeyValuePair<string, object>> query)
    {
        if (parameters is null || query is null)
            return;

        foreach (var parameter in query)
        {
            if (!parameters.ContainsKey(parameter.Key))
                parameters.Add(parameter.Key, parameter.Value);
        }
    }
}
