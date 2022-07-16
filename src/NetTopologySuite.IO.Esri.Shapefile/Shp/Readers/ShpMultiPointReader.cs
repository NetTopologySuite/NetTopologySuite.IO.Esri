using NetTopologySuite.Geometries;
using System.IO;

namespace NetTopologySuite.IO.Esri.Shp.Readers
{

    /// <summary>
    /// MultiPoint SHP file reader. 
    /// </summary>
    public class ShpMultiPointReader : ShpReader<MultiPoint>
    {
        internal ShpMultiPointReader(Stream shpStream, GeometryFactory factory, Envelope mbrFilter, int dbfRecrodCount)
            : base(shpStream, factory, mbrFilter, dbfRecrodCount)
        {
            if (!ShapeType.IsMultiPoint())
                ThrowUnsupportedShapeTypeException();
        }

        /// <inheritdoc/>
        public ShpMultiPointReader(Stream shpStream, GeometryFactory factory, Envelope mbrFilter) : this(shpStream, factory, mbrFilter, int.MaxValue)
        {
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
            return true;
        }
    }


}
