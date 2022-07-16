using NetTopologySuite.Geometries;
using System.IO;

namespace NetTopologySuite.IO.Esri.Shp.Readers
{

    /// <summary>
    /// Point SHP file reader. 
    /// </summary>
    public class ShpPointReader : ShpReader<Point>
    {
        internal ShpPointReader(Stream shpStream, GeometryFactory factory, Envelope mbrFilter, int dbfRecrodCount)
            : base(shpStream, factory, mbrFilter, dbfRecrodCount)
        {
            if (!ShapeType.IsPoint())
                ThrowUnsupportedShapeTypeException();
        }

        /// <inheritdoc/>
        public ShpPointReader(Stream shpStream, GeometryFactory factory, Envelope mbrFilter) : this(shpStream, factory, mbrFilter, int.MaxValue)
        {
        }

        internal override Point GetEmptyGeometry()
        {
            return Point.Empty;
        }

        internal override bool ReadGeometry(Stream shapeBinary, out Point geometry)
        {
            var coordinateSequence = CreateCoordinateSequence(1);
            shapeBinary.ReadPoint(coordinateSequence);

            geometry = Factory.CreatePoint(coordinateSequence);
            if (!IsInMbr(geometry))
            {
                return false;
            }
            return true;
        }
    }


}
