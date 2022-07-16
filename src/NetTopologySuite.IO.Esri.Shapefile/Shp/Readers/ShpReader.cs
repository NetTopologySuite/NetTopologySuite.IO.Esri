using NetTopologySuite.Geometries;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NetTopologySuite.IO.Esri.Shp.Readers
{
    /// <summary>
    /// Base class class for reading a fixed-length file header and variable-length records from a *.SHP file. 
    /// </summary>
    public abstract class ShpReader : Shp, IEnumerable<Geometry>
    {
        /// <inheritdoc/>
        internal ShpReader(ShapeType shapeType) : base(shapeType)
        {
        }

        /// <summary>
        /// The Bounding Box stored in the SHP file header representing the actual extent of the shapes in the file.
        /// If the shapefile is empty (that is, has no records), the Bounding Box values are unspecified.
        /// </summary>
        public abstract Envelope BoundingBox { get; }

        /// <summary>
        /// Shape geometry.
        /// </summary>
        public abstract Geometry Geometry { get; }

        internal abstract void Restart();

        /// <summary>
        /// Reads content of the <see cref="Geometry"/> from the underlying stream.
        /// </summary>
        /// <returns>Value indicating if reading next record was successful.</returns>
        public abstract bool Read();

        #region IEnumerable

        IEnumerator<Geometry> IEnumerable<Geometry>.GetEnumerator()
        {
            Restart();
            while (Read())
            {
                yield return Geometry;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            Restart();
            while (Read())
            {
                yield return Geometry;
            }
        }

        #endregion
    }
}
