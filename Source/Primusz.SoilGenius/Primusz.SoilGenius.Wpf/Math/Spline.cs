using System;

namespace Primusz.SoilGenius.Wpf.Math
{
    public class Spline
    {
        #region Members

        private alglib.spline1dfitreport report;
        private alglib.spline1dinterpolant spline;

        #endregion

        #region Properties

        public double[] X { get; set; }

        public double[] Y { get; set; }

        /// <summary>
        /// Amount of smoothing.
        /// </summary>
        public double Rho { get; set; }

        /// <summary>
        /// Number of basis functions.
        /// </summary>
        public int Nodes { get; set; }

        #endregion

        #region Constructors

        public Spline()
        {
            Rho = -5;
            Nodes = 50;
        }

        #endregion

        #region Methods

        public bool DataFitting()
        {
            int info = 0;

            if (X != null && Y != null)
                alglib.spline1dfitpenalized(X, Y, Nodes, Rho, out info, out spline, out report);

            // EXPECTED: 1
            if (info == 1) return true;

            return false;
        }

        public double Calculation(double x)
        {
            if (spline != null)
                return alglib.spline1dcalc(spline, x);

            return 0.0;
        }

        #endregion
    }
}