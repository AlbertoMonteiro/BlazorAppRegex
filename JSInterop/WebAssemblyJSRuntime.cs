// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using WebAssembly.JSInterop;

namespace Microsoft.JSInterop.WebAssembly;

public class WebAssemblyJSRuntime : JSInProcessRuntime, IJSUnmarshalledRuntime
{
    public static WebAssemblyJSRuntime Instance { get; } = new WebAssemblyJSRuntime();
    protected WebAssemblyJSRuntime()
    {
    }

    protected override string InvokeJS(string identifier, string? argsJson, JSCallResultType resultType, long targetInstanceId)
    {
        var callInfo = new JSCallInfo
        {
            FunctionIdentifier = identifier,
            TargetInstanceId = targetInstanceId,
            ResultType = resultType,
            MarshalledCallArgsJson = argsJson ?? "[]",
            MarshalledCallAsyncHandle = default
        };

        var result = InternalCalls.InvokeJS<object, object, object, string>(out var exception, ref callInfo, null, null, null);

        return null!;
    }

    protected override void BeginInvokeJS(long asyncHandle, string identifier, string? argsJson, JSCallResultType resultType, long targetInstanceId)
    {
        var callInfo = new JSCallInfo
        {
            FunctionIdentifier = identifier,
            TargetInstanceId = targetInstanceId,
            ResultType = resultType,
            MarshalledCallArgsJson = argsJson ?? "[]",
            MarshalledCallAsyncHandle = asyncHandle
        };

        InternalCalls.InvokeJS<object, object, object, string>(out _, ref callInfo, null, null, null);
    }

    internal TResult InvokeUnmarshalled<T0, T1, T2, TResult>(string identifier, T0 arg0, T1 arg1, T2 arg2, long targetInstanceId)
    {
        var resultType = JSCallResultTypeHelper.FromGeneric<TResult>();

        var callInfo = new JSCallInfo
        {
            FunctionIdentifier = identifier,
            TargetInstanceId = targetInstanceId,
            ResultType = resultType,
        };

        string exception;

        switch (resultType)
        {
            case JSCallResultType.Default:
            case JSCallResultType.JSVoidResult:
                var result = InternalCalls.InvokeJS<T0, T1, T2, TResult>(out exception, ref callInfo, arg0, arg1, arg2);
                return exception != null
                    ? throw new JSException(exception)
                    : result;
            case JSCallResultType.JSObjectReference:
                var id = InternalCalls.InvokeJS<T0, T1, T2, int>(out exception, ref callInfo, arg0, arg1, arg2);
                return exception != null
                    ? throw new JSException(exception)
                    : (TResult)(object)new WebAssemblyJSObjectReference(this, id);
            case JSCallResultType.JSStreamReference:
                var serializedStreamReference = InternalCalls.InvokeJS<T0, T1, T2, string>(out exception, ref callInfo, arg0, arg1, arg2);
                return exception != null
                    ? throw new JSException(exception)
                    : default;
            default:
                throw new InvalidOperationException($"Invalid result type '{resultType}'.");
        }
    }

    public TResult InvokeUnmarshalled<TResult>(string identifier)
        => InvokeUnmarshalled<object?, object?, object?, TResult>(identifier, null, null, null, 0);
}
