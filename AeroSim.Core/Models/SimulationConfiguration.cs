/// <summary>
/// Defines the complete configuration for a single simulation run.
/// </summary>

using AeroSim.Core.Mathematics;

namespace AeroSim.Core.Models;

public sealed record SimulationConfiguration
{

    // ---------------------------
    // Configuration
    // ---------------------------
    
    public RocketConfiguration Rocket { get; init; } = new();

    public EnvironmentConfiguration Environment { get; init; } = new();

    public double InitialXPositionMeters { get; init; }

    public double InitialAltitudeMeters { get; init; }

    public double InitialSpeedMetersPerSecond { get; init; }

    public double LaunchAngleDegrees { get; init; }

    public double TimeStepSeconds { get; init; } = 0.01;

    public double MaximumSimulationTimeSeconds { get; init; } = 120;


    // ---------------------------
    // Derived properties
    // ---------------------------

    public Vector2D InitialPosition =>
        new(
            InitialXPositionMeters,
            InitialAltitudeMeters);

    public Vector2D InitialVelocity
    {
        get
        {
            double radians =
                LaunchAngleDegrees *
                Math.PI /
                180.0;

            return new Vector2D(
                InitialSpeedMetersPerSecond *
                Math.Cos(radians),

                InitialSpeedMetersPerSecond *
                Math.Sin(radians));
        }
    }

    public double InitialMassKg =>
        Rocket.InitialTotalMassKg;
}