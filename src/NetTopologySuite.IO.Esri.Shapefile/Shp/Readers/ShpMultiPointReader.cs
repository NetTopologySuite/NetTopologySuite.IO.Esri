using NetTopologySuite.Geometries;
using System.IO;

namespace NetTopologySuite.IO.Esri.Shp.Readers
{

    /// <summary>
    /// MultiPoint SHP file reader. 
    /// </summary>
    public class ShpMultiPointReader : ShpReader<MultiPoint>
    {
        /// <inheritdoc/>
        public ShpMultiPointReader(Stream shpStream, GeometryFactory factory) : base(shpStream, factory)
        {
            if (!ShapeType.IsMultiPoint())
                ThrowUnsupportedShapeTypeException();
        }
        internal override MultiPoint GetEmptyGeometry()
        {
            return MultiPoint.Empty;
        }

        internal override MultiPoint ReadGeometry(Stream stream)
        {
            stream.ReadXYBoundingBox();

            var pointCount = stream.ReadPointCount();
            var coordinateSequence = CreateCoordinateSequence(pointCount);

            stream.ReadXYCoordinates(coordinateSequence);

            if (HasZ)
            {
                stream.ReadZRange();
                stream.ReadZCoordinates(coordinateSequence);
            }
            if (HasM)
            {
                stream.ReadMRange();
                stream.ReadMValues(coordinateSequence);
            }

            return Factory.CreateMultiPoint(coordinateSequence);
        }
    }


}
