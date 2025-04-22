using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTracker.Helpers;

public static class FilenameManager
{
    public static string TrackingFilename(string template, TrackingFilenameArgs args)
    {
        return String.Format(
            CultureInfo.InvariantCulture,
            template,
            args.Date
        );
    }
}

public record struct TrackingFilenameArgs
{
    public DateTime Date { get; init; }
}
