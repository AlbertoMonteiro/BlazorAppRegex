﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;

namespace Microsoft.JSInterop;

internal static class JSCallResultTypeHelper
{
    // We avoid using Assembly.GetExecutingAssembly() because this is shared code.
    private static readonly Assembly _currentAssembly = typeof(JSCallResultType).Assembly;

    public static JSCallResultType FromGeneric<TResult>()
    {
        return JSCallResultType.Default;
    }
}