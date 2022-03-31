using System;

namespace NetTopologySuite.IO.Esri
{


    /// <summary>
    /// Shapefile geometry types.
    /// </summary>
    public enum ShapeType : int
    {

        /// <summary>
        /// Null Shape
        /// </summary>
        /// <remarks>
        /// Indicates a null shape, with no geometric data for the shape. Each feature type (point, line, polygon, etc.)
        /// supports nulls - it is valid to have points and null points in the same shapefile. Often null shapes are place
        /// holders - they are used during shapefile creation and are populated with geometric data soon after they are created.
        /// </remarks>
        NullShape = 0,

        /// <summary>
        /// Point in the X, Y space.
        /// </summary>
        Point = 1,

        /// <summary>
        /// PolyLine in the X, Y space.
        /// </summary>
        /// <remarks>
        /// A PolyLine is an ordered set of vertices that consists of one or more parts.
        /// A part is a connected sequence of two or more points.
        /// Parts may or may not be connected to one another. Parts may or may not intersect one another.
        /// </remarks>
        PolyLine = 3,

        /// <summary>
        /// Polygon in the X, Y space.
        /// </summary>
        /// <remarks>
        ///  A polygon consists of one or more rings. A ring is a connected sequence of four or more points that form a closed, non-self-intersecting loop.
        ///  A polygon may contain multiple outer rings. The order of vertices or orientation for a ring indicates which side of the ring
        ///  is the interior of the polygon. The neighborhood to the right of an observer walking along
        ///  the ring in vertex order is the neighborhood inside the polygon. Vertices of rings defining
        ///  holes in polygons are in a counterclockwise direction. Vertices for a single, ringed
        ///  polygon are, therefore, always in clockwise order. The rings of a polygon are referred to
        ///  as its parts.
        /// </remarks>
        Polygon = 5,

        /// <summary>
        /// MultiPoint in the X, Y space.
        /// </summary>
        /// <remarks>
        /// A MultiPoint represents a set of points.
        /// </remarks>
        MultiPoint = 8,




        /// <summary>
        /// Measured Point in the X, Y, Z space.
        /// </summary>
        /// <remarks>Named as <i>PointZ</i> in ESRI Shapefile Technical Description. </remarks>
        PointZM = 11,

        /// <summary>
        /// Measured PolyLine in the X, Y, Z space.
        /// </summary>
        /// <remarks>Named as <i>PolyLineZ</i> in ESRI Shapefile Technical Description. </remarks>
        PolyLineZM = 13,

        /// <summary>
        /// Measured Polygon in the X, Y, Z space.
        /// </summary>
        /// <remarks>Named as <i>PolygonZ</i> in ESRI Shapefile Technical Description. </remarks>
        PolygonZM = 15,

        /// <summary>
        /// Measured MultiPoint in the X, Y, Z space.
        /// </summary>
        /// <remarks>Named as <i>MultiPointZ</i> in ESRI Shapefile Technical Description. </remarks>
        MultiPointZM = 18,




        /// <summary>
        /// Measured Point in the X, Y space.
        /// </summary>
        PointM = 21,

        /// <summary>
        /// Measured LineString in the X, Y space.
        /// </summary>
        PolyLineM = 23,

        /// <summary>
        /// Measured Polygon in the X, Y space.
        /// </summary>
        PolygonM = 25,

        /// <summary>
        /// Measured MultiPoint in the X, Y space.
        /// </summary>
        MultiPointM = 28,

        /// <summary>
        /// MultiPatch
        /// </summary>
        [Obsolete("This shape type is not supported.")]
        MultiPatch = 31,



        // GeoTools

        /// <summary>
        /// Point in the X, Y, Z space.
        /// </summary>
        /// <remarks>
        /// This shape type is not conformant with ESRI Shapefile Technical Description. Use PointZM instead.
        /// </remarks>
        [Obsolete("This shape type is not conformant with ESRI Shapefile Technical Description. Use PointZM instead.")]
        PointZ = 9,

        /// <summary>
        /// PolyLine in the X, Y, Z space.
        /// </summary>
        /// <remarks>
        /// This shape type is not conformant with ESRI Shapefile Technical Description. Use PointZM instead.
        /// </remarks>
        [Obsolete("This shape type is not conformant with ESRI Shapefile Technical Description. Use PolyLineZM instead.")]
        PolyLineZ = 10,

        /// <summary>
        /// Polygon in the X, Y, Z space.
        /// </summary>
        /// <remarks>
        /// This shape type is not conformant with ESRI Shapefile Technical Description. Use PolygonZM instead.
        /// </remarks>
        [Obsolete("This shape type is not conformant with ESRI Shapefile Technical Description. Use PointZM instead.")]
        PolygonZ = 19,

        /// <summary>
        /// MultiPoint in the X, Y, Z space.
        /// </summary>
        /// <remarks>
        /// This shape type is not conformant with ESRI Shapefile Technical Description. Use MultiPointZM instead.
        /// </remarks>
        [Obsolete("This shape type is not conformant with ESRI Shapefile Technical Description. Use PointZM instead.")]
        MultiPointZ = 20,

    }



#pragma warning disable CS0618 // Type or member is obsolete

    /// <summary>
    /// Helper methods for ShapeType enumeration.
    /// </summary>
    public static class ShapeTypeExtensions
    {
        /// <summary>
        /// Indicates if the geometric data for the shape associated with this type contains Z coordinate.
        /// </summary>
        public static bool HasZ(this ShapeType type)
        {
            return type == ShapeType.PointZ
                || type == ShapeType.MultiPointZ
                || type == ShapeType.PolyLineZ
                || type == ShapeType.PolygonZ

                || type == ShapeType.PointZM
                || type == ShapeType.MultiPointZM
                || type == ShapeType.PolyLineZM
                || type == ShapeType.PolygonZM;
        }

        /// <summary>
        /// Indicates if the geometric data for the shape associated with this type contains Measure value.
        /// </summary>
        public static bool HasM(this ShapeType type)
        {
            return type == ShapeType.PointM
                || type == ShapeType.MultiPointM
                || type == ShapeType.PolyLineM
                || type == ShapeType.PolygonM

                || type == ShapeType.PointZM
                || type == ShapeType.MultiPointZM
                || type == ShapeType.PolyLineZM
                || type == ShapeType.PolygonZM;
        }


        /// <summary>
        /// Indicates if shape associated with this type is a Point.
        /// </summary>
        public static bool IsPoint(this ShapeType type)
        {
            return type == ShapeType.Point
                || type == ShapeType.PointM
                || type == ShapeType.PointZM
                || type == ShapeType.PointZ;
        }

        /// <summary>
        /// Indicates if shape associated with this type is a MultiPoint.
        /// </summary>
        public static bool IsMultiPoint(this ShapeType type)
        {
            return type == ShapeType.MultiPoint
                || type == ShapeType.MultiPointM
                || type == ShapeType.MultiPointZM
                || type == ShapeType.MultiPointZ;
        }

        /// <summary>
        /// Indicates if shape associated with this type is a PolyLine.
        /// </summary>
        public static bool IsPolyLine(this ShapeType type)
        {
            return type == ShapeType.PolyLine
                || type == ShapeType.PolyLineM
                || type == ShapeType.PolyLineZM
                || type == ShapeType.PolyLineZ;
        }

        /// <summary>
        /// Indicates if shape associated with this type is a Polygon.
        /// </summary>
        public static bool IsPolygon(this ShapeType type)
        {
            return type == ShapeType.Polygon
                || type == ShapeType.PolygonM
                || type == ShapeType.PolygonZM
                || type == ShapeType.PolygonZ;
        }

        internal static int Dimension(this ShapeType shpType)
        {
            int dimension = shpType.HasZ() ? 3 : 2;
            return dimension + shpType.Measures();
        }

        internal static int Measures(this ShapeType shpType)
        {
            return shpType.HasM() ? 1 : 0;
        }
    }

#pragma warning restore CS0618 // Type or member is obsolete

}
