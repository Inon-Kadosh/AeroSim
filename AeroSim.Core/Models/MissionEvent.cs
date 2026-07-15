/// <summary>
/// Represents a significant event that occurred during the mission timeline.
/// </summary>

namespace AeroSim.Core.Models;

public sealed record MissionEvent
{
    public double TimeSeconds { get; init; }

    public string EventType { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;
}