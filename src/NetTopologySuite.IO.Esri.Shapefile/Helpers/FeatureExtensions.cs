using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Dbf.Fields;
using System;
using System.Collections.Generic;

namespace NetTopologySuite.IO.Esri
{
    /// <summary>
    /// Feature helper methods.
    /// </summary>
    internal static class FeatureExtensions
    {
        /// <summary>
        /// Gets default <see cref="ShapeType"/> for specified geometry.
        /// </summary>
        /// <param name="geometry">A Geometry object.</param>
        /// <returns>Shape type.</returns>
        internal static ShapeType GetShapeType(this Geometry geometry)
        {
            geometry = FindNonEmptyGeometry(geometry);

            if (geometry == null || geometry.IsEmpty)
                return ShapeType.NullShape;

            var ordinates = geometry.GetOrdinates();

            if (geometry is Point)
                return GetPointType(ordinates);

            if (geometry is MultiPoint)
                return GetMultiPointType(ordinates);

            if (geometry is LineString || geometry is MultiLineString)
                return GetPolyLineType(ordinates);

            if (geometry is Polygon || geometry is MultiPolygon)
                return GetPolygonType(ordinates);

            throw new ArgumentException("Unsupported shapefile geometry: " + geometry.GetType().Name);
        }

        internal static Feature ToFeature(this Geometry geometry, AttributesTable attributes)
        {
            var feature = new Feature(geometry, attributes);
            feature.BoundingBox = geometry.EnvelopeInternal;
            return feature;
        }


        internal static AttributesTable GetAttributesTable(this DbfFieldCollection fields)
        {
            return new AttributesTable(fields.GetValues());
        }


        internal static DbfField[] GetDbfFields(this IAttributesTable attributes)
        {
            var names = attributes.GetNames();
            var fields = new DbfField[names.Length];

            for (int i = 0; i < names.Length; i++)
            {
                var name = names[i];
                var type = attributes.GetType(name);
                fields[i] = DbfField.Create(name, type);
            }
            return fields;
        }


        private static Geometry FindNonEmptyGeometry(Geometry geometry)
        {
            if (geometry == null || geometry.IsEmpty)
                return null;

            var geomColl = geometry as GeometryCollection;

            // Shapefile specification distinguish between Point and MultiPoint.
            // That not the case for PolyLine and Polygon.
            if (geomColl is MultiPoint || geomColl == null)
            {
                return geometry;
            }

            for (int i = 0; i < geomColl.Count; i++)
            {
                var geom = geomColl[i];

                // GeometryCollection -> MultiPolygon -> Polygon
                if (geom is GeometryCollection)
                    geom = FindNonEmptyGeometry(geom);

                if (geom != null && !geom.IsEmpty)
                    return geom;
            }
            return null;
        }

        internal static Geometry FindNonEmptyGeometry(this IEnumerable<IFeature> features)
        {
            if (features == null)
                return null;

            foreach (var feature in features)
            {
                var geometry = FindNonEmptyGeometry(feature.Geometry);
                if (geometry != null)
                    return geometry;
            }

            return null;
        }


        private static Ordinates GetOrdinates(this Geometry geometry)
        {
            if (geometry == null)
                throw new ArgumentNullException(nameof(geometry));

            if (geometry is Point point)
                return point.CoordinateSequence.Ordinates;

            if (geometry is LineString line)
                return line.CoordinateSequence.Ordinates;

            if (geometry is Polygon polygon)
                return polygon.Shell.CoordinateSequence.Ordinates;

            if (geometry.NumGeometries > 0)
                return GetOrdinates(FindNonEmptyGeometry(geometry));

            throw new ArgumentException("Unsupported shapefile geometry: " + geometry.GetType().Name);
        }


        private static ShapeType GetPointType(Ordinates ordinates)
        {
            if (ordinates == Ordinates.XYM)
                return ShapeType.PointM;

            if (ordinates == Ordinates.XYZ || ordinates == Ordinates.XYZM)
                return ShapeType.PointZM;

            return ShapeType.Point;
        }


        private static ShapeType GetMultiPointType(Ordinates ordinates)
        {
            if (ordinates == Ordinates.XYM)
                return ShapeType.MultiPointM;

            if (ordinates == Ordinates.XYZ || ordinates == Ordinates.XYZM)
                return ShapeType.MultiPointZM;

            return ShapeType.MultiPoint;
        }


        private static ShapeType GetPolyLineType(Ordinates ordinates)
        {
            if (ordinates == Ordinates.XYM)
                return ShapeType.PolyLineM;

            if (ordinates == Ordinates.XYZ || ordinates == Ordinates.XYZM)
                return ShapeType.PolyLineZM;

            return ShapeType.PolyLine;
        }


        private static ShapeType GetPolygonType(Ordinates ordinates)
        {
            if (ordinates == Ordinates.XYM)
                return ShapeType.PolygonM;

            if (ordinates == Ordinates.XYZ || ordinates == Ordinates.XYZM)
                return ShapeType.PolygonZM;

            return ShapeType.Polygon;
        }
    }
}
