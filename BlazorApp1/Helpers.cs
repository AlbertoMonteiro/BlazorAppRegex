using Microsoft.JSInterop;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace BlazorApp1;

public sealed class CustomRuntime : Microsoft.JSInterop.WebAssembly.WebAssemblyJSRuntime
{
}

public static class Helpers
{
    private const string TRUE = "true";
    private const string FALSE = "false";
    public static IJSUnmarshalledRuntime JsRuntime = new CustomRuntime();
    private static string _pattern;
    private static string _value;
    private static int _flags;

    public static void Init() => JsRuntime.InvokeUnmarshalled<string>("engineInit");

    [JSInvokable]
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    public static void RegexMatches(string pattern, string value, int flags)
    {
        _pattern = pattern;
        _value = value;
        _flags = flags;
        System.Threading.Tasks.Task.Run(RunRegexMatches);
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    private static void RunRegexMatches()
    {
        var results = Regex.Matches(_value, _pattern, (RegexOptions)_flags);
        var builder = new StringBuilder();
        var i = 0;
        var result = results[i];
        builder.Append('[');
        var regexGroups = result.Groups;
        var j = 0;
        builder.Append('[');
        CreateGroup(regexGroups[j], builder, j);
        for (j++; j < regexGroups.Count; j++)
        {
            builder.Append(',');
            CreateGroup(regexGroups[j], builder, j);
        }
        builder.Append(']');

        for (i++; i < results.Count; i++)
        {
            result = results[i];
            builder.Append(',');
            regexGroups = result.Groups;
            j = 0;
            builder.Append('[');
            CreateGroup(regexGroups[j], builder, j);
            for (j++; j < regexGroups.Count; j++)
            {
                builder.Append(',');
                CreateGroup(regexGroups[j], builder, j);
            }
            builder.Append(']');
        }
        builder = builder.Append(']');

        JsRuntime.InvokeUnmarshalled<string, int>("regexCallback", builder.ToString());
    }

    [JSInvokable]
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    public static void RegexMatch(string pattern, string value, int flags)
        => System.Threading.Tasks.Task.Run(() =>
        {
            var result = Regex.Match(value, pattern, (RegexOptions)flags);
            var builder = new StringBuilder();
            builder.Append('[');
            var regexGroups = result.Groups;
            var j = 0;
            builder.Append('[');
            CreateGroup(regexGroups[j], builder, j);
            for (j++; j < regexGroups.Count; j++)
            {
                builder.Append(',');
                CreateGroup(regexGroups[j], builder, j);
            }
            builder.Append(']');
            builder.Append(']');

            JsRuntime.InvokeUnmarshalled<string, int>("regexCallback", builder.ToString());
        });

    [JSInvokable]
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    public static void RegexReplace(string pattern, string value, string replacement, int flags)
        => System.Threading.Tasks.Task.Run(() =>
        {
            var regex = new Regex(pattern, (RegexOptions)flags);
            var result = regex.Replace(value, replacement);
            JsRuntime.InvokeUnmarshalled<string, int>("regexCallback", result);
        });

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CreateGroup(Group currentGroup, StringBuilder stringBuilder, int groupIndex)
    {
        stringBuilder.Append($"{{\"start\":{currentGroup.Index},\"end\":{currentGroup.Index + currentGroup.Length},\"isParticipating\":{(currentGroup.Success ? TRUE : FALSE)},\"groupNum\":{groupIndex},\"groupName\":\"{currentGroup.Name}\",\"content\":\"");
        EscapeJs(currentGroup.ValueSpan, stringBuilder);
        stringBuilder.Append('"');
        stringBuilder.Append('}');
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