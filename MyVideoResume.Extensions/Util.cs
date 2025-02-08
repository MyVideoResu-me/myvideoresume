using Microsoft.AspNetCore.Http;
using System.Dynamic;

namespace MyVideoResume.Extensions;

public partial class Util
{
    public static bool HasProperty(dynamic item, string propertyName)
    {
        if (item is ExpandoObject eo)
        {
            return (eo as IDictionary<string, object>).ContainsKey(propertyName);
        }
        else
        {
            return item.GetType().GetProperty(propertyName);
        }
    }
}

public static class Extensions
{
    public static string Url(this HttpRequest request)
    {
        var fullUrl = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";

        return fullUrl;
    }
    public static bool HasValue(this string value)
    {
        var result = !string.IsNullOrWhiteSpace(value);

        return result;
    }
}
