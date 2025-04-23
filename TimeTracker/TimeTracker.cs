// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.CommandPalette.Extensions;
using Serilog;

namespace TimeTracker;

[Guid("975d905c-95eb-4775-92ca-4110a9251120")]
public sealed partial class TimeTracker : IExtension, IDisposable
{
    private readonly ManualResetEvent _extensionDisposedEvent;
    private readonly TimeTrackerCommandsProvider _provider;

    public TimeTracker(ManualResetEvent extensionDisposedEvent, ILogger logger)
    {
        this._extensionDisposedEvent = extensionDisposedEvent;
        _provider = new(logger);
    }

    public object? GetProvider(ProviderType providerType)
    {
        return providerType switch
        {
            ProviderType.Commands => _provider,
            _ => null,
        };
    }

    public void Dispose() => this._extensionDisposedEvent.Set();
}
