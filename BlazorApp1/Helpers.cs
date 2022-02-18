using Microsoft.JSInterop;
using System.Text;

namespace BlazorApp1;

public sealed class CustomRuntime : Microsoft.JSInterop.WebAssembly.WebAssemblyJSRuntime
{
}

public static class Helpers
{
    private const string TRUE = "true";
    private const string FALSE = "false";
    public static readonly CustomRuntime JsRuntime = new();

    [JSInvokable]
    public static void RegexMatches(string regex, string value)
        => System.Threading.Tasks.Task.Run(() =>
        {
            var regexSp = ValueStopwatch.StartNew();
            var results = System.Text.RegularExpressions.Regex.Matches(value, regex);
            var builder = new StringBuilder();
            builder.Append('[');
            var i = 0;
            var result = results[i];
            builder.Append($"{{\"success\":{(result.Success ? TRUE : FALSE)},\"groups\":[");
            var regexGroups = result.Groups;
            var j = 0;
            var currentGroup = regexGroups[j];
            builder.Append(CreateGroup(currentGroup));
            for (j++; j < regexGroups.Count; j++)
            {
                currentGroup = regexGroups[j];
                builder.Append($",{CreateGroup(currentGroup)}");
            }
            builder.Append("]}");

            for (i++; i < results.Count; i++)
            {
                result = results[i];
                builder.Append($",{{\"success\":{(result.Success ? TRUE : FALSE)},\"groups\":[");
                regexGroups = result.Groups;
                j = 0;
                currentGroup = regexGroups[j];
                builder.Append(CreateGroup(currentGroup));
                for (j++; j < regexGroups.Count; j++)
                {
                    currentGroup = regexGroups[j];
                    builder.Append($",{CreateGroup(currentGroup)}");
                }
                builder.Append("]}");
            }
            builder = builder.Append(']');

            JsRuntime.InvokeUnmarshalled<string, int, int>("regexCallback", builder.ToString(), (int)regexSp.GetElapsedTime().TotalMilliseconds);
        });

    [JSInvokable]
    public static void RegexMatch(string regex, string value)
        => System.Threading.Tasks.Task.Run(() =>
        {
            var regexSp = ValueStopwatch.StartNew();
            var result = System.Text.RegularExpressions.Regex.Match(value, regex);
            var builder = new StringBuilder();
            builder.Append($"{{\"success\":{(result.Success ? TRUE : FALSE)},\"groups\":[");
            var regexGroups = result.Groups;
            var j = 0;
            var currentGroup = regexGroups[j];
            builder.Append(CreateGroup(currentGroup));
            for (j++; j < regexGroups.Count; j++)
            {
                currentGroup = regexGroups[j];
                builder.Append($",{CreateGroup(currentGroup)}");
            }
            builder.Append("]}");

            JsRuntime.InvokeUnmarshalled<string, int, int>("regexCallback", builder.ToString(), (int)regexSp.GetElapsedTime().TotalMilliseconds);
        });

    [JSInvokable]
    public static void RegexReplace(string regex, string value, string replacement)
        => System.Threading.Tasks.Task.Run(() =>
        {
            var regexSp = ValueStopwatch.StartNew();
            var result = System.Text.RegularExpressions.Regex.Replace(value, regex, replacement);

            JsRuntime.InvokeUnmarshalled<string, int, int>("regexCallback", $"{{ \"result\": \"{EscapeJs(result)}\" }}", (int)regexSp.GetElapsedTime().TotalMilliseconds);
        });

    static string CreateGroup(System.Text.RegularExpressions.Group currentGroup)
        => $"{{\"index\":{currentGroup.Index},\"length\":{currentGroup.Length},\"success\":{(currentGroup.Success ? TRUE : FALSE)},\"name\":\"{currentGroup.Name}\",\"value\":\"{EscapeJs(currentGroup.Value)}\"}}";

    static string EscapeJs(string s)
    {
        if (s is null)
            return "";

        if (NeedsEscape(s))
        {
            var stringBuilder = new StringBuilder(s.Length);
            foreach (var c in s)
            {
                switch (c)
                {
                    case '\\':
                        stringBuilder.Append("\\\\");
                        continue;
                    case '"':
                        stringBuilder.Append("\\\"");
                        continue;
                    case '\n':
                        stringBuilder.Append("\\n");
                        continue;
                    case '\t':
                        stringBuilder.Append("\\t");
                        continue;
                    case '\r':
                        continue;
                }

                if (c >= ' ')
                {
                    if (c <= '\u00FF')
                    {
                        stringBuilder.Append(c);
                        continue;
                    }

                    stringBuilder.Append("\\u");
                    ushort num = c;
                    stringBuilder.Append(num.ToString("X4"));
                }
            }

            return stringBuilder.ToString();
        }

        return s;
        static bool NeedsEscape(string s2)
        {
            foreach (var c2 in s2)
            {
                if (c2 is > '\u00FF' or < ' ' or '\\' or '"' or '\r' or '\n' or '\t')
                {
                    return true;
                }
            }

            return false;
        }
    }
}