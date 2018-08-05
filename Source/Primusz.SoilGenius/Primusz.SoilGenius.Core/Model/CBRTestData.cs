using System;
using System.IO;
using System.Collections.Generic;
using Primusz.SoilGenius.Core.IO;
using Primusz.SoilGenius.Core.Extensions;

namespace Primusz.SoilGenius.Core.Model
{
    /// <summary>
    /// California Bearing Ratio (CBR) Test Data Sheet
    /// </summary>
    public class CbrTestData : ITestData
    {
        #region Properties

        /// <summary>
        /// Test Id
        /// </summary>
        public Guid Id { get; }

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
        /// Name or Code of Operator
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// Test Date and Time
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Test Setting
        /// </summary>
        public TestSetting TestSetting { get; set; }

        /// <summary>
        /// Test Results
        /// </summary>
        public TestResults TestResults { get; set; }

        /// <summary>
        /// Test Mixture
        /// </summary>
        public TestMixture TestMixture { get; set; }
        
        /// <summary>
        /// Test points
        /// </summary>
        public IList<ITestPoint> TestDataPoints { get; set; }

        #endregion

        #region Constructors

        public CbrTestData()
        {
            Id = Guid.NewGuid();
            DateTime = DateTime.UtcNow;
            Standard = "MSZ EN 13286-47";
            TestSetting = new TestSetting();
            TestResults = new TestResults();
            TestMixture = new TestMixture();
            TestDataPoints = new List<ITestPoint>();
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

                        TestDataPoints.Add(new CbrTestPoint { Stroke = x, Force = y });
                    }
                }
            }
        }
    }
}