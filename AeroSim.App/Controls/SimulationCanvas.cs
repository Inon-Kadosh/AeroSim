/// <summary>
/// Renders the live rocket simulation, including the camera,
/// grid, axes, trajectory, ground, rocket, and pointer interaction.
/// </summary>

using System;
using System.Collections.Generic;
using System.Globalization;
using AeroSim.App.Rendering;
using AeroSim.Core.Mathematics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;

namespace AeroSim.App.Controls;

public class SimulationCanvas : Control
{

    // ---------------------------
    // Avalonia properties
    // ---------------------------

    public static readonly StyledProperty<Vector2D>
        RocketPositionProperty =
            AvaloniaProperty.Register<
                SimulationCanvas,
                Vector2D>(
                    nameof(RocketPosition));

    public static readonly StyledProperty<Vector2D>
        RocketVelocityProperty =
            AvaloniaProperty.Register<
                SimulationCanvas,
                Vector2D>(
                    nameof(RocketVelocity));

    public static readonly StyledProperty<bool>
        EngineIsRunningProperty =
            AvaloniaProperty.Register<
                SimulationCanvas,
                bool>(
                    nameof(EngineIsRunning));

    public static readonly StyledProperty<IReadOnlyList<Vector2D>>
        TrailProperty =
            AvaloniaProperty.Register<
                SimulationCanvas,
                IReadOnlyList<Vector2D>>(
                    nameof(Trail),
                    Array.Empty<Vector2D>());

    public static readonly StyledProperty<bool> FollowRocketProperty =
        AvaloniaProperty.Register<SimulationCanvas, bool>(nameof(FollowRocket),true);



    // ---------------------------
    // Interaction state
    // ---------------------------

    private readonly Camera2D _camera = new();

    private bool _isPanning;
    private Point _lastPointerPosition;



    // ---------------------------
    // Construction
    // ---------------------------

    static SimulationCanvas()
    {
        AffectsRender<SimulationCanvas>(
            RocketPositionProperty,
            RocketVelocityProperty,
            EngineIsRunningProperty,
            TrailProperty);
    }

    public SimulationCanvas()
    {
        Focusable = true;

        _camera.SetView(
            center: new Vector2D(300, 150),
            pixelsPerMeter: 1.5);
    }



    // ---------------------------
    // Bound properties
    // ---------------------------

    public Vector2D RocketPosition
    {
        get => GetValue(RocketPositionProperty);
        set => SetValue(RocketPositionProperty, value);
    }

    public Vector2D RocketVelocity
    {
        get => GetValue(RocketVelocityProperty);
        set => SetValue(RocketVelocityProperty, value);
    }

    public bool EngineIsRunning
    {
        get => GetValue(EngineIsRunningProperty);
        set => SetValue(EngineIsRunningProperty, value);
    }

    public IReadOnlyList<Vector2D> Trail
    {
        get => GetValue(TrailProperty);
        set => SetValue(TrailProperty, value);
    }

    public bool FollowRocket
    {
        get => GetValue(FollowRocketProperty);
        set => SetValue(FollowRocketProperty, value);
    }



    // ---------------------------
    // Rendering
    // ---------------------------

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        context.FillRectangle(Brushes.Black,Bounds);

        if (FollowRocket)
        {
            Vector2D followCenter = new(RocketPosition.X , RocketPosition.Y - 80);

            _camera.SetView(center: followCenter , pixelsPerMeter: _camera.PixelsPerMeter);
        }

        DrawGrid(context);
        DrawAxisLabels(context);
        DrawGround(context);
        DrawTrail(context);

        Point rocketScreenPosition =
            _camera.WorldToScreen(
                RocketPosition,
                Bounds.Size);

        DrawRocket(
            context,
            rocketScreenPosition,
            RocketVelocity,
            EngineIsRunning);
    }

    
    
    // ---------------------------
    // Pointer interaction
    // ---------------------------

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {  
        base.OnPointerPressed(e);

        PointerPoint point = e.GetCurrentPoint(this);

        if (!point.Properties.IsLeftButtonPressed)
        {
            return;
        }

        _isPanning = true;

        SetCurrentValue(FollowRocketProperty,false);

        _lastPointerPosition = e.GetPosition(this);

        e.Pointer.Capture(this);

        e.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (!_isPanning)
        {
            return;
        }

        Point currentPosition = e.GetPosition(this);

        Vector delta = currentPosition - _lastPointerPosition;

        _camera.PanByScreenDelta(delta);

        _lastPointerPosition = currentPosition;

        InvalidateVisual();

        e.Handled = true;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (!_isPanning)
        {
            return;
        }

        _isPanning = false;

        e.Pointer.Capture(null);

        InvalidateVisual();

        e.Handled = true;
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);

        double zoomFactor =
            e.Delta.Y > 0
                ? 1.15
                : 1.0 / 1.15;

        _camera.ZoomAt(
            screenPoint: e.GetPosition(this),
            viewport: Bounds.Size,
            zoomFactor: zoomFactor);

        InvalidateVisual();

        e.Handled = true;
    }



    // ---------------------------
    // Grid and axes
    // ---------------------------

    private void DrawGrid(DrawingContext context)
    {
        GetVisibleWorldBounds(
            out double minimumX,
            out double maximumX,
            out double minimumY,
            out double maximumY);

        double gridStep = CalculateGridStep();

        Pen minorGridPen = new(new SolidColorBrush(Color.Parse("#26313D")) , 1);

        Pen majorGridPen = new(new SolidColorBrush(Color.Parse("#354252")) , 1.4);

        double firstX = Math.Floor(minimumX / gridStep) * gridStep;

        double firstY = Math.Floor(minimumY / gridStep) * gridStep;

        for (double x = firstX ; x <= maximumX ; x += gridStep)
        {
            Point top = _camera.WorldToScreen(new Vector2D(x,maximumY),Bounds.Size);

            Point bottom = _camera.WorldToScreen(new Vector2D(x,minimumY),Bounds.Size);

            bool isMajor = IsMajorGridLine(x , gridStep);

            context.DrawLine(
                isMajor
                    ? majorGridPen
                    : minorGridPen,
                top,
                bottom);
        }

        for (double y = firstY ; y <= maximumY ; y += gridStep)
        {
            Point left = _camera.WorldToScreen(new Vector2D(minimumX , y) , Bounds.Size);

            Point right = _camera.WorldToScreen(new Vector2D(maximumX , y) , Bounds.Size);

            bool isMajor = IsMajorGridLine(y , gridStep);

            context.DrawLine(
                isMajor
                    ? majorGridPen
                    : minorGridPen,
                left,
                right);
        }
    }

    private void DrawAxisLabels(DrawingContext context)
    {
        GetVisibleWorldBounds(
            out double minimumX,
            out double maximumX,
            out double minimumY,
            out double maximumY);

        double gridStep = CalculateGridStep();

        Typeface typeface = new("Segoe UI");

        IBrush labelBrush = new SolidColorBrush(Color.Parse("#9CA6B5"));

        double firstX = Math.Floor(minimumX / gridStep) * gridStep;

        double firstY = Math.Floor(minimumY / gridStep) * gridStep;

        for (double altitude = firstY ; altitude <= maximumY ; altitude += gridStep)
        {
            Point screen = _camera.WorldToScreen(new Vector2D(minimumX,altitude),Bounds.Size);

            if (screen.Y < 10 || screen.Y > Bounds.Height - 30)
            {
                continue;
            }

            FormattedText label =
                new(
                    $"{altitude:0} m",
                    CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    typeface,
                    12,
                    labelBrush);

            context.DrawText(
                label,
                new Point(
                    10,
                    screen.Y - 8));
        }

        for (double range = firstX ; range <= maximumX ; range += gridStep)
        {
            Point screen = _camera.WorldToScreen(new Vector2D(range , 0) , Bounds.Size);

            if (screen.X < 30 || screen.X > Bounds.Width - 30)
            {
                continue;
            }

            FormattedText label =
                new(
                    $"{range:0}",
                    CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    typeface,
                    12,
                    labelBrush);

            context.DrawText(label,new Point(screen.X - 15,Bounds.Height - 24));
        }
    }

    private double CalculateGridStep()
    {
        double desiredWorldStep = 130.0 / _camera.PixelsPerMeter;

        double exponent =
            Math.Floor(
                Math.Log10(
                    Math.Max(
                        desiredWorldStep,
                        0.0001)));

        double powerOfTen = Math.Pow(10,exponent);

        double normalizedStep = desiredWorldStep / powerOfTen;

        double niceStep;

        if (normalizedStep <= 1)
        {
            niceStep = 1;
        }
        else if (normalizedStep <= 2)
        {
            niceStep = 2;
        }
        else if (normalizedStep <= 5)
        {
            niceStep = 5;
        }
        else
        {
            niceStep = 10;
        }

        return niceStep * powerOfTen;
    }

    private static bool IsMajorGridLine(double value , double gridStep)
    {
        double majorStep = gridStep * 5;

        double remainder = Math.Abs(value % majorStep);

        const double tolerance = 0.0001;

        return remainder < tolerance ||
               Math.Abs(
                   remainder -
                   majorStep) <
               tolerance;
    }

    private void GetVisibleWorldBounds(out double minimumX , out double maximumX , out double minimumY , out double maximumY)
    {
        Vector2D topLeft = _camera.ScreenToWorld(new Point(0, 0),Bounds.Size);

        Vector2D bottomRight = _camera.ScreenToWorld(new Point(Bounds.Width,Bounds.Height),Bounds.Size);

        minimumX = Math.Min(topLeft.X,bottomRight.X);

        maximumX = Math.Max(topLeft.X,bottomRight.X);

        minimumY = Math.Min(topLeft.Y,bottomRight.Y);

        maximumY = Math.Max(topLeft.Y,bottomRight.Y);
    }



    // ---------------------------
    // Ground
    // ---------------------------

    private void DrawGround(DrawingContext context)
    {
        Point groundPoint =
            _camera.WorldToScreen(
                new Vector2D(
                    _camera.Center.X,
                    0),
                Bounds.Size);

        double groundY =
            groundPoint.Y;

        if (groundY < 0 || groundY > Bounds.Height)
        {
            return;
        }

        context.DrawLine(
            new Pen(
                new SolidColorBrush(
                    Color.Parse("#E4E7EB")),
                2.5),
            new Point(
                0,
                groundY),
            new Point(
                Bounds.Width,
                groundY));
    }

    // ---------------------------
    // Trail
    // ---------------------------

    private void DrawTrail(DrawingContext context)
    {
        if (Trail is null || Trail.Count < 2)
        {
            return;
        }

        Pen trailPen = new(Brushes.Yellow,3);

        for (int i = 1 ; i < Trail.Count ; i++)
        {
            Point previous =
                _camera.WorldToScreen(
                    Trail[i - 1],
                    Bounds.Size);

            Point current =
                _camera.WorldToScreen(
                    Trail[i],
                    Bounds.Size);

            context.DrawLine(
                trailPen,
                previous,
                current);
        }
    }

    // ---------------------------
    // Rocket rendering
    // ---------------------------

    private static void DrawRocket(DrawingContext context , Point center , Vector2D velocity , bool engineIsRunning)
    {
        Point direction = CalculateScreenDirection(velocity);

        Point perpendicular = new(-direction.Y,direction.X);

        Point nose = Add(center,direction,32);

        Point rear = Add(center,direction,-22);

        Point leftRear = Add(rear,perpendicular,8);

        Point rightRear = Add(rear,perpendicular,-8);

        Point leftMiddle = Add(center,perpendicular,8);

        Point rightMiddle = Add(center,perpendicular,-8);

        StreamGeometry body = new();

        using (StreamGeometryContext geometry = body.Open())
        {
            geometry.BeginFigure(nose,true);

            geometry.LineTo(leftMiddle);

            geometry.LineTo(leftRear);

            geometry.LineTo(rightRear);

            geometry.LineTo(rightMiddle);

            geometry.EndFigure(true);
        }

        context.DrawGeometry(
            new SolidColorBrush(
                Color.Parse("#EAEAEA")),
            new Pen(
                new SolidColorBrush(
                    Color.Parse("#707070")),
                1),
            body);

        DrawFins(
            context,
            rear,
            direction,
            perpendicular);

        if (engineIsRunning)
        {
            DrawEnginePlume(
                context,
                rear,
                direction,
                perpendicular);
        }
    }

    private static void DrawFins(DrawingContext context , Point rear , Point direction , Point perpendicular)
    {
        Point leftFinTip =
            Add(
                Add(
                    rear,
                    direction,
                    8),
                perpendicular,
                16);

        Point rightFinTip =
            Add(
                Add(
                    rear,
                    direction,
                    8),
                perpendicular,
                -16);

        context.DrawLine(
            new Pen(
                Brushes.White,
                2),
            rear,
            leftFinTip);

        context.DrawLine(
            new Pen(
                Brushes.White,
                2),
            rear,
            rightFinTip);
    }

    private static void DrawEnginePlume(DrawingContext context , Point rear , Point direction , Point perpendicular)
    {
        Point plumeEnd = Add(rear,direction,-28);

        Point plumeLeft = Add(rear,perpendicular,5);

        Point plumeRight = Add(rear,perpendicular,-5);

        StreamGeometry plume = new();

        using (StreamGeometryContext geometry =plume.Open())
        {
            geometry.BeginFigure(plumeLeft,true);

            geometry.LineTo(plumeEnd);

            geometry.LineTo(plumeRight);

            geometry.EndFigure(true);
        }

        context.DrawGeometry(new SolidColorBrush(Color.Parse("#3FA9F5")),null,plume);

        Point innerEnd = Add(rear,direction,-12);

        Point innerLeft = Add(rear,perpendicular,2.5);

        Point innerRight = Add(rear,perpendicular,-2.5);

        StreamGeometry inner =new();

        using (StreamGeometryContext geometry = inner.Open())
        {
            geometry.BeginFigure(innerLeft,true);

            geometry.LineTo(innerEnd);

            geometry.LineTo(innerRight);

            geometry.EndFigure(true);
        }

        context.DrawGeometry(Brushes.White,null,inner);
    }



    // ---------------------------
    // Geometry helpers
    // ---------------------------

    private static Point CalculateScreenDirection(Vector2D velocity)
    {
        double screenX = velocity.X;

        double screenY = -velocity.Y;

        double magnitude = Math.Sqrt(screenX * screenX + screenY * screenY);

        if (magnitude < 0.0001)
        {
            return new Point(0,-1);
        }

        return new Point(screenX / magnitude , screenY / magnitude);
    }

    private static Point Add(Point origin , Point direction , double distance)
    {
        return new Point(
            origin.X + direction.X * distance,

            origin.Y + direction.Y * distance);
    }
}