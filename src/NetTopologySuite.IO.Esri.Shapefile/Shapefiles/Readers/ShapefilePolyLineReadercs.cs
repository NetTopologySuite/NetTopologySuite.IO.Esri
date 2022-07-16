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
        public ShapefilePolyLineReader(Stream shpStream, Stream dbfStream, ShapefileReaderOptions options)
            : base(shpStream, dbfStream, options)
        { }

        /// <inheritdoc/>
        public ShapefilePolyLineReader(string shpPath, ShapefileReaderOptions options)
            : base(shpPath, options)
        { }

        internal override ShpReader<MultiLineString> CreateShpReader(Stream shpStream, ShapefileReaderOptions options)
        {
            return new ShpPolyLineReader(shpStream, options);
        }
    }

}
