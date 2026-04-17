using System.Text;

namespace BlogPlatform.Api.Services;

public static class SlugNormalizer
{
    public static string FromTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return "post";

        var sb = new StringBuilder();
        var lastWasHyphen = false;
        foreach (var c in title.Trim().ToLowerInvariant())
        {
            if (char.IsAsciiLetterOrDigit(c))
            {
                sb.Append(c);
                lastWasHyphen = false;
            }
            else if (c is ' ' or '-' or '_')
            {
                if (!lastWasHyphen && sb.Length > 0)
                {
                    sb.Append('-');
                    lastWasHyphen = true;
                }
            }
        }

        var s = sb.ToString().TrimEnd('-');
        while (s.Contains("--", StringComparison.Ordinal))
            s = s.Replace("--", "-", StringComparison.Ordinal);

        return string.IsNullOrEmpty(s) ? "post" : s;
    }
}
