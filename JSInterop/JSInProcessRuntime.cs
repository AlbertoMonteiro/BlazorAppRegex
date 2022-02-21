// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.JSInterop;

public abstract class JSInProcessRuntime : JSRuntime, IJSInProcessRuntime
{
    [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed.")]
    internal TValue Invoke<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue>(string identifier, long targetInstanceId, params object?[]? args)
    {
        var resultJson = InvokeJS(
            identifier,
            "",
            JSCallResultTypeHelper.FromGeneric<TValue>(),
            targetInstanceId);

        // While the result of deserialization could be null, we're making a
        // quality of life decision and letting users explicitly determine if they expect
        // null by specifying TValue? as the expected return type.
        return resultJson is null ? default : default;
    }

    [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed.")]
    public TValue Invoke<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue>(string identifier, params object?[]? args)
        => Invoke<TValue>(identifier, 0, args);

    protected virtual string? InvokeJS(string identifier, string? argsJson)
        => InvokeJS(identifier, argsJson, JSCallResultType.Default, 0);

    protected abstract string? InvokeJS(string identifier, string? argsJson, JSCallResultType resultType, long targetInstanceId);
}
