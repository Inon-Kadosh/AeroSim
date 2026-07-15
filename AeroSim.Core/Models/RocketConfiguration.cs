/// <summary>
/// Defines the physical and aerodynamic properties of the rocket.
/// </summary>

namespace AeroSim.Core.Models;

public sealed record RocketConfiguration
{

    // ---------------------------
    // Configuration
    // ---------------------------

    public string Name { get; init; } = "Custom Rocket";

    public double DryMassKg { get; init; }

    public double PayloadMassKg { get; init; }

    public double InitialFuelMassKg { get; init; }

    public double DiameterMeters { get; init; }

    public double LengthMeters { get; init; }

    public double DragCoefficient { get; init; }

    public EngineConfiguration Engine { get; init; } = new();



    // ---------------------------
    // Derived properties
    // ---------------------------
    
    public double CrossSectionAreaSquareMeters
    {
        get
        {
            double radius = DiameterMeters / 2;

            return Math.PI * radius * radius;
        }
    }

    public double InitialTotalMassKg =>
        DryMassKg +
        PayloadMassKg +
        InitialFuelMassKg;

    public double FuelConsumptionKgPerSecond
    {
        get
        {
            if (Engine.BurnDurationSeconds <= 0)
            {
                return 0;
            }

            return InitialFuelMassKg /
                   Engine.BurnDurationSeconds;
        }
    }
}