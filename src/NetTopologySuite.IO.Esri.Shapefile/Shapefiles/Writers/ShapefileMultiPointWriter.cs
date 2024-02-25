using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Shp.Writers;
using System.IO;

namespace NetTopologySuite.IO.Esri.Shapefiles.Writers
{

    /// <summary>
    /// MultiPoint shapefile writer.
    /// </summary>
    public class ShapefileMultiPointWriter : ShapefileWriter<MultiPoint>
    {
        /// <inheritdoc/>
        public ShapefileMultiPointWriter(Stream shpStream, Stream shxStream, Stream dbfStream, Stream prjStream, ShapefileWriterOptions options)
            : base(shpStream, shxStream, dbfStream, prjStream, options)
        { }

        /// <inheritdoc/>
        public ShapefileMultiPointWriter(string shpPath, ShapefileWriterOptions options)
            : base(shpPath, options)
        { }


        internal override ShpWriter<MultiPoint> CreateShpWriter(Stream shpStream, Stream shxStream)
        {
            return new ShpMultiPointWriter(shpStream, shxStream, ShapeType);
        }
    }
}
