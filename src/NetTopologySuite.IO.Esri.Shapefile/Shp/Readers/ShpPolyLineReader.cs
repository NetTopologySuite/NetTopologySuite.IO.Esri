using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Shapefiles.Readers;
using System;
using System.IO;

namespace NetTopologySuite.IO.Esri.Shp.Readers
{
    internal class ShpPolyLineReader : ShpReader<MultiLineString>
    {
        /// <inheritdoc/>
        public ShpPolyLineReader(Stream shpStream, ShapefileReaderOptions options = null)
            : base(shpStream, options)
        {
            if (!ShapeType.IsPolyLine())
                ThrowUnsupportedShapeTypeException();
        }

        internal override MultiLineString GetEmptyGeometry()
        {
            return Factory.CreateMultiLineString();
        }

        internal override bool ReadGeometry(Stream stream, out MultiLineString geometry)
        {
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
            if (GeometryBuilderMode == GeometryBuilderMode.FixInvalidShapes)
            {
                geometry.Normalize();
            }
            return true;
        }
    }
}
