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

    public static void Init() => JsRuntime.InvokeUnmarshalled<string>("engineInit");

    [JSInvokable]
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    public static void RegexMatches(string pattern, string value, int flags)
        => System.Threading.Tasks.Task.Run(() =>
        {
            var results = Regex.Matches(value, pattern, (RegexOptions)flags);
            var builder = new StringBuilder();
            var i = 0;
            var result = results[i];
            builder.Append('[');
            var regexGroups = result.Groups;
            var j = 0;
            builder.Append('[');
            CreateGroup(regexGroups[j], builder, j, i);
            for (j++; j < regexGroups.Count; j++)
            {
                builder.Append(',');
                CreateGroup(regexGroups[j], builder, j, i);
            }
            builder.Append(']');

            for (i++; i < results.Count; i++)
            {
                result = results[i];
                builder.Append(',');
                regexGroups = result.Groups;
                j = 0;
                builder.Append('[');
                CreateGroup(regexGroups[j], builder, j, i);
                for (j++; j < regexGroups.Count; j++)
                {
                    builder.Append(',');
                    CreateGroup(regexGroups[j], builder, j, i);
                }
                builder.Append(']');
            }
            builder = builder.Append(']');

            JsRuntime.InvokeUnmarshalled<string, int>("regexCallback", builder.ToString());
        }).ContinueWith(t =>
        {
            if (t.IsFaulted)
                JsRuntime.InvokeUnmarshalled<string, int>("regexCallback", "[]");
        });

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
            CreateGroup(regexGroups[j], builder, j, 0);
            for (j++; j < regexGroups.Count; j++)
            {
                builder.Append(',');
                CreateGroup(regexGroups[j], builder, j, 0);
            }
            builder.Append(']');
            builder.Append(']');

            JsRuntime.InvokeUnmarshalled<string, int>("regexCallback", builder.ToString());
        }).ContinueWith(t =>
        {
            if (t.IsFaulted)
                JsRuntime.InvokeUnmarshalled<string, int>("regexCallback", "[]");
        });

    [JSInvokable]
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    public static void RegexReplace(string pattern, string value, string replacement, int flags, bool global)
        => System.Threading.Tasks.Task.Run(() =>
        {
            var regex = new Regex(pattern, (RegexOptions)flags);
            var result = global ? regex.Replace(value, replacement) : regex.Replace(value, replacement, 1);
            JsRuntime.InvokeUnmarshalled<string, int>("regexCallback", result);
        }).ContinueWith(t =>
        {
            if (t.IsFaulted)
                JsRuntime.InvokeUnmarshalled<string, int>("regexCallback", value);
        });

    [JSInvokable]
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    public static void RegexListReplace(string pattern, string value, string replacement, int flags, bool global)
        => System.Threading.Tasks.Task.Run(() =>
        {
            var sb = new StringBuilder();
            var regex = new Regex(pattern, (RegexOptions)flags);
            _ = global ? regex.Replace(value, Append) : regex.Replace(value, Append, 1);
            JsRuntime.InvokeUnmarshalled<string, int>("regexCallback", sb.ToString());

            string Append(Match match)
            {
                sb.Append(match.Result(replacement));
                return null!;
            }
        }).ContinueWith(t =>
        {
            if (t.IsFaulted)
                JsRuntime.InvokeUnmarshalled<string, int>("regexCallback", value);
        });

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CreateGroup(Group currentGroup, StringBuilder stringBuilder, int groupIndex, int matchIdx)
    {
        stringBuilder.Append($"{{\"start\":{currentGroup.Index},\"end\":{currentGroup.Index + currentGroup.Length},\"isParticipating\":{(currentGroup.Success ? TRUE : FALSE)},\"groupNum\":{groupIndex},\"match\":{matchIdx},\"groupName\":\"{currentGroup.Name}\",\"content\":\"");
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
