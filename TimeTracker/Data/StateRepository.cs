using System;
using System.IO;
using System.Text.Json;
using TimeTracker.Helpers;

namespace TimeTracker.Data;

public class StateRepository
{
    private readonly string _stateFilePath;

    public StateRepository(string stateFilePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(stateFilePath, nameof(stateFilePath));

        _stateFilePath = stateFilePath;
    }

    public State LoadState()
    {
        if (!File.Exists(_stateFilePath))
        {
            return new State { };
        }

        var json = File.ReadAllText(_stateFilePath);
        return JsonSerializer.Deserialize(json, SourceGenerationContext.Default.State)!; // TODO: gestire il nullable
    }

    public void SaveState(State state)
    {
        ArgumentNullException.ThrowIfNull(state, nameof(state));

        var json = JsonSerializer.Serialize(state, SourceGenerationContext.Default.State);
        File.WriteAllText(_stateFilePath, json);
    }
}

public class State
{
    public string? CurrentFile { get; set; }
    public string? CurrentTrack { get; set; }
    public TimeOnly? StartTime { get; set; }

    public StateType Type { 
        get
        {
            if(string.IsNullOrWhiteSpace(CurrentFile))
                return StateType.None;

            if(string.IsNullOrWhiteSpace(CurrentTrack))
                return StateType.Stopped;

            if(StartTime == null)
                return StateType.Paused;

            return StateType.Tracking;
        } 
    }

    public enum StateType
    {
        None,
        Tracking,
        Paused,
        Stopped
    }
}

