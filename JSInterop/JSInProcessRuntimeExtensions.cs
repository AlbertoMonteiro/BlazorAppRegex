// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.JSInterop.Infrastructure;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.JSInterop;

public static class JSInProcessRuntimeExtensions
{
    [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "The method returns void, so nothing is deserialized.")]
    public static void InvokeVoid(this IJSInProcessRuntime jsRuntime, string identifier, params object?[]? args)
    {
        if (jsRuntime == null)
        {
            throw new ArgumentNullException(nameof(jsRuntime));
        }

        jsRuntime.Invoke<IJSVoidResult>(identifier, args);
    }
}
