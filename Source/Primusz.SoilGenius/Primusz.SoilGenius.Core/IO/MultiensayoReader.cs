using System;
using System.IO;
using Primusz.SoilGenius.Core.Model;
using Primusz.SoilGenius.Core.Extensions;

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

        public ITestData Read()
        {
            ITestData sample = new CbrTestData();

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();

                if (!string.IsNullOrEmpty(line))
                {
                    if (line.Contains("Speed:"))
                        sample.TestSetting.ControlSpeed = ParseDoubleTextRow(line, "mm/min");

                    if (line.Contains("Sampling interval:"))
                        sample.TestSetting.SamplingInterval = ParseDoubleTextRow(line, "s");

                    if (line.Contains("Upper force limit:"))
                        UpperForceLimit = ParseDoubleTextRow(line, "kN");

                    if (line.Contains("Lower force limit:"))
                        LowerForceLimit = ParseDoubleTextRow(line, "kN");

                    if (line.Contains("Upper stroke limit:"))
                        UpperStrokeLimit = ParseDoubleTextRow(line, "mm");

                    if (line.Contains("Lower stroke limit:"))
                        LowerStrokeLimit = ParseDoubleTextRow(line, "mm");

                    if (line.Contains("Control variable:"))
                        sample.TestSetting.ControlStyle = ParseControlVariableTextRow(line);

                    if (line == "Number;Time (s);Force (kN);Stroke (mm);Strain (mm)")
                    {
                        var row = reader.ReadLine();
                        while (row != "Final de los datos adquiridos en el ensayo")
                        {
                            var point = ParseTestPointTextRow(row);
                            sample.TestDataPoints.Add(point);
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
            return variable.ToDouble();
        }

        private ControlStyle ParseControlVariableTextRow(string text)
        {
            string variable = text.Substring(text.IndexOf(";", StringComparison.Ordinal) + 1).ToLower();

            if (variable == "stroke")
                return ControlStyle.Stroke;

            return ControlStyle.Force;
        }

        private ITestPoint ParseTestPointTextRow(string text)
        {
            TestReadings++;

            ITestPoint point = new CbrTestPoint();

            if (!string.IsNullOrEmpty(text))
            {
                string[] buffer = text.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                if (buffer.Length == 5)
                {
                    point.Time = buffer[1].ToDouble();
                    point.Force = buffer[2].ToDouble();
                    point.Stroke = buffer[3].ToDouble();
                    point.Strain = buffer[4].ToDouble();
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