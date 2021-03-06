﻿using System.Collections.Generic;
using System.Linq;

namespace Primusz.SoilGenius.Core.Numerics
{
    public static class FindPeak
    {
        public static IList<int> FindPeaks(IList<double> values, int rangeOfPeaks)
        {
            List<int> peaks = new List<int>();
            double current;
            IEnumerable<double> range;

            int checksOnEachSide = rangeOfPeaks / 2;
            for (int i = 0; i < values.Count; i++)
            {
                current = values[i];
                range = values;

                if (i > checksOnEachSide)
                {
                    range = range.Skip(i - checksOnEachSide);
                }

                range = range.Take(rangeOfPeaks);
                if ((range.Count() > 0) && (current == range.Max()))
                {
                    peaks.Add(i);
                }
            }

            return peaks;
        }

        private static double[] Invert(double[] data, int low, int high)
        {
            double[] buffer = new double[high - low];

            for (int i = low; i < high; i++)
            {
                buffer[i - low] = -1.0d * data[i];
            }

            return buffer;
        }
    }
}