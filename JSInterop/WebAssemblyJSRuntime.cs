// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using WebAssembly.JSInterop;

namespace Microsoft.JSInterop.WebAssembly;

public static class WebAssemblyJSRuntime
{
    public static TResult InvokeUnmarshalled<TResult>(string identifier)
    {
        var callInfo = new JSCallInfo
        {
            FunctionIdentifier = identifier,
            TargetInstanceId = 0,
            ResultType = JSCallResultType.Default,
        };

        string exception;

        _ = InternalCalls.InvokeJS<object?, object?, object?, TResult>(out exception, ref callInfo, null, null, null);
        return default;
    }
}
