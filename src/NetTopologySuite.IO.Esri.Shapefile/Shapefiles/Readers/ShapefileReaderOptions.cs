using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetTopologySuite.IO.Esri.Shapefiles.Readers
{
    /// <summary>
    ///  Shapefile reader options
    /// </summary>
    public class ShapefileReaderOptions
    {
        /// <summary>
        /// Geometry factory
        /// </summary>
        public GeometryFactory Factory { get; set; } = null;

        /// <summary>
        /// DBF file encoding. If null encoding will be guess from related .CPG file or from reserved DBF bytes
        /// </summary>
        public Encoding Encoding { get; set; } = null;

        /// <summary>
        /// Specifies the geometry building algorithm to use.
        /// </summary>
        public GeometryBuilderMode GeometryBuilderMode { get; set; } = GeometryBuilderMode.Strict;

        /// <summary>
        /// The minimum bounding rectangle (BMR) used to filter out shapes located outside it.
        /// </summary>
        public Envelope MbrFilter { get; set; } = null;


        /// <summary>
        ///  Minimum bounding rectangle (MBR) filtering options.
        /// </summary>
        public MbrFilterOption MbrFilterOption { get; set; } = MbrFilterOption.FilterByExtent;

        internal int DbfRecordCount { get; set; } = int.MaxValue;

        internal static ShapefileReaderOptions Default = new ShapefileReaderOptions();
    }


    /// <summary>
    ///  Minimum bounding rectangle (MBR) filtering options.
    /// </summary>
    public enum MbrFilterOption
    {
        /// <summary>
        /// Filter by geometry extent. This option is faster but gives less acurate results (it can get some shapes that are not actually in the MBR).
        /// </summary>
        FilterByExtent,

        /// <summary>
        /// Filter by geometry shape. This option gives precise results but it also greatly affect the performance.
        /// </summary>
        FilterByGeometry
    }
}
