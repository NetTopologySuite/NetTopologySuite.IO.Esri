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
        /// <param name="factory">Geometry factory.</param>
        /// <param name="mbrFilter">The minimum bounding rectangle (BMR) used to filter out shapes located outside it.</param>
        /// <returns>SHP reader.</returns>
        public static ShpReader OpenRead(Stream shpStream, GeometryFactory factory = null, Envelope mbrFilter = null)
        {
            var shapeType = Shapefile.GetShapeType(shpStream);

            if (shapeType.IsPoint())
            {
                return new ShpPointReader(shpStream, factory, mbrFilter);
            }
            else if (shapeType.IsMultiPoint())
            {
                return new ShpMultiPointReader(shpStream, factory, mbrFilter);
            }
            else if (shapeType.IsPolyLine())
            {
                return new ShpPolyLineReader(shpStream, factory, mbrFilter);
            }
            else if (shapeType.IsPolygon())
            {
                return new ShpPolygonReader(shpStream, factory, mbrFilter);
            }
            else
            {
                throw new FileLoadException("Unsupported shapefile type: " + shapeType);
            }
        }

        #endregion
    }
}
