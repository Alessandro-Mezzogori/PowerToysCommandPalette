using Microsoft.CommandPalette.Extensions.Toolkit;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using TimeTracker.Data;
using TimeTracker.Helpers;

namespace TimeTracker.Forms;

[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Sample code")]
internal sealed partial class CloseTrackingForm : FormContent
{
    private readonly SettingsManager _settings;
    private readonly StateRepository _stateRepository;
    private readonly ILogger _logger;

    public CloseTrackingForm(
        SettingsManager settings,
        StateRepository stateRepository,
        ILogger logger,
        CloseTrackingFormData data
    )
    {
        _settings = settings;
        _stateRepository = stateRepository;
        _logger = logger;

        TemplateJson = """
        {
            "type": "AdaptiveCard",
            "body": [
                {
                    "type": "TextBlock",
                    "size": "Medium",
                    "weight": "Bolder",
                    "text": "${cardtitle}"
                },
                {
                    "type": "Input.Text",
                    "placeholder": "Placeholder text",
                    "id": "title",
                    "label": "Title",
                    "isRequired": true,
                    "errorMessage": "Required",
                    "value": "${$root.title}"
                },
                {
                    "type": "Input.Text",
                    "placeholder": "Placeholder text",
                    "id": "description",
                    "label": "Description",
                    "isMultiline": true
                },
                {
                    "type": "Input.Time",
                    "id": "duration",
                    "label": "Duration",
                    "min": "0",
                    "isRequired": true,
                    "errorMessage": "Required",
                    "value": "${$root.duration}"
                },
                {
                    "type": "ActionSet"
                }
            ],
            "$schema": "http://adaptivecards.io/schemas/adaptive-card.json",
            "version": "1.6",
            "actions": [
                {
                    "type": "Action.Submit",
                    "title": "Save",
                    "style": "positive"
                },
                {
                    "type": "Action.Execute",
                    "title": "Cancel",
                    "id": "cancel"
                }
            ]
        }
        """;

        DataJson = JsonSerializer.Serialize(data, SourceGenerationContext.Default.CloseTrackingFormData);
    }

    public override CommandResult SubmitForm(string payload) { 
        var response = JsonSerializer.Deserialize(payload, SourceGenerationContext.Default.CloseTrackingFormResponse);

        var filename = FilenameManager.TrackingFilename(_settings.FilenameTemplate!, new TrackingFilenameArgs
        {
            Date = DateTime.Today,
        });

        var trackingfilePath = Path.Combine(_settings.StorageFolder!, filename);
        var taskfileService = new TrackfileService(trackingfilePath, new TrackfileInstanceTransformerMarkdown());
        var state = _stateRepository.LoadState();

        TrackfileInstance instance = new()
        {
            Title = response.title,
            Description = response.description,
            StartTime = state.StartTime,
            EndTime = state.StartTime + TimeSpan.Parse(response.duration)
        }
        //     Description = "test description",

            //     StartTime = state.StartTime ?? throw new InvalidOperationException("Start time is null"),
            //     EndTime = TimeOnly.FromDateTime(DateTime.Now),
            // };

            // state.StartTime = null;
            // state.CurrentTrack = null;

            // _logger.Verbose("Saving state...");
            // taskfileService.AddTrackfileInstance(instance);

            // _stateRepository.SaveState(state);


        return CommandResult.Confirm(new ConfirmationArgs{ 
            Description = payload,
            PrimaryCommand = new NoOpCommand(),
            IsPrimaryCommandCritical = false,
            Title = "test"
        });
    }

    public class CloseTrackingFormData
    {
        public string cardtitle { get; init; } = "Close tracking task";
        public required string title { get; init; } = string.Empty;
        public required string duration { get; init; } = string.Empty;
    }

    public class CloseTrackingFormResponse
    {
        public string title { get; init; }
        public string description { get; init; }
        public string duration { get; init; } 
    }
}
