/// <summary>
/// Advances the vehicle state by one simulation step using the active force models.
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using AeroSim.Core.Mathematics;
using AeroSim.Core.Models;
using AeroSim.Simulation.Forces;
using AeroSim.Simulation.Systems;

namespace AeroSim.Simulation.Physics;

public sealed class PhysicsEngine
{
    // ---------------------------
    // Dependencies
    // ---------------------------
    private readonly IReadOnlyList<IForceModel> _forceModels;
    private readonly FuelSystem _fuelSystem;


    // ---------------------------
    // Construction
    // ---------------------------
    public PhysicsEngine(IEnumerable<IForceModel> forceModels,FuelSystem fuelSystem)
    {
        _forceModels = forceModels.ToList();

        _fuelSystem = fuelSystem ??
            throw new ArgumentNullException(nameof(fuelSystem));
    }


    // ---------------------------
    // Simulation
    // ---------------------------

    public void Step(VehicleState state,double deltaTimeSeconds)
    {
        if (state is null)
        {
            throw new ArgumentNullException(nameof(state));
        }

        if (deltaTimeSeconds <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(deltaTimeSeconds),
                "Time step must be greater than zero.");
        }

        if (state.MassKg <= 0)
        {
            throw new InvalidOperationException(
                "Vehicle mass must be greater than zero.");
        }

        _fuelSystem.Update(state,deltaTimeSeconds);

        Vector2D totalForce = Vector2D.Zero;

        foreach (IForceModel forceModel in _forceModels)
        {
            totalForce += forceModel.CalculateForce(state);
        }

        state.Acceleration = totalForce / state.MassKg;

        state.Velocity = state.Velocity + state.Acceleration * deltaTimeSeconds;

        state.Position = state.Position + state.Velocity * deltaTimeSeconds;

        state.TimeSeconds += deltaTimeSeconds;
    }
}