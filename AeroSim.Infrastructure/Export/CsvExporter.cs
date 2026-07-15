using AeroSim.Core.Models;
using System.Globalization;
using System.Text;

namespace AeroSim.Infrastructure.Export;

public sealed class CsvExporter
{
    public void ExportTelemetry(
        IReadOnlyList<TelemetryPoint> telemetry,
        string filePath)
    {
        StringBuilder builder = new();

        builder.AppendLine(
            "Time,PositionX,PositionY,VelocityX,VelocityY,AccelerationX,AccelerationY,Speed");

        foreach (TelemetryPoint point in telemetry)
        {
            builder.AppendLine(
                string.Join(",",
                    point.TimeSeconds.ToString(CultureInfo.InvariantCulture),
                    point.Position.X.ToString(CultureInfo.InvariantCulture),
                    point.Position.Y.ToString(CultureInfo.InvariantCulture),
                    point.Velocity.X.ToString(CultureInfo.InvariantCulture),
                    point.Velocity.Y.ToString(CultureInfo.InvariantCulture),
                    point.Acceleration.X.ToString(CultureInfo.InvariantCulture),
                    point.Acceleration.Y.ToString(CultureInfo.InvariantCulture),
                    point.SpeedMetersPerSecond.ToString(CultureInfo.InvariantCulture)
                ));
        }

        File.WriteAllText(
            filePath,
            builder.ToString());
    }
}