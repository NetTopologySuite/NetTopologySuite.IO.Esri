using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetTopologySuite.IO.Esri
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
        /// The minimum bounding rectangle (BMR) used to filter out shapes located outside it.
        /// </summary>
        public Envelope MbrFilter { get; set; } = null;


        /// <summary>
        ///  Minimum bounding rectangle (MBR) filtering options.
        /// </summary>
        public MbrFilterOption MbrFilterOption { get; set; } = MbrFilterOption.FilterByExtent;

        internal int DbfRecordCount { get; set; } = int.MaxValue;

        /// <summary>
        /// Set it to true if you want to proceed the enumeration until the end of the file is reached
        /// even if some features are corrupted (so possibly valid shapes are not ignored).
        /// </summary>
        /// <remarks>
        /// https://github.com/NetTopologySuite/NetTopologySuite.IO.ShapeFile/issues/46
        /// </remarks>
        public bool SkipFailures { get; set; } = false;

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
