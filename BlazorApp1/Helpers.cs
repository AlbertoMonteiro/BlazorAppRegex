using Microsoft.JSInterop;

public static class Helpers
{
    private const string TRUE = "true";
    private const string FALSE = "false";
    public static IJSUnmarshalledRuntime? JsRuntime;

    [JSInvokable]
    public static void RegexMatches(string regex, string value)
    {
        System.Threading.Tasks.Task.Run(() =>
        {
            var builder = new System.Text.StringBuilder();
            var buildSp = System.Diagnostics.Stopwatch.StartNew();
            var results = System.Text.RegularExpressions.Regex.Matches(value, regex);
            builder.Append("{\"matches\":[");
            int i = 0;
            var result = results[i];
            builder.Append($"{{\"success\":{(result.Success ? TRUE : FALSE)},\"groups\":[");
            var regexGroups = result.Groups;
            int j = 0;
            var currentGroup = regexGroups[j];
            builder.Append($"{{\"index\":{currentGroup.Index},\"length\":{currentGroup.Length},\"Success\":{(currentGroup.Success ? TRUE : FALSE)},\"name\":{currentGroup.Name},\"value\":\"{currentGroup.Value}\"}}");
            for (j++; j < regexGroups.Count; j++)
            {
                currentGroup = regexGroups[j];
                builder.Append($",{{\"index\":{currentGroup.Index},\"length\":{currentGroup.Length},\"Success\":{(currentGroup.Success ? TRUE : FALSE)},\"name\":{currentGroup.Name},\"value\":\"{currentGroup.Value}\"}}");
            }
            builder.Append("]}");

            for (i++; i < results.Count; i++)
            {
                result = results[i];
                builder.Append($",{{\"success\":{(result.Success ? TRUE : FALSE)},\"groups\":[");
                regexGroups = result.Groups;
                j = 0;
                currentGroup = regexGroups[j];
                builder.Append($"{{\"index\":{currentGroup.Index},\"length\":{currentGroup.Length},\"Success\":{(currentGroup.Success ? TRUE : FALSE)},\"name\":{currentGroup.Name},\"value\":\"{currentGroup.Value}\"}}");
                for (j++; j < regexGroups.Count; j++)
                {
                    currentGroup = regexGroups[j];
                    builder.Append($",{{\"index\":{currentGroup.Index},\"length\":{currentGroup.Length},\"Success\":{(currentGroup.Success ? TRUE : FALSE)},\"name\":{currentGroup.Name},\"value\":\"{currentGroup.Value}\"}}");
                }
                builder.Append("]}");
            }
            buildSp.Stop();
            builder.Append($"], \"elapsed\": {buildSp.ElapsedMilliseconds} }}");
            JsRuntime!.InvokeUnmarshalled<string, int>("regexCallback", builder.ToString());
        });
    }
}