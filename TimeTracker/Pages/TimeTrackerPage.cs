// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using System;
using TimeTracker.Commands;
using TimeTracker.Helpers;
using TimeTracker.Services;

namespace TimeTracker;

internal sealed partial class TimeTrackerPage : ListPage
{
    private SettingsManager _settingsManager;
    private StateService _stateService;

    public TimeTrackerPage(SettingsManager settingsManager)
    {
        Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");
        Title = "Time Tracker";
        Name = "Open";

        _settingsManager = settingsManager;

        ArgumentNullException.ThrowIfNull(_settingsManager.StateJsonPath, nameof(_settingsManager.StateJsonPath));
        _stateService = new StateService(_settingsManager.StateJsonPath);
    }

    public override IListItem[] GetItems()
    {
        return [
            new ListItem(new StartTrackingCommand(_settingsManager, _stateService)) {
                Title = "Start tracking",
            },
            new ListItem(new NoOpCommand()) {
                Title = "Stop tracking",
            },
            new ListItem(new NoOpCommand()) {
                Title = "Pause tracking",
            },
            new ListItem(new NoOpCommand()) {
                Title = "Show tracked",
            },
        ];
    }
}
