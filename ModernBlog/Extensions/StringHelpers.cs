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
}