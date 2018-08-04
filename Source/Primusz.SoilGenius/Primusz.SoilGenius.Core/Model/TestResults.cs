namespace Primusz.SoilGenius.Core.Model
{
    public class TestResults
    {
        public double F25 { get; set; }

        public double F50 { get; set; }

        public double Slope { get; set; }

        public double Intercept { get; set; }

        public double SplineRho { get; set; }

        public double SplineNodes { get; set; }

        public TestResults()
        {
            SplineRho = 2.0d;
            SplineNodes = 50;
        }
    }
}