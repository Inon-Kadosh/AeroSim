/// <summary>
/// Stores the key performance metrics collected during a simulation run.
/// </summary>

namespace AeroSim.Core.Models;

public sealed record SimulationSummary
{
    public double MaximumAltitudeMeters { get; init; }

    public double MaximumSpeedMetersPerSecond { get; init; }

    public double FlightDurationSeconds { get; init; }

    public double HorizontalRangeMeters { get; init; }
}