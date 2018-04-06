using System.IO;
using Primusz.SoilGenius.Core.Extensions;
using Primusz.SoilGenius.Core.IO;

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
        public string Name { get; set; }

        /// <summary>
        /// CBR Test File
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// Applicable Standard
        /// </summary>
        public string Standard { get; set; }

        /// <summary>
        /// Control speed [mm/min]
        /// </summary>
        public double ControlSpeed { get; set; }

        /// <summary>
        /// CBR Test Points
        /// </summary>
        public System.Collections.Generic.List<CbrTestPoint> Points { get; set; }

        #endregion

        #region Constructors

        public CbrTestData()
        {
            Name = "CBR";
            Standard = "MSZ EN 13286-47";
            ControlSpeed = 1.27;
            Points = new System.Collections.Generic.List<CbrTestPoint>();
        }

        #endregion

        public void LoadTestFile(Stream stream)
        {
            using (DataReader reader = new DataReader(stream))
            {
                if (reader.Read())
                {
                    foreach (var row in reader.GetRows())
                    {
                        double x = row[0].ToDouble();
                        double y = row[1].ToDouble();

                        Points.Add(new CbrTestPoint { Penetration = x, Force = y });
                    }
                }
            }
        }
    }
}