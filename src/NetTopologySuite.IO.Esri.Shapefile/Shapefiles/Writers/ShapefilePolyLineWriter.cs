using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Shp.Writers;
using System.IO;

namespace NetTopologySuite.IO.Esri.Shapefiles.Writers
{

    /// <summary>
    /// PolyLine shapefile writer.
    /// </summary>
    public class ShapefilePolyLineWriter : ShapefileWriter<MultiLineString>
    {
        /// <inheritdoc/>
        public ShapefilePolyLineWriter(Stream shpStream, Stream shxStream, Stream dbfStream, Stream prjStream, ShapefileWriterOptions options)
            : base(shpStream, shxStream, dbfStream, prjStream, options)
        { }

        /// <inheritdoc/>
        public ShapefilePolyLineWriter(string shpPath, ShapefileWriterOptions options)
            : base(shpPath, options)
        { }

        internal override ShpWriter<MultiLineString> CreateShpWriter(Stream shpStream, Stream shxStream)
        {
            return new ShpPolyLineWriter(shpStream, shxStream, ShapeType);
        }
    }
}
