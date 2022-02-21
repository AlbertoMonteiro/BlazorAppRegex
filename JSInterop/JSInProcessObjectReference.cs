// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.JSInterop.Implementation;

public class JSInProcessObjectReference : JSObjectReference, IJSInProcessObjectReference
{
    private readonly JSInProcessRuntime _jsRuntime;

    protected internal JSInProcessObjectReference(JSInProcessRuntime jsRuntime, long id) : base(jsRuntime, id)
    {
        _jsRuntime = jsRuntime;
    }

    [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed.")]
    public TValue Invoke<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue>(string identifier, params object?[]? args)
    {
        ThrowIfDisposed();

        return _jsRuntime.Invoke<TValue>(identifier, Id, args);
    }

    public void Dispose()
    {
        if (!Disposed)
        {
            Disposed = true;

            _jsRuntime.InvokeVoid("DotNet.jsCallDispatcher.disposeJSObjectReferenceById", Id);
        }
    }
}
