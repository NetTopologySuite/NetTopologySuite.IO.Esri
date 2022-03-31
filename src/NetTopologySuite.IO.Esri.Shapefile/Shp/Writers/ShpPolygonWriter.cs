using NetTopologySuite.Geometries;
using System.IO;

namespace NetTopologySuite.IO.Esri.Shp.Writers
{

    /// <summary>
    /// Polygon SHP file writer. 
    /// </summary>
    public class ShpPolygonWriter : ShpWriter<MultiPolygon>
    {
        /// <inheritdoc/>
        public ShpPolygonWriter(Stream shpStream, Stream shxStream, ShapeType type) : base(shpStream, shxStream, type)
        {
            if (!ShapeType.IsPolygon())
                ThrowUnsupportedShapeTypeException();
        }

        internal override void WriteGeometry(MultiPolygon multiPolygon, Stream stream)
        {
            // SHP Docs: A ring is a connected sequence of four or more points (page 8)
            var partsBuilder = new ShpMultiPartBuilder(multiPolygon.Count, 4);
            for (int i = 0; i < multiPolygon.Count; i++)
            {
                var pg = (Polygon)multiPolygon[i];

                // SHP Spec: Vertices for a single polygon are always in clockwise order.
                var shellCoordinates = pg.Shell.IsCCW ? pg.Shell.CoordinateSequence.Reversed() : pg.Shell.CoordinateSequence;
                partsBuilder.AddPart(shellCoordinates);

                foreach (var hole in pg.Holes)
                {
                    // SHP Spec: Vertices of rings defining holes in polygons are in a counterclockwise direction.
                    // https://gis.stackexchange.com/a/147971/26684
                    var holeCoordinates = hole.IsCCW ? hole.CoordinateSequence : hole.CoordinateSequence.Reversed();
                    partsBuilder.AddPart(holeCoordinates);
                }
            }

            partsBuilder.WriteParts(stream, HasZ, HasM);
            partsBuilder.UpdateExtent(Extent);
        }
    }


}
