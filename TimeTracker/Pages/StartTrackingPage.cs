using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Data;
using TimeTracker.Forms;
using TimeTracker.Helpers;

namespace TimeTracker.Pages;

internal sealed partial class StartTrackingPage : ContentPage
{
    private readonly SettingsManager _settings;
    private readonly StateRepository _stateService;
    private readonly ILogger _logger;

    public StartTrackingPage(
        SettingsManager settings,
        StateRepository stateRepository,
        ILogger logger
    )
    {
        _settings = settings;
        _stateService = stateRepository;
        _logger = logger;

        Icon = new IconInfo("\uF147");
        Title = "My Second Page";
        Name = "Open";
        Id = "start-tracking-page";
    }


    public override IContent[] GetContent()
    {
        // TODO: logging
        // TODO: trovare un modo migliore ? vedere come gestisce l'estensione di github per prendere spunto

        try
        {
            List<IContent> content = new List<IContent>();
            var state = _stateService.LoadState();

            if (state.IsTracking)
            {
                var currenttime = TimeOnly.FromDateTime(DateTime.Now);
                var closeTrackingForm = new CloseTrackingForm(
                    _settings,
                    _stateService,
                    _logger,
                    new CloseTrackingForm.CloseTrackingFormData
                    {
                        title = state.CurrentTrack,
                        duration = (currenttime - state.StartTime).Value.ToString(@"hh\:mm", CultureInfo.InvariantCulture),
                    },
                    () =>
                    {
                        RaiseItemsChanged();
                        return CommandResult.KeepOpen();
                    }
                );
                content.Add(closeTrackingForm);
            }
            else
            {
                var startTrackingForm = new StartTrackingForm(
                    _settings,
                    _stateService,
                    _logger
                );
                content.Add(startTrackingForm);
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
