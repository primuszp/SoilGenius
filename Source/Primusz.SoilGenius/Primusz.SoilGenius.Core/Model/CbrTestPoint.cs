namespace Primusz.SoilGenius.Core.Model
{
    public class CbrTestPoint : TestPoint
    {
        /// <summary>
        /// Penetration [mm]
        /// </summary>
        public double Penetration
        {
            get => Stroke;
            set => Stroke = value;
        }
    }
}