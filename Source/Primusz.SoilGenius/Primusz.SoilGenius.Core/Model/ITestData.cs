using System;
using System.Collections.Generic;

namespace Primusz.SoilGenius.Core.Model
{
    public interface ITestData
    {
        Guid Id { get; }

        string Name { get; set; }

        string Operator { get; set; }

        DateTime DateTime { get; set; }

        TestSetting TestSetting { get; set; }

        IList<ITestPoint> TestDataPoints { get; set; }
    }
}