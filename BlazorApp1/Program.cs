using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

var builder = Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHostBuilder.CreateDefault(args);
Helpers.JsRuntime = (IJSUnmarshalledRuntime)builder.Build().Services.GetService<IJSRuntime>();
