// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.JSInterop;

public class JSException : Exception
{
    public JSException(string message) : base(message)
    {
    }

    public JSException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
