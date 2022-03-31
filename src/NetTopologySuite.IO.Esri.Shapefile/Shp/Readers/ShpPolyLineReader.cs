using NetTopologySuite.Geometries;
using System.IO;

namespace NetTopologySuite.IO.Esri.Shp.Readers
{
    internal class ShpPolyLineReader : ShpReader<MultiLineString>
    {
        public ShpPolyLineReader(Stream shpStream, GeometryFactory factory) : base(shpStream, factory)
        {
            if (!ShapeType.IsPolyLine())
                ThrowUnsupportedShapeTypeException();
        }

        internal override MultiLineString GetEmptyGeometry()
        {
            return MultiLineString.Empty;
        }

        internal override MultiLineString ReadGeometry(Stream shapeBinary)
        {
            // SHP Docs: A part is a connected sequence of two or more points. (page 7)
            var partsBuilder = new ShpMultiPartBuilder(1, 2);
            partsBuilder.ReadParts(shapeBinary, HasZ, HasM, CreateCoordinateSequence);

            var lines = new LineString[partsBuilder.Count];
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = Factory.CreateLineString(partsBuilder[i]);
            }
            return Factory.CreateMultiLineString(lines);
        }
    }
}
