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
        public ShapefilePolyLineReader(Stream shpStream, Stream dbfStream, GeometryFactory factory, Encoding encoding = null, Envelope mbrFilter = null)
            : base(shpStream, dbfStream, factory, encoding, mbrFilter)
        { }

        /// <inheritdoc/>
        public ShapefilePolyLineReader(string shpPath, GeometryFactory factory, Encoding encoding = null, Envelope mbrFilter = null)
            : base(shpPath, factory, encoding, mbrFilter)
        { }

        internal override ShpReader<MultiLineString> CreateShpReader(Stream shpStream, GeometryFactory factory, Envelope mbrFilter, int dbfRecordCount)
        {
            return new ShpPolyLineReader(shpStream, factory, mbrFilter, dbfRecordCount);
        }
    }

}
