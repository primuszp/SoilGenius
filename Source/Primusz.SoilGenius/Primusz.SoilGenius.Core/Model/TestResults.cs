using System;

namespace Primusz.SoilGenius.Core.Model
{
    public class TestResults
    {
        public double Force1 { get; set; }

        public double Force2 { get; set; }

        public double SplineRho { get; set; }

        public double SplineNodes { get; set; }

        public double Slope { get; set; }

        public double Intercept { get; set; }

        public Boolean IsCorrected { get; set; }

        public TestResults()
        {
            Slope = 0.0;
            Intercept = 0.0d;
            SplineRho = 2.0d;
            SplineNodes = 50;
            IsCorrected = false;
        }
    }
}