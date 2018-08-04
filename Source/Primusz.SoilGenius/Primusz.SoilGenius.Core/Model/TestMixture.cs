namespace Primusz.SoilGenius.Core.Model
{
    public class TestMixture
    {
        /// <summary>
        /// Age [hour]
        /// </summary>
        public double Age { get; set; }

        /// <summary>
        /// Name of Soil
        /// </summary>
        public string SoilName { get; set; }

        /// <summary>
        /// Name of Binder
        /// </summary>
        public string BinderName { get; set; }

        /// <summary>
        /// Binder Content [%]
        /// </summary>
        public double BinderContent { get; set; }

        /// <summary>
        /// Moisture Content [%]
        /// </summary>
        public double MoistureContent { get; set; }
    }
}