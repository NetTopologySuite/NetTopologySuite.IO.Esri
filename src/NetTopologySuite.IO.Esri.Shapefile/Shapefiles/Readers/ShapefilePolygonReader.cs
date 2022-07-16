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
        public ShapefilePolygonReader(Stream shpStream, Stream dbfStream, GeometryFactory factory, Encoding encoding = null, Envelope mbrFilter = null)
            : base(shpStream, dbfStream, factory, encoding, mbrFilter)
        { }

        /// <inheritdoc/>
        public ShapefilePolygonReader(string shpPath, GeometryFactory factory, Encoding encoding = null, Envelope mbrFilter = null)
            : base(shpPath, factory, encoding, mbrFilter)
        { }

        internal override ShpReader<MultiPolygon> CreateShpReader(Stream shpStream, GeometryFactory factory, Envelope mbrFilter, int dbfRecordCount)
        {
            return new ShpPolygonReader(shpStream, factory, mbrFilter, dbfRecordCount);
        }
    }

}
