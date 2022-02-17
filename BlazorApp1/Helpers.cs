using Microsoft.JSInterop;
using System.Runtime.CompilerServices;

namespace BlazorApp1;

public sealed class CustomRuntime : Microsoft.JSInterop.WebAssembly.WebAssemblyJSRuntime
{
}

public static class Helpers
{
    private const string TRUE = "true";
    private const string FALSE = "false";
    public static CustomRuntime JsRuntime = new();

    [JSInvokable]
    public static void RegexMatches(string regex, string value)
     => System.Threading.Tasks.Task.Run(() =>
     {
         var regexSp = ValueStopwatch.StartNew();
         var results = System.Text.RegularExpressions.Regex.Matches(value, regex);
         var builder = new System.Text.StringBuilder();
         builder.Append("[");
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
         builder = builder.Append("]");

         JsRuntime.InvokeUnmarshalled<string, int, int>("regexCallback", builder.ToString(), (int)regexSp.GetElapsedTime().TotalMilliseconds);

         static string CreateGroup(System.Text.RegularExpressions.Group currentGroup)
            => $"{{\"index\":{currentGroup.Index},\"length\":{currentGroup.Length},\"success\":{(currentGroup.Success ? TRUE : FALSE)},\"name\":\"{currentGroup.Name.Replace("\"", "\\\"")}\",\"value\":\"{currentGroup.Value.Replace("\"", "\\\"")}\"}}";
     });
}