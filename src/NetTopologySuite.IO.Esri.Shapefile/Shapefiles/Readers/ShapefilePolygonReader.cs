using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Shp.Readers;
using System.IO;
using System.Text;

namespace NetTopologySuite.IO.Esri.Shapefiles.Readers
{

    /// <summary>
    /// Polygon shapefile reader.
    /// </summary>
    public class ShapefilePolygonReader : ShapefileReader<MultiPolygon>
    {
        /// <inheritdoc/>
        public ShapefilePolygonReader(Stream shpStream, Stream dbfStream, GeometryFactory factory, Encoding encoding = null)
            : base(shpStream, dbfStream, factory, encoding)
        { }

        /// <inheritdoc/>
        public ShapefilePolygonReader(string shpPath, GeometryFactory factory, Encoding encoding = null)
            : base(shpPath, factory, encoding)
        { }

        internal override ShpReader<MultiPolygon> CreateShpReader(Stream shpStream, GeometryFactory factory)
        {
            return new ShpPolygonReader(shpStream, factory);
        }
    }

}
