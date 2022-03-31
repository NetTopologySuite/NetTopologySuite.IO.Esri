using NetTopologySuite.Geometries;
using System.IO;

namespace NetTopologySuite.IO.Esri.Shp.Readers
{

    /// <summary>
    /// Point SHP file reader. 
    /// </summary>
    public class ShpPointReader : ShpReader<Point>
    {
        /// <inheritdoc/>
        public ShpPointReader(Stream shpStream, GeometryFactory factory) : base(shpStream, factory)
        {
            if (!ShapeType.IsPoint())
                ThrowUnsupportedShapeTypeException();
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
