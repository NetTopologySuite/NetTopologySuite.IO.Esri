using System.IO;

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
    }
}
