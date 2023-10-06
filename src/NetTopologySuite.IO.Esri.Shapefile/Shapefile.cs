using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Shapefiles.Readers;
using NetTopologySuite.IO.Esri.Shapefiles.Writers;
using NetTopologySuite.IO.Esri.Shp;

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
                throw new ShapefileException("Invalid shapefile format.");

            shpStream.Advance(28);
            return shpStream.ReadShapeType();
        }


        /// <summary>
        /// Gets default <see cref="ShapeType"/> for specified geometry.
        /// </summary>
        /// <param name="geometry">A Geometry object.</param>
        /// <returns>Shape type.</returns>
        public static ShapeType GetShapeType(Geometry geometry)
        {
            return geometry.GetShapeType();
        }

        /// <summary>
        /// Reads shape type information from SHP file.
        /// </summary>
        /// <param name="shpPath">Path to SHP file.</param>
        /// <returns>Shape type.</returns>
        public static ShapeType GetShapeType(string shpPath)
        {
            shpPath = Path.ChangeExtension(shpPath, ".shp");
            using (var shpStream = new FileStream(shpPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return GetShapeType(shpStream);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shpStream">shp stream.</param>
        /// <param name="dbfStream">dbf stream.</param>
        /// <param name="options">Reader options.</param>
        /// <returns></returns>
        /// <exception cref="ShapefileException"></exception>
        public static ShapefileReader OpenStreamRead(Stream shpStream, Stream dbfStream, ShapefileReaderOptions options = null)
        {
            var shapeType = GetShapeType(shpStream);

            if (shapeType.IsPoint())
            {
                return new ShapefilePointReader(shpStream, dbfStream, options);
            }
            else if (shapeType.IsMultiPoint())
            {
                return new ShapefileMultiPointReader(shpStream, dbfStream, options);
            }
            else if (shapeType.IsPolyLine())
            {
                return new ShapefilePolyLineReader(shpStream, dbfStream, options);
            }
            else if (shapeType.IsPolygon())
            {
                return new ShapefilePolygonReader(shpStream, dbfStream, options);
            }
            else
            {
                throw new ShapefileException("Unsupported shape file stream");
            }
        }

        /// <summary>
        /// Opens shapefile reader.
        /// </summary>
        /// <param name="shpPath">Path to shapefile.</param>
        /// <param name="options">Reader options.</param>
        /// <returns>Shapefile reader.</returns>
        public static ShapefileReader OpenRead(string shpPath, ShapefileReaderOptions options = null)
        {
            var shapeType = GetShapeType(shpPath);

            if (shapeType.IsPoint())
            {
                return new ShapefilePointReader(shpPath, options);
            }
            else if (shapeType.IsMultiPoint())
            {
                return new ShapefileMultiPointReader(shpPath, options);
            }
            else if (shapeType.IsPolyLine())
            {
                return new ShapefilePolyLineReader(shpPath, options);
            }
            else if (shapeType.IsPolygon())
            {
                return new ShapefilePolygonReader(shpPath, options);
            }
            else
            {
                throw new ShapefileException("Unsupported shapefile type: " + shapeType, shpPath);
            }
        }

        /// <summary>
        /// Reads all features from shapefile.
        /// </summary>
        /// <param name="shpStream">shp stream.</param>
        /// <param name="dbfStream">dbf stream.</param>
        /// <param name="options">Reader options.</param>
        /// <returns></returns>
        public static Feature[] ReadAllFeatures(Stream shpStream, Stream dbfStream, ShapefileReaderOptions options = null)
        {
            using (var shp = OpenStreamRead(shpStream, dbfStream, options))
            {
                return shp.ToArray();
            }
        }

        /// <summary>
        /// Reads all features from shapefile.
        /// </summary>
        /// <param name="shpPath">Path to shapefile.</param>
        /// <param name="options">Reader options.</param>
        /// <returns>Shapefile features.</returns>
        public static Feature[] ReadAllFeatures(string shpPath, ShapefileReaderOptions options = null)
        {
            using (var shp = OpenRead(shpPath, options))
            {
                return shp.ToArray();
            }
        }

        /// <summary>
        /// Reads all geometries from SHP file.
        /// </summary>
        /// <param name="shpPath">Path to SHP file.</param>
        /// <param name="options">Reader options.</param>
        /// <returns>Shapefile geometries.</returns>
        public static Geometry[] ReadAllGeometries(string shpPath, ShapefileReaderOptions options = null)
        {
            shpPath = Path.ChangeExtension(shpPath, ".shp");
            using (var shpStream = File.OpenRead(shpPath))
            {
                var shp = Shp.Shp.OpenRead(shpStream, options);
                return shp.ToArray();
            }
        }


        /// <summary>
        /// Opens shapefile writer.
        /// </summary>
        /// <param name="shpPath">Path to shapefile.</param>
        /// <param name="options">Writer options.</param>
        /// <returns>Shapefile writer.</returns>
        public static ShapefileWriter OpenWrite(string shpPath, ShapefileWriterOptions options)
        {
            options = options ?? throw new ArgumentNullException(nameof(options));
            if (options.ShapeType.IsPoint())
            {
                return new ShapefilePointWriter(shpPath, options);
            }
            else if (options.ShapeType.IsMultiPoint())
            {
                return new ShapefileMultiPointWriter(shpPath, options);
            }
            else if (options.ShapeType.IsPolyLine())
            {
                return new ShapefilePolyLineWriter(shpPath, options);
            }
            else if (options.ShapeType.IsPolygon())
            {
                return new ShapefilePolygonWriter(shpPath, options);
            }
            else
            {
                throw new ShapefileException("Unsupported shapefile type: " + options.ShapeType, shpPath);
            }
        }


        /// <summary>
        /// Writes features to the shapefile.
        /// </summary>
        /// <param name="features">Features to be written.</param>
        /// <param name="shpPath">Path to shapefile.</param>
        /// <param name="projection">Projection metadata for the shapefile (content of the PRJ file).</param>
        /// <param name="encoding">DBF file encoding (if not set UTF8 is used).</param>
        public static void WriteAllFeatures(IEnumerable<IFeature> features, string shpPath, string projection = null, Encoding encoding = null)
        {
            if (features == null)
                throw new ArgumentNullException(nameof(features));

            var firstFeature = features.FirstOrDefault();
            if (firstFeature == null)
                throw new ArgumentException(nameof(ShapefileWriter) + " requires at least one feature to be written.");

            var fields = firstFeature.Attributes.GetDbfFields();
            var shapeType = features.FindNonEmptyGeometry().GetShapeType();
            var options = new ShapefileWriterOptions(shapeType, fields)
            {
                Projection = projection,
                Encoding = encoding
            };

            using (var shpWriter = OpenWrite(shpPath, options))
            {
                shpWriter.Write(features);
            }
        }

        #endregion

    }

}
