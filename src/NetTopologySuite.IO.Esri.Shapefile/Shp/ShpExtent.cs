using NetTopologySuite.Geometries;

namespace NetTopologySuite.IO.Esri.Shp
{
    internal class ShpExtent
    {
        public ShpRange X { get; }

        public ShpRange Y { get; }

        public ShpRange Z { get; }

        public ShpRange M { get; }

        public ShpExtent()
        {
            X = new ShpRange();
            Y = new ShpRange();
            Z = new ShpRange();
            M = new ShpRange();
        }
        public void Clear()
        {
            X.Clear();
            Y.Clear();
            Z.Clear();
            M.Clear();
        }

        public void Expand(CoordinateSequence coordinateSequence)
        {
            for (int i = 0; i < coordinateSequence.Count; i++)
            {
                X.Expand(coordinateSequence.GetX(i));
                Y.Expand(coordinateSequence.GetY(i));
                if (coordinateSequence.HasZ)
                {
                    Z.Expand(coordinateSequence.GetZ(i));
                }
                if (coordinateSequence.HasM)
                {
                    M.Expand(coordinateSequence.GetM(i));
                }
            }
        }

        public void Expand(Coordinate coordinate)
        {
            X.Expand(coordinate.X);
            Y.Expand(coordinate.Y);
            Z.Expand(coordinate.Z);
            M.Expand(coordinate.M);
        }

        public void Expand(ShpExtent other)
        {
            X.Expand(other.X);
            Y.Expand(other.Y);
            Z.Expand(other.Z);
            M.Expand(other.M);
        }
    }


}
