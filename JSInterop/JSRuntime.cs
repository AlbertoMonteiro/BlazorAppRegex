// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.JSInterop;

public abstract partial class JSRuntime : IJSRuntime
{
    private long _nextPendingTaskId = 1; // Start at 1 because zero signals "no response needed"
    private readonly ConcurrentDictionary<long, object> _pendingTasks = new();
    private readonly ConcurrentDictionary<long, CancellationTokenRegistration> _cancellationRegistrations = new();

    protected JSRuntime()
    {
    }

    protected TimeSpan? DefaultAsyncTimeout { get; set; }

    public ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue>(string identifier, object?[]? args)
        => InvokeAsync<TValue>(0, identifier, args);

    public ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
        => InvokeAsync<TValue>(0, identifier, cancellationToken, args);

    internal async ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue>(long targetInstanceId, string identifier, object?[]? args)
    {
        if (DefaultAsyncTimeout.HasValue)
        {
            using var cts = new CancellationTokenSource(DefaultAsyncTimeout.Value);
            // We need to await here due to the using
            return await InvokeAsync<TValue>(targetInstanceId, identifier, cts.Token, args);
        }

        return await InvokeAsync<TValue>(targetInstanceId, identifier, CancellationToken.None, args);
    }

    internal ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue>(
        long targetInstanceId,
        string identifier,
        CancellationToken cancellationToken,
        object?[]? args)
    {
        var taskId = Interlocked.Increment(ref _nextPendingTaskId);
        var tcs = new TaskCompletionSource<TValue>();
        if (cancellationToken.CanBeCanceled)
        {
            _cancellationRegistrations[taskId] = cancellationToken.Register(() =>
            {
                tcs.TrySetCanceled(cancellationToken);
                CleanupTasksAndRegistrations(taskId);
            });
        }
        _pendingTasks[taskId] = tcs;

        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                tcs.TrySetCanceled(cancellationToken);
                CleanupTasksAndRegistrations(taskId);

                return new ValueTask<TValue>(tcs.Task);
            }

            var resultType = JSCallResultTypeHelper.FromGeneric<TValue>();

            BeginInvokeJS(taskId, identifier, null, resultType, targetInstanceId);

            return new ValueTask<TValue>(tcs.Task);
        }
        catch
        {
            CleanupTasksAndRegistrations(taskId);
            throw;
        }
    }

    private void CleanupTasksAndRegistrations(long taskId)
    {
        _pendingTasks.TryRemove(taskId, out _);
        if (_cancellationRegistrations.TryRemove(taskId, out var registration))
        {
            registration.Dispose();
        }
    }

    protected abstract void BeginInvokeJS(long taskId, string identifier, string? argsJson, JSCallResultType resultType, long targetInstanceId);
}
