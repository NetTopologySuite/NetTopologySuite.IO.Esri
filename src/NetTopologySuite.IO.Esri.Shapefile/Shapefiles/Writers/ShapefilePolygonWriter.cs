using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Shp.Writers;
using System.IO;

namespace NetTopologySuite.IO.Esri.Shapefiles.Writers
{

    /// <summary>
    /// Polygon shapefile writer.
    /// </summary>
    public class ShapefilePolygonWriter : ShapefileWriter<MultiPolygon>
    {
        /// <inheritdoc/>
        public ShapefilePolygonWriter(Stream shpStream, Stream shxStream, Stream dbfStream, Stream prjStream, ShapefileWriterOptions options)
            : base(shpStream, shxStream, dbfStream, prjStream, options)
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
