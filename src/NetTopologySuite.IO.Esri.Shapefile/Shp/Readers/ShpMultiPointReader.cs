using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Shapefiles.Readers;
using System.IO;

namespace NetTopologySuite.IO.Esri.Shp.Readers
{

    /// <summary>
    /// MultiPoint SHP file reader. 
    /// </summary>
    public class ShpMultiPointReader : ShpReader<MultiPoint>
    {

        /// <inheritdoc/>
        public ShpMultiPointReader(Stream shpStream, ShapefileReaderOptions options = null)
            : base(shpStream, options)
        {
            if (!ShapeType.IsMultiPoint())
                ThrowUnsupportedShapeTypeException();
        }

        internal override MultiPoint GetEmptyGeometry()
        {
            return MultiPoint.Empty;
        }

        internal override bool ReadGeometry(Stream stream, out MultiPoint geometry)
        {
            var bbox = stream.ReadXYBoundingBox();
            if (!IsInMbr(bbox))
            {
                geometry = null;
                return false;
            }

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

            geometry = Factory.CreateMultiPoint(coordinateSequence);

            if (!IsInMbr(geometry))
            {
                return false;
            }
            if (GeometryBuilderMode == GeometryBuilderMode.FixInvalidShapes)
            {
                geometry.Normalize();
            }
            return true;
        }
    }


}
