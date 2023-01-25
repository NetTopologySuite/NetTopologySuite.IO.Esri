using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Dbf;
using NetTopologySuite.IO.Esri.Dbf.Fields;
using System;
using System.Collections.Generic;

namespace NetTopologySuite.IO.Esri.Shapefiles.Writers
{

    /// <summary>
    /// Base class for writing a shapefile.
    /// </summary>
    public abstract class ShapefileWriter : Shapefile
    {
        internal readonly DbfWriter DbfWriter;

        /// <inheritdoc/>
        public DbfFieldCollection Fields => DbfWriter.Fields;

        /// <summary>
        /// Shape geometry.
        /// </summary>
        public abstract Geometry Geometry { get; set; }


        /// <summary>
        /// Initializes a new instance of the writer class.
        /// </summary>
        /// <param name="dbfWriter">DBF file stream writer.</param>
        public ShapefileWriter(DbfWriter dbfWriter)
        {
            DbfWriter = dbfWriter ?? throw new ArgumentNullException(nameof(dbfWriter));
        }


        /// <summary>
        /// Wrties geometry and attributes into underlying SHP and DBF files.
        /// Attribute values must be set using Value property of DbfFiled(s) provided during initialization.
        /// </summary>
        public abstract void Write();


        /// <summary>
        /// Writes feature to the shapefile.
        /// </summary>
        public abstract void Write(IFeature feature);

        /// <summary>
        /// Writes features to the shapefile.
        /// </summary>
        public void Write(IEnumerable<IFeature> features)
        {
            foreach (var feature in features)
            {
                Write(feature);
            }
        }

        /// <inheritdoc/>
        protected override void DisposeManagedResources()
        {
            DbfWriter?.Dispose();
            base.DisposeManagedResources(); // This will dispose streams used by DbfReader. Do it at the end. We need to store DBF header first.
        }
    }


}
