using NUnit.Framework;
using System.IO;
using NetTopologySuite.Geometries;

namespace NetTopologySuite.IO.Esri.Test.Issues
{
    [TestFixture]
    [ShapeFileIssueNumber(79)]
    public class Issue79Fixture
    {
        /// <summary>
        /// <see href="https://github.com/NetTopologySuite/NetTopologySuite.IO.ShapeFile/issues/79"/>
        /// </summary>
        [Test]
        public void TestReadEmptyShapefile()
        {
            string filePath = TestShapefiles.PathTo("__emptyShapefile.shp");
            Assert.That(File.Exists(filePath), Is.True);

            Assert.Throws(typeof(ShapefileException), () => {
                using var shpReader = Shapefile.OpenRead(filePath);

                // TODO: Changed test logic.

                // This will not be executed.
                // ShpNullReader and ShpNullWriter can be implemented
                // but what's the point for having Shapefile without geometries at all?
                // QGIS reads this Shapefile as a table(!).
                bool success = shpReader.Read(out var deleted);
                Assert.That(success, Is.False);

                // Empty Shapefiles with specified ShapeType (Point, Polygon, ...) can be read whithout problem.
            });
        }
    }
}
