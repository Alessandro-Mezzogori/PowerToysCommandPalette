using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Helpers;
using TimeTracker.Properties;
using TimeTracker.Data;
using Serilog;
using TimeTracker.Forms;

namespace TimeTracker.Commands;

internal sealed partial class StartTrackingCommand : InvokableCommand
{
    // ##### Private fields #####
    private readonly SettingsManager _settings;
    private readonly StateRepository _stateService;
    private readonly ILogger _logger;
    private readonly StartTrackingForm.StartTrackingFormResponse _data;

    // ##### Public properties #####
    public override string Name => "Start tracking";
    public override IconInfo Icon => new("\uE8A7");

    // ##### Impl #####
    public StartTrackingCommand(
        SettingsManager settings,
        StateRepository stateService,
        ILogger logger,
        StartTrackingForm.StartTrackingFormResponse data
    )
    {
        _settings = settings;
        _stateService = stateService;
        _logger = logger;
        _data = data;
    }

    public override ICommandResult Invoke()
    {
        var ensureStorageFolderResult = EnsureStorageFolder();
        if(ensureStorageFolderResult != null)
            return ensureStorageFolderResult;

        var ensureFilenameTemplateResult = EnsureFilenameTemplate();
        if (ensureFilenameTemplateResult != null)
            return ensureFilenameTemplateResult;

        // Todo spostare da qualche parte condivisa
        try
        {
            _logger.Information("Loading state...");
            var state = _stateService.LoadState();

            var filename = FilenameManager.TrackingFilename(_settings.FilenameTemplate!, new TrackingFilenameArgs
            {
                Date = DateTime.Today,
            });

            var trackingfilePath = Path.Combine(_settings.StorageFolder!, filename);

                var taskfileService = new TrackfileService(trackingfilePath, new TrackfileInstanceTransformerMarkdown());

            // TODO input
            _logger.Information("Start tracking");
            state.CurrentFile = filename;
            state.CurrentTrack = _data.title;
            state.StartTime = TimeOnly.FromDateTime(DateTime.Now);

            _stateService.SaveState(state);
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            _logger.Error(ex.StackTrace ?? string.Empty);
        }

        return CommandResult.ShowToast(new ToastArgs
        {
            Message = Resources.msg_tracking_started,
            Result = CommandResult.Dismiss(),
        });
    }

    private CommandResult? EnsureStorageFolder()
    {
        if(_settings.StorageFolder == null)
        {
            return CommandResult.ShowToast(new ToastArgs {
                Message = Resources.conf_missing_folderpath,
                Result = CommandResult.GoHome(),
            });
        }

        // Create the storage folder if it doesn't exist
        Directory.CreateDirectory(_settings.StorageFolder);
        return null;
    }

    private CommandResult? EnsureFilenameTemplate()
    {
        if(_settings.FilenameTemplate == null)
        {
            return CommandResult.ShowToast(new ToastArgs {
                Message = Resources.conf_missing_filename_template,
                Result = CommandResult.GoHome(),
            });
        }

        return null;
    }

}
