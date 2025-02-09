using Microsoft.AspNetCore.Http;
using System.ComponentModel;
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

    static public SortedList<string, string> ToSortedList(this Enum enumValue)
    {
        var field = enumValue.GetType().GetFields();
        var y = field.Where(x =>
        {
            var attributes = x.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes != null && attributes.Length > 0)
                return true;
            return false;
        }).Select(x =>
        {
            var attributes = x.GetCustomAttributes(typeof(DescriptionAttribute), false);
            var key = string.Empty;
            var value = string.Empty;
            value = (attributes.FirstOrDefault() as DescriptionAttribute).Description;
            key = x.Name;
            return new KeyValuePair<string, string>(key, value);
        }).ToDictionary();
        return new SortedList<string, string>(y);
    }
}
