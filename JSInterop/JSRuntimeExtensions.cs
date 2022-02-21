// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.JSInterop.Infrastructure;

namespace Microsoft.JSInterop;

public static class JSRuntimeExtensions
{
    public static async ValueTask InvokeVoidAsync(this IJSRuntime jsRuntime, string identifier, params object?[]? args)
    {
        if (jsRuntime is null)
        {
            throw new ArgumentNullException(nameof(jsRuntime));
        }

        await jsRuntime.InvokeAsync<IJSVoidResult>(identifier, args);
    }
}
