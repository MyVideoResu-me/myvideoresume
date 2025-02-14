using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.Dynamic;
using System.Security.Cryptography;
using System.Text;

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

    public static SortedList<string, string> ToSortedList(this Enum enumValue)
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

    public static string GenerateSHA256Hash(this string input)
    {
        // Create a SHA256 hash object
        using (SHA256 sha256 = SHA256.Create())
        {
            // Convert the input string to a byte array
            byte[] byteArray = Encoding.UTF8.GetBytes(input);

            // Compute the hash and get the byte array result
            byte[] hashBytes = sha256.ComputeHash(byteArray);

            // Convert the byte array to a hex string
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                stringBuilder.Append(b.ToString("x2")); // Format byte as hexadecimal
            }

            return stringBuilder.ToString(); // Return the hash as a string
        }
    }
}
