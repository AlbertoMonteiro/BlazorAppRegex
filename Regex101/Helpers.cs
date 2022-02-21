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
        var results = Regex.Matches(value, pattern, (RegexOptions)flags);
        var builder = new StringBuilder();
        var i = 0;
        var result = results[i];
        var regexGroups = result.Groups;
        var j = 0;
        _ = builder.Append('[').Append('[');
        CreateGroup(regexGroups[j], builder, j, i);
        for (j++; j < regexGroups.Count; j++)
        {
            _ = builder.Append(',');
            CreateGroup(regexGroups[j], builder, j, i);
        }
        _ = builder.Append(']');

        for (i++; i < results.Count; i++)
        {
            result = results[i];
            regexGroups = result.Groups;
            j = 0;
            _ = builder.Append(',').Append('[');
            CreateGroup(regexGroups[j], builder, j, i);
            for (j++; j < regexGroups.Count; j++)
            {
                _ = builder.Append(',');
                CreateGroup(regexGroups[j], builder, j, i);
            }
            _ = builder.Append(']');
        }
        return builder.Append(']').ToString();
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    public static string RegexMatch(string pattern, string value, int flags)
    {
        var result = Regex.Match(value, pattern, (RegexOptions)flags);
        var builder = new StringBuilder();
        var regexGroups = result.Groups;
        var j = 0;
        builder.Append('[').Append('[');
        CreateGroup(regexGroups[j], builder, j, 0);
        for (j++; j < regexGroups.Count; j++)
        {
            builder.Append(',');
            CreateGroup(regexGroups[j], builder, j, 0);
        }
        return builder.Append(']').Append(']').ToString();
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
