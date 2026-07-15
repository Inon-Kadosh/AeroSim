/// <summary>
/// Represents a snapshot of the vehicle state at a specific point in time.
/// </summary>

using AeroSim.Core.Mathematics;

namespace AeroSim.Core.Models;

public sealed record TelemetryPoint
{
    public double TimeSeconds { get; init; }

    public Vector2D Position { get; init; }

    public Vector2D Velocity { get; init; }

    public Vector2D Acceleration { get; init; }

    public double AccelerationMetersPerSecondSquared { get; init; }

    public double GForce { get; init; }

    public double SpeedMetersPerSecond { get; init; }

    public double HorizontalSpeedMetersPerSecond { get; init; }

    public double VerticalSpeedMetersPerSecond { get; init; }

    public double MassKg { get; init; }

    public double FuelMassKg { get; init; } 

    public bool EngineIsRunning { get; init; }

    public double AirDensityKgPerCubicMeter { get; init; }

}