// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using System;
using TimeTracker.Commands;
using TimeTracker.Helpers;
using TimeTracker.Data;
using Serilog;
using TimeTracker.Pages;
using TimeTracker.Forms;
using System.Collections.Generic;

namespace TimeTracker;

internal sealed partial class TimeTrackerPage : ListPage
{
    private SettingsManager _settingsManager;
    private StateRepository _stateService;
    private ILogger _logger;

    public TimeTrackerPage(SettingsManager settingsManager, ILogger logger)
    {
        Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");
        Title = "Time Tracker";
        Name = "Open";

        _settingsManager = settingsManager;

        ArgumentNullException.ThrowIfNull(_settingsManager.StateJsonPath, nameof(_settingsManager.StateJsonPath));
        _stateService = new StateRepository(_settingsManager.StateJsonPath);

        _logger = logger;
    }

    public override IListItem[] GetItems()
    {
        var state = _stateService.LoadState();

        List<ListItem> items = [
            new ListItem(new StartTrackingCommand(_settingsManager, _stateService, _logger)) {
                Title = "Start tracking",
            },
            new ListItem(new NoOpCommand()) {
                Title = "Pause tracking",
            },
            new ListItem(new NoOpCommand()) {
                Title = "Show tracked",
            },
            new ListItem(new ExtensionInfoCommand(_logger)) {
                Title = "Extension info",
            },
        ];

        if(state.Type == State.StateType.Tracking || state.Type == State.StateType.Paused)
        {
            items.Insert(1,
                new ListItem(new CloseTrackingPage(_settingsManager, _stateService, _logger)) {
                    Title = "Stop tracking",
                }
            );
        }

        return items.ToArray();
    }
}
