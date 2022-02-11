using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Text.RegularExpressions;

_ = WebAssemblyHostBuilder.CreateDefault(args);

public static class Sample
{
    [Microsoft.JSInterop.JSInvokable]
    public static AllResults SayHelloCS(string regex, string value)
    {
        var date = System.DateTime.Now;
        var results = Regex.Matches(value, regex);
        var allResults = new Result[results.Count];
        for (int i = 0; i < allResults.Length; i++)
        {
            var result = results[i];
            var regexGroups = result.Groups;
            var groups = new RegexGroup[regexGroups.Count];
            for (int j = 0; j < groups.Length; j++)
            {
                var currentGroup = regexGroups[j];
                groups[j] = new RegexGroup(currentGroup.Index, currentGroup.Length, currentGroup.Success, currentGroup.Name, currentGroup.Value);
            }
            allResults[i] = new Result(result.Success, groups);
        }

        var ms = System.DateTime.Now.Subtract(date).TotalMilliseconds;
        return new AllResults(allResults, ms);
    }
}

public record RegexGroup(int Index, int Length, bool Success, string Name, string Value);
public record Result(bool Success, RegexGroup[] Groups);
public record AllResults(Result[] allResults, double Elapsed);