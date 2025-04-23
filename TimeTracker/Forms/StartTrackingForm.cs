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

public sealed partial class StartTrackingForm : FormContent
{
    private readonly SettingsManager _settings;
    private readonly StateRepository _stateRepository;
    private readonly ILogger _logger;

    public StartTrackingForm(
        SettingsManager settings,
        StateRepository stateRepository,
        ILogger logger
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
                    "text": "Start task"
                },
                {
                    "type": "Input.Text",
                    "placeholder": "Placeholder text",
                    "id": "title",
                    "label": "Title",
                    "isRequired": true,
                    "errorMessage": "Required"
                }
            ],
            "$schema": "http://adaptivecards.io/schemas/adaptive-card.json",
            "version": "1.6",
            "actions": [
                {
                    "type": "Action.Submit",
                    "title": "Ok",
                    "style": "positive",
                    "verb": "submit",
                    "data": {
                        "action": "submit"
                    }
                },
                {
                    "type": "Action.Submit",
                    "title": "Cancel",
                    "id": "cancel",
                    "verb": "cancel",
                    "data": {
                        "action": "cancel"
                    },
                    "associatedInputs": "none"
                }
            ]
        }
        """;

        DataJson = "{}";
    }

    public override ICommandResult SubmitForm(string inputs, string data)
    {
        _logger.Information("SubmitForm: {inputs} {data}", inputs, data);
        var response = JsonSerializer.Deserialize(inputs, SourceGenerationContext.Default.StartTrackingFormResponse);
        var formdata = JsonSerializer.Deserialize(data, SourceGenerationContext.Default.StartTrackingFormData);

        _logger.Information("SubmitForm: {data}", formdata?.action);
        if(formdata?.action == "cancel")
            return CommandResult.Dismiss();

        var startTrackingCommand = new StartTrackingCommand(
            _settings,
            _stateRepository,
            _logger,
            response
        );
        startTrackingCommand.Invoke();

        return CommandResult.Dismiss();
    }

    public class StartTrackingFormResponse
    {
        public string title { get; init; }
    }

    public class StartTrackingFormData
    {
        public string? action { get; init; }
    }
}
