/// <summary>
/// Calculates the gravitational force acting on the vehicle.
/// </summary>

using AeroSim.Core.Mathematics;
using AeroSim.Core.Models;

namespace AeroSim.Simulation.Forces;

public sealed class GravityForce : IForceModel
{

    // ---------------------------
    // Constants
    // ---------------------------
    private const double Gravity = 9.80665;


    // ---------------------------
    // Force calculation
    // ---------------------------

    public Vector2D CalculateForce(VehicleState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        
        return new Vector2D(0 , -state.MassKg * Gravity);
    }
}