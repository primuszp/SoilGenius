namespace Primusz.SoilGenius.Core.Abstractions
{
    public interface ITestPoint
    {
        /// <summary>
        /// Time [s]
        /// </summary>
        double Time { get; set; }

        /// <summary>
        /// Force [N]
        /// </summary>
        double Force { get; set; }

        /// <summary>
        /// Stroke [mm]
        /// </summary>
        double Stroke { get; set; }

        /// <summary>
        /// Strain [mm]
        /// </summary>
        double Strain { get; set; }
    }
}