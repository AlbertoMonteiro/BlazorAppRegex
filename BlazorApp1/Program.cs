using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using System.Linq;
using System.Text.RegularExpressions;

_ = WebAssemblyHostBuilder.CreateDefault(args);

public static class Sample
{
    [JSInvokable]
    public static object SayHelloCS(string regex, string value)
    {
        var result = Regex.Match(value, regex);
        return new
        {
            result.Success,
            Captures = result.Captures.Cast<Capture>().Select(x => new { x.Index, x.Length, x.Value }),
            Groups = result.Groups.Cast<Group>().Select(x => new { x.Index, x.Length, x.Success, x.Name, x.Value })
        };
    }
}