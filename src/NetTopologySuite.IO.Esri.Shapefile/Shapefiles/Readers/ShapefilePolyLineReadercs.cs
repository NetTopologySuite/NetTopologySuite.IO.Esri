using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Shp.Readers;
using System.IO;
using System.Text;

namespace NetTopologySuite.IO.Esri.Shapefiles.Readers
{

    /// <summary>
    /// PolyLine shapefile reader.
    /// </summary>
    public class ShapefilePolyLineReader : ShapefileReader<MultiLineString>
    {
        /// <inheritdoc/>
        public ShapefilePolyLineReader(Stream shpStream, Stream dbfStream, GeometryFactory factory, Encoding encoding = null)
            : base(shpStream, dbfStream, factory, encoding)
        { }

        /// <inheritdoc/>
        public ShapefilePolyLineReader(string shpPath, GeometryFactory factory, Encoding encoding = null)
            : base(shpPath, factory, encoding)
        { }

        internal override ShpReader<MultiLineString> CreateShpReader(Stream shpStream, GeometryFactory factory)
        {
            return new ShpPolyLineReader(shpStream, factory);
        }
    }

}
