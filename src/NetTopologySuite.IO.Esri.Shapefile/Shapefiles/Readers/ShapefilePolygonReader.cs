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
        public ShapefilePolygonReader(Stream shpStream, Stream dbfStream, ShapefileReaderOptions options = null)
            : base(shpStream, dbfStream, options)
        { }

        /// <inheritdoc/>
        public ShapefilePolygonReader(string shpPath, ShapefileReaderOptions options = null)
            : base(shpPath, options)
        { }

        internal override ShpReader<MultiPolygon> CreateShpReader(Stream shpStream, ShapefileReaderOptions options)
        {
            return new ShpPolygonReader(shpStream, options);
        }
    }

}
