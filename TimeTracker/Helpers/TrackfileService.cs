using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTracker.Helpers;

public class TrackfileService
{
    private readonly string _trackfilePath;
    private readonly ITrackfileInstanceTransformer _transformer;

    public TrackfileService(string trackfilePath, ITrackfileInstanceTransformer transformer)
    {
        _trackfilePath = trackfilePath;
        _transformer = transformer;
    }

    public void AddTrackfileInstance(TrackfileInstance instance)
    {
        ArgumentNullException.ThrowIfNull(instance, nameof(instance));

        using var filestream = File.Open(_trackfilePath, FileMode.Append, FileAccess.Write);   
        filestream.Write(Encoding.UTF8.GetBytes(_transformer.Transform(instance)));
    }
}

public record TrackfileInstance
{
    public required string Title { get; init; }
    public required TimeOnly StartTime { get; init; }
    public TimeOnly? EndTime { get; init; }
    public required string Description { get; init; }
}

public interface ITrackfileInstanceTransformer
{
    String Transform(TrackfileInstance instance);
}

public class TrackfileInstanceTransformerMarkdown: ITrackfileInstanceTransformer
{
    public string Transform(TrackfileInstance instance)
    {
        TimeSpan? duration = instance.EndTime == null
            ? null
            : instance.EndTime - instance.StartTime;

        return $$"""
        {{Environment.NewLine}}
        ## {{instance.Title}} 
        {{instance.StartTime.ToString(CultureInfo.InvariantCulture)}} - {{instance.EndTime?.ToString(CultureInfo.InvariantCulture) ?? "Non terminata"}} {{(duration != null ? $"{duration.Value.Hours}:{duration.Value.Minutes}" : "Non registrata")}}

        {{instance.Description}}
        """;
    }
}
