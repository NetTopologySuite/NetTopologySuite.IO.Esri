using System;
using System.IO;
using System.Runtime.Serialization;

namespace NetTopologySuite.IO.Esri
{
    /// <summary>
    /// The exception that is thrown when a non-fatal application error occurs related to Shapefile functionality.
    /// </summary>
    public class ShapefileException : FileLoadException
    {
        /// <inheritdoc/>
        public ShapefileException() { }

        /// <inheritdoc/>
        public ShapefileException(string message) : base(message) { }

        /// <inheritdoc/>
        public ShapefileException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        /// <inheritdoc/>
        public ShapefileException(string message, Exception innerException) : base(message, innerException) { }


        /// <inheritdoc/>
        public ShapefileException(string message, string fileName) : base(message, fileName) { }

        /// <inheritdoc/>
        public ShapefileException(string message, string fileName, Exception inner) : base(message, fileName, inner) { }
    }
}
