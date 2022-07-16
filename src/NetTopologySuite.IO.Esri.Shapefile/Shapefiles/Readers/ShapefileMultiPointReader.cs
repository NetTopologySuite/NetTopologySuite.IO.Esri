using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Shp.Readers;
using System.IO;
using System.Text;

namespace NetTopologySuite.IO.Esri.Shapefiles.Readers
{

    /// <summary>
    /// MultiPoint shapefile reader.
    /// </summary>
    public class ShapefileMultiPointReader : ShapefileReader<MultiPoint>
    {

        /// <inheritdoc/>
        public ShapefileMultiPointReader(Stream shpStream, Stream dbfStream, GeometryFactory factory, Encoding encoding = null, Envelope mbrFilter = null)
            : base(shpStream, dbfStream, factory, encoding, mbrFilter)
        { }


        /// <inheritdoc/>
        public ShapefileMultiPointReader(string shpPath, GeometryFactory factory, Encoding encoding = null, Envelope mbrFilter = null)
            : base(shpPath, factory, encoding, mbrFilter)
        { }


        internal override ShpReader<MultiPoint> CreateShpReader(Stream shpStream, GeometryFactory factory, Envelope mbrFilter, int dbfRecordCount)
        {
            return new ShpMultiPointReader(shpStream, factory, mbrFilter, dbfRecordCount);
        }
    }


}
