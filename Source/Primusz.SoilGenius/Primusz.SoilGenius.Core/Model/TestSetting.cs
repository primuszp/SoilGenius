namespace Primusz.SoilGenius.Core.Model
{
    public class TestSetting
    {
        #region Properties

        /// <summary>
        /// Control speed [mm/min]
        /// </summary>
        public double ControlSpeed { get; set; }

        /// <summary>
        /// Control variable [Force / Stroke]
        /// </summary>
        public ControlStyle ControlStyle { get; set; }

        /// <summary>
        /// Sampling interval [s]
        /// </summary>
        public double SamplingInterval { get; set; }

        #endregion

        #region Constructors

        public TestSetting()
        {
            ControlSpeed = 1.27;
            ControlStyle = ControlStyle.Stroke;
            SamplingInterval = 1.0;
        }

        #endregion
    }
}