﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Omnifactotum;

/// <summary>
///     Contains extension methods for the <see cref="EventHandlerAsync{TEventArgs}"/> delegate.
/// </summary>
public static class EventHandlerAsyncExtensions
{
    /// <summary>
    ///     Asynchronously and sequentially invokes the delegates representing the invocation list of
    ///     the specified <see cref="EventHandlerAsync{TEventArgs}"/> delegate.
    /// </summary>
    /// <param name="delegate">
    ///     The delegate to invoke asynchronously.
    /// </param>
    /// <typeparam name="TEventArgs">
    ///     The type of the event data generated by the event.
    /// </typeparam>
    /// <param name="sender">
    ///     The source of the event.
    /// </param>
    /// <param name="eventArgs">
    ///     An object that contains the event data.
    /// </param>
    /// <param name="cancellationToken">
    ///     The token to monitor for cancellation requests.
    /// </param>
    public static async Task InvokeAsync<TEventArgs>(
        this EventHandlerAsync<TEventArgs>? @delegate,
        object? sender,
        TEventArgs eventArgs,
        CancellationToken cancellationToken = default)
    {
        var invocations = @delegate.GetTypedInvocations();
        if (invocations.Length == 0)
        {
            return;
        }

        foreach (var invocation in invocations)
        {
            await invocation(sender, eventArgs, cancellationToken);
        }
    }

    /// <summary>
    ///     Asynchronously and concurrently invokes the delegates representing the invocation list of
    ///     the specified <see cref="EventHandlerAsync{TEventArgs}"/> delegate.
    /// </summary>
    /// <param name="delegate">
    ///     The delegate to invoke asynchronously.
    /// </param>
    /// <typeparam name="TEventArgs">
    ///     The type of the event data generated by the event.
    /// </typeparam>
    /// <param name="sender">
    ///     The source of the event.
    /// </param>
    /// <param name="eventArgs">
    ///     An object that contains the event data.
    /// </param>
    /// <param name="cancellationToken">
    ///     The token to monitor for cancellation requests.
    /// </param>
    public static async Task InvokeParallelAsync<TEventArgs>(
        this EventHandlerAsync<TEventArgs>? @delegate,
        object? sender,
        TEventArgs eventArgs,
        CancellationToken cancellationToken = default)
    {
        var invocations = @delegate.GetTypedInvocations();
        if (invocations.Length == 0)
        {
            return;
        }

        await invocations.Select(invocation => invocation(sender, eventArgs, cancellationToken)).AwaitAllAsync();
    }
}