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


    /// <summary>
    /// Specifies the geometry building algorithm to use.
    /// </summary>
    /// <seealso href="https://gis.stackexchange.com/a/147971/26684">Rings order explained.</seealso>
    public enum GeometryBuilderMode
    {
        /// <summary>
        /// Shape geometry will be read according to ESRI Shapefile specification.
        /// For example polygons will be created based on Shapefile's ring orientation semantics.
        /// <list type="table">
        /// <listheader><term>Ring type</term><description>Orientation</description></listheader>
        /// <item><term>Shell</term><description>Clockwise, Shapefile's left-hand-rule</description></item>
        /// <item><term>Hole</term><description>Counter-Clockwise, Shapefile's right-hand-rule</description></item>
        /// </list>
        /// Shape geometries that do not follow ESRI Shapefile specification will trhow an ShapefileException.
        /// </summary>
        Strict,

        /// <summary>
        /// Shape geometry will be read assuming that it does conform to ESRI Shapefile specification.
        /// All errors will be ignored wich will result in geometries having <see cref="Geometry.IsValid"/> property set to <c>false</c>.
        /// </summary>
        IngoreFailures,

        /// <summary>
        /// Shape geometry that does not conform to ESRI Shapefile specification will be skipped.
        /// The reader will omit the geometry and related attributes as if they didn't exist.
        /// </summary>
        SkipFailures,

        /// <summary>
        /// Invalid shape geometry will will be tried to be fixed using all possible methods.
        /// For polygons following logic will be applied:
        /// <list type="number">
        /// <item>Sort all rings.</item>
        /// <item>Consider all rings as a potential shell, search the valid holes for any possible shell.</item>
        /// <item>Check if the ring is inside any shell: if <c>true</c>, it can be considered a potential hole for the shell.</item>
        /// <item>Check if the ring is inside any hole of the shell: if <c>true</c>, this means that is actually a shell of a distinct geometry,
        /// and NOT a valid hole for the shell. A hole inside another hole is not allowed.</item>
        /// </list>
        /// </summary>
        /// <remarks>
        /// This option is considerably slower, especially for complex geometries
        /// (i.e.: polygons with a large number of holes having large numer of vertices).
        /// </remarks>
        FixFailures,

        /// <summary>
        /// Invalid shape geometry will will be tried to be fixed using only basic methods.
        /// For polygons following logic will be applied:
        /// <list type="number">
        /// <item>Do not sorting rings but assume that polygons are serialized in the following order: <c>Shell[, Holes][, Shell[, Holes][, ...]]</c>.</item>
        /// <item>Assume that the first ring that is not contained by the current polygon is the start of a new polygon.</item>
        /// </list>
        /// </summary>
        /// <remarks>
        /// This option is is faster than <see cref="FixFailures"/> but can result in geometries having <see cref="Geometry.IsValid"/> property set to <c>false</c>.
        /// </remarks>
        FixBasicFailures,
    }
}
