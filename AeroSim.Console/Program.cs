using AeroSim.Core.Models;
using AeroSim.Simulation.Engine;
using AeroSim.Simulation.Physics;
using AeroSim.Infrastructure.Export;
using AeroSim.Simulation.Forces;
using AeroSim.Simulation.Systems;
using AeroSim.Simulation.Environment;


// ---------------------------
// Rocket configuration
// ---------------------------

RocketConfiguration rocket =
    new()
    {
        Name = "AeroSim Test Rocket",
        DryMassKg = 70,
        PayloadMassKg = 0,
        InitialFuelMassKg = 30,
        DiameterMeters = 0.252,
        LengthMeters = 3,
        DragCoefficient = 0.5,
        Engine = new EngineConfiguration
        {
            ThrustNewtons = 1500,
            BurnDurationSeconds = 3,
            ThrustAngleDegrees = 53.13
        }
    };


// ---------------------------
// Simulation configuration
// ---------------------------

SimulationConfiguration configuration =
    new()
    {
        Rocket = rocket,
        Environment = new EnvironmentConfiguration
        {
            GravityMetersPerSecondSquared = 9.80665,
            WindSpeedMetersPerSecond = 0
        },
        InitialXPositionMeters = 0,
        InitialAltitudeMeters = 100,
        InitialSpeedMetersPerSecond = 36.06,
        LaunchAngleDegrees = 56.31,
        TimeStepSeconds = 0.01,
        MaximumSimulationTimeSeconds = 30
    };


// ---------------------------
// Physics engine
// ---------------------------

AtmosphereModel liveAtmosphereModel = new();
FuelSystem liveFuelSystem = new(rocket);
PhysicsEngine livePhysicsEngine =
    new(
        new IForceModel[]
        {
            new GravityForce(),

            new DragForce(
                atmosphereModel: liveAtmosphereModel,
                rocket: rocket),

            new ThrustForce(
                rocket: rocket)
        },
        liveFuelSystem);



// ---------------------------
// Live simulation
// ---------------------------

LiveSimulationSession liveSession = new(livePhysicsEngine , configuration , liveAtmosphereModel);

while (!liveSession.IsCompleted)
{
    liveSession.Step();
}

VehicleState liveFinalState = liveSession.CurrentState;


// ---------------------------
// Simulation results
// ---------------------------

Console.WriteLine();

Console.WriteLine("Live simulation result");

Console.WriteLine($"Time: {liveFinalState.TimeSeconds:F2} seconds");

Console.WriteLine($"X position: {liveFinalState.Position.X:F2} meters");

Console.WriteLine($"Y position: {liveFinalState.Position.Y:F2} meters");

Console.WriteLine($"X velocity: {liveFinalState.Velocity.X:F2} m/s");

Console.WriteLine($"Y velocity: {liveFinalState.Velocity.Y:F2} m/s");

Console.WriteLine($"Speed: {liveFinalState.Velocity.Magnitude:F2} m/s");

Console.WriteLine($"Final mass: {liveFinalState.MassKg:F2} kg");

Console.WriteLine($"Remaining fuel: {liveFinalState.FuelMassKg:F2} kg");


// ---------------------------
// Telemetry export
// ---------------------------

CsvExporter exporter = new();
exporter.ExportTelemetry(liveSession.Telemetry , "telemetry.csv");

