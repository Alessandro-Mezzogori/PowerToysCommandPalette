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

namespace TimeTracker.Commands;

internal sealed partial class StopTrackingCommand : InvokableCommand
{
    // ##### Private fields #####
    private readonly SettingsManager _settings;
    private readonly StateRepository _stateService;

    // ##### Public properties #####
    public override string Name => "Start tracking";
    public override IconInfo Icon => new("\uE8A7");

    // ##### Impl #####
    public StopTrackingCommand(SettingsManager settings, StateRepository stateService)
    {
        _settings = settings;
        _stateService = stateService;
    }

    public override ICommandResult Invoke()
    {
        StringBuilder sb = new(); 

        var ensureStorageFolderResult = EnsureStorageFolder();
        if(ensureStorageFolderResult != null)
            return ensureStorageFolderResult;

        var ensureFilenameTemplateResult = EnsureFilenameTemplate();
        if (ensureFilenameTemplateResult != null)
            return ensureFilenameTemplateResult;

        // Todo spostare da qualche parte condivisa
        try
        {

            sb.AppendLine("Loading state...");
            var state = _stateService.LoadState();

        // if tracking => stop tracking
        // if paused => stop tracking
        // if stopped => start tracking
        // if none => start tracking

        var filename = FilenameManager.TrackingFilename(_settings.FilenameTemplate!, new TrackingFilenameArgs
        {
            Date = DateTime.Today,
        });

        var trackingfilePath = Path.Combine(_settings.StorageFolder!, filename);

            var taskfileService = new TrackfileService(trackingfilePath, new TrackfileInstanceTransformerMarkdown());

        if (state.Type == State.StateType.Tracking || state.Type == State.StateType.Paused)
        {
            sb.AppendLine("was tracking...");
            TrackfileInstance instance = new()
            {
                // TODO input
                Title = state.CurrentTrack ?? string.Empty,

                // TODO input
                Description = "test description",

                StartTime = state.StartTime ?? throw new InvalidOperationException("Start time is null"),
                EndTime = TimeOnly.FromDateTime(DateTime.Now),
            };

            state.StartTime = null;
            state.CurrentTrack = null;

            sb.AppendLine("Saving state...");
            taskfileService.AddTrackfileInstance(instance);
        }

        // TODO input
        sb.AppendLine("Start tracking...");
        state.CurrentFile = filename;
        state.CurrentTrack = "test new track";
        state.StartTime = TimeOnly.FromDateTime(DateTime.Now);

        sb.AppendLine("Save state...");
        _stateService.SaveState(state);
        }
        catch(Exception ex)
        {
            sb.AppendLine(ex.Message);
            sb.AppendLine(ex.StackTrace);
        }
        finally
        {
            File.WriteAllText(_settings.StorageFolder + "\\error.log", sb.ToString());
        }

        return CommandResult.ShowToast(new ToastArgs
        {
            Message = Resources.msg_tracking_started,
            Result = CommandResult.Hide(),
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
