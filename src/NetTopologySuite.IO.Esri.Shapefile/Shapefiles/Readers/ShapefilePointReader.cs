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
        public ShapefilePointReader(Stream shpStream, Stream dbfStream, GeometryFactory factory, Encoding encoding = null, Envelope mbrFilter = null)
            : base(shpStream, dbfStream, factory, encoding, mbrFilter)
        { }

        /// <inheritdoc/>
        public ShapefilePointReader(string shpPath, GeometryFactory factory = null, Encoding encoding = null, Envelope mbrFilter = null)
            : base(shpPath, factory, encoding, mbrFilter)
        { }


        internal override ShpReader<Point> CreateShpReader(Stream shpStream, GeometryFactory factory, Envelope mbrFilter, int dbfRecordCount)
        {
            return new ShpPointReader(shpStream, factory, mbrFilter, dbfRecordCount);
        }
    }


}
