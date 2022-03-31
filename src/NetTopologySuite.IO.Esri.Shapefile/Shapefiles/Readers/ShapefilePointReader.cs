using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Shp.Readers;
using System.IO;
using System.Text;

namespace NetTopologySuite.IO.Esri.Shapefiles.Readers
{


    /// <summary>
    /// Point shapefile reader.
    /// </summary>
    public class ShapefilePointReader : ShapefileReader<Point>
    {
        /// <inheritdoc/>
        public ShapefilePointReader(Stream shpStream, Stream dbfStream, GeometryFactory factory, Encoding encoding = null)
            : base(shpStream, dbfStream, factory, encoding)
        { }

        /// <inheritdoc/>
        public ShapefilePointReader(string shpPath, GeometryFactory factory = null, Encoding encoding = null)
            : base(shpPath, factory, encoding)
        { }


        internal override ShpReader<Point> CreateShpReader(Stream shpStream, GeometryFactory factory)
        {
            return new ShpPointReader(shpStream, factory);
        }
    }


}
