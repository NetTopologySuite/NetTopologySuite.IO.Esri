using NetTopologySuite.Geometries;
using System;
using NUnit.Framework;
using NetTopologySuite.Operation.Buffer;
using NetTopologySuite.Features;

namespace NetTopologySuite.IO.Esri.Test.Deprecated.Geometries
{
    public class PolygonSamples
    {
        protected GeometryFactory Factory { get; private set; }

        protected WKTReader Reader { get; private set; }

        private Polygon polygon = null;
        private LinearRing shell = null;
        private LinearRing hole = null;

        /// <summary>
        ///
        /// </summary>
        public PolygonSamples()
        {
            this.Factory = new GeometryFactory(new PrecisionModel(PrecisionModels.Fixed));
            this.Reader = new WKTReader();

            shell = Factory.CreateLinearRing(new Coordinate[] { new Coordinate(100,100),
                                                                 new Coordinate(200,100),
                                                                 new Coordinate(200,200),
                                                                 new Coordinate(100,200),
                                                                 new Coordinate(100,100), });
            hole = Factory.CreateLinearRing(new Coordinate[] {  new Coordinate(120,120),
                                                                 new Coordinate(180,120),
                                                                 new Coordinate(180,180),
                                                                 new Coordinate(120,180),
                                                                 new Coordinate(120,120), });
            polygon = Factory.CreatePolygon(shell, new LinearRing[] { hole, });
        }

        [Test]
        public void Start()
        {
            var interiorPoint = Factory.CreatePoint(new Coordinate(130, 150));
            var exteriorPoint = Factory.CreatePoint(new Coordinate(650, 1500));
            var aLine = Factory.CreateLineString(new Coordinate[] { new Coordinate(23, 32.2), new Coordinate(10, 222) });
            var anotherLine = Factory.CreateLineString(new Coordinate[] { new Coordinate(0, 1), new Coordinate(30, 30) });
            var intersectLine = Factory.CreateLineString(new Coordinate[] { new Coordinate(0, 1), new Coordinate(300, 300) });

            Write(polygon.Area);
            Write(polygon.Boundary);
            Write(polygon.BoundaryDimension);
            Write(polygon.Centroid);
            Write(polygon.Coordinate);
            Write(polygon.Coordinates.Length);
            Write(polygon.Dimension);
            Write(polygon.Envelope);
            Write(polygon.EnvelopeInternal);
            Write(polygon.ExteriorRing);
            Write(polygon.InteriorPoint);
            Write(polygon.InteriorRings.Length);
            Write(polygon.IsEmpty);
            Write(polygon.IsSimple);
            Write(polygon.IsValid);
            Write(polygon.Length);
            Write(polygon.NumInteriorRings);
            Write(polygon.NumPoints);
            if (polygon.UserData != null)
                Write(polygon.UserData);
            else Write("UserData null");

            Write(polygon.Buffer(10));
            Write(polygon.Buffer(10, new BufferParameters { EndCapStyle = EndCapStyle.Flat}));
            Write(polygon.Buffer(10, new BufferParameters { EndCapStyle = EndCapStyle.Square }));
            Write(polygon.Buffer(10, 20));
            Write(polygon.Buffer(10, new BufferParameters(20) { EndCapStyle = EndCapStyle.Flat }));
            Write(polygon.Buffer(10, new BufferParameters(20) { EndCapStyle = EndCapStyle.Square }));
            Write(polygon.Contains(interiorPoint));
            Write(polygon.Contains(exteriorPoint));
            Write(polygon.Contains(aLine));
            Write(polygon.Contains(anotherLine));
            Write(polygon.Crosses(interiorPoint));
            Write(polygon.Crosses(exteriorPoint));
            Write(polygon.Crosses(aLine));
            Write(polygon.Crosses(anotherLine));
            Write(polygon.Difference(interiorPoint));
            Write(polygon.Difference(exteriorPoint));
            Write(polygon.Difference(aLine));
            Write(polygon.Difference(anotherLine));
            Write(polygon.Disjoint(interiorPoint));
            Write(polygon.Disjoint(exteriorPoint));
            Write(polygon.Disjoint(aLine));
            Write(polygon.Disjoint(anotherLine));
            Write(polygon.Distance(interiorPoint));
            Write(polygon.Distance(exteriorPoint));
            Write(polygon.Distance(aLine));
            Write(polygon.Distance(anotherLine));
            Write(polygon.Intersection(interiorPoint));
            Write(polygon.Intersection(exteriorPoint));
            Write(polygon.Intersection(aLine));
            Write(polygon.Intersection(anotherLine));
            Write(polygon.Intersects(interiorPoint));
            Write(polygon.Intersects(exteriorPoint));
            Write(polygon.Intersects(aLine));
            Write(polygon.Intersects(anotherLine));
            Write(polygon.IsWithinDistance(interiorPoint, 300));
            Write(polygon.IsWithinDistance(exteriorPoint, 300));
            Write(polygon.IsWithinDistance(aLine, 300));
            Write(polygon.IsWithinDistance(anotherLine, 300));
            Write(polygon.Overlaps(interiorPoint));
            Write(polygon.Overlaps(exteriorPoint));
            Write(polygon.Overlaps(aLine));
            Write(polygon.Overlaps(anotherLine));
            Write(polygon.Relate(interiorPoint));
            Write(polygon.Relate(exteriorPoint));
            Write(polygon.Relate(aLine));
            Write(polygon.Relate(anotherLine));
            Write(polygon.SymmetricDifference(interiorPoint));
            Write(polygon.SymmetricDifference(exteriorPoint));
            Write(polygon.SymmetricDifference(aLine));
            Write(polygon.SymmetricDifference(anotherLine));
            Write(polygon.ToString());
            Write(polygon.AsText());
            Write(polygon.Touches(interiorPoint));
            Write(polygon.Touches(exteriorPoint));
            Write(polygon.Touches(aLine));
            Write(polygon.Touches(anotherLine));
            Write(polygon.Union(interiorPoint));
            Write(polygon.Union(exteriorPoint));
            Write(polygon.Union(aLine));
            Write(polygon.Union(anotherLine));

            string aPoly = "POLYGON ((20 20, 100 20, 100 100, 20 100, 20 20))";
            string anotherPoly = "POLYGON ((20 20, 100 20, 100 100, 20 100, 20 20), (50 50, 60 50, 60 60, 50 60, 50 50))";
            var geom1 = Reader.Read(aPoly);
            Write(geom1.AsText());
            var geom2 = Reader.Read(anotherPoly);
            Write(geom2.AsText());

            // ExpandToInclude tests
            var envelope = new Envelope(0, 0, 0, 0);
            envelope.ExpandToInclude(geom1.EnvelopeInternal);
            envelope.ExpandToInclude(geom2.EnvelopeInternal);
            Write(envelope.ToString());

            // The polygon is not correctly ordered! Calling normalize we fix the problem...
            polygon.Normalize();

            byte[] bytes = polygon.AsBinary();
            var test1 = new WKBReader().Read(bytes);
            Write(test1.ToString());

            var features = new Feature[]
            {
                CreateFeature(polygon),
                CreateFeature((Polygon)geom1),
                CreateFeature((Polygon)geom2),
            };
            Shapefile.WriteAllFeatures(features, GetType().Name);
            Shapefile.ReadAllFeatures(GetType().Name);
        }

        private Feature CreateFeature(Polygon polygon)
        {
            var mls = new MultiPolygon(new Polygon[] { polygon });
            return new Feature(mls, null);
        }

        protected void Write(object o)
        {
            Console.WriteLine(o.ToString());
        }

        protected void Write(string s)
        {
            Console.WriteLine(s);
        }
    }
}
