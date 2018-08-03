using System.IO;
using System.Collections.Generic;
using Primusz.SoilGenius.Core.IO;
using Primusz.SoilGenius.Core.Extensions;

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

        #endregion

        #region Constructors

        public CbrTestData()
        {
            Name = "CBR";
            Standard = "MSZ EN 13286-47";
            ControlSpeed = 1.27;
            Points = new List<TestPoint>();
        }

        #endregion

        public void LoadTestFile(Stream stream)
        {
            using (var reader = new DataReader(stream))
            {
                if (reader.Read())
                {
                    foreach (var row in reader.GetRows())
                    {
                        var x = row[0].ToDouble();
                        var y = row[1].ToDouble();

                        Points.Add(new TestPoint { Stroke = x, Force = y });
                    }
                }
            }
        }
    }
}