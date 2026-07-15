/// <summary>
/// Defines a force model that computes the force applied
/// to the vehicle for the current simulation state.
/// </summary>

using AeroSim.Core.Mathematics;
using AeroSim.Core.Models;

namespace AeroSim.Simulation.Forces;

public interface IForceModel
{

/// <summary>
/// Calculates the force vector applied to the vehicle.
/// </summary>
/// 
    Vector2D CalculateForce(VehicleState state);
}