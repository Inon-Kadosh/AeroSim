/// <summary>
/// Creates and configures live simulation sessions with their required physics components.
/// </summary>

using System;
using AeroSim.Core.Models;
using AeroSim.Simulation.Engine;
using AeroSim.Simulation.Forces;
using AeroSim.Simulation.Physics;
using AeroSim.Simulation.Systems;
using AeroSim.Simulation.Environment;

namespace AeroSim.App.Services;

public sealed class SimulationService
{

    // ---------------------------
    // Session creation
    // ---------------------------

    public LiveSimulationSession CreateLiveSession(SimulationConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        RocketConfiguration rocket = configuration.Rocket;

        AtmosphereModel atmosphereModel = new();

        FuelSystem fuelSystem = new(rocket);

        PhysicsEngine physicsEngine =
            new(
                new IForceModel[]
                {
                    new GravityForce(),
                    new DragForce(atmosphereModel , rocket:rocket),
                    new ThrustForce(rocket:rocket)
                },
                fuelSystem);

        

        return new LiveSimulationSession(physicsEngine , configuration , atmosphereModel);
    }

}