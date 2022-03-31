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
        public ShapefilePolyLineWriter(Stream shpStream, Stream shxStream, Stream dbfStream, ShapeType type, IReadOnlyList<DbfField> fields, Encoding encoding = null)
            : base(shpStream, shxStream, dbfStream, type, fields, encoding)
        { }

        /// <inheritdoc/>
        public ShapefilePolyLineWriter(string shpPath, ShapeType type, IReadOnlyList<DbfField> fields, Encoding encoding = null, string projection = null)
            : base(shpPath, type, fields, encoding, projection)
        { }

        /// <inheritdoc/>
        public ShapefilePolyLineWriter(string shpPath, ShapeType type, params DbfField[] fields)
            : base(shpPath, type, fields)
        {
        }

        internal override ShpWriter<MultiLineString> CreateShpWriter(Stream shpStream, Stream shxStream)
        {
            return new ShpPolyLineWriter(shpStream, shxStream, ShapeType);
        }
    }
}
