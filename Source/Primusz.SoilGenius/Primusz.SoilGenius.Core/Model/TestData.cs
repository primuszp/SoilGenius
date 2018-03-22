using System;

namespace Primusz.SoilGenius.Core.Model
{
    public abstract class TestData
    {
        public Guid Id { get; }

        public string Code { get; set; }

        public string Operator { get; set; }

        public DateTime DateTime { get; set; }

        protected TestData()
        {
            Id = Guid.NewGuid();
            DateTime = DateTime.UtcNow;
        }
    }
}