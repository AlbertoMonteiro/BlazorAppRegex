using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using System.Text.RegularExpressions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

await builder.Build().RunAsync();

public static class Sample
{
    [JSInvokable]
    public static string SayHelloCS(string regex, string value)
    {
        var result = Regex.IsMatch(value, regex).ToString();
        return result;
    }
}