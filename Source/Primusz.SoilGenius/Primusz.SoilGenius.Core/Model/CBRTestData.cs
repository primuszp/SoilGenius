using System;
using System.Collections.Generic;

namespace Primusz.SoilGenius.Core.Model
{
    /// <summary>
    /// California Bearing Ratio (CBR) Test Data Sheet
    /// </summary>
    public class CbrTestData : TestData
    {
        #region Properties

        /// <summary>
        /// Test Name
        /// </summary>
        public string TestName { get; }

        /// <summary>
        /// Applicable Standard
        /// </summary>
        public string Standard { get; set; }

        /// <summary>
        /// Control speed [mm/min]
        /// </summary>
        public double ControlSpeed { get; set; }

        public IList<CbrTestPoint> TestPoints { get; set; }

        #endregion

        #region Constructors

        public CbrTestData()
        {
            TestName = "California Bearing Ratio (CBR)";
            TestPoints = new List<CbrTestPoint>();
        }

        #endregion
    }
}