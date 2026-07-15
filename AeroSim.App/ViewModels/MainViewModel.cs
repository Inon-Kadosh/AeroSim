using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AeroSim.App.Services;
using AeroSim.Core.Mathematics;
using AeroSim.Core.Models;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AeroSim.App.Views;
using AeroSim.Simulation.Engine;
using AeroSim.Simulation.Analysis;


namespace AeroSim.App.ViewModels;

public partial class MainViewModel : ViewModelBase
{

    // ---------------------------
    // Input
    // ---------------------------
    public SimulationInputViewModel Input { get; } = new();


    // ---------------------------
    // Dependencies
    // ---------------------------

    private readonly SimulationService _simulationService;
    private readonly DispatcherTimer _simulationTimer;
   


    // ---------------------------
    // Simulation state
    // ---------------------------

    private LiveSimulationSession? _liveSession;
    private LiveMissionEventTracker _missionEventTracker = new();


    // ---------------------------
    // Rendering state
    // ---------------------------

    private readonly List<Vector2D> _trailPoints = new();
    private Vector2D _rocketPosition = Vector2D.Zero;
    private Vector2D _rocketVelocity = Vector2D.Zero;
    private IReadOnlyList<Vector2D> _trail = Array.Empty<Vector2D>();


    // ---------------------------
    // Live telemetry state
    // ---------------------------

    private double _horizontalSpeed;
    private double _verticalSpeed;
    private double _currentAcceleration;
    private double _currentGForce;
    private double _currentAirDensity;
    private double _currentMassKg;
    private double _currentFuelMassKg;
    private string _engineStatus = "OFF";
    private bool _engineIsRunning;


    // ---------------------------
    // Flight summary state
    // ---------------------------

    private double _maximumAltitude;
    private double _maximumSpeed;
    private double _flightDuration;
    private double _horizontalRange;


    // ---------------------------
    // Camera state
    // ---------------------------

    private bool _followRocket = true;


    // ---------------------------
    // Construction
    // ---------------------------

    public MainViewModel()
    {
        _simulationService = new SimulationService();

        _liveSession = _simulationService.CreateLiveSession( BuildConfiguration() );

        _missionEventTracker = new LiveMissionEventTracker();

        InitializeMissionEvents();

        _simulationTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(16)
        };

        _simulationTimer.Tick += OnSimulationTick;

        UpdateDisplayedStateFromLiveSession();
    }


    // ---------------------------
    // Live telemetry properties
    // ---------------------------


    [ObservableProperty]
    public partial double CurrentTimeSeconds { get; set; }

    [ObservableProperty]
    public partial double CurrentX { get; set; }

    [ObservableProperty]
    public partial double CurrentY { get; set; }

    [ObservableProperty]
    public partial double CurrentSpeed { get; set; }

    [ObservableProperty]
    public partial bool IsPlaying { get; set; }


    public double HorizontalSpeed
    {
        get => _horizontalSpeed;
        private set => SetProperty(
            ref _horizontalSpeed,
            value);
    }
    public double VerticalSpeed
    {
        get => _verticalSpeed;
        private set => SetProperty(
            ref _verticalSpeed,
            value);
    }

    public double CurrentAcceleration
    {
        get => _currentAcceleration;
        private set => SetProperty(
            ref _currentAcceleration,
            value);
    }

    public double CurrentGForce
    {
        get => _currentGForce;
        private set => SetProperty(
            ref _currentGForce,
            value);
    }

    public double CurrentAirDensity
    {
        get => _currentAirDensity;
        private set => SetProperty(
            ref _currentAirDensity,
            value);
    }

    public double CurrentMassKg
    {
        get => _currentMassKg;
        private set => SetProperty(
            ref _currentMassKg,
            value);
    }

    public double CurrentFuelMassKg
    {
        get => _currentFuelMassKg;
        private set => SetProperty(
            ref _currentFuelMassKg,
            value);
    }

    public string EngineStatus
    {
        get => _engineStatus;
        private set => SetProperty(
            ref _engineStatus,
            value);
    }

    public bool EngineIsRunning
    {
        get => _engineIsRunning;
        private set => SetProperty(
            ref _engineIsRunning,
            value);
    }



    // ---------------------------
    // Rendering properties
    // ---------------------------

    public Vector2D RocketPosition
    {
        get => _rocketPosition;
        private set => SetProperty(
            ref _rocketPosition,
            value);
    }

    public Vector2D RocketVelocity
    {
        get => _rocketVelocity;
        private set => SetProperty(
            ref _rocketVelocity,
            value);
    }

    public IReadOnlyList<Vector2D> Trail
    {
        get => _trail;
        private set => SetProperty(
            ref _trail,
            value);
    }



    // ---------------------------
    // Flight summary properties
    // ---------------------------

    public double MaximumAltitude
    {
        get => _maximumAltitude;
        private set => SetProperty(
            ref _maximumAltitude,
            value);
    }

    public double MaximumSpeed
    {
        get => _maximumSpeed;
        private set => SetProperty(
            ref _maximumSpeed,
            value);
    }

    public double FlightDuration
    {
        get => _flightDuration;
        private set => SetProperty(
            ref _flightDuration,
            value);
    }

    public double HorizontalRange
    {
        get => _horizontalRange;
        private set => SetProperty(
            ref _horizontalRange,
            value);
    }



    // ---------------------------
    // Camera properties
    // ---------------------------

    public bool FollowRocket
    {
        get => _followRocket;
        set => SetProperty(
            ref _followRocket,
            value);
    }



    // ---------------------------
    // Mission events
    // ---------------------------

    public ObservableCollection<MissionEvent> MissionEvents { get; } = new();

    private void InitializeMissionEvents()
    {
        MissionEvents.Clear();

        MissionEvents.Add(
            new MissionEvent
            {
                TimeSeconds = 0,
                EventType = "LAUNCH",
                Description = "Simulation started."
            });
    }



    // ---------------------------
    // Commands
    // ---------------------------

    [RelayCommand]
    private void RunSimulation()
    {
        _simulationTimer.Stop();
        IsPlaying = false;

        SimulationConfiguration configuration =
            BuildConfiguration();

        _liveSession =
            _simulationService.CreateLiveSession(
                configuration);

        _missionEventTracker = new LiveMissionEventTracker();
            
        InitializeMissionEvents();

        ClearTrail();
        FollowRocket = true;

        UpdateDisplayedStateFromLiveSession();
    }

    [RelayCommand]
    private void Play()
    {

        if (_liveSession is null || _liveSession.IsCompleted)
        {
            return;
        }
    
        IsPlaying = true;
        _simulationTimer.Start();
    }

    [RelayCommand]
    private void Pause()
    {
        _simulationTimer.Stop();
        IsPlaying = false;
    }

    [RelayCommand]
    private void Reset()
    {
        _simulationTimer.Stop();
        IsPlaying = false;

        _liveSession =
            _simulationService.CreateLiveSession(
                BuildConfiguration());

        _missionEventTracker = new LiveMissionEventTracker();

        InitializeMissionEvents();

        ClearTrail();
        FollowRocket = true;

        UpdateDisplayedStateFromLiveSession();
    }

    [RelayCommand]
    private void EnableFollow()
    {
        FollowRocket = true;
    }

    [RelayCommand]
    private void OpenMissionLog()
    {
        MissionLogWindow window =
            new(MissionEvents);

        window.Show();
    }



    // ---------------------------
    // Simulation loop
    // ---------------------------

    private void OnSimulationTick( object? sender , EventArgs e)
    {
        AdvanceLiveSimulation();
    }

    private void AdvanceLiveSimulation()
    {
        if (_liveSession is null)
        {
            return;
        }

        if (_liveSession.IsCompleted)
        {
            _simulationTimer.Stop();
            IsPlaying = false;
            return;
        }

        for (int i = 0; i < 2 && !_liveSession.IsCompleted ; i++)
        {
            _liveSession.Step();
            TelemetryPoint currentPoint = _liveSession.Telemetry[^1];

            IReadOnlyList<MissionEvent> newEvents = _missionEventTracker.Process(currentPoint);

            foreach (MissionEvent missionEvent in newEvents)
            {
                MissionEvents.Add(missionEvent);
            }
        }

        UpdateDisplayedStateFromLiveSession();
    }



    // ---------------------------
    // Display updates
    // ---------------------------

    private void UpdateDisplayedStateFromLiveSession()
    {
        if (_liveSession is null)
        {
            return;
        }

        TelemetryPoint currentPoint = _liveSession.Telemetry[^1];

        VehicleState current = _liveSession.CurrentState;

        CurrentTimeSeconds = current.TimeSeconds;

        CurrentX = current.Position.X;

        CurrentY = current.Position.Y;

        CurrentSpeed = current.Velocity.Magnitude;

        HorizontalSpeed = current.Velocity.X;

        VerticalSpeed = current.Velocity.Y;

        CurrentAcceleration = current.Acceleration.Magnitude;

        CurrentGForce = current.Acceleration.Magnitude / 9.80665;

        CurrentMassKg = current.MassKg;

        CurrentFuelMassKg = current.FuelMassKg;

        EngineIsRunning = current.FuelMassKg > 0 && current.TimeSeconds < Input.BurnDurationSeconds;

        EngineStatus =
            EngineIsRunning
                ? "RUNNING"
                : "OFF";

        RocketPosition = current.Position;

        RocketVelocity = current.Velocity;

        MaximumAltitude = _liveSession.Summary.MaximumAltitudeMeters;

        MaximumSpeed = _liveSession.Summary.MaximumSpeedMetersPerSecond;

        FlightDuration = _liveSession.Summary.FlightDurationSeconds;

        HorizontalRange = _liveSession.Summary.HorizontalRangeMeters;

        CurrentAirDensity = currentPoint.AirDensityKgPerCubicMeter;

        AddTrailPoint(current.Position);

    }



    // ---------------------------
    // Configuration
    // ---------------------------

    private SimulationConfiguration BuildConfiguration()
    {
        RocketConfiguration rocket =
            new()
            {
                Name = "Custom Rocket",

                DryMassKg = Input.DryMassKg,
                PayloadMassKg = Input.PayloadMassKg,
                InitialFuelMassKg = Input.FuelMassKg,

                DiameterMeters = Input.DiameterMeters,
                LengthMeters = Input.LengthMeters,
                DragCoefficient = Input.DragCoefficient,

                Engine = new EngineConfiguration
                {
                    ThrustNewtons = Input.ThrustNewtons,
                    BurnDurationSeconds = Input.BurnDurationSeconds,
                    ThrustAngleDegrees = Input.ThrustAngleDegrees
                }
            };

        EnvironmentConfiguration environment =
            new()
            {
                GravityMetersPerSecondSquared = Input.GravityMetersPerSecondSquared,

                WindSpeedMetersPerSecond = Input.WindSpeedMetersPerSecond
            };

        return new SimulationConfiguration
        {
            Rocket = rocket,

            Environment = environment,

            InitialXPositionMeters = Input.InitialXPositionMeters,

            InitialAltitudeMeters = Input.InitialAltitudeMeters,

            InitialSpeedMetersPerSecond = Input.InitialSpeedMetersPerSecond,

            LaunchAngleDegrees = Input.LaunchAngleDegrees,

            TimeStepSeconds = Input.TimeStepSeconds,

            MaximumSimulationTimeSeconds = Input.MaximumSimulationTimeSeconds
        };
    }


    // ---------------------------
    // Trail management
    // ---------------------------

    private void AddTrailPoint(
        Vector2D position)
    {
        bool duplicate = _trailPoints.Count > 0 && _trailPoints[^1] == position;
        if (duplicate)
        {
            return;
        }
        _trailPoints.Add(position);
        Trail = _trailPoints.ToArray();
    }

    private void ClearTrail()
    {
        _trailPoints.Clear();
        Trail = Array.Empty<Vector2D>();
    }

}