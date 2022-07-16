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
        public ShapefileMultiPointReader(Stream shpStream, Stream dbfStream, ShapefileReaderOptions options)
            : base(shpStream, dbfStream, options)
        { }


        /// <inheritdoc/>
        public ShapefileMultiPointReader(string shpPath, ShapefileReaderOptions options)
            : base(shpPath, options)
        { }


        internal override ShpReader<MultiPoint> CreateShpReader(Stream shpStream, ShapefileReaderOptions options)
        {
            return new ShpMultiPointReader(shpStream, options);
        }
    }


}
