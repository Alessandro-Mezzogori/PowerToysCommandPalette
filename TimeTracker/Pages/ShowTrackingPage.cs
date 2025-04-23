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

namespace TimeTracker.Pages;

internal sealed partial class ShowTrackingPage: ContentPage
{
    private readonly SettingsManager _settings;
    private readonly StateRepository _stateService;
    private readonly ILogger _logger;

    public ShowTrackingPage(
        SettingsManager settings,
        StateRepository stateRepository,
        ILogger logger
    )
    {
        _settings = settings;
        _stateService = stateRepository;
        _logger = logger;

        Icon = new IconInfo("\uF147");
        Title = "Current tracking page";
        Name = "Open";
        Id = "current-tracking-page";
    }


    public override IContent[] GetContent()
    {
        // TODO: logging
        // TODO: trovare un modo migliore ? vedere come gestisce l'estensione di github per prendere spunto

        try
        {
            List<IContent> content = new List<IContent>();
            var state = _stateService.LoadState();

            if (!string.IsNullOrEmpty(state.CurrentFile))
            {
                var markdown = File.ReadAllText(state.CurrentFile);
                content.Add(new MarkdownContent(markdown));
            }
            else
            {
                content.Add(new MarkdownContent("No tracking file found"));
            }

            return content.ToArray();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error loading state");
        }

        return [];
    }
}
