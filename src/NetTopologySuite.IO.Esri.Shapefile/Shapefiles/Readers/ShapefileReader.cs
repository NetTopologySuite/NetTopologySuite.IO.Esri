using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Dbf;
using NetTopologySuite.IO.Esri.Dbf.Fields;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NetTopologySuite.IO.Esri.Shapefiles.Readers
{


    /// <summary>
    /// Base class for reading a shapefile.
    /// </summary>
    public abstract class ShapefileReader : Shapefile, IEnumerable<Feature>
    {

        private protected readonly DbfReader DbfReader;

        /// <summary>
        /// Encoding used by the shapefile.
        /// </summary>
        public Encoding Encoding => DbfReader.Encoding;

        /// <summary>
        /// Well-known text representation of coordinate reference system metadata from .prj file.
        /// </summary>
        /// <remarks>
        /// <a href="https://support.esri.com/en/technical-article/000001897">https://support.esri.com/en/technical-article/000001897</a>
        /// </remarks>/>
        public abstract string Projection { get; }

        /// <summary>
        /// The Bounding Box stored in the SHP file header representing the actual extent of the shapes in the file.
        /// If the shapefile is empty (that is, has no records), the Bounding Box values are unspecified.
        /// </summary>
        public abstract Envelope BoundingBox { get; }

        /// <summary>
        /// Shape geometry.
        /// </summary>
        public abstract Geometry Geometry { get; }

        /// <summary>
        /// Shapefile attribute definitions with current attribute values.
        /// </summary>
        public DbfFieldCollection Fields => DbfReader.Fields;

        /// <summary>
        /// Record count.
        /// </summary>
        public int RecordCount => DbfReader.RecordCount;


        /// <summary>
        /// Initializes a new instance of the reader class.
        /// </summary>
        /// <param name="dbfReader">DBF file stream reader.</param>
        public ShapefileReader(DbfReader dbfReader)
        {
            DbfReader = dbfReader ?? throw new ArgumentNullException(nameof(dbfReader));
            AddManagedResource(DbfReader);
        }


        /// <summary>
        /// Moves reader back to its initial position. 
        /// </summary>
        public abstract void Restart();

        /// <summary>
        /// Reads feature geometry and attributes from underlying SHP and DBF files. 
        /// </summary>
        /// <param name="deleted">Indicates if the record was marked as deleted.</param>
        /// <param name="feature">Shapefile feature.</param>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next record;
        /// false if the enumerator has passed the end of the table.
        /// </returns>
        public abstract bool Read(out bool deleted, out Feature feature);

        /// <summary>
        /// Reads feature geometry and attributes from underlying SHP and DBF files into <see cref="Geometry"/> and <see cref="Fields"/> properties. 
        /// </summary>
        /// <param name="deleted">Indicates if the record was marked as deleted.</param>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next record;
        /// false if the enumerator has passed the end of the table.
        /// </returns>
        public abstract bool Read(out bool deleted);

        /// <summary>
        /// Reads feature geometry and attributes from underlying SHP and DBF files into <see cref="Geometry"/> and <see cref="Fields"/> properties. 
        /// </summary>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next record;
        /// false if the enumerator has passed the end of the table.
        /// </returns>
        public bool Read()
        {
            var read = Read(out var deleted);
            if (read && deleted)
            {
                return Read();
            }
            return read;
        }

        #region *** Enumerator ***

        IEnumerator<Feature> IEnumerable<Feature>.GetEnumerator()
        {
            return new FeatureEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new FeatureEnumerator(this);
        }

        private class FeatureEnumerator : IEnumerator<Feature>
        {
            private readonly ShapefileReader Owner;
            public Feature Current { get; private set; }
            object IEnumerator.Current => Current;

            public FeatureEnumerator(ShapefileReader owner)
            {
                Owner = owner;
            }

            public void Reset()
            {
                Owner.Restart();
            }

            public bool MoveNext()
            {
                if (!Owner.Read(out var deleted, out var feature))
                {
                    return false;
                }

                if (deleted)
                {
                    return MoveNext();
                }

                Current = feature;
                return true;
            }

            public void Dispose()
            {
                // Nothing to dispose
            }
        }

        #endregion
    }



}
