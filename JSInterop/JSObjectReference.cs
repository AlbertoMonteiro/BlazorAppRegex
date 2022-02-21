// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.JSInterop.Implementation;

public class JSObjectReference : IJSObjectReference
{
    private readonly JSRuntime _jsRuntime;

    internal bool Disposed { get; set; }

    protected internal long Id { get; }

    protected internal JSObjectReference(JSRuntime jsRuntime, long id)
    {
        _jsRuntime = jsRuntime;

        Id = id;
    }

    public ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue>(string identifier, object?[]? args)
    {
        ThrowIfDisposed();

        return _jsRuntime.InvokeAsync<TValue>(Id, identifier, args);
    }

    public ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
    {
        ThrowIfDisposed();

        return _jsRuntime.InvokeAsync<TValue>(Id, identifier, cancellationToken, args);
    }

    public async ValueTask DisposeAsync()
    {
        if (!Disposed)
        {
            Disposed = true;

            await _jsRuntime.InvokeVoidAsync("DotNet.jsCallDispatcher.disposeJSObjectReferenceById", Id);
        }
    }

    protected void ThrowIfDisposed()
    {
        if (Disposed)
        {
            throw new ObjectDisposedException(GetType().Name);
        }
    }
}
