using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using TimeTracker.Commands;
using TimeTracker.Data;
using TimeTracker.Helpers;

namespace TimeTracker.Forms;

[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Sample code")]
public sealed partial class CloseTrackingForm : FormContent
{
    private readonly SettingsManager _settings;
    private readonly StateRepository _stateRepository;
    private readonly ILogger _logger;
    private readonly Func<ICommandResult>? _endAction;

    public CloseTrackingForm(
        SettingsManager settings,
        StateRepository stateRepository,
        ILogger logger,
        CloseTrackingFormData data,
        Func<ICommandResult>? endAction = null
    )
    {
        _settings = settings;
        _stateRepository = stateRepository;
        _logger = logger;
        _endAction = endAction;

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

        return CommandResult.Confirm(new ConfirmationArgs{ 
            Description = payload,
            PrimaryCommand = new StopTrackingCommand(
                _settings,
                _stateRepository,
                _logger,
                response,
                commandResult: _endAction
            ),
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
