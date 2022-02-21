// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.JSInterop;

public interface IJSInProcessRuntime : IJSRuntime
{
    [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed.")]
    TResult Invoke<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResult>(string identifier, params object?[]? args);
}
