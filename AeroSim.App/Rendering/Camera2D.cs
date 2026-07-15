/// <summary>
/// Converts between world and screen coordinates and manages camera pan and zoom.
/// </summary>
/// 
using System;
using AeroSim.Core.Mathematics;
using Avalonia;

namespace AeroSim.App.Rendering;

public sealed class Camera2D
{

    // ---------------------------
    // Camera state
    // ---------------------------

    private double _pixelsPerMeter = 1.5;
    private Vector2D _center = Vector2D.Zero;



    // ---------------------------
    // Camera properties
    // ---------------------------

    public double MinimumPixelsPerMeter { get; set; } = 0.05;

    public double MaximumPixelsPerMeter { get; set; } = 20;

    public double PixelsPerMeter
    {
        get => _pixelsPerMeter;
        private set =>
            _pixelsPerMeter = Math.Clamp(
                value,
                MinimumPixelsPerMeter,
                MaximumPixelsPerMeter);
    }

    public Vector2D Center => _center;



    // ---------------------------
    // Coordinate conversion
    // ---------------------------

    public Point WorldToScreen(Vector2D world , Size viewport)
    {
        double screenX =
            viewport.Width / 2 + (world.X - _center.X) * PixelsPerMeter;

        double screenY =
            viewport.Height / 2 - (world.Y - _center.Y) * PixelsPerMeter;

        return new Point(screenX , screenY);
    }

    public Vector2D ScreenToWorld(Point screen , Size viewport)
    {
        double worldX =
            _center.X +
            (screen.X - viewport.Width / 2) /
            PixelsPerMeter;

        double worldY =
            _center.Y -
            (screen.Y - viewport.Height / 2) /
            PixelsPerMeter;

        return new Vector2D(worldX , worldY);
    }



    // ---------------------------
    // Camera controls
    // ---------------------------

    public void PanByScreenDelta(Vector delta)
    {
        _center = new Vector2D(
            _center.X - delta.X / PixelsPerMeter,
            _center.Y + delta.Y / PixelsPerMeter);
    }

    public void ZoomAt(Point screenPoint , Size viewport , double zoomFactor)
    {

        if (zoomFactor <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(zoomFactor),"Zoom factor must be greater than zero.");
        }
        Vector2D worldBefore = ScreenToWorld(screenPoint , viewport);

        PixelsPerMeter *= zoomFactor;

        Vector2D worldAfter = ScreenToWorld(screenPoint , viewport);

        _center = new Vector2D(
            _center.X +
            worldBefore.X -
            worldAfter.X,

            _center.Y +
            worldBefore.Y -
            worldAfter.Y);
    }

    public void SetView(Vector2D center , double pixelsPerMeter)
    {
        if (pixelsPerMeter <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pixelsPerMeter) , "Pixels per meter must be greater than zero.");
        }
        _center = center;
        PixelsPerMeter = pixelsPerMeter;
    }
}