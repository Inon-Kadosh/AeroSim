/// <summary>
/// Detects mission events from consecutive live telemetry points
/// and ensures that each event is reported only once.
/// </summary>

using AeroSim.Core.Models;

namespace AeroSim.Simulation.Analysis;

public sealed class LiveMissionEventTracker
{

    // ---------------------------
    // Dependencies
    // ---------------------------

    private TelemetryPoint? _previousPoint;
    private bool _engineIgnitionDetected;
    private bool _engineShutdownDetected;
    private bool _apogeeDetected;
    private bool _impactDetected;


    // ---------------------------
    // Event detection
    // ---------------------------

    public IReadOnlyList<MissionEvent> Process(TelemetryPoint current)
    {
        // Detect engine ignition
        ArgumentNullException.ThrowIfNull(current);

        List<MissionEvent> newEvents = new();

        if (!_engineIgnitionDetected && current.EngineIsRunning)
        {
            _engineIgnitionDetected = true;

            newEvents.Add(
                new MissionEvent
                {
                    TimeSeconds = current.TimeSeconds,
                    EventType = "ENGINE_IGNITION",
                    Description = "Engine ignition detected."
                });
        }

        // Detect events that require the previous telemetry point
        if (_previousPoint is not null)
        {
            if (!_engineShutdownDetected && _previousPoint.EngineIsRunning && !current.EngineIsRunning)
            {
                _engineShutdownDetected = true;

                newEvents.Add(
                    new MissionEvent
                    {
                        TimeSeconds = current.TimeSeconds,
                        EventType = "ENGINE_SHUTDOWN",
                        Description = "Engine shutdown detected."
                    });
            }

            if (!_apogeeDetected && _previousPoint.VerticalSpeedMetersPerSecond > 0 && current.VerticalSpeedMetersPerSecond <= 0)
            {
                _apogeeDetected = true;

                newEvents.Add(
                    new MissionEvent
                    {
                        TimeSeconds = current.TimeSeconds,
                        EventType = "APOGEE",
                        Description = $"Maximum altitude reached at " + $"{current.Position.Y:F2} m."
                    });
            }
        }

        if (!_impactDetected && current.TimeSeconds > 0 && current.Position.Y <= 0)
        {
            _impactDetected = true;

            newEvents.Add(
                new MissionEvent
                {
                    TimeSeconds = current.TimeSeconds,
                    EventType = "IMPACT",
                    Description = $"Ground impact at range " + $"{current.Position.X:F2} m, " + $"speed {current.SpeedMetersPerSecond:F2} m/s."
                });
        }
        
        // Store the current telemetry point for the next update
        _previousPoint = current;

        return newEvents;
    }
}