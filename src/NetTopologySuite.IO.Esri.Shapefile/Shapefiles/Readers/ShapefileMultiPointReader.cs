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
        public ShapefileMultiPointReader(Stream shpStream, Stream dbfStream, GeometryFactory factory, Encoding encoding = null)
            : base(shpStream, dbfStream, factory, encoding)
        { }


        /// <inheritdoc/>
        public ShapefileMultiPointReader(string shpPath, GeometryFactory factory, Encoding encoding = null)
            : base(shpPath, factory, encoding)
        { }


        internal override ShpReader<MultiPoint> CreateShpReader(Stream shpStream, GeometryFactory factory)
        {
            return new ShpMultiPointReader(shpStream, factory);
        }
    }


}
