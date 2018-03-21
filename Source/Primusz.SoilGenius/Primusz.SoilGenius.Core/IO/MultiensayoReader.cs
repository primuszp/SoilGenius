using System;
using System.IO;
using System.Globalization;
using Primusz.SoilGenius.Core.Model;

namespace Primusz.SoilGenius.Core.IO
{
    public class MultiensayoReader : IDisposable
    {
        #region Members

        private StreamReader reader;

        #endregion

        #region Properties

        public int TestReadings { get; private set; }

        public double UpperForceLimit { get; private set; }

        public double LowerForceLimit { get; private set; }

        public double UpperStrokeLimit { get; private set; }

        public double LowerStrokeLimit { get; private set; }

        #endregion

        #region Constructors

        public MultiensayoReader(Stream stream)
        {
            reader = new StreamReader(stream);
        }

        #endregion

        #region Methods

        public CbrTestSample Read()
        {
            CbrTestSample sample = new CbrTestSample();

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();

                if (!string.IsNullOrEmpty(line))
                {
                    if (line.Contains("Speed:"))
                        sample.ControlSpeed = ParseDoubleTextRow(line, "mm/min");

                    if (line.Contains("Sampling interval:"))
                        sample.SamplingInterval = ParseDoubleTextRow(line, "s");

                    if (line.Contains("Upper force limit:"))
                        UpperForceLimit = ParseDoubleTextRow(line, "kN");

                    if (line.Contains("Lower force limit:"))
                        LowerForceLimit = ParseDoubleTextRow(line, "kN");

                    if (line.Contains("Upper stroke limit:"))
                        UpperStrokeLimit = ParseDoubleTextRow(line, "mm");

                    if (line.Contains("Lower stroke limit:"))
                        LowerStrokeLimit = ParseDoubleTextRow(line, "mm");

                    if (line.Contains("Control variable:"))
                        sample.ControlVariable = ParseControlVariableTextRow(line);

                    if (line == "Number;Time (s);Force (N);Stroke (mm);Strain (mm)")
                    {
                        string row = reader.ReadLine();
                        while (row != "Final de los datos adquiridos en el ensayo")
                        {
                            var point = ParseTestPointTextRow(row);
                            sample.TestPoints.Add(point);
                            row = reader.ReadLine();
                        }
                    }
                }
            }
            return sample;
        }

        private double ParseDoubleTextRow(string text, string suffix)
        {
            string variable = text.Substring(text.IndexOf(";", StringComparison.Ordinal) + 1)
                .Replace(suffix, string.Empty);
            return double.Parse(variable.Replace(',', '.'), CultureInfo.InvariantCulture);
        }

        private ControlVariable ParseControlVariableTextRow(string text)
        {
            string variable = text.Substring(text.IndexOf(";", StringComparison.Ordinal) + 1).ToLower();

            if (variable == "stroke")
                return ControlVariable.Stroke;

            return ControlVariable.Force;
        }

        private TestPoint ParseTestPointTextRow(string text)
        {
            TestReadings++;

            TestPoint point = new TestPoint();

            if (!string.IsNullOrEmpty(text))
            {
                string[] buffer = text.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);

                if (buffer.Length == 5)
                {
                    point.Time = double.Parse(buffer[1].Replace(',', '.'), CultureInfo.InvariantCulture);
                    point.Force = double.Parse(buffer[2].Replace(',', '.'), CultureInfo.InvariantCulture);
                    point.Stroke = double.Parse(buffer[3].Replace(',', '.'), CultureInfo.InvariantCulture);
                    point.Strain = double.Parse(buffer[4].Replace(',', '.'), CultureInfo.InvariantCulture);
                }
            }
            return point;
        }

        #endregion

        #region IDisposable Interface

        ~MultiensayoReader()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (reader != null)
                {
                    reader.Dispose();
                    reader = null;
                }
            }
        }

        #endregion
    }
}