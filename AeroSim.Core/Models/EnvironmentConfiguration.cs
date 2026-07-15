/// <summary>
/// Defines the environmental parameters used during the simulation.
/// </summary>

namespace AeroSim.Core.Models;

public sealed record EnvironmentConfiguration
{
    public double GravityMetersPerSecondSquared { get; init; } = 9.80665;
    public double WindSpeedMetersPerSecond { get; init; } = 0;
}