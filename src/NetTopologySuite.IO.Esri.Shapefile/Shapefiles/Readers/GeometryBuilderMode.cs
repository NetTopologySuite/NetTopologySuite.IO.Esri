using System;
using System.Collections.Generic;
using System.Text;
using NetTopologySuite.Geometries;

namespace NetTopologySuite.IO.Esri.Shapefiles.Readers
{

    /// <summary>
    /// Specifies the geometry building algorithm to use.
    /// </summary>
    /// <seealso href="https://gis.stackexchange.com/a/147971/26684">Rings order explained.</seealso>
    public enum GeometryBuilderMode
    {
        /// <summary>
        /// Shape geometry will be read assuming that it does conform to ESRI Shapefile specification.
        /// For example polygons will be created based on Shapefile's ring orientation semantics.
        /// <list type="table">
        /// <listheader><term>Ring type</term><description>Orientation</description></listheader>
        /// <item><term>Shell</term><description>Clockwise, Shapefile's left-hand-rule</description></item>
        /// <item><term>Hole</term><description>Counter-Clockwise, Shapefile's right-hand-rule</description></item>
        /// </list>
        /// In case of an invalid shape an error will be thrown.
        /// </summary>
        Strict,

        /// <summary>
        /// Shape geometry will be read assuming that it does conform to ESRI Shapefile specification.
        /// In case of an invalid shape generated geometry will be empty or will have <see cref="Geometry.IsValid"/> property set to <c>false</c>.
        /// </summary>
        IgnoreInvalidShapes,

        /// <summary>
        /// Shape geometry that does not conform to ESRI Shapefile specification will be skipped.
        /// The reader will omit the geometry and related attributes as if they didn't exist.
        /// This will result in proceding the enumeration until the end of the file is reached
        /// even if some features are corrupted (so possibly valid shapes are not ignored).
        /// </summary>
        SkipInvalidShapes,

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
        FixInvalidShapes,

        /// <summary>
        /// Invalid shape geometry will will be tried to be fixed using only basic methods.
        /// For polygons following logic will be applied:
        /// <list type="number">
        /// <item>Do not sort rings but assume that polygons are serialized in the following order: <c>Shell[, Holes][, Shell[, Holes][, ...]]</c>.</item>
        /// <item>Assume that the first ring that is not contained by the current polygon is the start of a new polygon.</item>
        /// </list>
        /// </summary>
        /// <remarks>
        /// This option is is faster than <see cref="FixInvalidShapes"/> but can result in geometries having <see cref="Geometry.IsValid"/> property set to <c>false</c>.
        /// </remarks>
        QuickFixInvalidShapes,
    }
}
