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
        protected ShpReader(ShapeType shapeType) : base(shapeType)
        {
        }

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
