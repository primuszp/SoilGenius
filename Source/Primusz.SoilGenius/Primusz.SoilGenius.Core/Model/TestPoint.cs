namespace Primusz.SoilGenius.Core.Model
{
    public class TestPoint
    {
        /// <summary>
        /// Time [s]
        /// </summary>
        public double Time { get; set; }

        /// <summary>
        /// Force [N]
        /// </summary>
        public double Force { get; set; }

        /// <summary>
        /// Stroke [mm]
        /// </summary>
        public double Stroke { get; set; }

        /// <summary>
        /// Strain [mm]
        /// </summary>
        public double Strain { get; set; }
    }
}