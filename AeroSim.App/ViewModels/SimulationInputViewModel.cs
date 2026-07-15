namespace AeroSim.App.ViewModels;

public sealed class SimulationInputViewModel : ViewModelBase
{
    // Launch
    private double _initialXPositionMeters = 0;
    private double _initialAltitudeMeters = 100;
    private double _initialSpeedMetersPerSecond = 36.06;
    private double _launchAngleDegrees = 56.31;

    // Rocket
    private double _dryMassKg = 70;
    private double _payloadMassKg = 0;
    private double _fuelMassKg = 30;
    private double _diameterMeters = 0.252;
    private double _lengthMeters = 3;
    private double _dragCoefficient = 0.5;

    // Engine
    private double _thrustNewtons = 1500;
    private double _burnDurationSeconds = 3;
    private double _thrustAngleDegrees = 53.13;

    // Environment
    private double _gravityMetersPerSecondSquared = 9.80665;
    private double _windSpeedMetersPerSecond = 0;

    // Simulation
    private double _timeStepSeconds = 0.01;
    private double _maximumSimulationTimeSeconds = 120;

    public double InitialXPositionMeters
    {
        get => _initialXPositionMeters;
        set => SetProperty(ref _initialXPositionMeters, value);
    }

    public double InitialAltitudeMeters
    {
        get => _initialAltitudeMeters;
        set => SetProperty(ref _initialAltitudeMeters, value);
    }

    public double InitialSpeedMetersPerSecond
    {
        get => _initialSpeedMetersPerSecond;
        set => SetProperty(ref _initialSpeedMetersPerSecond, value);
    }

    public double LaunchAngleDegrees
    {
        get => _launchAngleDegrees;
        set => SetProperty(ref _launchAngleDegrees, value);
    }

    public double DryMassKg
    {
        get => _dryMassKg;
        set => SetProperty(ref _dryMassKg, value);
    }

    public double PayloadMassKg
    {
        get => _payloadMassKg;
        set => SetProperty(ref _payloadMassKg, value);
    }

    public double FuelMassKg
    {
        get => _fuelMassKg;
        set => SetProperty(ref _fuelMassKg, value);
    }

    public double DiameterMeters
    {
        get => _diameterMeters;
        set => SetProperty(ref _diameterMeters, value);
    }

    public double LengthMeters
    {
        get => _lengthMeters;
        set => SetProperty(ref _lengthMeters, value);
    }

    public double DragCoefficient
    {
        get => _dragCoefficient;
        set => SetProperty(ref _dragCoefficient, value);
    }

    public double ThrustNewtons
    {
        get => _thrustNewtons;
        set => SetProperty(ref _thrustNewtons, value);
    }

    public double BurnDurationSeconds
    {
        get => _burnDurationSeconds;
        set => SetProperty(ref _burnDurationSeconds, value);
    }

    public double ThrustAngleDegrees
    {
        get => _thrustAngleDegrees;
        set => SetProperty(ref _thrustAngleDegrees, value);
    }

    public double GravityMetersPerSecondSquared
    {
        get => _gravityMetersPerSecondSquared;
        set => SetProperty(ref _gravityMetersPerSecondSquared, value);
    }

    public double WindSpeedMetersPerSecond
    {
        get => _windSpeedMetersPerSecond;
        set => SetProperty(ref _windSpeedMetersPerSecond, value);
    }

    public double TimeStepSeconds
    {
        get => _timeStepSeconds;
        set => SetProperty(ref _timeStepSeconds, value);
    }

    public double MaximumSimulationTimeSeconds
    {
        get => _maximumSimulationTimeSeconds;
        set => SetProperty(ref _maximumSimulationTimeSeconds, value);
    }
}