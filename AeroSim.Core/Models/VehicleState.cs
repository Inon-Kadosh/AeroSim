/// <summary>
/// Represents the mutable state of the vehicle during the simulation.
/// </summary>

using AeroSim.Core.Mathematics;

namespace AeroSim.Core.Models;

public sealed class VehicleState
{
    public double TimeSeconds { get; set; }

    public Vector2D Position { get; set; }

    public Vector2D Velocity { get; set; }

    public Vector2D Acceleration { get; set; }

    public double MassKg { get; set; }

    public double FuelMassKg { get; set; }
}