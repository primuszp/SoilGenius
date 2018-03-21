using System;

namespace Primusz.SoilGenius.Core.Model
{
    public abstract class TestSample
    {
        public Guid Id { get; }

        public string Code { get; set; }

        public string Operator { get; set; }

        public double Diameter { get; set; }

        public DateTime DateTime { get; set; }

        protected TestSample()
        {
            Id = Guid.NewGuid();
            DateTime = DateTime.UtcNow;
        }
    }
}