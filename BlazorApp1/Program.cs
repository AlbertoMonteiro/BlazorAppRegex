BlazorApp1.Helpers.JsRuntime = new CustomRuntime();
BlazorApp1.Helpers.RegexMatches("", ".");
public class CustomRuntime : Microsoft.JSInterop.WebAssembly.WebAssemblyJSRuntime
{
}