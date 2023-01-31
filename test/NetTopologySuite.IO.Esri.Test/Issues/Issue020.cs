using NetTopologySuite.Geometries;
using NetTopologySuite.Features;
using NetTopologySuite.IO.Esri.Shapefiles.Readers;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace NetTopologySuite.IO.Esri.Test.Issues
{
    internal class Issue020
    {
        private static readonly GeometryFactory _geomFactory = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(5432);
        private static readonly WKTReader _reader = new WKTReader(_geomFactory);
        private static readonly AttributesTable _att1 = new () { { "id", 1 }, { "label", "Att1" } };
        private static readonly AttributesTable _att2 = new () { { "id", 2 }, { "label", "Att2" } };

        private static IEnumerable<IFeature> CreateFeatures(ShapeType shapeType)
        {
            switch (shapeType)
            {
                case ShapeType.Point:
                    yield return new Feature(_reader.Read("POINT(10 10)"), _att1);
                    yield return new Feature(_reader.Read("POINT EMPTY"), _att2);
                    break;
                case ShapeType.MultiPoint:
                    yield return new Feature(_reader.Read("MULTIPOINT((10 10), (20 20))"), _att1);
                    yield return new Feature(_reader.Read("MULTIPOINT EMPTY"), _att2);
                    break;
                case ShapeType.PolyLine:
                    yield return new Feature(_reader.Read("LINESTRING(10 10, 20 20)"), _att1);
                    yield return new Feature(_reader.Read("LINESTRING EMPTY"), _att2);
                    break;
                case ShapeType.Polygon:
                    yield return new Feature(_reader.Read("POLYGON((10 10, 20 10, 20 20, 10 20, 10 10))"), _att1);
                    yield return new Feature(_reader.Read("POLYGON EMPTY"), _att2);
                    break;
            }
        }

        private static Geometry Empty(ShapeType shapeType, bool single = false)
        {
            switch(shapeType)
            {
                case ShapeType.Point:
                    return Point.Empty;
                case ShapeType.MultiPoint:
                    return MultiPoint.Empty;
                case ShapeType.PolyLine:
                    return single ? LineString.Empty : MultiLineString.Empty;
                case ShapeType.Polygon:
                    return single ? Polygon.Empty : MultiPolygon.Empty;
            }
            throw new NotSupportedException();
        }

        [TestCase(ShapeType.Point)]
        [TestCase(ShapeType.MultiPoint)]
        [TestCase(ShapeType.PolyLine)]
        [TestCase(ShapeType.Polygon)]
        public void TestNullShapeToEmptyGeometry(ShapeType shapeType)
        {
            var shpPath = TestShapefiles.GetTempShpPath();
            Shapefile.WriteAllFeatures(CreateFeatures(shapeType), shpPath);

            var options = new ShapefileReaderOptions { Factory = _reader.Factory };
            using (var shpReader = Shapefile.OpenRead(shpPath, options))
            {
                Assert.That(shpReader.Read(), Is.True);
                Assert.That(shpReader.Read(), Is.True);
                Assert.That(shpReader.Geometry.IsEmpty, Is.True);
                Assert.That(shpReader.Geometry, Is.Not.SameAs(Empty(shapeType)));
                Assert.That(shpReader.Geometry.SRID, Is.EqualTo(_reader.DefaultSRID));
                Assert.That(shpReader.Read(), Is.False);
            }

            TestShapefiles.DeleteShp(shpPath);
        }
    }
}
