using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Dbf.Fields;
using NetTopologySuite.IO.Esri.Shapefiles.Readers;
using NetTopologySuite.IO.Esri.Shapefiles.Writers;
using NetTopologySuite.IO.Esri.Shp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NetTopologySuite.IO.Esri
{


    /// <summary>
    /// Base shapefile class.
    /// </summary>
    public abstract class Shapefile : ManagedDisposable
    {
        internal const int FileCode = 9994; // 0x0000270A; 
        internal const int Version = 1000;

        internal const int FileHeaderSize = 100;
        internal const int RecordHeaderSize = 2 * sizeof(int);


        /// <summary>
        /// Minimal Measure value considered as not "no-data".
        /// </summary>
        /// <remarks>
        /// Any floating point number smaller than –10E38 is considered by a shapefile reader
        /// to represent a "no data" value. This rule is used only for measures (M values).
        /// <br />
        /// http://www.esri.com/library/whitepapers/pdfs/shapefile.pdf (page 2, bottom)
        /// </remarks>
        internal const double MeasureMinValue = -10e38;

        /// <summary>
        /// Shape type.
        /// </summary>
        public abstract ShapeType ShapeType { get; }


        #region Static methods

        /// <summary>
        /// Reads shape type information from SHP stream.
        /// </summary>
        /// <param name="shpStream">SHP file stream.</param>
        /// <returns>Shape type.</returns>
        internal static ShapeType GetShapeType(Stream shpStream)
        {
            if (shpStream == null)
            {
                throw new ArgumentNullException("Uninitialized SHP stream.", nameof(shpStream));
            }

            shpStream.Position = 0;
            var fileCode = shpStream.ReadInt32BigEndian();
            if (fileCode != Shapefile.FileCode)
                throw new FileLoadException("Invalid shapefile format.");

            shpStream.Advance(28);
            return shpStream.ReadShapeType();
        }

        /// <summary>
        /// Reads shape type information from SHP file.
        /// </summary>
        /// <param name="shpPath">Path to SHP file.</param>
        /// <returns>Shape type.</returns>
        public static ShapeType GetShapeType(string shpPath)
        {
            if (Path.GetExtension(shpPath).ToLowerInvariant() != ".shp")
                throw new FileLoadException("Specified file must have .shp extension.");

            using (var shpStream = new FileStream(shpPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return GetShapeType(shpStream);
            }
        }

        /// <summary>
        /// Opens shapefile reader.
        /// </summary>
        /// <param name="shpPath">Path to shapefile.</param>
        /// <param name="factory">Geometry factory.</param>
        /// <param name="encoding">DBF file encoding. If null encoding will be guess from related .CPG file or from reserved DBF bytes.</param>
        /// <returns>Shapefile reader.</returns>
        public static ShapefileReader OpenRead(string shpPath, GeometryFactory factory = null, Encoding encoding = null)
        {
            var shapeType = GetShapeType(shpPath);

            if (shapeType.IsPoint())
            {
                return new ShapefilePointReader(shpPath, factory, encoding);
            }
            else if (shapeType.IsMultiPoint())
            {
                return new ShapefileMultiPointReader(shpPath, factory, encoding);
            }
            else if (shapeType.IsPolyLine())
            {
                return new ShapefilePolyLineReader(shpPath, factory, encoding);
            }
            else if (shapeType.IsPolygon())
            {
                return new ShapefilePolygonReader(shpPath, factory, encoding);
            }
            else
            {
                throw new FileLoadException("Unsupported shapefile type: " + shapeType, shpPath);
            }
        }

        /// <summary>
        /// Reads all features from shapefile.
        /// </summary>
        /// <param name="shpPath">Path to shapefile.</param>
        /// <param name="factory">Geometry factory.</param>
        /// <param name="encoding">DBF file encoding. If null encoding will be guess from related .CPG file or from reserved DBF bytes.</param>
        /// <returns>Shapefile features.</returns>
        public static Feature[] ReadAllFeatures(string shpPath, GeometryFactory factory = null, Encoding encoding = null)
        {
            using (var shp = OpenRead(shpPath, factory, encoding))
            {
                return shp.ToArray();
            }
        }


        /// <summary>
        /// Opens shapefile writer.
        /// </summary>
        /// <param name="shpPath">Path to shapefile.</param>
        /// <param name="type">Shape type.</param>
        /// <param name="fields">Shapefile fields definitions.</param>
        /// <param name="encoding">DBF file encoding. If null encoding will be guess from related .CPG file or from reserved DBF bytes.</param>
        /// <param name="projection">Projection metadata for the shapefile (.prj file).</param>
        /// <returns>Shapefile writer.</returns>
        public static ShapefileWriter OpenWrite(string shpPath, ShapeType type, IReadOnlyList<DbfField> fields, Encoding encoding = null, string projection = null)
        {
            if (type.IsPoint())
            {
                return new ShapefilePointWriter(shpPath, type, fields, encoding, projection);
            }
            else if (type.IsMultiPoint())
            {
                return new ShapefileMultiPointWriter(shpPath, type, fields, encoding, projection);
            }
            else if (type.IsPolyLine())
            {
                return new ShapefilePolyLineWriter(shpPath, type, fields, encoding, projection);
            }
            else if (type.IsPolygon())
            {
                return new ShapefilePolygonWriter(shpPath, type, fields, encoding, projection);
            }
            else
            {
                throw new FileLoadException("Unsupported shapefile type: " + type, shpPath);
            }
        }


        /// <summary>
        /// Writes features to the shapefile.
        /// </summary>
        /// <param name="features">Features to be written.</param>
        /// <param name="shpPath">Path to shapefile.</param>
        /// <param name="encoding">DBF file encoding. If null encoding will be guess from related .CPG file or from reserved DBF bytes.</param>
        /// <param name="projection">Projection metadata for the shapefile (.prj file).</param>
        public static void WriteAllFeatures(IEnumerable<IFeature> features, string shpPath, Encoding encoding = null, string projection = null)
        {
            if (features == null)
                throw new ArgumentNullException(nameof(features));

            var firstFeature = features.FirstOrDefault();
            if (firstFeature == null)
                throw new ArgumentException(nameof(ShapefileWriter) + " requires at least one feature to be written.");

            var fields = firstFeature.Attributes.GetDbfFields();
            var shapeType = features.FindNonEmptyGeometry().GetShapeType();

            using (var shpWriter = OpenWrite(shpPath, shapeType, fields, encoding, projection))
            {
                shpWriter.Write(features);
            }
        }

        #endregion

    }

}
