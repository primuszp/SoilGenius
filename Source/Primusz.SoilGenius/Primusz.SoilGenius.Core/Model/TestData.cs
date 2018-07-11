using System;

namespace Primusz.SoilGenius.Core.Model
{
    public class TestData
    {
        public Guid Id { get; }

        public string Code { get; set; }

        public string Operator { get; set; }

        public DateTime DateTime { get; set; }

        /// <summary>
        /// Control speed [mm/min]
        /// </summary>
        public double ControlSpeed { get; set; }

        /// <summary>
        /// Sampling interval [s]
        /// </summary>
        public double SamplingInterval { get; set; }

        /// <summary>
        /// Control variable [Force / Stroke]
        /// </summary>
        public ControlVariable ControlVariable { get; set; }

        /// <summary>
        /// Test points
        /// </summary>
        public System.Collections.Generic.List<TestPoint> Points { get; set; }

        #region Constructors

        public TestData()
        {
            Id = Guid.NewGuid();
            DateTime = DateTime.UtcNow;
            ControlVariable = ControlVariable.Stroke;
            Points = new System.Collections.Generic.List<TestPoint>();
        }

        #endregion
    }
}