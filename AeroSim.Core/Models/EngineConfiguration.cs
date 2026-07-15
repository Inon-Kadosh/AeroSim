/// <summary>
/// Defines the rocket engine characteristics used during the simulation.
/// </summary>

namespace AeroSim.Core.Models;

public sealed record EngineConfiguration
{
    public double ThrustNewtons { get; init; }

    public double BurnDurationSeconds { get; init; }

    public double ThrustAngleDegrees { get; init; }
}