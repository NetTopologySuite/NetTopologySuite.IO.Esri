using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Dbf.Fields;
using NetTopologySuite.IO.Esri.Shp.Writers;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetTopologySuite.IO.Esri.Shapefiles.Writers
{

    /// <summary>
    /// Point shapefile writer.
    /// </summary>
    public class ShapefilePointWriter : ShapefileWriter<Point>
    {
        /// <inheritdoc/>
        public ShapefilePointWriter(Stream shpStream, Stream shxStream, Stream dbfStream, ShapefileWriterOptions options)
            : base(shpStream, shxStream, dbfStream, options)
        { }

        /// <inheritdoc/>
        public ShapefilePointWriter(string shpPath, ShapefileWriterOptions options)
            : base(shpPath, options)
        { }

        internal override ShpWriter<Point> CreateShpWriter(Stream shpStream, Stream shxStream)
        {
            return new ShpPointWriter(shpStream, shxStream, ShapeType);
        }

    }
}
