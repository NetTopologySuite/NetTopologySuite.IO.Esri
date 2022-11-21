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
        public ShpPointReader(Stream shpStream, ShapefileReaderOptions options = null)
            : base(shpStream, options)
        {
            if (!ShapeType.IsPoint())
                ThrowUnsupportedShapeTypeException();
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
