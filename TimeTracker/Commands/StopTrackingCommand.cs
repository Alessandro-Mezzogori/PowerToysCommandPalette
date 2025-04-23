using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Data;
using TimeTracker.Forms;
using TimeTracker.Helpers;

namespace TimeTracker.Commands;

public sealed partial class StopTrackingCommand : InvokableCommand
{
    private readonly SettingsManager _settings;
    private readonly StateRepository _stateRepository;
    private readonly ILogger _logger;
    private readonly CloseTrackingForm.CloseTrackingFormResponse _data;

    private readonly Func<ICommandResult>? _commandResult;

    public StopTrackingCommand(
        SettingsManager settings,
        StateRepository stateRepository,
        ILogger logger,
        CloseTrackingForm.CloseTrackingFormResponse data,
        Func<ICommandResult>? commandResult = null
    )
    {
        _settings = settings;
        _stateRepository = stateRepository;
        _logger = logger;
        _data = data;

        _commandResult = commandResult;
    }

    public override ICommandResult Invoke()
    {
        var filename = FilenameManager.TrackingFilename(_settings.FilenameTemplate!, new TrackingFilenameArgs
        {
            Date = DateTime.Today,
        });

        var trackingfilePath = Path.Combine(_settings.StorageFolder!, filename);
        var taskfileService = new TrackfileService(trackingfilePath, new TrackfileInstanceTransformerMarkdown());
        var state = _stateRepository.LoadState();

        TrackfileInstance instance = new()
        {
            Title = _data.title,
            Description = _data.description,
            StartTime = state.StartTime.Value,
            EndTime = state.StartTime.Value.Add(TimeSpan.Parse(_data.duration, CultureInfo.InvariantCulture))
        };

        _logger.Information("Saving trackfile instance: {starttime} {endtime}", instance.StartTime, instance.EndTime);

        state.StartTime = null;
        state.CurrentTrack = null;

        _logger.Verbose("Saving state...");
        taskfileService.AddTrackfileInstance(instance);
        _stateRepository.SaveState(state);

        return _commandResult?.Invoke() ?? CommandResult.Dismiss();
    }
}
