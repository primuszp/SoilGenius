using System;
using System.Collections.Generic;
using Primusz.SoilGenius.Core.Model;

namespace Primusz.SoilGenius.Core.Abstractions
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