using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Dbf.Fields;
using NetTopologySuite.IO.Esri.Shp.Writers;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetTopologySuite.IO.Esri.Shapefiles.Writers
{

    /// <summary>
    /// Polygon shapefile writer.
    /// </summary>
    public class ShapefilePolygonWriter : ShapefileWriter<MultiPolygon>
    {
        /// <inheritdoc/>
        public ShapefilePolygonWriter(Stream shpStream, Stream shxStream, Stream dbfStream, ShapefileWriterOptions options)
            : base(shpStream, shxStream, dbfStream, options)
        { }

        /// <inheritdoc/>
        public ShapefilePolygonWriter(string shpPath, ShapefileWriterOptions options)
            : base(shpPath, options)
        { }

        internal override ShpWriter<MultiPolygon> CreateShpWriter(Stream shpStream, Stream shxStream)
        {
            return new ShpPolygonWriter(shpStream, shxStream, ShapeType);
        }
    }
}
