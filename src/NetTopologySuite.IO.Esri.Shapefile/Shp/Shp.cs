using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Shp.Readers;
using System.IO;
using System.Linq;

namespace NetTopologySuite.IO.Esri.Shp
{
    /// <summary>
    /// Base class class for reading and writing a fixed-length file header and variable-length records from a *.SHP file.
    /// </summary>
    public abstract class Shp : ManagedDisposable
    {
        internal readonly bool HasM;
        internal readonly bool HasZ;

        /// <summary>
        /// Shape type.
        /// </summary>
        public ShapeType ShapeType { get; }


        /// <summary>
        /// Initializes a new instance of the reader class.
        /// </summary>
        /// <param name="shapeType">Shape type.</param>
        public Shp(ShapeType shapeType)
        {
            ShapeType = shapeType;
            if (ShapeType == ShapeType.NullShape)
            {
                ThrowUnsupportedShapeTypeException();
            }

            HasM = shapeType.HasM();
            HasZ = shapeType.HasZ();
        }

        internal void ThrowUnsupportedShapeTypeException()
        {
            throw new FileLoadException(GetType().Name + $" does not support {ShapeType} shapes.");
        }

        #region Static Methods

        /// <summary>
        /// Opens SHP reader.
        /// </summary>
        /// <param name="shpStream">SHP file stream.</param>
        /// <param name="options">Reader options.</param>
        /// <returns>SHP reader.</returns>
        public static ShpReader OpenRead(Stream shpStream, ShapefileReaderOptions options = null)
        {
            var shapeType = Shapefile.GetShapeType(shpStream);

            if (shapeType.IsPoint())
            {
                return new ShpPointReader(shpStream, options);
            }
            else if (shapeType.IsMultiPoint())
            {
                return new ShpMultiPointReader(shpStream, options);
            }
            else if (shapeType.IsPolyLine())
            {
                return new ShpPolyLineReader(shpStream, options);
            }
            else if (shapeType.IsPolygon())
            {
                return new ShpPolygonReader(shpStream, options);
            }
            else
            {
                throw new FileLoadException("Unsupported shapefile type: " + shapeType);
            }
        }

        #endregion
    }
}
