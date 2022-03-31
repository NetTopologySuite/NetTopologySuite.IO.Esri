namespace NetTopologySuite.IO.Esri.Shp
{

    internal class ShpRange
    {
        public ShpRange()
        {
            Clear();
        }

        public double Min { get; private set; }

        public double Max { get; private set; }

        public bool IsEmpty => double.IsNaN(Min) || double.IsNaN(Max);

        public void Expand(double value)
        {
            if (double.IsNaN(value) || value == double.MinValue) // ArcMap 10.6 saves empty point coordinates as doubule.MinValue
                return;

            if (double.IsNaN(Min)) // NaN > value => false;
            {
                Min = value;
            }
            else if (Min > value) // Min > NaN => false;   
            {
                Min = value;
            }

            if (double.IsNaN(Max))  //   NaN < value => false;
            {
                Max = value;
            }
            else if (Max < value) // Max < NaN => false; 
            {
                Max = value;
            }
        }

        public void Expand(ShpRange other)
        {
            Expand(other.Min);
            Expand(other.Max);
        }

        public void Clear()
        {
            this.Min = double.NaN;
            this.Max = double.NaN;
        }

        public override string ToString()
        {
            return $"{Min}:{Max}";
        }
    }

}
