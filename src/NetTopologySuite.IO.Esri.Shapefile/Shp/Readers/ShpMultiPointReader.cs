using NetTopologySuite.Geometries;
using System.IO;

namespace NetTopologySuite.IO.Esri.Shp.Readers
{

    /// <summary>
    /// MultiPoint SHP file reader. 
    /// </summary>
    public class ShpMultiPointReader : ShpReader<MultiPoint>
    {
        internal ShpMultiPointReader(Stream shpStream, GeometryFactory factory, int dbfRecrodCount) : base(shpStream, factory, dbfRecrodCount)
        {
            if (!ShapeType.IsMultiPoint())
                ThrowUnsupportedShapeTypeException();
        }

        /// <inheritdoc/>
        public ShpMultiPointReader(Stream shpStream, GeometryFactory factory) : this(shpStream, factory, int.MaxValue)
        {
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
