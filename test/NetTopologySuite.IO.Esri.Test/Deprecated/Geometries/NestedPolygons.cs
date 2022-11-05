using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;
using NUnit.Framework;

namespace NetTopologySuite.IO.Esri.Test.Deprecated.Geometries
{
    [TestFixture]
    public class NestedPolygonsTest
    {
        public readonly string Wkt = @"
            MULTIPOLYGON(
                (
                    (1 9, 9 9, 9 1, 1 1, 1 9),
                    (2 8, 2 2, 8 2, 8 8, 2 8)
                ),
                (
                    (3 7, 7 7, 7 3, 3 3, 3 7),
                    (4 6, 4 4, 6 4, 6 6, 4 6)
                )
            )";

        [Test]
        public void WriteNestedPolygons()
        {
            // This will check reading nested polygons from SHP file
            var nestedPolygons = Shapefile.ReadAllFeatures(TestShapefiles.PathTo("nested_polygons.shp"));
            var nestedPolygonsBytes = File.ReadAllBytes(TestShapefiles.PathTo("nested_polygons.shp"));
            Assert.AreEqual(1, nestedPolygons.Length);

            var multiPolygon = nestedPolygons[0].Geometry as MultiPolygon;
            Assert.AreEqual(2, multiPolygon.Count);

            var firstPolygon = multiPolygon[0] as Polygon;
            Assert.AreEqual(1, firstPolygon.Holes.Length);

            var secondPolygon = multiPolygon[0] as Polygon;
            Assert.AreEqual(1, secondPolygon.Holes.Length);

            Assert.IsTrue(firstPolygon.EnvelopeInternal.Contains(secondPolygon.EnvelopeInternal));
            Assert.IsTrue(firstPolygon.Shell.EnvelopeInternal.Contains(secondPolygon.EnvelopeInternal));


            // This will check writing nested polygons from SHP file
            var tempShpPath = TestShapefiles.GetTempShpPath();
            Shapefile.WriteAllFeatures(nestedPolygons, tempShpPath);
            var nestedPolygonsTest = Shapefile.ReadAllFeatures(tempShpPath);
            var nestedPolygonsTestBytes = File.ReadAllBytes(tempShpPath);

            Assert.AreEqual(nestedPolygons.Length, nestedPolygonsTest.Length);
            Assert.IsTrue(nestedPolygonsBytes.SequenceEqual(nestedPolygonsTestBytes));
        }
    }
}
