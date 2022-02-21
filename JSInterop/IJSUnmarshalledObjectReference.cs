// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.JSInterop;

public interface IJSUnmarshalledObjectReference : IJSInProcessObjectReference
{
    TResult InvokeUnmarshalled<TResult>(string identifier);

    TResult InvokeUnmarshalled<T0, TResult>(string identifier, T0 arg0);

    TResult InvokeUnmarshalled<T0, T1, TResult>(string identifier, T0 arg0, T1 arg1);

    TResult InvokeUnmarshalled<T0, T1, T2, TResult>(string identifier, T0 arg0, T1 arg1, T2 arg2);
}
