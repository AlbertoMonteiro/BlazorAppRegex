using Microsoft.JSInterop.WebAssembly;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Regex101;

public static class Helpers
{
    private const string TRUE = "true";
    private const string FALSE = "false";

    public static void Init() => WebAssemblyJSRuntime.InvokeUnmarshalled<int>("engineInit");

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    public static string RegexMatches(string pattern, string value, int flags)
    {
        var matches = Regex.Matches(value, pattern, (RegexOptions)flags);
        var sb = new StringBuilder();

        _ = sb.Append('[');

        for (var matchIdx = 0; matchIdx < matches.Count; matchIdx++)
        {
            var match = matches[matchIdx];
            var groups = match.Groups;

            _ = sb.Append('[');

            for (var groupIdx = 0; groupIdx < groups.Count; groupIdx++)
            {
                var group = groups[groupIdx];

                CreateGroup(group, sb, groupIdx, matchIdx);

                sb.Append(',');
            }

            // Remove last ','
            sb.Length--;
            sb.Append(']');
            sb.Append(',');
        }

        // Remove last ','
        if (matches.Count > 0)
        {
            sb.Length--;
        }

        sb.Append(']');

        return sb.ToString();
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    public static string RegexMatch(string pattern, string value, int flags)
    {
        var match = Regex.Match(value, pattern, (RegexOptions)flags);
        var sb = new StringBuilder();

        sb.Append('[');

        if (match.Success)
        {
            sb.Append('[');

            var groups = match.Groups;

            for (var groupIdx = 0; groupIdx < groups.Count; groupIdx++)
            {
                var group = groups[groupIdx];

                CreateGroup(group, sb, groupIdx, 0);
                sb.Append(',');
            }

            // Remove last ','
            sb.Length--;
            sb.Append(']');
        }

        sb.Append(']');

        return sb.ToString();
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    public static string RegexReplace(string pattern, string value, string replacement, int flags, bool global)
    {
        var regex = new Regex(pattern, (RegexOptions)flags);
        var result = global ? regex.Replace(value, replacement) : regex.Replace(value, replacement, 1);
        return result;
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    public static string RegexListReplace(string pattern, string value, string replacement, int flags, bool global)
    {
        var sb = new StringBuilder();
        var regex = new Regex(pattern, (RegexOptions)flags);
        _ = global ? regex.Replace(value, Append) : regex.Replace(value, Append, 1);
        return sb.ToString();

        string Append(Match match)
        {
            sb.Append(match.Result(replacement));
            return null!;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CreateGroup(Group currentGroup, StringBuilder stringBuilder, int groupIndex, int matchIdx)
    {
        stringBuilder.Append($"{{\"start\":{currentGroup.Index},\"end\":{currentGroup.Index + currentGroup.Length},\"isParticipating\":{(currentGroup.Success ? TRUE : FALSE)},\"groupNum\":{groupIndex},\"match\":{matchIdx},\"groupName\":\"{currentGroup.Name}\",\"content\":\"");
        EscapeJs(currentGroup.ValueSpan, stringBuilder);
        stringBuilder.Append('"').Append('}');
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void EscapeJs(ReadOnlySpan<char> s, StringBuilder stringBuilder)
    {
        if (s.IsEmpty)
            return;

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
    }
}
