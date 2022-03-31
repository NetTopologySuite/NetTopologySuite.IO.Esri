using NetTopologySuite.Geometries;
using System.IO;

namespace NetTopologySuite.IO.Esri.Shp.Writers
{

    /// <summary>
    /// Point SHP file writer. 
    /// </summary>
    public class ShpPointWriter : ShpWriter<Point>
    {
        /// <inheritdoc/>
        public ShpPointWriter(Stream shpStream, Stream shxStream, ShapeType type) : base(shpStream, shxStream, type)
        {
            if (!ShapeType.IsPoint())
                ThrowUnsupportedShapeTypeException();
        }

        internal override void WriteGeometry(Point point, Stream shapeBinary)
        {
            shapeBinary.WritePoint(point.CoordinateSequence);
            Extent.Expand(point.CoordinateSequence);
        }
    }


}
