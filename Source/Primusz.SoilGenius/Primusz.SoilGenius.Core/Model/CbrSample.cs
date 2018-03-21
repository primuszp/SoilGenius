using System;
using System.Collections.Generic;

namespace Primusz.SoilGenius.Core.Model
{
    public class CbrTestSample : TestSample
    {
        #region Properties

        /// <summary>
        /// Soil Test Name
        /// </summary>
        public string TestName { get; private set; }

        public string ApplicableStandard { get; set; }

        /// <summary>
        /// Control speed [mm/min]
        /// </summary>
        public double ControlSpeed { get; set; }

        /// <summary>
        /// Sampling interval [s]
        /// </summary>
        public double SamplingInterval { get; set; }

        public ControlVariable ControlVariable { get; set; }

        public IList<TestPoint> TestPoints { get; set; }

        #endregion

        #region Constructors

        public CbrTestSample()
        {
            TestName = "California Bearing Ratio (CBR)";
            TestPoints = new List<TestPoint>();
            ControlVariable = ControlVariable.Stroke;
        }

        #endregion
    }
}