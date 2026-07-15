/// <summary>
/// Calculates air density based on altitude using an exponential atmosphere model.
/// </summary>

namespace AeroSim.Simulation.Environment;

public sealed class AtmosphereModel
{

    // ---------------------------
    // Constants
    // ---------------------------

    private const double SeaLevelDensity = 1.225;
    private const double ScaleHeightMeters = 8500;
    public double GetAirDensity(double altitudeMeters)
    {
        if (altitudeMeters < 0)
        {
            altitudeMeters = 0;
        }

        return SeaLevelDensity * Math.Exp(-altitudeMeters / ScaleHeightMeters);
    }
}