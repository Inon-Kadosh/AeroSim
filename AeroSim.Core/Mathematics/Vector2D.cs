/// <summary>
/// Represents a two-dimensional vector and provides common vector operations.
/// </summary>

namespace AeroSim.Core.Mathematics;
public readonly record struct Vector2D(double X,double Y)
{

    // ---------------------------
    // Constants
    // ---------------------------

    public static Vector2D Zero => new(0,0);


    // ---------------------------
    // Properties
    // ---------------------------

    public double Magnitude => Math.Sqrt(X * X + Y * Y);


    // ---------------------------
    // Vector operations
    // ---------------------------

    public Vector2D Normalize()
    {
        if(Magnitude == 0){return Zero;}
        return new Vector2D(X/Magnitude , Y/Magnitude);
    }


    // ---------------------------
    // Operators
    // ---------------------------

    public static Vector2D operator +(Vector2D left,Vector2D right)
    {
        return new Vector2D(left.X+right.X , left.Y+right.Y);
    }

    public static Vector2D operator -(Vector2D left,Vector2D right)
    {
        return new Vector2D(left.X-right.X , left.Y-right.Y);
    }

    public static Vector2D operator *(Vector2D vector,double scalar)
    {
        return new Vector2D(vector.X * scalar , vector.Y * scalar);
    }

    public static Vector2D operator /(Vector2D vector,double scalar)
    {
        if(scalar == 0){throw new DivideByZeroException();}
        return new Vector2D(vector.X / scalar , vector.Y / scalar);
    }



}