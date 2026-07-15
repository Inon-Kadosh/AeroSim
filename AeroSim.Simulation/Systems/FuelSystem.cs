/// <summary>
/// Updates fuel consumption and reduces the vehicle mass during engine burn.
/// </summary>

using System;
using AeroSim.Core.Models;

namespace AeroSim.Simulation.Systems;

public sealed class FuelSystem
{

    // ---------------------------
    // Dependencies
    // ---------------------------
    private readonly RocketConfiguration _rocket;


    // ---------------------------
    // Construction
    // ---------------------------

    public FuelSystem(RocketConfiguration rocket)
    {
        _rocket = rocket ??
            throw new ArgumentNullException(nameof(rocket));
    }


    // ---------------------------
    // Fuel update
    // ---------------------------

    public void Update(VehicleState state,double deltaTimeSeconds)
    {

        ArgumentNullException.ThrowIfNull(state);
        if (deltaTimeSeconds <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(deltaTimeSeconds),"Time step must be greater than zero.");
        }

        if (state.FuelMassKg <= 0)
        {
            state.FuelMassKg = 0;
            return;
        }

        bool engineIsBurning = state.TimeSeconds < _rocket.Engine.BurnDurationSeconds;

        if (!engineIsBurning)
        {
            return;
        }

        double consumedFuel = _rocket.FuelConsumptionKgPerSecond * deltaTimeSeconds;

        consumedFuel = Math.Min(consumedFuel , state.FuelMassKg);

        state.FuelMassKg -= consumedFuel;
        state.MassKg -= consumedFuel;
    }
}