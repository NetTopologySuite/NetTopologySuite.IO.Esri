using NetTopologySuite.Geometries;
using System.IO;

namespace NetTopologySuite.IO.Esri.Shp.Readers
{

    /// <summary>
    /// Point SHP file reader. 
    /// </summary>
    public class ShpPointReader : ShpReader<Point>
    {
        internal ShpPointReader(Stream shpStream, GeometryFactory factory, int dbfRecrodCount) : base(shpStream, factory, dbfRecrodCount)
        {
            if (!ShapeType.IsPoint())
                ThrowUnsupportedShapeTypeException();
        }

        /// <inheritdoc/>
        public ShpPointReader(Stream shpStream, GeometryFactory factory) : this(shpStream, factory, int.MaxValue)
        {
        }

        internal override Point GetEmptyGeometry()
        {
            return Point.Empty;
        }

        internal override Point ReadGeometry(Stream shapeBinary)
        {
            var coordinateSequence = CreateCoordinateSequence(1);
            shapeBinary.ReadPoint(coordinateSequence);
            return Factory.CreatePoint(coordinateSequence);
        }
    }


}
