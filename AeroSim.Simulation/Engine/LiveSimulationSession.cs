/// <summary>
/// Represents a single live simulation session.
/// Responsible for simulation progression, telemetry collection,
/// flight summary updates, and completion tracking.
/// </summary>
 

using AeroSim.Core.Mathematics;
using AeroSim.Core.Models;
using AeroSim.Simulation.Physics;
using AeroSim.Simulation.Environment;

namespace AeroSim.Simulation.Engine;

public sealed class LiveSimulationSession
{

    // ---------------------------
    // Dependencies
    // ---------------------------

    private readonly PhysicsEngine _physicsEngine;
    private readonly SimulationConfiguration _configuration;
    private readonly AtmosphereModel _atmosphereModel;


    // ---------------------------
    // Internal state
    // ---------------------------

    private readonly List<TelemetryPoint> _telemetry = new();


    // ---------------------------
    // Public state
    // ---------------------------

    public VehicleState CurrentState { get; }
    public SimulationSummary Summary { get; private set; }
    public IReadOnlyList<TelemetryPoint> Telemetry => _telemetry;
    public bool IsCompleted { get; private set; }


    // ---------------------------
    // Construction
    // ---------------------------

    public LiveSimulationSession(PhysicsEngine physicsEngine, SimulationConfiguration configuration, AtmosphereModel atmosphereModel)
    {
        _physicsEngine =
            physicsEngine ??
            throw new ArgumentNullException(
                nameof(physicsEngine));

        _configuration =
            configuration ??
            throw new ArgumentNullException(
                nameof(configuration));

        CurrentState = CreateInitialState(configuration);

        _atmosphereModel =
            atmosphereModel ??
            throw new ArgumentNullException(
                nameof(atmosphereModel));

        Summary =
            new SimulationSummary
            {
                MaximumAltitudeMeters = CurrentState.Position.Y,

                MaximumSpeedMetersPerSecond = CurrentState.Velocity.Magnitude,

                FlightDurationSeconds = CurrentState.TimeSeconds,

                HorizontalRangeMeters = CurrentState.Position.X
            };

        _telemetry.Add(CreateTelemetryPoint(CurrentState));
    }


    // ---------------------------
    // Simulation lifecycle
    // ---------------------------

    public void Step()
    {
        if (IsCompleted){ return; }

        _physicsEngine.Step(CurrentState, _configuration.TimeStepSeconds);

        if (CurrentState.Position.Y <= 0 && CurrentState.TimeSeconds > 0)
        {
            CurrentState.Position = new Vector2D(CurrentState.Position.X,0);
            IsCompleted = true;
        }

        UpdateSummary();

        _telemetry.Add(CreateTelemetryPoint(CurrentState));

        if (CurrentState.TimeSeconds >= _configuration.MaximumSimulationTimeSeconds)
        {
            IsCompleted = true;
        }
    }


    // ---------------------------
    // State creation
    // ---------------------------
    private static VehicleState CreateInitialState(SimulationConfiguration configuration)
    {
        return new VehicleState
        {
            TimeSeconds = 0,

            Position = configuration.InitialPosition,

            Velocity = configuration.InitialVelocity,

            Acceleration = Vector2D.Zero,

            MassKg = configuration.InitialMassKg,

            FuelMassKg = configuration.Rocket.InitialFuelMassKg
        };
    }


    // ---------------------------
    // Telemetry
    // ---------------------------
    private TelemetryPoint CreateTelemetryPoint(VehicleState state)
    {
        double accelerationMagnitude = state.Acceleration.Magnitude;

        bool engineIsRunning = state.FuelMassKg > 0 && state.TimeSeconds < _configuration.Rocket.Engine.BurnDurationSeconds;

        return new TelemetryPoint
        {
            TimeSeconds = state.TimeSeconds,

            Position = state.Position,

            Velocity = state.Velocity,

            Acceleration = state.Acceleration,

            SpeedMetersPerSecond = state.Velocity.Magnitude,

            HorizontalSpeedMetersPerSecond = state.Velocity.X,

            VerticalSpeedMetersPerSecond = state.Velocity.Y,

            AccelerationMetersPerSecondSquared = accelerationMagnitude,

            GForce = accelerationMagnitude / 9.80665,

            MassKg = state.MassKg,

            FuelMassKg = state.FuelMassKg,

            AirDensityKgPerCubicMeter = _atmosphereModel.GetAirDensity(state.Position.Y),

            EngineIsRunning = engineIsRunning
        };
    }


    // ---------------------------
    // Summary
    // ---------------------------

    private void UpdateSummary()
    {
        double currentAltitude = CurrentState.Position.Y;

        double currentSpeed = CurrentState.Velocity.Magnitude;

        Summary =
            new SimulationSummary
            {
                MaximumAltitudeMeters = Math.Max(Summary.MaximumAltitudeMeters,currentAltitude),

                MaximumSpeedMetersPerSecond = Math.Max(Summary.MaximumSpeedMetersPerSecond,currentSpeed),

                FlightDurationSeconds = CurrentState.TimeSeconds,

                HorizontalRangeMeters = CurrentState.Position.X
            };
    }

}