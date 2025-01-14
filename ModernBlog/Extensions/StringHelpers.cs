using System.Text.RegularExpressions;

namespace ModernBlog.Extensions;

public static class StringHelpers
{
    public static string ManageSlug(this string name)
    {
        string newSlug = Regex.Replace(name.ToLower(), @"[^a-z0-9\-_]"
            , "_", RegexOptions.Compiled, TimeSpan.FromSeconds(1))
            .Replace("--", "-")
            .Trim('-');
        return newSlug;
    }

    public static string Truncate(this string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return maxLength < 5 && value.Length > maxLength
            ? value.Substring(0, maxLength)
            : value.Length <= maxLength ? value : value.Substring(0, maxLength - 3) + "...";
    }
}