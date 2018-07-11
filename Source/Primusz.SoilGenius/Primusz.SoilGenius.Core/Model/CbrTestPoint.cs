namespace Primusz.SoilGenius.Core.Model
{
    public class CbrTestPoint : TestPoint
    {
        /// <summary>
        /// Penetration [mm]
        /// </summary>
        public double Penetration
        {
            get
            {
                return Stroke;
            }
            set
            {
                Stroke = value;
            }
        }
    }
}