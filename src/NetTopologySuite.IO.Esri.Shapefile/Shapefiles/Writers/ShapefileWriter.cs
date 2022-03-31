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
    }


}
