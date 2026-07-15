/// <summary>
/// Calculates aerodynamic drag opposite to the vehicle's direction of motion.
/// </summary>

using System;
using AeroSim.Core.Mathematics;
using AeroSim.Core.Models;
using AeroSim.Simulation.Environment;

namespace AeroSim.Simulation.Forces;

public sealed class DragForce : IForceModel
{

    // ---------------------------
    // Dependencies
    // ---------------------------

    private readonly AtmosphereModel _atmosphereModel;
    private readonly RocketConfiguration _rocket;


    // ---------------------------
    // Construction
    // ---------------------------

    public DragForce( AtmosphereModel atmosphereModel,RocketConfiguration rocket)
    {
        _atmosphereModel =
            atmosphereModel ??
            throw new ArgumentNullException(
                nameof(atmosphereModel));

        _rocket =
            rocket ??
            throw new ArgumentNullException(
                nameof(rocket));
    }


    // ---------------------------
    // Force calculation
    // ---------------------------

    public Vector2D CalculateForce(VehicleState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        double speed = state.Velocity.Magnitude;

        if (speed == 0)
        {
            return Vector2D.Zero;
        }

        double airDensity = _atmosphereModel.GetAirDensity(state.Position.Y);

        double dragMagnitude =
            0.5 *
            airDensity *
            _rocket.DragCoefficient *
            _rocket.CrossSectionAreaSquareMeters *
            speed *
            speed;

        Vector2D oppositeDirection = state.Velocity.Normalize() * -1;

        return oppositeDirection * dragMagnitude;
    }
}