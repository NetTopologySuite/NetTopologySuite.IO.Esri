using NetTopologySuite.Geometries;
using System.IO;

namespace NetTopologySuite.IO.Esri.Shp.Readers
{
    internal class ShpPolyLineReader : ShpReader<MultiLineString>
    {
        internal ShpPolyLineReader(Stream shpStream, GeometryFactory factory, Envelope mbrFilter, int dbfRecrodCount)
            : base(shpStream, factory, mbrFilter, dbfRecrodCount)
        {
            if (!ShapeType.IsPolyLine())
                ThrowUnsupportedShapeTypeException();
        }

        /// <inheritdoc/>
        public ShpPolyLineReader(Stream shpStream, GeometryFactory factory, Envelope mbrFilter) : this(shpStream, factory, mbrFilter, int.MaxValue)
        {
        }

        internal override MultiLineString GetEmptyGeometry()
        {
            return MultiLineString.Empty;
        }

        internal override bool ReadGeometry(Stream stream, out MultiLineString geometry)
        {
            stream.ReadXYBoundingBox();
            var bbox = stream.ReadXYBoundingBox();
            if (!IsInMbr(bbox))
            {
                geometry = null;
                return false;
            }

            // SHP Docs: A part is a connected sequence of two or more points. (page 7)
            var partsBuilder = new ShpMultiPartBuilder(1, 2);
            partsBuilder.ReadParts(stream, HasZ, HasM, CreateCoordinateSequence);

            var lines = new LineString[partsBuilder.Count];
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = Factory.CreateLineString(partsBuilder[i]);
            }

            geometry = Factory.CreateMultiLineString(lines);
            if (!IsInMbr(geometry))
            {
                return false;
            }
            return true;
        }
    }
}
