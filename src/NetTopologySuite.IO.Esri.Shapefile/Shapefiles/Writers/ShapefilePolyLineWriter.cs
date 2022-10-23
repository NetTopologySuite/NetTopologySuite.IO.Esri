using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Dbf.Fields;
using NetTopologySuite.IO.Esri.Shp.Writers;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetTopologySuite.IO.Esri.Shapefiles.Writers
{

    /// <summary>
    /// PolyLine shapefile writer.
    /// </summary>
    public class ShapefilePolyLineWriter : ShapefileWriter<MultiLineString>
    {
        /// <inheritdoc/>
        public ShapefilePolyLineWriter(Stream shpStream, Stream shxStream, Stream dbfStream, ShapefileWriterOptions options)
            : base(shpStream, shxStream, dbfStream, options)
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
