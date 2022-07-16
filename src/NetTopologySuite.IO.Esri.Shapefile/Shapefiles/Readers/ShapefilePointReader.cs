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
        public ShapefilePointReader(Stream shpStream, Stream dbfStream, ShapefileReaderOptions options)
            : base(shpStream, dbfStream, options)
        { }

        /// <inheritdoc/>
        public ShapefilePointReader(string shpPath, ShapefileReaderOptions options)
            : base(shpPath, options)
        { }


        internal override ShpReader<Point> CreateShpReader(Stream shpStream, ShapefileReaderOptions options)
        {
            return new ShpPointReader(shpStream, options);
        }
    }


}
